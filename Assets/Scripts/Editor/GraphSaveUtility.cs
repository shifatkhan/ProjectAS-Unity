using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/** Class that will add save and load functionality to
 * the Dialogue's graph view.
 * 
 * @author ShifatKhan
 */
public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        // Don't save if there's nothing.
        if (!Edges.Any())
            return;

        // Create a new Dialogue Container scriptable object & populate it
        DialogueContainer dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        Edge[] connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

        for (int i = 0; i < connectedPorts.Length; i++)
        {
            DialogueNode outputNode = connectedPorts[i].output.node as DialogueNode;
            DialogueNode inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.nodeLinks.Add(new NodeLinkData
            {
                baseNodeGuid = outputNode.GUID,
                portName = connectedPorts[i].output.portName,
                targetNodeGuid = inputNode.GUID
            });
        }

        foreach (DialogueNode dialogueNode in Nodes.Where(node => !node.entryPoint))
        {
            dialogueContainer.dialogueNodeData.Add(new DialogueNodeData
            {
                GUID = dialogueNode.GUID,
                dialogueText = dialogueNode.dialogueText,
                position = dialogueNode.GetPosition().position
            });
        }

        // Create the Resources folder if it wasn't already.
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        // Save the Dialogue Container.
        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<DialogueContainer>(fileName);
        if(_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found!", "Target dialogue graph file does not exist.", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();

    }

    private void ConnectNodes()
    {
        // Set and replace entry point (start) guid back from save.
        Nodes.Find(x => x.entryPoint).GUID = _containerCache.nodeLinks[0].baseNodeGuid;

        foreach (DialogueNode node in Nodes)
        {
            if (node.entryPoint)
                return;

            // Remove edges connected to this node.
            Edges.Where(x => x.input.node == node).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));

            // Remove the node.
            _targetGraphView.RemoveElement(node);
        }
    }

    private void CreateNodes()
    {
        foreach (DialogueNodeData nodeData in _containerCache.dialogueNodeData)
        {
            DialogueNode tempNode = _targetGraphView.CreateDialogueNode(nodeData.dialogueText);
            tempNode.GUID = nodeData.GUID;
            _targetGraphView.AddElement(tempNode);

            List<NodeLinkData> nodePorts = _containerCache.nodeLinks.Where(x => x.baseNodeGuid == nodeData.GUID).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.portName));
        }
    }

    private void ClearGraph()
    {
        throw new NotImplementedException();
    }
}

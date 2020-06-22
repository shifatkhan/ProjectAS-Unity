using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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
        DialogueContainer dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

        // DELETE everything before saving.
        if (AssetDatabase.IsValidFolder($"Assets/Resources/Dialogue/{fileName}"))
        {
            AssetDatabase.DeleteAsset($"Assets/Resources/Dialogue/{fileName}");
        }
        AssetDatabase.CreateFolder("Assets", "Resources");

        // Save dialogues.
        if (!SaveNodes(dialogueContainer)) // move below assets folder creation
        {
            return;
        }

        //SaveExposedProperties(dialogueContainer);

        // Save the Dialogue Container.
        //AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        //AssetDatabase.SaveAssets();
    }

    private void SaveExposedProperties(DialogueContainer dialogueContainer)
    {
        dialogueContainer.exposedProperties.AddRange(_targetGraphView.exposedProperties);
    }

    private bool SaveNodes(DialogueContainer dialogueContainer)
    {
        // Don't save if there's nothing.
        if (!Edges.Any())
            return false;

        // Create a new Dialogue Container scriptable object & populate it
        Edge[] connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

        for (int i = 0; i < connectedPorts.Length; i++)
        {
            Debug.Log($"Port out saved: {connectedPorts[i].output.portName}");
            Debug.Log($"Output node: {connectedPorts[i].output.node.title}");

            DialogueNode outputNode = connectedPorts[i].output.node as DialogueNode;
            DialogueNode inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.nodeLinks.Add(new NodeLinkData
            {
                baseNodeGuid = outputNode.GUID,
                portName = connectedPorts[i].output.portName,
                targetNodeGuid = inputNode.GUID
            });

            // SAVE RESPONSE OBJECTS

        }
        Debug.Log("-----------------------");
        foreach (DialogueNode dialogueNode in Nodes.Where(node => !node.entryPoint))
        {
            
            dialogueContainer.dialogueNodeData.Add(new DialogueNodeData
            {
                GUID = dialogueNode.GUID,
                dialogueText = dialogueNode.dialogueText,
                dialogueObject = dialogueNode.dialogueObject,
                position = dialogueNode.GetPosition().position
            });

            // SAVE DIALOGUE OBJECTS
        }

        return true;
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
        //CreateExposedProperties();
    }

    /** Clear existing properties on hot-reload
     * And then add the properties from the save data.
     */
    private void CreateExposedProperties()
    {
        _targetGraphView.ClearBlackBoardAndExposedProperties();

        foreach (ExposedProperty exposedProperty in _containerCache.exposedProperties)
        {
            _targetGraphView.AddPropertyToBlackboard(exposedProperty);
        }
    }

    /** Link the node's ports with other node's ports.
     */
    private void ConnectNodes()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            List<NodeLinkData> connections = _containerCache.nodeLinks.Where(x => x.baseNodeGuid == Nodes[i].GUID).ToList();
            for (int j = 0; j < connections.Count; j++)
            {
                string targetNodeGuid = connections[j].targetNodeGuid;
                DialogueNode targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect(
                    _containerCache.dialogueNodeData.First(x => x.GUID == targetNodeGuid).position,
                    _targetGraphView.NODE_SIZE
                    ));
            }
        }
    }

    /** Connects port pair with an edge.
     */
    private void LinkNodes(Port outputPort, Port inputport)
    {
        Edge tempEdge = new Edge
        {
            output = outputPort,
            input = inputport
        };

        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);

        _targetGraphView.Add(tempEdge);

    }

    private void CreateNodes()
    {
        foreach (DialogueNodeData nodeData in _containerCache.dialogueNodeData)
        {
            // Put vec2.zero since we change position after anyway.
            DialogueNode tempNode = _targetGraphView.CreateDialogueNode(nodeData.dialogueText, Vector2.zero);
            tempNode.GUID = nodeData.GUID;
            _targetGraphView.AddElement(tempNode);

            List<NodeLinkData> nodePorts = _containerCache.nodeLinks.Where(x => x.baseNodeGuid == nodeData.GUID).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.portName));
        }
    }

    private void ClearGraph()
    {
        // Set and replace entry point (start) guid back from save.
        Nodes.Find(x => x.entryPoint).GUID = _containerCache.nodeLinks[0].baseNodeGuid;

        foreach (DialogueNode node in Nodes)
        {
            if (node.entryPoint)
                continue;

            // Remove edges connected to this node.
            Edges.Where(x => x.input.node == node).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));

            // Remove the node.
            _targetGraphView.RemoveElement(node);
        }
    }
}

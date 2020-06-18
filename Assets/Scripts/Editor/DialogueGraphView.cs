using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

/** Class that takes care of the dialogue graph window.
 * 
 * @author ShifatKhan
 */
public class DialogueGraphView : GraphView
{
    private readonly Vector2 NODE_SIZE = new Vector2(150, 200);

    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

        // Add ability to zoom.
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        // Add ability to select nodes.
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // Add background.
        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
    }

    /** Creates the START node.
     */
    private DialogueNode GenerateEntryPointNode()
    {
        DialogueNode node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            dialogueText = "ENTRYPOINT",
            entryPoint = true
        };

        // TODO: Maybe have multiple output ports
        // Since this node is the start node, it will only have 1 Port
        // >The output port.
        Port generatedPort = GeneratePort(node, Direction.Output, Port.Capacity.Single);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        // Update node since we changed it.
        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));

        return node;
    }

    /** Checks for ports that are not from the same node.
     */
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        // Can't do a normal foreach loop since 'ports' doesn't have GetEnumerator
        ports.ForEach( port =>
        {
            Port portView = port;
            if(startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts; ;
    }

    /** Ports are the slots where we can plug in the edges (lines) into.
     * This function creates custom Ports.
     */
    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity)
    {
        // Arbitrary type.
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    /** Create a new Dialogue node with a name and add it to the graph.
     */
    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    /** Create a new Dialogue node with a name.
     * Ports will be setup automatically.
     */
    public DialogueNode CreateDialogueNode(string nodeName)
    {
        DialogueNode dialogueNode = new DialogueNode
        {
            title = nodeName,
            dialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        Port inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        Button choiceButton = new Button(clickEvent: () =>
        {
            AddChoicePort(dialogueNode);
        });
        choiceButton.text = "New Choice";

        dialogueNode.titleContainer.Add(choiceButton);

        // Update node since we changed it.
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

        dialogueNode.SetPosition(new Rect(Vector2.zero, NODE_SIZE));

        return dialogueNode;
    }

    public void AddChoicePort(DialogueNode dialogueNode, string overridenPortName = "")
    {
        Port generatedPort = GeneratePort(dialogueNode, Direction.Output, Port.Capacity.Single);

        Label oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        int outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;

        string choicePortName = string.IsNullOrEmpty(overridenPortName) 
            ? $"Choice {outputPortCount+1}" : overridenPortName;
        generatedPort.portName = choicePortName;

        // Add text field for port's choice name
        TextField textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label("  ")); // Optional gap for styling.
        generatedPort.contentContainer.Add(textField);

        // Button to delete choice ports.
        Button deleteButton = new Button(clickEvent: () => RemovePort(dialogueNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);

        dialogueNode.outputContainer.Add(generatedPort);
        // Update node since we changed it.
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }

    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        // Get edges that have generated ports.
        IEnumerable<Edge> targetEdge = edges.ToList().Where(x => 
            x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        if (!targetEdge.Any())
            return;

        // Disconnect the edges before deleting a port.
        var edge = targetEdge.First();
        edge.input.Disconnect(edge);
        RemoveElement(targetEdge.First());

        dialogueNode.outputContainer.Remove(generatedPort);
        // Update node since we changed it.
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

    }
}

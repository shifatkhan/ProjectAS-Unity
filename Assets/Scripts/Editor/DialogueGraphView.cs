using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

/** Class that takes care of the dialogue graph window.
 * 
 * @author ShifatKhan
 * @special thanks to Mert Kirimgeri - https://youtu.be/OMDfr1dzBco
 */
public class DialogueGraphView : GraphView
{
    public readonly Vector2 NODE_SIZE = new Vector2(150, 200);

    public Blackboard Blackboard;
    public List<ExposedProperty> exposedProperties = new List<ExposedProperty>();
    private NodeSearchWindow _searchWindow;

    public DialogueGraphView(EditorWindow editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

        // Add ability to zoom.
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        // Add ability to select nodes.
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // Add background.
        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
        AddSearchWindow(editorWindow);
    }

    public void AddPropertyToBlackboard(ExposedProperty exposedProperty)
    {
        // Deal with duplicate property names.
        string localPropertyName = exposedProperty.propertyName;
        string localPropertyValue = exposedProperty.propertyValue;
        while(exposedProperties.Any(x => x.propertyName == localPropertyName))
        {
            localPropertyName = $"{localPropertyName}1"; // E.G.: USERNAME(1), USERNAME(1)(1), ETC.
        }

        ExposedProperty property = new ExposedProperty();

        property.propertyName = localPropertyName;
        property.propertyValue = localPropertyValue;

        exposedProperties.Add(property);

        VisualElement container = new VisualElement();

        BlackboardField blackboardField = new BlackboardField
        {
            text = property.propertyName,
            typeText = "string property"
        };
        container.Add(blackboardField);

        TextField propertyValueTextField = new TextField("Value:")
        {
            value = property.propertyValue
        };

        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            int changingPropertyIdnex = exposedProperties.FindIndex(x => x.propertyName == property.propertyName);
            exposedProperties[changingPropertyIdnex].propertyValue = evt.newValue;
        });
        BlackboardRow blackBoardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackBoardValueRow);

        Blackboard.Add(container);
        Blackboard.scrollable = true;
    }

    public void ClearBlackBoardAndExposedProperties()
    {
        exposedProperties.Clear();
        Blackboard.Clear();
    }

    private void AddSearchWindow(EditorWindow editorWindow)
    {
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindow.Init(editorWindow, this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
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

        node.capabilities -= Capabilities.Movable; // TODO: Maybe remove to allow it to move.
        node.capabilities -= Capabilities.Deletable;

        node.styleSheets.Add(Resources.Load<StyleSheet>("StartNode"));
        
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
        Port newPort = node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        newPort.portColor = new Color(1f, 0.647f, 0);
        return newPort;
    }

    /** Create a new Dialogue node with a name and add it to the graph.
     */
    public void CreateNode(string nodeName, Vector2 position)
    {
        AddElement(CreateDialogueNode(nodeName, position));
    }

    /** Create a new Dialogue node with a name.
     * Ports will be setup automatically.
     */
    public DialogueNode CreateDialogueNode(string nodeName, Vector2 position)
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

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        // ADD button to create new choices.
        Button choiceButton = new Button(clickEvent: () =>
        {
            AddChoicePort(dialogueNode);
        });
        choiceButton.text = "New Choice";
        dialogueNode.titleContainer.Add(choiceButton);

        // ADD text field for Dialogue text.
        TextField textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.dialogueText = evt.newValue;
            dialogueNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogueNode.title);
        dialogueNode.mainContainer.Add(textField);

        // Update node since we changed it.
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

        dialogueNode.SetPosition(new Rect(position, NODE_SIZE));

        return dialogueNode;
    }

    public void AddChoicePort(DialogueNode dialogueNode, string overridenPortName = "")
    {
        Port generatedPort = GeneratePort(dialogueNode, Direction.Output, Port.Capacity.Single);

        // Remove port label and keep only a text field.
        //Label oldLabel = generatedPort.contentContainer.Q<Label>("type");
        //generatedPort.contentContainer.Remove(oldLabel);

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

        if (targetEdge.Any())
        {
            // Disconnect the edges before deleting a port (if connected)
            Edge edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        dialogueNode.outputContainer.Remove(generatedPort);
        // Update node since we changed it.
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

    }
}

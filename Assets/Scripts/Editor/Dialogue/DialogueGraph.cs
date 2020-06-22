using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/** Class that Sets up the graphs in which nodes will be placed.
 * 
 * @author ShifatKhan
 */
public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "New Narrative";

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        DialogueGraph window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph"); // text:
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        GenerateMiniMap();
        //GenerateBlackboard();
    }

    /** Created black board with property variables.
     */
    private void GenerateBlackboard()
    {
        Blackboard blackboard = new Blackboard();

        // ADD character name and sprite
        //blackboard.Add(new BlackboardSection { title = "Character details" });
        //TextField charName = new TextField("Character name:");
        //blackboard.Add(charName);

        // ADD exposed properties
        blackboard.Add(new BlackboardSection { title = "Exposed Properties"});
        blackboard.addItemRequested = _blackboard => { _graphView.AddPropertyToBlackboard(new ExposedProperty()); };
        blackboard.editTextRequested = (blackboard1, currElement, newName) => 
        {
            string oldPropertyValue = ((BlackboardField)currElement).text;
            if (_graphView.exposedProperties.Any(x => x.propertyName == newName))
            {
                EditorUtility.DisplayDialog("Error", "This property name already exists. Please choose another one.",
                    "OK");
                return;
            }

            int propertyIndex = _graphView.exposedProperties.FindIndex(x => x.propertyName == oldPropertyValue);
            _graphView.exposedProperties[propertyIndex].propertyName = newName;
            ((BlackboardField)currElement).text = newName;
        };

        blackboard.SetPosition(new Rect(10, 30, 200, 300));
        
        _graphView.Add(blackboard);
        _graphView.Blackboard = blackboard;
    }

    /** Creates a small minimap to view the whole graph.
     */
    private void GenerateMiniMap()
    {
        MiniMap miniMap = new MiniMap { anchored = true };
        Vector2 coords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
        miniMap.SetPosition(new Rect(coords.x, coords.y, 200, 140));

        _graphView.Add(miniMap);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView(this)
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        Toolbar toolbar = new Toolbar();

        // ADD file name for saving purposes.
        TextField fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);

        // Notify Editor that changes has been made. Therefore allowing user to save.
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);

        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => RequestDataOperation(true))
        {
            text = "Save Data"
        });
        toolbar.Add(new Button(() => RequestDataOperation(false))
        {
            text = "Load Data"
        });

        rootVisualElement.Add(toolbar);
    }

    /** Save or Load graph data.
     * If save = true, it will save.
     * If save = false, it will load.
     */
    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "OK");
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);

        if (save)
        {
            saveUtility.SaveGraph(_fileName);
        }
        else
        {
            saveUtility.LoadGraph(_fileName);
        }
    }
}

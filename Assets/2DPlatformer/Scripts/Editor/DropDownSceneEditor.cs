using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/** A custom editor that lets developers select the scene to load under a drop down list.
 * This helps us avoid any spelling mistakes when trying to state which scene to load.
 * @author Shifat Khan
 * @Special thanks to Micfok - http://micfok.com/tutorials/2017/07/10/ten-min-tut-custom-dropdown-editors.html
 */
 
[CustomEditor(typeof(SceneLoader))]
public class DropDownSceneEditor : Editor
{
    string[] scenes;
    int choiceIndex;

    SceneLoader sceneLoader;

    private void Awake()
    {
        // Get a list of all scene names in build.
        if (scenes == null)
        {
            GetAllScenes();
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        sceneLoader = (SceneLoader)target;

        // Set value of scene name if it was already set.
        choiceIndex = System.Array.IndexOf(scenes, sceneLoader.sceneName);

        // Create drop down menu.
        choiceIndex = EditorGUILayout.Popup("Scene to load", choiceIndex, scenes);

        // Update selected choice
        sceneLoader.sceneName = scenes[choiceIndex];

        // Thanks to afonsolfm
        // https://forum.unity.com/threads/custom-editor-not-saving-changes.424675/
        if (GUI.changed)
        {
            // Save changes made to SceneLoader
            EditorUtility.SetDirty(sceneLoader);
            EditorSceneManager.MarkSceneDirty(sceneLoader.gameObject.scene);
        }
    }

    /** Thanks to yasirkula
     * http://answers.unity.com/answers/1356841/view.html
     */
    private void GetAllScenes()
    {
        // Load all scene names in build.
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        scenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }
    }
}

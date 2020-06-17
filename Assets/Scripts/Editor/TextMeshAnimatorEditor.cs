using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextMeshAnimator))]
public class TextMeshAnimatorEditor : Editor
{
    SerializedProperty useCustomText;
    SerializedProperty customText;
    SerializedProperty shakeAmount;
    SerializedProperty shakeIndependency;
    SerializedProperty waveAmount;
    SerializedProperty waveSpeed;
    SerializedProperty waveSeparation;
    SerializedProperty waveIndependency;
    SerializedProperty wiggleAmount;
    SerializedProperty wiggleSpeed;
    SerializedProperty wiggleMinimumDuration;
    SerializedProperty wiggleIndependency;
    SerializedProperty charsVisible;
    SerializedProperty openingChar, closingChar;

    void OnEnable()
    {
        useCustomText = serializedObject.FindProperty("useCustomText");
        customText = serializedObject.FindProperty("customText");
        shakeAmount = serializedObject.FindProperty("shakeAmount");
        shakeIndependency = serializedObject.FindProperty("shakeIndependency");
        waveAmount = serializedObject.FindProperty("waveAmount");
        waveSpeed = serializedObject.FindProperty("waveSpeed");
        waveSeparation = serializedObject.FindProperty("waveSeparation");
        waveIndependency = serializedObject.FindProperty("waveIndependency");
        wiggleAmount = serializedObject.FindProperty("wiggleAmount");
        wiggleSpeed = serializedObject.FindProperty("wiggleSpeed");
        wiggleMinimumDuration = serializedObject.FindProperty("wiggleMinimumDuration");
        wiggleIndependency = serializedObject.FindProperty("wiggleIndependency");

        charsVisible = serializedObject.FindProperty("charsVisible");

        openingChar = serializedObject.FindProperty("openingChar");
        closingChar = serializedObject.FindProperty("closingChar");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (useCustomText.boolValue = EditorGUILayout.Toggle("Custom Text", useCustomText.boolValue))
        {
            customText.stringValue = EditorGUILayout.TextArea(customText.stringValue, GUILayout.Height(96));

        }
        if (GUILayout.Button("Update"))
        {
            TextMeshAnimator script = (TextMeshAnimator)target;
            script.UpdateText();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Opening/Closing Characters", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(openingChar);
        EditorGUILayout.PropertyField(closingChar);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Text Visibility Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(charsVisible);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Shake Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(shakeAmount);
        EditorGUILayout.PropertyField(shakeIndependency);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Wave Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(waveAmount);
        EditorGUILayout.PropertyField(waveSpeed);
        EditorGUILayout.PropertyField(waveSeparation);
        EditorGUILayout.PropertyField(waveIndependency);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Wiggle Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(wiggleAmount);
        EditorGUILayout.PropertyField(wiggleSpeed);
        wiggleMinimumDuration.floatValue = EditorGUILayout.Slider("Wiggle Minimum Duration", wiggleMinimumDuration.floatValue, 0.0f, 1.0f);
        EditorGUILayout.PropertyField(wiggleIndependency);

        serializedObject.ApplyModifiedProperties();
    }
}

//hi skawo :>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(CarController))]
public class CarControllerInspector : Editor
{
    private SerializedProperty dustTrail;
    private bool showParticles = false;
    private bool showSetup = false;

    int editModeIndex = 0;
    string[] EditModes = {"Simple", "Advanced"};
    private void OnEnable()
    {
        dustTrail = serializedObject.FindProperty("dustTrail");
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        serializedObject.Update();
        
        editModeIndex = GUILayout.Toolbar(editModeIndex, EditModes);

        serializedObject.FindProperty("totalTime").floatValue = EditorGUILayout.FloatField("totalTime", serializedObject.FindProperty("totalTime").floatValue);

        if (editModeIndex == 1)
        {
            GUILayout.Space(20);

            GUILayout.Label("Advanced", "WhiteLargeLabel");
            
            GUILayout.Space(10);

            serializedObject.FindProperty("theRB").objectReferenceValue = EditorGUILayout.ObjectField("Motor Rigidbody", serializedObject.FindProperty("theRB").objectReferenceValue, typeof(Rigidbody), true);
            
            GUILayout.Space(10);

            showParticles = EditorGUILayout.Foldout(showParticles, "Particles");
            if (showParticles)
            {
                for (int i = 0; i < dustTrail.arraySize; i++)
                {
                    var dialogue = dustTrail.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(dialogue, new GUIContent("Particles " + i), true);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", "MiniButtonLeft"))
                {
                    dustTrail.arraySize++;
                }

                if (GUILayout.Button("-", "MiniButtonRight"))
                {
                    dustTrail.arraySize--;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            GUILayout.Space(10);
            GUILayout.Label("" + serializedObject.FindProperty("whatIsGround").floatValue);
            serializedObject.FindProperty("whatIsGround").floatValue = EditorGUILayout.LayerField("What is ground", (int) serializedObject.FindProperty("whatIsGround").floatValue);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

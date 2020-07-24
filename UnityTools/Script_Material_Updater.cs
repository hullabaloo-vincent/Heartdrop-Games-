using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class Script_Material_Updater : EditorWindow { 
    string shaderSelection = "kShading/Toon Lit";

    [MenuItem("Window/Material Updater")]

    static void Init() {
        Script_Material_Updater window = (Script_Material_Updater)EditorWindow.GetWindow(typeof(Script_Material_Updater), true, "Material Updater");
        window.Show();
    }

    void OnGUI() {
        string currentSelection = "No Game Object Selected";
        string materialList = "";
        if (Selection.activeObject) {
            currentSelection = Selection.activeObject.name.ToString();
            GameObject selected = Selection.activeGameObject;
            Material[] objectMaterials = selected.GetComponent<Renderer>().materials;
            for (int i = 0; i < objectMaterials.Length; i++) {
                materialList = materialList + "Instance: " + i + "  " + objectMaterials[i].name.ToString() + " (" + objectMaterials[i].shader.name.ToString() + ")\n";
            }
        }
        GUILayout.Label("Currently Selected: " + currentSelection, EditorStyles.boldLabel);
        GUILayout.Space(20);
        GUILayout.Label("Shader Selection");
        GUILayout.TextField(shaderSelection);
        
        GUILayout.Space(20);

        if (GUILayout.Button("Update mesh")) { 
            GameObject selected = Selection.activeGameObject;
            Material[] objectMaterials = selected.GetComponent<Renderer>().materials;
            for (int i = 0; i < objectMaterials.Length; i++) {
                selected.GetComponent<MeshRenderer>().materials[i].shader = Shader.Find(shaderSelection);
            }
        }
        GUILayout.Space(20);
        GUILayout.Label(materialList);
    }
    void OnInspectorUpdate() {
        Repaint();
    }
}

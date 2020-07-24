using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class Script_Material_Updater : EditorWindow { 
    string shaderSelection = "kShading/Toon Lit";
  //  bool textureSelection = true;
   // bool normalSelection = true;

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
     //   GUILayout.Toggle(textureSelection, "Migrate Color Texture");
     //   textureSelection = EditorGUILayout.Toggle(textureSelection, "Migrate Color Texture");
     //   if (textureSelection) {
     //   }
     //   GUILayout.Toggle(normalSelection, "Migrate Normal");
        GUILayout.Space(20);

        if (GUILayout.Button("Update mesh")) { 
            GameObject selected = Selection.activeGameObject;
            Material[] objectMaterials = selected.GetComponent<Renderer>().materials;
            for (int i = 0; i < objectMaterials.Length; i++) {
              /*  Renderer rend = selected.GetComponent<Renderer>();
                Material newMat = new Material(Shader.Find(shaderSelection));
                 try {
                     Debug.Log(objectMaterials[i].GetTexture("_MainTex").name.ToString());
                     newMat.SetTexture("_BaseMap", objectMaterials[i].GetTexture("_MainTex"));
                 } catch (NullReferenceException nre_Tex) {
                     Debug.Log("No main texture");
                 }
                 try {
                     Debug.Log(objectMaterials[i].GetTexture("_BumpMap").name.ToString());
                     newMat.SetTexture("_BumpMap", objectMaterials[i].GetTexture("_BumpMap"));
                 } catch(NullReferenceException nre_Nor) {
                     Debug.Log("No normal");
                 }*/

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

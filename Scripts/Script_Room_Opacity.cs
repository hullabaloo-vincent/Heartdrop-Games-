﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Room_Opacity : MonoBehaviour {
      /*
    * ENUMS
    */

    //the blendmodes that materials can be changed to
    public enum BlendMode {
         Opaque,
         Cutout,
         Fade,
         Transparent
     }
     //display values in the inspector
    [Range(0f, 100f)] public int customOpacity;
    private int offOpacity = 0;

    List<GameObject> roomAssets;
    List<GameObject> roomAssetsFiltered;
    public bool isStartRoom;
    GameObject room;
    public GameObject[] walls;

     void Start(){
        room = transform.parent.gameObject;
        //get list of children assets
        roomAssets = new List<GameObject>();
        roomAssetsFiltered = new List<GameObject>();
        for (int i = 0; i < room.transform.childCount; i++) {
            roomAssets.Add(room.transform.GetChild(i).gameObject);
        }
        if (!isStartRoom){
            TransparentWalls();
            MakeTransparent();
        }
     }

    /*
    * Code to change the blend modes
    */
     public void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode) {
         switch (blendMode) {
             case BlendMode.Opaque:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = -1;
                break;
             case BlendMode.Cutout:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 2450;
                break;
             case BlendMode.Fade:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
             case BlendMode.Transparent:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
         }
     }

     public void RefreshRoomAssets(){
         roomAssets.Clear();
         for (int i = 0; i < room.transform.childCount; i++) {
            roomAssets.Add(room.transform.GetChild(i).gameObject);
        }
        MakeTransparent();
     }

     private void TransparentWalls(){
        for (int i = 0; i < walls.Length; i++) {
            Script_Transparency_Manager test = walls[i].GetComponent<Script_Transparency_Manager>();
            test.GetAssets();
            test.MakeTransparent(false); 
        }
     }

     private void ResetWalls(){
        for (int i = 0; i < walls.Length; i++) {
            Script_Transparency_Manager test = walls[i].GetComponent<Script_Transparency_Manager>();
            test.ResetWallValues();
        }
     }

     public void MakeTransparent(){
         //currentTask is for exception handling
         GameObject currentTask = null;
         for (int i = 0; i < roomAssets.Count; i++) {
            try {
                currentTask = roomAssets[i];
                //get a list of materials in gameobject and set each blendmode to transparent
                foreach (Material m in roomAssets[i].GetComponent<Renderer>().materials){
                    roomAssetsFiltered.Add(roomAssets[i]);
                    ChangeRenderMode (m, BlendMode.Transparent);
                    Color32 col = m.GetColor("_Color");
                    col.a = (byte)customOpacity;
                    m.SetColor("_Color", col);
                }
            } catch (MissingComponentException MCE) {
                //get gameobject that caused the exception
                GameObject problemObject = currentTask;
                //get children of problem gameobject
                List<GameObject> exceptionObject = new List<GameObject>();
                if (!problemObject.name.Contains("Wall")){
                    for (int i2 = 0; i2 < problemObject.transform.childCount; i2++) {
                        exceptionObject.Add(problemObject.transform.GetChild(i2).gameObject);
                    }
                    //set its children to transparent
                    for (int i3 = 0; i3 < exceptionObject.Count; i3++){
                        //get a list of materials in gameobject and set each blendmode to transparent
                        try{
                            foreach (Material m in exceptionObject[i3].GetComponent<Renderer>().materials){
                                roomAssetsFiltered.Add(exceptionObject[i3]);
                                ChangeRenderMode (m, BlendMode.Transparent);
                                Color32 col = m.GetColor("_Color");
                                col.a = (byte)customOpacity;
                                m.SetColor("_Color", col);
                            }
                            continue;
                        } catch (MissingComponentException MCE2){
                            continue;
                        }
                    }
                }
            }
        }
     }

     public void MakeOpaque(){
         for (int i = 0; i < roomAssetsFiltered.Count; i++){
            foreach (Material m in roomAssetsFiltered[i].GetComponent<Renderer>().materials){
                //no need to chaneg alpha channel. The new blend mode will not use it
                ChangeRenderMode (m, BlendMode.Opaque); 
            }
         }
     }

     private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player"){
           // Debug.Log("Entering: " + room.name);
            MakeOpaque();
            ResetWalls();
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player"){
          //  Debug.Log("Exiting: " + room.name);
            MakeTransparent();
            TransparentWalls();
        }
    }
}
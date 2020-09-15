using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Transparency_Manager : MonoBehaviour {

    List<GameObject> wallAssets;
    public enum EnumWallType {
         North,
         South,
         East,
         West,
         Outer,
         Door
     }
    [Header("Select Wall Type")]
     public EnumWallType wallType;

    [Range(0f, 360f)] private float northHeading;


    [Header("Set Wall Opacity")]
    [Tooltip("Default value should be 30")]
    [Range(0f, 100f)] public int customOpacity;


    private float myHeading;
    private float dif;
    private Heading heading;

    public enum Heading {
        North,
        East,
        South,
        West
    }

    public enum BlendMode {
         Opaque,
         Cutout,
         Fade,
         Transparent
     }
 
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

     void Start(){
        wallAssets = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++) {
            wallAssets.Add(transform.GetChild(i).gameObject);
        }
        myHeading = transform.eulerAngles.y;
        northHeading = Input.compass.magneticHeading;

        dif = myHeading - northHeading;
        if (dif < 0) dif += 360f;

        if (dif > 45 && dif <= 135) {
            heading = Heading.East;
        } else if (dif > 135 && dif <= 225) {
            heading = Heading.South;
        } else if (dif > 225 && dif <= 315) {
            heading = Heading.West;
        } else {
            heading = Heading.North;
        }
     //   Debug.Log(gameObject.name + " Global Direction: " + heading.ToString());
     //   Debug.Log(gameObject.name + " Local Direction: " + wallType.ToString());
        if (heading.ToString() == "North" && wallType.ToString() == "North") {
            MakeTransparent();
        }
        if (heading.ToString() == "North" && wallType.ToString() == "East") {
            MakeTransparent();
        }
        if (heading.ToString() == "South" && wallType.ToString() == "South") {
            MakeTransparent();
        }
        if (heading.ToString() == "South" && wallType.ToString() == "West") {
            MakeTransparent();
        }
        if (heading.ToString() == "East" && wallType.ToString() == "South") {
            MakeTransparent();
        }
        if (heading.ToString() == "East" && wallType.ToString() == "East") {
            MakeTransparent();
        }
        if (heading.ToString() == "West" && wallType.ToString() == "North") {
            MakeTransparent();
        }
        if (heading.ToString() == "West" && wallType.ToString() == "West") {
            MakeTransparent();
        }
     }

     public void MakeTransparent(){
         //currentTask is for exception handling
         GameObject currentTask = null;
         try {
            for (int i = 0; i < wallAssets.Count; i++) {
                currentTask = wallAssets[i];
                //get a list of materials in gameobject and set each blendmode to transparent
                foreach (Material m in wallAssets[i].GetComponent<Renderer>().materials){
                    ChangeRenderMode (m, BlendMode.Transparent);
                    Color32 col = m.GetColor("_Color");
                    col.a = (byte)customOpacity;
                    m.SetColor("_Color", col);
                }
            }
         } catch (MissingComponentException MCE) {
             //get gameobject that caused the exception
            GameObject problemObject = currentTask;

            //get children of problem gameobject
            List<GameObject> exceptionObject = new List<GameObject>();
            for (int i = 0; i < problemObject.transform.childCount; i++) {
                exceptionObject.Add(problemObject.transform.GetChild(i).gameObject);
            }
            //set its children to transparent
            for (int i = 0; i < exceptionObject.Count; i++){
                //get a list of materials in gameobject and set each blendmode to transparent
                foreach (Material m in exceptionObject[i].GetComponent<Renderer>().materials){
                    ChangeRenderMode (m, BlendMode.Transparent);
                    Color32 col = m.GetColor("_Color");
                    col.a = (byte)customOpacity;
                    m.SetColor("_Color", col);
                }
            }
         }
     }

     public void MakeOpaque(){
         for (int i = 0; i < wallAssets.Count; i++){
            foreach (Material m in wallAssets[i].GetComponent<Renderer>().materials){
                Debug.Log("Changing materials: " + m.name);
                ChangeRenderMode (m, BlendMode.Opaque); 
            }
         }
     }
}
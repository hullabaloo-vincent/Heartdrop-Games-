using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Transparency_Manager : MonoBehaviour
{
    void Start()
    {
        //get list of children assets
        wallAssets = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            wallAssets.Add(transform.GetChild(i).gameObject);
        }

        /*
        * Note the direction of the wall based on the cardinal direction
        */
        GetDirection();
        /*
        * Conditions to make which walls transparent
        */
        CheckWallStatus();
    }
    /*
    * Get direction
    */
    private void GetDirection()
    {
        myDirection = transform.eulerAngles.y;
        northDirection = Input.compass.magneticHeading;

        dif = myDirection - northDirection;
        if (dif < 0) dif += 360f;

        if (dif > 45 && dif <= 135)
        {
            cardinalDirection = Direction.East;
        }
        else if (dif > 135 && dif <= 225)
        {
            cardinalDirection = Direction.South;
        }
        else if (dif > 225 && dif <= 315)
        {
            cardinalDirection = Direction.West;
        }
        else
        {
            cardinalDirection = Direction.North;
        }
    }

    public void ResetWallValues()
    {
        MakeOpaque();
    }

    public void CheckWallStatus()
    {

        if (cardinalDirection.ToString() == "North" && wallType.ToString() == "North")
        {
            MakeTransparent(true);
            defaultStatus = false;
        }
        if (cardinalDirection.ToString() == "North" && wallType.ToString() == "East")
        {
            MakeTransparent(true);
            defaultStatus = false;
        }
        if (cardinalDirection.ToString() == "South" && wallType.ToString() == "South")
        {
            MakeTransparent(true);
            defaultStatus = false;
        }
        if (cardinalDirection.ToString() == "South" && wallType.ToString() == "West")
        {
            MakeTransparent(true);
            defaultStatus = false;
        }
        if (cardinalDirection.ToString() == "East" && wallType.ToString() == "South")
        {
            MakeTransparent(true);
            defaultStatus = false;
        }
        if (cardinalDirection.ToString() == "East" && wallType.ToString() == "East")
        {
            MakeTransparent(true);
            defaultStatus = false;
        }
        if (cardinalDirection.ToString() == "West" && wallType.ToString() == "North")
        {
            MakeTransparent(true);
            defaultStatus = false;
        }
        if (cardinalDirection.ToString() == "West" && wallType.ToString() == "West")
        {
            MakeTransparent(true);
            defaultStatus = false;
        }
    }

    /*
    * Code to change the blend modes
    */
    public void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
    {
        switch (blendMode)
        {
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
    public void GetAssets()
    {
        wallAssets = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            wallAssets.Add(transform.GetChild(i).gameObject);
        }
    }
    public void MakeTransparent(bool checkStatus)
    {
        //currentTask is for exception handling
        GameObject currentTask = null;
        try
        {
            for (int i = 0; i < wallAssets.Count; i++)
            {
                currentTask = wallAssets[i];
                //cull wall assets for transparent walls
                if (wallAssets[i].name.Contains("KCWall") || !checkStatus)
                {
                    if (checkStatus)
                    {
                        wallAssets[i].GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    }
                    else
                    {
                        wallAssets[i].GetComponent<MeshRenderer>().enabled = false;
                    }
                }
                else
                {
                    if (checkStatus)
                    {
                        Destroy(wallAssets[i]);
                    }
                }
            }
        }
        catch (MissingComponentException MCE)
        {
            //get gameobject that caused the exception
            GameObject problemObject = currentTask;

            //get children of problem gameobject
            List<GameObject> exceptionObject = new List<GameObject>();
            for (int i = 0; i < problemObject.transform.childCount; i++)
            {
                exceptionObject.Add(problemObject.transform.GetChild(i).gameObject);
            }
            //set its children to transparent
            for (int i = 0; i < exceptionObject.Count; i++)
            {
                //get a list of materials in gameobject and set each blendmode to transparent
                foreach (Material m in exceptionObject[i].GetComponent<Renderer>().materials)
                {
                    ChangeRenderMode(m, BlendMode.Transparent);
                    Color32 col = m.GetColor("_Color");
                    col.a = (byte)customOpacity;
                    m.SetColor("_Color", col);
                }
            }
        }
    }

    public void MakeOpaque()
    {
        //currentTask is for exception handling
        GameObject currentTask = null;
        try
        {
            for (int i = 0; i < wallAssets.Count; i++)
            {
                currentTask = wallAssets[i];
                //get a list of materials in gameobject and set each blendmode to opaque
                if (wallAssets[i] != null)
                {
                    if (wallAssets[i].name.Contains("KCFloor"))
                    {
                        foreach (Material m in wallAssets[i].GetComponent<Renderer>().materials)
                        {
                            ChangeRenderMode(m, BlendMode.Opaque);
                        }
                    }
                    else
                    {
                        wallAssets[i].GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }
        }
        catch (MissingComponentException MCE)
        {
            //get gameobject that caused the exception
            GameObject problemObject = currentTask;

            //get children of problem gameobject
            List<GameObject> exceptionObject = new List<GameObject>();
            for (int i = 0; i < problemObject.transform.childCount; i++)
            {
                exceptionObject.Add(problemObject.transform.GetChild(i).gameObject);
            }
            //set its children to transparent
            for (int i = 0; i < exceptionObject.Count; i++)
            {
                //get a list of materials in gameobject and set each blendmode to transparent
                foreach (Material m in exceptionObject[i].GetComponent<Renderer>().materials)
                {
                    ChangeRenderMode(m, BlendMode.Opaque);
                }
            }
        }
    }

    /*
    * ENUMS
    */
    //what type of wall it is natively facing
    public enum EnumWallType
    {
        North,
        South,
        East,
        West
    }
    //the directions that wall can face
    public enum Direction
    {
        North,
        East,
        South,
        West
    }
    //the blendmodes that materials can be changed to
    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }
    //display values in the inspector
    [Header("Select Wall Type")]
    public EnumWallType wallType;
    [Range(0f, 360f)] private float northDirection;

    [Header("Set Wall Opacity")]
    [Tooltip("Default value should be 30")]
    [Range(0f, 100f)] public int customOpacity;

    private float myDirection;
    private float dif;
    private Direction cardinalDirection;

    List<GameObject> wallAssets;
    List<GameObject> wallAssetsFiltered = new List<GameObject>();

    bool defaultStatus = true; //true = visible; false = invisible;
    bool filteredAssets = false;
}
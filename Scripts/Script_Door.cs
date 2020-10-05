using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Door : MonoBehaviour {

    GameObject ExitRoom;
    GameObject EntranceRoom;

    public GameObject OtherDoor;

    Script_Door OtherDoorScript;

    List<GameObject> EntranceAssets;
    List<GameObject> ExitAssets;

    GameObject floorDecoration;

    Script_Asset_Loader AssetLoaderEntrance;
    Script_Asset_Loader AssetLoaderExit;

    [Tooltip("Determines whether the door is the entrance or the exit of the connecting room")]
    public bool isExit;

    //check if the player already entered and exited the door
    bool reverseDirection = false;
    bool reset = false;

    void Start(){
        OtherDoorScript = OtherDoor.GetComponent<Script_Door>();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Door"){
            if (isExit){
                ExitRoom = other.gameObject.transform.parent.parent.gameObject;
                AssetLoaderExit = ExitRoom.GetComponent<Script_Asset_Loader>();
            } else {
                EntranceRoom = other.gameObject.transform.parent.parent.gameObject;
                AssetLoaderEntrance = EntranceRoom.GetComponent<Script_Asset_Loader>();
            }
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Player"){
            reset = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player"){
            if (!reverseDirection && reset){
                reverseDirection = true;
                reset = false;
                //Entrance code
                if (!isExit){
                    //Instantiate the exit assets
                    string assetName = "Room_Decoration_Prefabs/" + OtherDoorScript.GetAssetLoaderExit().Asset;
                    GameObject assetToLoad = Resources.Load<GameObject>(assetName);
                    floorDecoration = Instantiate(assetToLoad, 
                    OtherDoorScript.GetExitRoomPosition(), OtherDoorScript.GetExitRoomRotation());
                    floorDecoration.transform.parent = OtherDoorScript.GetExitRoom().transform;
                    OtherDoorScript.GetAssetLoaderExit().VisibilityController.GetComponent<Script_Room_Opacity>().RefreshRoomAssets();
                } else {
                    //If the player passes through, delete the entrance room assets
                    for (int i = 0; i < OtherDoorScript.GetEntranceRoom().transform.childCount; i++) {
                        if (OtherDoorScript.GetEntranceRoom().transform.GetChild(i).gameObject.tag == "Decoration"){
                            Destroy(OtherDoorScript.GetEntranceRoom().transform.GetChild(i).gameObject);
                        }
                    }
                }
            }

            if (reverseDirection & reset){
                reverseDirection = false;
                reset = false;
                if (!isExit) {
                    for (int i = 0; i < OtherDoorScript.GetExitRoom().transform.childCount; i++) {
                        if (OtherDoorScript.GetExitRoom().transform.GetChild(i).gameObject.tag == "Decoration"){
                            Destroy(OtherDoorScript.GetExitRoom().transform.GetChild(i).gameObject);
                        }
                    }
                } else {
                    string assetName = "Room_Decoration_Prefabs/" + OtherDoorScript.GetAssetLoaderEntrance().Asset;
                    GameObject assetToLoad = Resources.Load<GameObject>(assetName);
                    floorDecoration = Instantiate(assetToLoad, 
                    OtherDoorScript.GetEntranceRoomPosition(), OtherDoorScript.GetEntranceRoomRotation());
                    floorDecoration.transform.parent = OtherDoorScript.GetEntranceRoom().transform;
                    OtherDoorScript.GetAssetLoaderEntrance().VisibilityController.GetComponent<Script_Room_Opacity>().RefreshRoomAssets();
                }
            }
        }
    }

    public Script_Asset_Loader GetAssetLoaderExit(){
        return AssetLoaderExit;
    }
    public Script_Asset_Loader GetAssetLoaderEntrance(){
        return AssetLoaderEntrance;
    }
    public Vector3 GetEntranceRoomPosition(){
        return EntranceRoom.transform.position;
    }
    public Quaternion GetEntranceRoomRotation(){
        return EntranceRoom.transform.rotation;
    }
    public Vector3 GetExitRoomPosition(){
        return ExitRoom.transform.position;
    }
    public Quaternion GetExitRoomRotation(){
        return ExitRoom.transform.rotation;
    }
    public GameObject GetExitRoom(){
        return ExitRoom;
    }
    public GameObject GetEntranceRoom(){
        return EntranceRoom;
    }
    public void DestroyAsset(){
        if (floorDecoration != null){
            Destroy(floorDecoration);
        }
    }
}
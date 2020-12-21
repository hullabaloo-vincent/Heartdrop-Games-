using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Health : MonoBehaviour {

    GameObject healthBubblesSurface;
    GameObject healthLiquid;
    float playerHealth = 1.0f;
    void Start() {
        for (int i = 0; i < transform.childCount - 1; i++) {
          //  Debug.Log(transform.GetChild(i).gameObject.name.ToString());
            if (transform.GetChild(i).gameObject.name.ToString() == "Health_Liquid") {
                healthLiquid = transform.GetChild(i).gameObject;
            }
            if (transform.GetChild(i).gameObject.name.ToString() == "Health_Surface_Bubbles") {
                healthBubblesSurface = transform.GetChild(i).gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Script_Player_Movement>().Health;
        if (playerHealth != 1) {
            healthBubblesSurface.SetActive(false);
        }
        if (playerHealth <= 0) {
                healthLiquid.GetComponent<RectTransform>().localScale = new Vector3(1f, 0f, 1f);
        } else {
                healthLiquid.GetComponent<RectTransform>().localScale = new Vector3(1f, playerHealth, 1f);
        } 
          //  health_bar_bubbles.GetComponent<RectTransform>().localScale = new Vector3(3.448276f / health_bar.transform.localScale.x, 2.564103f / health_bar.transform.localScale.y, 1f / health_bar.transform.localScale.z);
    }
}
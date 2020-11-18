using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Flickering_Light : MonoBehaviour {
    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    public new Light light;
    [Tooltip("Minimum random light intensity")]
    public float minIntensity = 0f;
    [Tooltip("Maximum random light intensity")]
    public float maxIntensity = 1f;
    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
    [Range(1, 50)]
    public int smoothing = 5;

    Queue<float> smoothQueue;
    float lastSum = 0;

    bool CanFlicker = true;

    public void Reset() {
        smoothQueue.Clear();
        lastSum = 0;
    }

    void Start() {
        smoothQueue = new Queue<float>(smoothing);
        if (light == null) {
            light = GetComponent<Light>();
        }
        StartCoroutine("TurnOnFlicker");
    }

    void Update() {
        if (CanFlicker){
            if (light == null)
                return;

            while (smoothQueue.Count >= smoothing) {
                lastSum -= smoothQueue.Dequeue();
            }

            float newVal = Random.Range(minIntensity, maxIntensity);
            smoothQueue.Enqueue(newVal);
            lastSum += newVal;

            light.intensity = lastSum / (float)smoothQueue.Count;
        } else {
            light.intensity = maxIntensity;
        }
    }
    IEnumerator TurnOnFlicker() {
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        StartCoroutine("TurnOffFlicker");
        CanFlicker = true;
        yield return 0;
    }
    IEnumerator TurnOffFlicker() {
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        StartCoroutine("TurnOnFlicker");
        CanFlicker = false;
        yield return 0;
    }
}
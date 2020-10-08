using UnityEngine;

public class Script_Camera_Shake : MonoBehaviour {
    private Transform transform;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.05f;
    private float dampingSpeed = 2f;
    Vector3 initialPosition;

    void Awake() {
        if (transform == null) {
            transform = GetComponent(typeof(Transform)) as Transform;
        }
    }
    void Update() {
        initialPosition = transform.localPosition;
        if (shakeDuration > 0) {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        } else {
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }
    public void TriggerShake(float duration) {
        shakeDuration = duration;
    }
}

using UnityEngine;
using System.Collections;

public class Script_Camera : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }

    public Transform target;
    public float smoothing = 5f;
    Vector3 offset;
}
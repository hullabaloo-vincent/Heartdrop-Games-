using UnityEngine;

public class Script_Camera_Shake : MonoBehaviour
{
    void Awake()
    {
        if (transform == null)
        {
            _Transform = GetComponent(typeof(Transform)) as Transform;
        }
    }
    void Update()
    {
        _InitialPosition = transform.localPosition;
        if (_ShakeDuration > 0)
        {
            transform.localPosition = _InitialPosition + Random.insideUnitSphere * _ShakeMagnitude;

            _ShakeDuration -= Time.deltaTime * _DampingSpeed;
        }
        else
        {
            _ShakeDuration = 0f;
            transform.localPosition = _InitialPosition;
        }
    }
    public void TriggerShake(float duration)
    {
        _ShakeDuration = duration;
    }

    private Transform _Transform;
    private float _ShakeDuration = 0f;
    private float _ShakeMagnitude = 0.05f;
    private float _DampingSpeed = 2f;
    private Vector3 _InitialPosition;
}

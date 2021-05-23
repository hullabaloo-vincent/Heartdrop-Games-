using UnityEngine;
using UnityEngine.UI;

public class CS_Notifications : MonoBehaviour
{
    private void Start() {
        _NotifText = target.GetComponent<Text>();
    }

    void Update()
    {
        if (_Expand)
        {
            hasExpanded = true;
            float newScale = Mathf.SmoothDamp(transform.localScale.y, 0.5f, ref yVelocity, smoothTime);
            transform.localScale = new Vector3(2.573437f, newScale, 1);
            target.SetActive(true);
        }
        if (!_Expand && hasExpanded)
        {
            target.SetActive(false);
            float newScale = Mathf.SmoothDamp(transform.localScale.y, 0, ref yVelocity, smoothTime);
            transform.localScale = new Vector3(2.573437f, newScale, 1);
            if (transform.localScale.y == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
    public void SetActiveNotif(bool state)
    {
        _Expand = state;
    }

    public void SetText(string newText) {
        _NotifText.text = newText;
    }

    public GameObject target;
    float smoothTime = 0.15f;
    float yVelocity = 0.0f;
    private bool _Expand = false;
    bool hasExpanded = false;
    private Text _NotifText;
}

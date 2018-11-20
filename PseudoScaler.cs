using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoScaler : MonoBehaviour
{
    [Range(1, 5)]
    public float upperScaleBound = 3f;
    [HideInInspector] public Vector3 origScale;
    public bool isDiningModel;
    Touch touch1, touch2;
    Vector2[] lastScaleValue;
    bool wasScalingLastFrame;
    float lowerScale = 1;

    void OnEnable()
    {
        if (isDiningModel)
            origScale = Vector3.one;
        else
            origScale = transform.localScale;

        if (GetComponent<PseudoValues>())
        {
            if (GetComponent<PseudoValues>().origScale != Vector3.zero)
                transform.localScale = GetComponent<PseudoValues>().origScale;
        }
    }

    private void OnDisable()
    {
        transform.localScale = isDiningModel ? Vector3.one : origScale;
    }

    void Update()
    {
        if (Input.touchCount != 2)
            return;

        touch1 = Input.GetTouch(0);
        touch2 = Input.GetTouch(1);

        if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
        {
            Vector2[] newScaleValue = { touch1.position, touch2.position };
            if (!wasScalingLastFrame)
            {
                lastScaleValue = newScaleValue;
                wasScalingLastFrame = true;
            }
            else
            {
                float newDistance = Vector2.Distance(newScaleValue[0], newScaleValue[1]);
                float oldDistance = Vector2.Distance(lastScaleValue[0], lastScaleValue[1]);
                float offset = (newDistance - oldDistance) / 30;
                Vector3 newScale = transform.localScale + origScale * offset;
                Vector3 clampedScale = new Vector3(Mathf.Clamp(newScale.x, origScale.x * lowerScale, origScale.x * upperScaleBound), Mathf.Clamp(newScale.y, origScale.y * lowerScale, origScale.y * upperScaleBound), Mathf.Clamp(newScale.z, origScale.z * lowerScale, origScale.z * upperScaleBound));
                transform.localScale = Vector3.Lerp(transform.localScale, clampedScale, .3f);
                lastScaleValue = newScaleValue;
            }
        }
        if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
            wasScalingLastFrame = false;
    }
}

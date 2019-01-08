using System.Collections;
using UnityEngine;

public class Pseudo3DSlider : MonoBehaviour
{
    [SerializeField] GameObject[] prefabs;
    [SerializeField] Transform slotTransform;
    [SerializeField] Vector3 prefabRotation = new Vector3(0, 120, 0);
    [SerializeField] float slotSize = 10f, slidingSpeed = 10f;

    bool isSliding;
    float limit;

    private void Start()
    {
        var spawnRotation = Quaternion.Euler(prefabRotation);
        int i;
        for (i = 0; i < prefabs.Length; i++)
            Instantiate(prefabs[i], Vector3.right * i * slotSize, spawnRotation, slotTransform);
        limit = (i - 1) * -slotSize;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftArrow) && slotTransform.position.x > limit)
            Slide(-1);
        else if (Input.GetKeyDown(KeyCode.RightArrow) && slotTransform.position.x < 0)
            Slide(1);
#else
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            float deltaX;
            if (Mathf.Abs(deltaX = touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y))
                if (deltaX < 0 && slotTransform.position.x > limit)
                    Slide(-1);
                else if (deltaX > 0 && slotTransform.position.x < 0)
                    Slide(1);
        }
#endif
    }

    private void Slide(float deltaX)
    {
        if (!isSliding)
            StartCoroutine(Sliding(deltaX < 0));
    }

    IEnumerator Sliding(bool slideLeft)
    {
        isSliding = true;
        Vector3 nextPos = slideLeft ? Vector3.left : Vector3.right;
        nextPos *= slotSize;
        nextPos += slotTransform.position;

        while (slideLeft ? slotTransform.position.x > nextPos.x : slotTransform.position.x < nextPos.x)
        {
            slotTransform.position = Vector3.MoveTowards(slotTransform.position, nextPos, Time.deltaTime * slidingSpeed);
            yield return new WaitForEndOfFrame();
        }
        isSliding = false;
    }
}

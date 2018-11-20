using UnityEngine;

public class PseudoDistributer : MonoBehaviour
{
    [SerializeField] Transform startTransform, endTransform;
    [SerializeField] GameObject spawnGO;
    [SerializeField] int divisions = 13;

    private void Start()
    {
        Vector3 current = startTransform.position, next = endTransform.position;
        float distance = Vector3.Distance(startTransform.position, endTransform.position);

        for (int i = 0; i < divisions; i++)
        {
            float moveDistance = (distance * i) / (divisions - 1);
            Instantiate(spawnGO, Vector3.MoveTowards(current, next, moveDistance), spawnGO.transform.rotation);
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace PseudoSignature
{

    [RequireComponent(typeof(Image))]
    public class PseudoDragImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        public Transform spawnParentTransform;
        public SofaType sofaType;
        public bool dragOnSurfaces = true;

        private GameObject m_DraggingIcon;
        private RectTransform m_DraggingPlane;
        Image image;

        public Image Image
        {
            get
            {
                return image;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.isValid)
            {
                if (!eventData.pointerCurrentRaycast.gameObject.Equals(m_DraggingIcon))
                {
                    Debug.LogErrorFormat("<color=magenta>Dropped on UI: {0}</color>", eventData.pointerCurrentRaycast.gameObject);
                    return;
                }
            }

            if (Utility.spawnSofaType == SofaType.NONE)
            {
                Debug.LogErrorFormat("<color=magenta>No Sofa Selected</color>");
                return;
            }
            RaycastHit raycastHit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
            {
                if (raycastHit.collider.CompareTag("Floor"))
                {
                    //GameObject spawnedSofa = Instantiate(Utility.Instance.GetSpawnPrefab(), new Vector3(raycastHit.point.x, 0, raycastHit.point.z),
                    //                                     Quaternion.Euler(0, (/*((int)(spawnParentTransform.rotation.eulerAngles.y / 90) * 90 + */ Utility.Instance.GetSpawnPrefab().transform.rotation.eulerAngles.y), 0));
                    //spawnedSofa.transform.parent = spawnParentTransform;
                    //spawnedSofa.transform.position = new Vector3(spawnedSofa.transform.position.x, 0, spawnedSofa.transform.position.z);

                    GameObject spawnedSofa = Instantiate(Utility.Instance.GetSpawnPrefab(), new Vector3(raycastHit.point.x, 0, raycastHit.point.z),
                                                        Quaternion.Euler(0, ((int)(Camera.main.transform.parent.rotation.eulerAngles.y / 90) * 90 + Utility.Instance.GetSpawnPrefab().transform.rotation.eulerAngles.y), 0));
                    spawnedSofa.transform.parent = spawnParentTransform;
                    spawnedSofa.transform.position = new Vector3(spawnedSofa.transform.position.x, 0, spawnedSofa.transform.position.z);
                    spawnedSofa.GetComponent<SofaMaterial>().signatureSofaType = sofaType;
                    SignatureController.signatureSofaList.Add(spawnedSofa.GetComponent<SofaMaterial>());
                    SignatureController.signatureSofaCountMap[sofaType]++;

                    Debug.LogErrorFormat("<color=white>Signature Sofa Count: {0}</color>", SignatureController.signatureSofaList.Count);
                    foreach (KeyValuePair<SofaType, int> kvp in SignatureController.signatureSofaCountMap)
                        Debug.LogErrorFormat("<color=white>{0} : {1}</color>", kvp.Key, kvp.Value);
                    foreach (var item in SignatureController.signatureSofaList)
                        Debug.LogErrorFormat("<color=cyan>Sofa PID: {0}, Type: {1}</color>", item.ProductCode, item.signatureSofaType);

                }
                else
                    Debug.LogErrorFormat("<color=magenta>Dropped somewhere else.</color>");
            }
            else
                Debug.LogErrorFormat("<color=magenta>Doesn't Hit Shit</color>");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var canvas = FindInParents<Canvas>(gameObject);
            if (canvas == null)
                return;

            // We have clicked something that can be dragged.
            // What we want to do is create an icon for this.
            m_DraggingIcon = new GameObject("icon")
            {
                tag = "ICON"
            };

            m_DraggingIcon.transform.SetParent(canvas.transform, false);
            m_DraggingIcon.transform.SetAsLastSibling();

            image = m_DraggingIcon.AddComponent<Image>();

            Image.sprite = GetComponent<Image>().sprite;
            Image.preserveAspect = true;
            //Image.SetNativeSize();
            //Image.transform.localScale *= .02f;
            Image.color = new Color(1, 1, 1, .5f);

            if (dragOnSurfaces)
                m_DraggingPlane = transform as RectTransform;
            else
                m_DraggingPlane = canvas.transform as RectTransform;

            SetDraggedPosition(eventData);
        }

        public void OnDrag(PointerEventData data)
        {
            if (m_DraggingIcon != null)
                SetDraggedPosition(data);
        }

        private void SetDraggedPosition(PointerEventData data)
        {
            if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
                m_DraggingPlane = data.pointerEnter.transform as RectTransform;

            var rt = m_DraggingIcon.GetComponent<RectTransform>();
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
            {
                rt.position = globalMousePos + Vector3.up * 60;
                rt.rotation = m_DraggingPlane.rotation;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (m_DraggingIcon != null)
                Destroy(m_DraggingIcon);
        }

        static public T FindInParents<T>(GameObject go) where T : Component
        {
            if (go == null) return null;
            var comp = go.GetComponent<T>();

            if (comp != null)
                return comp;

            Transform t = go.transform.parent;
            while (t != null && comp == null)
            {
                comp = t.gameObject.GetComponent<T>();
                t = t.parent;
            }
            return comp;
        }

    }
}

using UnityEngine;

namespace Unity.MVVMToolkit
{
    public class SafeArea : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        private RectTransform rt;
        private Rect last = new(0, 0, 0, 0);

        void Awake()
        {
            rt = GetComponent<RectTransform>();

            if (canvas == null)
                canvas = transform.GetComponentInParent<Canvas>();

            ApplySafeArea(Screen.safeArea);
        }

        void Update()
        {
            Rect safeArea = Screen.safeArea;

            if (safeArea != last)
                ApplySafeArea(safeArea);
        }

        void ApplySafeArea(Rect safeArea)
        {
            last = safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= canvas.pixelRect.width;
            anchorMin.y /= canvas.pixelRect.height;
            anchorMax.x /= canvas.pixelRect.width;
            anchorMax.y /= canvas.pixelRect.height;

            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
        }
    }
}

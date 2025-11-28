using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollToTarget : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform internalContainer;
    [SerializeField] private float scrollDuration = 0.5f;

    private VerticalLayoutGroup verticalLayoutGroup;
    private float normalizedPositionY = 0;
    private int index = -1;
    private bool markedForScroll = true;
    private float targetNormalizedVerticalPos;
    private bool isScrolling = false;
    private float scrollTimer = 0f;

    public ScrollRect ScrollRect => this.scrollRect;

    private void Awake()
    {
        verticalLayoutGroup = scrollRect.GetComponentInChildren<VerticalLayoutGroup>();
    }

    public void Scroll(float normalizedPositionY, bool instant = false)
    {
        index = -1;
        this.normalizedPositionY = Mathf.Clamp01(normalizedPositionY);
        markedForScroll = true;

        StartCoroutine(ScrollToPositionCoroutine(instant));
    }

    public void Scroll(int index, bool instant = false)
    {
        // Check if elements are ready, to avoid index out of bounds
        if (internalContainer != null && internalContainer.childCount == 0 && index < internalContainer.childCount)
            return;

        if (scrollRect.content.childCount == 0 && index < scrollRect.content.childCount)
            return;

        markedForScroll = true;
        this.index = index;

        StartCoroutine(ScrollToItemCoroutine(instant));
    }

    private IEnumerator ScrollToPositionCoroutine(bool instant)
    {
        LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
        yield return new WaitForEndOfFrame();

        if (markedForScroll)
        {
            markedForScroll = false;

            if (instant)
            {
                scrollRect.verticalNormalizedPosition = normalizedPositionY;
            }
            else
            {
                isScrolling = true;
                scrollTimer = 0f;
            }
        }
    }

    public IEnumerator ScrollToItemCoroutine(bool instant)
    {
        LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
        yield return new WaitForEndOfFrame();

        if (markedForScroll)
        {
            markedForScroll = false;

            RectTransform content = scrollRect.content;
            RectTransform viewport = scrollRect.viewport;

            // Resolve target item
            RectTransform target;
            if (internalContainer != null)
                target = internalContainer.GetChild(index).GetComponent<RectTransform>();
            else
                target = content.GetChild(index).GetComponent<RectTransform>();

            if (target == null)
                yield break;

            // Child bounds in viewport local space
            Bounds targetBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, target);

            // Viewport rect in local space
            Rect vp = viewport.rect;

            // Bounds min.y and max.y relative to viewport
            float childMin = targetBounds.min.y;
            float childMax = targetBounds.max.y;

            float currentY = content.anchoredPosition.y;
            float maxScrollY = Mathf.Max(0f, content.rect.height - viewport.rect.height);
            float spacing = verticalLayoutGroup != null ? verticalLayoutGroup.spacing : 0f;

            float deltaY = 0f;
            // If child is above the viewport (too high, top clipped)
            if (childMax > vp.yMax)
                deltaY = childMax - vp.yMax + spacing;

            // If child is below the viewport (too low, bottom clipped)
            if (childMin < vp.yMin)
                deltaY = childMin - vp.yMin - spacing;

            // Map to normalized (1 = top, 0 = bottom)
            float targetY = Mathf.Clamp(currentY - deltaY, 0f, maxScrollY);
            float targetNorm = Mathf.Clamp01(Mathf.Approximately(maxScrollY, 0f) ? 1f : 1f - (targetY / maxScrollY));

            if (instant)
            {
                scrollRect.verticalNormalizedPosition = targetNorm;
            }
            else
            {
                targetNormalizedVerticalPos = targetNorm;
                isScrolling = true;
                scrollTimer = 0f;
            }
        }
    }

    private void Update()
    {
        if (isScrolling)
        {
            scrollTimer += Time.deltaTime;

            if (index != -1)
            {
                float t = Mathf.Clamp01(scrollTimer / scrollDuration);

                // SmoothStep for a nicer ease
                float v = Mathf.Lerp(scrollRect.verticalNormalizedPosition, targetNormalizedVerticalPos, Mathf.SmoothStep(0f, 1f, t));
                scrollRect.verticalNormalizedPosition = v;

                if (t >= 1f || Mathf.Abs(scrollRect.verticalNormalizedPosition - targetNormalizedVerticalPos) < 0.0001f)
                {
                    scrollRect.verticalNormalizedPosition = targetNormalizedVerticalPos;
                    isScrolling = false;
                }
            }
            else
            {
                // TODO: old, rework to the above smooth lerp and use duration

                scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, normalizedPositionY, scrollTimer / 0.5f);

                if (scrollTimer >= 0.5f)
                {
                    isScrolling = false;
                    scrollRect.verticalNormalizedPosition = normalizedPositionY;
                }
            }
        }
    }
}

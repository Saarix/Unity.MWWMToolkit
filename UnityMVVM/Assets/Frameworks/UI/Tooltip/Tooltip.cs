using DG.Tweening;
using MVVMToolkit.DataBinding;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Tooltip : MonoBehaviour, ILayoutGroup
{
    #region Enum

    public enum RelativePosition
    {
        Left,
        Right,
        Top,
        Bottom
    }

    #endregion

    [SerializeField] private string defaultLocalizationTable = "LocalizationTable";
    [SerializeField] protected LocalizeStringEvent localizeStringEvent;
    [SerializeField] private RectTransform arrowTra;
    [SerializeField] protected float scaleDuration = 0.3f;

    #region Fields

    private RectTransform parentTra;
    private RectTransform rectTra;
    private RectTransform targetTra;
    private RelativePosition position;
    private float xOffset, yOffset;
    private bool isShown, overridePosition;

    protected Tween scaleTween;

    #endregion

    #region Properties

    public RectTransform ParentTra => parentTra ?? (parentTra = transform.parent.GetComponent<RectTransform>());
    public RectTransform RectTra => rectTra ?? (rectTra = GetComponent<RectTransform>());
    public LocalizableString LocalizableString { get; private set; }
    public bool IsShown => isShown;

    #endregion

    private void Awake()
    {
        Hide();
    }

    public void Show(LocalizableString localizableString, RectTransform targetTra, RelativePosition position, float xOffset, float yOffset, bool overridePosition, bool instant = false)
    {
        isShown = true;
        gameObject.SetActive(true);

        LocalizableString = localizableString;
        this.targetTra = targetTra;
        this.position = position;
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.overridePosition = overridePosition;

        if (!string.IsNullOrEmpty(localizableString.Table))
        {
            localizeStringEvent.Set(localizableString.Key, localizableString.Table, localizableString.Tokens);
        }
        else
        {
            localizeStringEvent.Set(localizableString.Key, defaultLocalizationTable, localizableString.Tokens);
        }

        if (instant)
        {
            RectTra.localScale = Vector2.one;
        }
        else
        {
            if (scaleTween != null)
            {
                scaleTween.Kill();
                scaleTween = null;
            }
        }

        scaleTween = RectTra.DOScale(1f, scaleDuration).SetUpdate(true);
        scaleTween.OnComplete(() => scaleTween = null);
        scaleTween.Play();

        LayoutRebuilder.MarkLayoutForRebuild(RectTra);
    }

    public void Hide(bool instant = false)
    {
        if (instant)
        {
            isShown = false;
            RectTra.localScale = Vector2.zero;
            gameObject.SetActive(false);
        }
        else
        {
            if (scaleTween != null)
            {
                scaleTween.Kill();
                scaleTween = null;
            }

            scaleTween = RectTra.DOScale(0f, scaleDuration).SetUpdate(true);
            scaleTween.OnComplete(() =>
            {
                isShown = false;
                scaleTween = null;
                gameObject.SetActive(false);
            });
            scaleTween.Play();
        }
    }

    /* As can be seen from the above, the auto layout system evaluates widths first and then evaluates heights afterwards. 
    * Thus, calculated heights may depend on widths, but calculated widths can never depend on heights. */
    public void SetLayoutHorizontal() { }

    public void SetLayoutVertical()
    {
        // Reset pivot, jsut in case
        arrowTra.pivot = new Vector2(0.5f, 0f);

        if (targetTra == null)
            return;

        if (!overridePosition)
        {
            // Decide on relative position
            float bottomY = (targetTra.position.y / targetTra.lossyScale.y) - (targetTra.rect.height / 2) - RectTra.rect.height - arrowTra.rect.height;
            position = (bottomY > 0) ? RelativePosition.Bottom : RelativePosition.Top;
        }

        // Calculate horizontal offset
        float xPadding = 0;
        float padding = 10;
        float leftCorner = (targetTra.position.x / targetTra.lossyScale.x) - (RectTra.rect.width / 2) - padding;
        float rightCorner = (targetTra.position.x / targetTra.lossyScale.y) + (RectTra.rect.width / 2) + padding;

        if (position == RelativePosition.Right)
            leftCorner = (targetTra.position.x / targetTra.lossyScale.x) + (targetTra.rect.width / 2) + padding;

        if (position == RelativePosition.Left)
            rightCorner = (targetTra.position.x / targetTra.lossyScale.y) - (targetTra.rect.width / 2) - padding;

        if (leftCorner < 0)
            xPadding = -leftCorner;
        else if (rightCorner > ParentTra.rect.width)
            xPadding = ParentTra.rect.width - rightCorner;

        // Calculate vertical offet
        float yPadding = 0;
        float topCorner = (targetTra.position.y / targetTra.lossyScale.y) + (RectTra.rect.height / 2) + padding;
        float bottomCorner = (targetTra.position.y / targetTra.lossyScale.y) - (RectTra.rect.height / 2) - padding;

        if (bottomCorner < 0)
            yPadding = -bottomCorner;
        else if (topCorner > ParentTra.rect.height)
            yPadding = ParentTra.rect.height - topCorner;

        switch (position)
        {
            case RelativePosition.Bottom:
                RectTra.position = new Vector2(
                    ((targetTra.position.x / targetTra.lossyScale.x) + xPadding + xOffset) * targetTra.lossyScale.x,
                    ((targetTra.position.y / targetTra.lossyScale.y) - (targetTra.rect.height / 2) - (RectTra.rect.height / 2) - arrowTra.rect.height + yOffset) * targetTra.lossyScale.y);
                arrowTra.anchorMin = new Vector2(0.5f, 1f);
                arrowTra.anchorMax = new Vector2(0.5f, 1f);
                arrowTra.anchoredPosition = new Vector2(xOffset - xPadding, -4);
                arrowTra.localRotation = Quaternion.AngleAxis(0f, Vector3.forward);
                break;

            case RelativePosition.Top:
                RectTra.position = new Vector2(
                    ((targetTra.position.x / targetTra.lossyScale.x) + xPadding + xOffset) * targetTra.lossyScale.x,
                    ((targetTra.position.y / targetTra.lossyScale.y) + (targetTra.rect.height / 2) + (RectTra.rect.height / 2) + arrowTra.rect.height + yOffset) * targetTra.lossyScale.y);
                arrowTra.anchorMin = new Vector2(0.5f, 0f);
                arrowTra.anchorMax = new Vector2(0.5f, 0f);
                arrowTra.anchoredPosition = new Vector2(xOffset + xPadding, 4);
                arrowTra.localRotation = Quaternion.AngleAxis(180f, Vector3.forward);
                break;

            case RelativePosition.Left:
                RectTra.position = new Vector2(
                    ((targetTra.position.x / targetTra.lossyScale.x) - (targetTra.rect.width / 2) - (RectTra.rect.width / 2) - arrowTra.rect.height + xPadding + xOffset) * targetTra.lossyScale.x,
                    ((targetTra.position.y / targetTra.lossyScale.y) + yPadding + yOffset) * targetTra.lossyScale.y);
                arrowTra.anchorMin = new Vector2(1f, 0.5f);
                arrowTra.anchorMax = new Vector2(1f, 0.5f);
                arrowTra.anchoredPosition = new Vector2(-xOffset - xPadding, -yOffset - yPadding);
                arrowTra.localRotation = Quaternion.AngleAxis(-90f, Vector3.forward);
                break;

            case RelativePosition.Right:
                RectTra.position = new Vector2(
                    ((targetTra.position.x / targetTra.lossyScale.x) + (targetTra.rect.width / 2) + (RectTra.rect.width / 2) + arrowTra.rect.height + xPadding + xOffset) * targetTra.lossyScale.x,
                    ((targetTra.position.y / targetTra.lossyScale.y) + yPadding + yOffset) * targetTra.lossyScale.y);
                arrowTra.anchorMin = new Vector2(0f, 0.5f);
                arrowTra.anchorMax = new Vector2(0f, 0.5f);
                arrowTra.anchoredPosition = new Vector2(xOffset + xPadding, -yOffset - yPadding);
                arrowTra.localRotation = Quaternion.AngleAxis(90f, Vector3.forward);
                break;
        }
    }
}

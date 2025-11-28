using DG.Tweening;
using UnityEngine;
using System.Linq;
using UnityEngine.Localization.Components;
using MVVMToolkit.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class Toast : MonoBehaviour
{
    [SerializeField]
    private LocalizeStringEvent labelText;

    [Tooltip("How far toast will travel during its lifetime.")]
    [SerializeField]
    private float yTravelDistance;

    [Tooltip("Duration of toast in seconds.")]
    [SerializeField]
    private float duration;

    #region Fields

    private RectTransform rectTra;
    private CanvasGroup canvasGroup;
    private Sequence sequence;
    private bool isShown;

    #endregion

    #region Properties

    public bool IsShown => isShown;

    #endregion

    private void Awake()
    {
        rectTra = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        Hide();
    }

    public void Show(string localizationEntry, params (string key, string value)[] dynamicValues)
    {
        if (isShown)
            Hide();

        // Reset position
        rectTra.anchoredPosition = new Vector2(0, 0);

        isShown = true;
        sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1f, duration * 0.25f).SetUpdate(true));
        sequence.Insert(duration * 0.25f, rectTra.DOAnchorPosY(rectTra.anchoredPosition.y + yTravelDistance, duration * 0.5f).SetUpdate(true));
        sequence.Insert(duration * 0.75f, canvasGroup.DOFade(0f, duration * 0.25f).SetUpdate(true));
        sequence.OnComplete(Hide);
        sequence.SetUpdate(true);
        sequence.Play();

        labelText.Set(localizationEntry, dynamicValues);
    }

    public void Hide()
    {
        KillTween();
        canvasGroup.alpha = 0f;
        isShown = false;
    }

    private void KillTween()
    {
        if (sequence == null)
            return;

        sequence.Kill();
        sequence = null;
    }
}

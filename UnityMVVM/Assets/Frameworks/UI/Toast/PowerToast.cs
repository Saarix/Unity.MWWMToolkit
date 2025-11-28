using System.Linq;
using DG.Tweening;
using MVVMToolkit.DataBinding;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class PowerToast : MonoBehaviour
{
    [SerializeField] private RectTransform labelPowerRectTra;
    [SerializeField] private Image iconGain;
    [SerializeField] private Image iconLoss;
    [SerializeField] private AnimatedNumber labelPower;
    [SerializeField] private AnimatedNumber labelPowerGain;
    [SerializeField] private AnimatedNumber labelPowerLoss;

    [Tooltip("Duration of toast in seconds.")]
    [SerializeField] private float duration = 3f;

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
        labelPower.OnAnimationFinished += LabelPower_OnAnimationFinished;

        Hide();
    }

    public void Show(int power, int powerDiff)
    {
        if (isShown)
            Hide();

        bool isGain = powerDiff > 0;
        iconGain.gameObject.SetActive(isGain);
        labelPowerGain.gameObject.SetActive(isGain);
        iconLoss.gameObject.SetActive(!isGain);
        labelPowerLoss.gameObject.SetActive(!isGain);

        // Reset
        canvasGroup.alpha = 1f;
        rectTra.localScale = Vector3.zero;

        isShown = true;
        sequence = DOTween.Sequence();
        sequence.Append(rectTra.DOScale(1.15f, 0.5f));
        sequence.Insert(0.5f, rectTra.DOScale(1f, 0.075f).OnComplete(() => ToastVisibleCallback(isGain, power, powerDiff)));
        sequence.AppendInterval(duration - 0.5f - 0.075f - 0.3f);
        sequence.Append(rectTra.DOScale(0f, 0.3f));
        sequence.OnComplete(Hide);
        sequence.SetUpdate(true);
        sequence.Play();

        labelPower.Init(power - powerDiff);

        if (isGain)
            labelPowerGain.Init(powerDiff);
        else
            labelPowerLoss.Init(powerDiff);
    }

    private void LabelPower_OnAnimationFinished()
    {
        labelPowerRectTra.DOKill();

        Sequence resultSequence = DOTween.Sequence();
        resultSequence.Append(labelPowerRectTra.DOScale(1.2f, 0.35f));
        resultSequence.Append(labelPowerRectTra.DOScale(1f, 0.175f));
        resultSequence.SetUpdate(true);
        resultSequence.Play();
    }

    private void ToastVisibleCallback(bool isGain, int power, int powerDiff)
    {
        labelPower.SetNumber(power);

        if (isGain)
            labelPowerGain.SetNumber(0);
        else
            labelPowerLoss.SetNumber(0);
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

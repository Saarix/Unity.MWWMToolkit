using MVVMToolkit.DataBinding;
using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] private Tooltip tooltip;

    public static TooltipManager Instance { get; private set; }

    private bool released;
    private LocalizableString lastHidTooltipText = null;
    private int lastShownTooltip;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!released && !AnyTouch())
            released = true;

        if (released && AnyTouch())
        {
            lastHidTooltipText = null;

            if (tooltip.IsShown)
                HideTooltip();
        }
    }

    private bool AnyTouch()
    {
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool touchPressed = Touchscreen.current != null && Touchscreen.current.press.wasPressedThisFrame;
        return mousePressed || touchPressed;
    }

    public void HideTooltip(bool instant = false)
    {
        if (tooltip.IsShown)
            lastHidTooltipText = tooltip.LocalizableString;

        tooltip.Hide(instant);
    }

    public void ShowTooltip(LocalizableString localizableString, RectTransform rect, Tooltip.RelativePosition position, float xOffset, float yOffset, bool overridePosition)
    {
        if (lastHidTooltipText != null && lastShownTooltip != -1 && lastShownTooltip == rect.GetInstanceID())
        {
            lastShownTooltip = -1;
            return;
        }

        tooltip.Show(localizableString, rect, position, xOffset, yOffset, overridePosition);
        lastShownTooltip = rect.GetInstanceID();
        released = false;
    }

    private void OnScreenLoaded()
    {
        // This hook is needed so that tooltips do not remain open between screens
        HideTooltip();
    }
}

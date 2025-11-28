using UnityEngine;

public class ToastManager : MonoBehaviour
{
    [SerializeField] private Toast toast;
    [SerializeField] private PowerToast powerToast;

    public static ToastManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowToast(string localizationEntry, params (string key, string value)[] dynamicValues)
    {
        Hide();
        toast.Show(localizationEntry, dynamicValues);
    }

    public void ShowPowerToast(int power, int powerDiff)
    {
        Hide();
        powerToast.Show(power, powerDiff);
    }

    public void Hide()
    {
        toast.Hide();
        powerToast.Hide();
    }
}

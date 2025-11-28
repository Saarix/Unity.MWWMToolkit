using MVVMToolkit.UI;
using UnityEngine;

public interface IUiScreen
{
    ScreenType Type { get; }
    RectTransform RectTra { get; }
    bool ShowTopbar { get; }
    bool ShowNavbar { get; }

    void Close();
    void Open(object data);
    void OnWindowClosed(WindowType type, object data, bool screenRedirect);
}

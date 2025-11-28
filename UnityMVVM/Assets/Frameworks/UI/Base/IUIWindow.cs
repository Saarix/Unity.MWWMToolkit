
public interface IUiWindow
{
    void Open(object data, bool useAnimation);
    void Close(bool useAnimation);
    WindowType Type { get; }
    void OnWindowClosed(WindowType type, object data);
}

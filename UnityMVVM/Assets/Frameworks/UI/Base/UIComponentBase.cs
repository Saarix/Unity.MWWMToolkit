
namespace MVVMToolkit.UI
{
    public abstract class UiComponentBase : UiBase, IUiComponent
    {
        public virtual void Open(object data = null)
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}

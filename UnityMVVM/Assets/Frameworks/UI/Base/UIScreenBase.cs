using UnityEngine;
using UnityEngine.Events;

namespace MVVMToolkit.UI
{
    public abstract class UiScreenBase : UiBase, IUiScreen
    {
        [field: SerializeField] public bool ShowTopbar { get; private set; } = true;
        [field: SerializeField] public bool ShowNavbar { get; private set; } = true;
        public abstract ScreenType Type { get; }

        public UnityEvent ScreenOpen;
        public UnityEvent ScreenClose;

        protected object OpenData;

        public virtual void Close()
        {
            gameObject.SetActive(false);
            ScreenClose?.Invoke();
        }

        public virtual void Open(object openData)
        {
            OpenData = openData;
            gameObject.SetActive(true);

            ScreenOpen?.Invoke();
        }

        public virtual void OnWindowClosed(WindowType type, object data, bool screenRedirect)
        {

        }
    }
}

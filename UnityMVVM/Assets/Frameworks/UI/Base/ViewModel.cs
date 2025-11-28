using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    public abstract class ViewModel<TView> : ObservableScriptableObject where TView : UiView
    {
        protected TView View { get; private set; }
        protected UIManager Manager { get; private set; }

        public virtual void Init(TView view, UIManager manager)
        {
            View = view;
            Manager = manager;
        }

        public abstract void LoadData(object openData = null);
        public abstract void ClearData();
    }
}

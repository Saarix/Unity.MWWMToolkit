using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(DataContext))]
    public abstract class UiBase : MonoBehaviour
    {
        public bool IsInitialized { get; protected set; }
        public IDataContext DataContext => dataContext ??= GetComponent<IDataContext>();
        public RectTransform RectTra => rectTra ??= GetComponent<RectTransform>();

        protected UIManager Manager => manager ??= GetComponentInParent<UIManager>();

        private IDataContext dataContext;
        private RectTransform rectTra;
        private UIManager manager;

        protected abstract void Awake();
    }
}

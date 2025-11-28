using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(DataContext))]
    [RequireComponent(typeof(UiView))]
    public abstract class UiWindow<TView, TViewModel> : UiWindowBase
        where TView : UiView
        where TViewModel : ViewModel<TView>
    {
        [SerializeField]
        private TViewModel viewModel;

        protected TViewModel ViewModel => viewModel;
        protected TView View => view;

        private TView view;

        protected override void Awake()
        {
            view = GetComponent<TView>();

            viewModel.Init(View, Manager);

            // Assign data to data context
            DataContext.Data = viewModel;

            IsInitialized = true;
        }

        public override void Open(object openData, bool useAnimation)
        {
            base.Open(openData, useAnimation);

            if (!IsInitialized)
                Debug.LogError($"Trying to call Open while object is not yet initialized. WindowType={Type}, openData={openData}");

            viewModel.LoadData(openData);
        }

        public override void Close(bool useAnimation)
        {
            base.Close(useAnimation);

            viewModel.ClearData();
        }
    }
}

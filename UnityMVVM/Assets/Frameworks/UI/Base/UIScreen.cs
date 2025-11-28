using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(DataContext))]
    [RequireComponent(typeof(UiView))]
    public abstract class UiScreen<TView, TViewModel> : UiScreenBase
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

        public override void Open(object openData)
        {
            base.Open(openData);

            if (!IsInitialized)
                Debug.LogError($"Trying to call Open while object is not yet initialized. ScreenType={Type}, openData={openData}");

            viewModel.LoadData(openData);
        }

        public override void Close()
        {
            base.Close();

            viewModel.ClearData();
        }
    }
}

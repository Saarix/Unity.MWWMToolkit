using MVVMToolkit.UI;
using UnityEngine;

[RequireComponent(typeof(UiView))]
public abstract class UiComponent<TView, TViewModel> : UiComponentBase
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
        Initialize();
    }

    public override void Open(object openData = null)
    {
        base.Open(openData);

        Initialize();

        viewModel.LoadData(openData);

        if (Manager != null)
            Manager.InvokeComponentOpened(GetType().Name);
    }

    public override void Close()
    {
        base.Close();

        viewModel.ClearData();
    }

    private void Initialize()
    {
        if (IsInitialized)
            return;

        view = GetComponent<TView>();

        viewModel.Init(View, Manager, Manager.UserActionHandler);

        // Assign data to data context
        DataContext.Data = viewModel;

        IsInitialized = true;
    }
}

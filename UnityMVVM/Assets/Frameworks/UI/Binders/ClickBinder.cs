using MVVMToolkit.DataBinding;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVVMToolkit.UI
{
    public class ClickBinder : Binder, IPointerClickHandler
    {
        [SerializeField]
        private AudioClip clickSfx;

        [Header("Confirmation")]
        [SerializeField]
        private bool requiresConfirmation = false;

        [SerializeField]
        private ConfirmationStyle confirmationStyle = ConfirmationStyle.ClassicYesNo;

        [SerializeField]
        private string confirmationTitle;

        [SerializeField]
        private string confirmationMessage;

        #region Fields

        private IRelayCommand command;

        #endregion

        #region Binder implementation

        public override void InitValue(object value)
        {
            base.InitValue(value);
            UpdateValue(value);
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (value is IRelayCommand command)
            {
                this.command = command;
            }
            else
            {
                Debug.LogError($"[{GetType().Name}] Given value is not a IRelayCommand. Path: {Path}. gameObject: {gameObject.name}, value is {value}");
            }
        }

        #endregion

        /// <summary>
        /// Can be used to invoke Click from the outside
        /// IE Button component can invoke some ClickBinder
        /// </summary>
        public void Click()
        {
            OnPointerClick(null);
        }

        public async void OnPointerClick(PointerEventData eventData)
        {
            UIManager.Instance.OnClick(eventData);

            if (requiresConfirmation)
            {
                DialogResult result = await DialogManager.Instance.ShowDialog(confirmationStyle, confirmationTitle, confirmationMessage, options: DialogOption.Yes | DialogOption.No);

                switch (result)
                {
                    case DialogResult.Yes:
                        ExecuteCommand(eventData);
                        break;
                }
            }
            else
            {
                ExecuteCommand(eventData);
            }
        }

        private void ExecuteCommand(object parameter)
        {
            if (command.CanExecute(parameter))
            {
                command.Execute(parameter); // Note: can pass some data

                if (clickSfx != null)
                    UIManager.Instance.PlaySfx(clickSfx);
            }
        }
    }
}

using MVVMToolkit.DataBinding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(Button))]
    public class PointerEventsBinder : Binder, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private IRelayCommand command;

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
                Debug.LogError($"[{GetType().Name}] Given value is not a IRelayCommand. value is {value}");
        }

        #endregion

        public void OnPointerClick(PointerEventData eventData)
        {
            ExecuteCommand(new ExtendedPointerEventData(eventData, PointerEventAction.Click));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ExecuteCommand(new ExtendedPointerEventData(eventData, PointerEventAction.Down));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ExecuteCommand(new ExtendedPointerEventData(eventData, PointerEventAction.Up));
        }

        private void ExecuteCommand(ExtendedPointerEventData eventData)
        {
            if (command.CanExecute(eventData))
                command.Execute(eventData);
        }
    }
}

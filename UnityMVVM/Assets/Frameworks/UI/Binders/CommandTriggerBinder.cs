using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public class CommandTriggerBinder : Binder
    {
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
                ExecuteCommand(command);
            }
            else
            {
                Debug.LogError($"[{GetType().Name}] Given value is not a IRelayCommand. gameObject: {gameObject.name}, value is {value}");
            }
        }

        #endregion

        private void ExecuteCommand(IRelayCommand command)
        {
            if (command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }
}

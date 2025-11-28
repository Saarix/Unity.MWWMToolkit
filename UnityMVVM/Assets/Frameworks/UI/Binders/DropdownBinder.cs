using System;
using System.Collections.Generic;
using System.Linq;
using MVVMToolkit.DataBinding;
using TMPro;
using UnityEngine;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class DropdownBinder : MultiBinder
    {
        [SerializeField] private TMP_Dropdown dropdown;

        protected override void Init()
        {
            base.Init();

            dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void Dispose()
        {
            dropdown.onValueChanged.RemoveListener(OnValueChanged);

            base.Dispose();
        }

        public override void InitValue(string sourcePath, int sourceIndex, object value)
        {
            base.InitValue(sourcePath, sourceIndex, value);
            UpdateValue(sourcePath, sourceIndex, value);
        }

        public override void UpdateValue(string sourcePath, int sourceIndex, object value)
        {
            base.UpdateValue(sourcePath, sourceIndex, value);

            if (value != null)
            {
                switch (sourceIndex)
                {
                    case 0:
                    {
                        dropdown.ClearOptions();

                        if (value is List<string> valList)
                        {
                            dropdown.AddOptions(valList);
                        }
                        else if (value is ObservableList<string> obsrList)
                        {
                            dropdown.AddOptions(obsrList.ToList());
                        }
                        else if (value is string[] valArray)
                        {
                            dropdown.AddOptions(valArray.ToList());
                        }
                        else if (value is Enum valEnum)
                        {
                            Array arr = Enum.GetValues(valEnum.GetType());
                            dropdown.AddOptions(arr.Cast<Enum>().Select(x => x.ToString()).ToList());
                        }
                        else
                        {
                            Debug.LogError($"[{GetType().Name}] Trying to use unsupported value type: {value.GetType()}");
                        }
                    }

                    break;
                    case 1:
                    {
                        if (dropdown.options.Count > 0)
                        {
                            if (dropdown.options.Any(x => x.text == value.ToString()))
                            {
                                int index = dropdown.options.IndexOf(dropdown.options.First(x => x.text == value.ToString()));
                                dropdown.value = index;
                            }
                        }

                        break;
                    }
                }
            }
        }

        private void OnValueChanged(int index)
        {
            if (Bindings.Count == 2)
            {
                // Second binding is used as output for selected value
                IBinding outputBinding = Bindings[1];
                if (outputBinding.IsBound)
                    outputBinding.UpdateProperty(dropdown.options[index].text);
            }
        }
    }
}

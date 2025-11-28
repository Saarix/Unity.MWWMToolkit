using System;
using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    [Serializable]
    public class TabSection
    {
        public GameObject[] DefaultItems;
        public GameObject[] SelectedItems;
    }

    public class TabSectionBinder : Binder
    {
        [Tooltip("Defines what game bojects get enabled when tab section gets selected")]
        [SerializeField]
        private TabSection[] tabSections;

        public override void InitValue(object value)
        {
            base.InitValue(value);
            UpdateValue(value);
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (value != null)
            {
                if (int.TryParse(value.ToString(), out int val))
                {
                    for (int i = 0; i < tabSections.Length; i++)
                    {
                        GameObject[] defaultItems = tabSections[i].DefaultItems;
                        GameObject[] selectedItems = tabSections[i].SelectedItems;
                        if (i == val)
                        {
                            foreach (GameObject item in defaultItems)
                            {
                                if (item != null)
                                    item.SetActive(false);
                            }

                            foreach (GameObject item in selectedItems)
                            {
                                if (item != null)
                                    item.SetActive(true);
                            }
                        }
                        else
                        {
                            foreach (GameObject item in defaultItems)
                            {
                                if (item != null)
                                    item.SetActive(true);
                            }

                            foreach (GameObject item in selectedItems)
                            {
                                if (item != null)
                                    item.SetActive(false);
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError($"TabSectionBinder - invalid value type: '{value?.GetType()}' ({value})!");
                }
            }
        }
    }
}

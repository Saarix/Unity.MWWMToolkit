using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public class DataContext : MonoBehaviour, IDataContext
    {
        private object data;
        private List<IBinding> bindingList = new();

        #region IDataContext implementation

        public object Data
        {
            get => data;
            set
            {
                data = value;
                RefreshBindings();
            }
        }

        private void RefreshBindings()
        {
            // Refresh binding connections
            for (int i = bindingList.Count - 1; i >= 0; i--)
            {
                if (i < bindingList.Count)
                {
                    IBinding binding = bindingList[i];
                    binding.Bind(this);
                }
            }
        }

        public void AddBindings(List<IBinding> bindings)
        {
            bindings.ForEach(x => AddBinding(x));
        }

        public void AddBinding(IBinding binding)
        {
            if (binding == null)
                return;

            if (bindingList.Contains(binding))
            {
                Debug.LogWarning($"Binding {binding} is already added.");
                return;
            }

            bindingList.Add(binding);

            if (data != null)
                binding.Bind(this);
        }

        public void RemoveBindings(List<IBinding> bindings)
        {
            bindings.ForEach(x => RemoveBinding(x));
        }

        public void RemoveBinding(IBinding binding)
        {
            if (binding == null)
                return;

            if (!bindingList.Contains(binding))
            {
                Debug.LogWarning($"Unknown binding {binding}.");
                return;
            }

            bindingList.Remove(binding);

            if (data != null)
                binding.Unbind();
        }

        public List<IBinding> GetBindings(string path)
        {
            return bindingList.FindAll(x => x.Path == path);
        }

        public List<IBinding> GetBindings()
        {
            return bindingList;
        }

        public IBinding GetBinding(string path)
        {
            return bindingList.FirstOrDefault(x => x.Path == path);
        }

        public IBinding GetBinding(int index)
        {
            return bindingList[index];
        }

        #endregion IDataContext implementation
    }
}

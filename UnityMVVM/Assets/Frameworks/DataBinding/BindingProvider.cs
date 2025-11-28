using System.Collections.Generic;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public static class BindingProvider
    {
        public static IDataContext FindDataContextUpwards(Transform current)
        {
            IDataContext result = null;

            while (current != null)
            {
                result = current.GetComponent<IDataContext>();
                if (result != null)
                    break;

                current = current.parent;
            }

            return result;
        }

        public static IDataContext FindDataContextDownwards(Transform current)
        {
            return current.GetComponentInChildren<IDataContext>();
        }

        public static void AddBinding(IBinding binding, IBinder binder, Transform transform, out IDataContext dataContext)
        {
            dataContext = FindDataContextUpwards(binder.IncludeSelf ? transform : transform.parent);

            if (dataContext == null)
                Debug.LogError($"Failed to find DataContext. Transform={transform.name}, Path={binder.Path}, Fallback={binder.FallbackValue}");
            else
                dataContext.AddBinding(binding);
        }

        public static void AddBindings(List<IBinding> bindings, IMultiBinder binder, Transform transform, out IDataContext dataContext)
        {
            dataContext = FindDataContextUpwards(binder.IncludeSelf ? transform : transform.parent);

            if (dataContext == null)
            {
                Debug.LogError($"Failed to find DataContext. Transform={transform.name}, Multibinder");
            }
            else
            {
                foreach (IBinding binding in bindings)
                    dataContext.AddBinding(binding);
            }
        }

        public static void RemoveBinding(IBinding binding, IDataContext dataContext)
        {
            if (dataContext != null)
                dataContext.RemoveBinding(binding);
        }

        public static void RemoveBindings(List<IBinding> bindings, IDataContext dataContext)
        {
            if (dataContext != null)
            {
                foreach (IBinding binding in bindings)
                    dataContext.RemoveBinding(binding);
            }
        }
    }
}

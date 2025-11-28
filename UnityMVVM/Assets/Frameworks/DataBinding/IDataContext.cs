using System.Collections.Generic;

namespace MVVMToolkit.DataBinding
{
    public interface IDataContext
    {
        object Data { get; set; }
        void AddBinding(IBinding binding);
        void AddBindings(List<IBinding> bindings);
        void RemoveBinding(IBinding binding);
        void RemoveBindings(List<IBinding> bindings);
        List<IBinding> GetBindings(string path);
        List<IBinding> GetBindings();
        IBinding GetBinding(string path);
        IBinding GetBinding(int index);
    }
}

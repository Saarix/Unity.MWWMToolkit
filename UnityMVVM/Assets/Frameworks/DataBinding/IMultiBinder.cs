using System.Collections.Generic;

namespace MVVMToolkit.DataBinding
{
    public interface IMultiBinder
    {
        bool IsDestroyed { get; }
        bool IncludeSelf { get; }
        List<IBinding> Bindings { get; }
        MultiBinder.BinderConfig[] BinderConfigs { get; }
        IDataContext DataContext { get; }

        void InitValue(string sourcePath, int sourceIndex, object value);
        void UpdateValue(string sourcePath, int sourceIndex, object value);
    }
}

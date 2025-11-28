using System.Collections.Generic;

namespace MVVMToolkit.DataBinding
{
    public interface IBinder
    {
        object Value { get; }
        bool IncludeSelf { get; }
        string Path { get; }
        string PropertyName { get; }
        object FallbackValue { get; }
        IList<ValueConverter> Converters { get; }
        IBinding Binding { get; }
        IDataContext DataContext { get; }
        UnityEngine.Object Object { get; }

        void InitValue(object value);
        void UpdateValue(object value);
    }
}

using System.Collections.Generic;

namespace MVVMToolkit.DataBinding
{
    public interface IBinding
    {
        string Path { get; }
        string PropertyName { get; }
        bool IsBound { get; }
        IDataContext Source { get; }
        IBinder Target { get; }
        BindingMode Mode { get; }
        UpdateSourceTrigger Trigger { get; }

        void Bind(IDataContext source);
        void Unbind();
        void UpdateProperty(object value);

#if UNITY_EDITOR
        IList<object> ValuesHistory { get; }
#endif
    }
}

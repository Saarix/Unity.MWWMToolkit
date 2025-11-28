using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public abstract class StringBinder<T> : Binder<T>, IStringBinder
        where T : Component
    {
        #region Inspector fields

        [SerializeField]
        protected string stringFormat;

        #endregion Inspector fields

        #region IBinder implementation

        public string StringFormat => stringFormat;

        #endregion IBinder implementation

        protected override IBinding CreateBinding()
        {
            return new Binding(Path, this, fallbackValue, Converters, stringFormat);
        }
    }
}

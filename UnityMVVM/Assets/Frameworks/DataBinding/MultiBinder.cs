using System;
using System.Collections.Generic;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public abstract class MultiBinder : MonoBehaviour, IMultiBinder
    {
        #region Nested classes

        [Serializable]
        public class BinderConfig : IBinder
        {
            #region Inspector fields

            [SerializeField]
            private string path;

            [SerializeField]
            private bool includeSelf = false;

            [SerializeField]
            private string fallbackValue;

            #endregion Inspector fields

            private string propertyName;
            private IBinding binding;
            private IDataContext dataContext;

            public object Value { get; private set; }
            public MultiBinder Owner { get; set; }

            #region IBinder implementation

            public bool IncludeSelf => includeSelf;
            public string Path => path;
            public string PropertyName => propertyName;
            public object FallbackValue => fallbackValue;
            public IList<ValueConverter> Converters => null;
            public IBinding Binding => binding;
            public IDataContext DataContext => dataContext;
            public UnityEngine.Object Object => Owner;

            #endregion IBinder implementation

            // Used for tests
            public void SetPath(string path)
            {
                this.path = path;
            }

            public void SetIncludeSelf(bool includeSelf)
            {
                this.includeSelf = includeSelf;
            }

            public void SetBinding(IBinding binding)
            {
                this.binding = binding;
            }

            public void InitValue(object value)
            {
                UpdateValue(value);
            }

            public void UpdateValue(object value)
            {
                Value = value;
                Owner.UpdateValue(Path, Array.IndexOf(Owner.BinderConfigs, this), value);
            }
        }

        #endregion

        #region Inspector fields

        /// <summary>
        /// Indicates if during lookup for DataContext we should include
        /// self transform as a possible source
        /// </summary>
        [SerializeField]
        [Tooltip("Indicates if during lookup for DataContext we should include self transform as a possible source")]
        private bool includeSelf = false;

        [SerializeField]
        protected BinderConfig[] binderConfigs;

        #endregion Inspector fields

        protected object[] values;
        protected List<IBinding> bindings;
        protected IDataContext dataContext;
        protected bool isDestroyed;

        #region IMultiBinder implementation

        public bool IsDestroyed => isDestroyed;
        public bool IncludeSelf => includeSelf;
        public List<IBinding> Bindings => bindings;
        public BinderConfig[] BinderConfigs => binderConfigs;
        public IDataContext DataContext => dataContext;

        public virtual void InitValue(string sourcePath, int sourceIndex, object value)
        {
            UpdateValue(sourcePath, sourceIndex, value);
        }

        public virtual void UpdateValue(string sourcePath, int sourceIndex, object value)
        {
            for (int i = 0; i < binderConfigs.Length; i++)
                values[i] = binderConfigs[i].Value;
        }

        #endregion IMultiBinder implementation

        protected virtual void Awake()
        {
            values = new object[binderConfigs.Length];

        }

        protected virtual void OnEnable()
        {
            Init();
        }

        protected virtual void OnDisable()
        {
            Dispose();
        }

        protected virtual void OnDestroy()
        {
            isDestroyed = true;
        }

        protected virtual void Init()
        {
            bindings = CreateBindings();

            BindingProvider.AddBindings(bindings, this, transform, out dataContext);
        }

        protected virtual void Dispose()
        {
            BindingProvider.RemoveBindings(bindings, dataContext);

            bindings = null;
            dataContext = null;
        }

        protected virtual List<IBinding> CreateBindings()
        {
            List<IBinding> tempList = new();

            foreach (BinderConfig config in binderConfigs)
            {
                config.Owner = this;
                Binding binding = new(config.Path, config, fallbackValue: config.FallbackValue);
                config.SetBinding(binding);
                tempList.Add(binding);
            }

            return tempList;
        }
    }
}

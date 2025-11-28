using System.Collections.Generic;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public abstract class Binder : MonoBehaviour, IBinder
    {
        #region Inspector fields

        /// <summary>
        /// Indicates if during lookup for DataContext we should include
        /// self transform as a possible source
        /// </summary>
        [SerializeField]
        [Tooltip("Indicates if during lookup for DataContext we should include self transform as a possible source")]
        protected bool includeSelf = false;

        [SerializeField]
        protected string path;

        [SerializeField]
        protected string fallbackValue;

        [SerializeField]
        protected BindingMode mode;

        [SerializeField]
        protected ValueConverter[] converters;

        #endregion Inspector fields

        protected string propertyName;
        protected object value;
        protected IBinding binding;
        protected IDataContext dataContext;
        protected bool isDestroyed;
        protected bool isQuitting;

        #region Properties

        public bool IsDestroyed => isDestroyed;

        #endregion Properties

        #region IBinder implementation

        public object Value => value;
        public bool IncludeSelf => includeSelf;
        public string Path => path;
        public string PropertyName => propertyName;
        public object FallbackValue => fallbackValue;
        public IList<ValueConverter> Converters
        {
            get
            {
                foreach (ValueConverter converter in converters)
                {
                    if (converter == null)
                        Debug.LogError($"Binder {name} has null converter");
                }

                return converters;
            }
        }
        public IBinding Binding => binding;
        public IDataContext DataContext => dataContext;
        public BindingMode Mode => mode;
        public Object Object => this;

        public virtual void InitValue(object value)
        {
            this.value = value;
        }

        public virtual void UpdateValue(object value)
        {
            this.value = value;
        }

        #endregion IBinder implementation

        protected virtual void Awake() { }

        private void Start()
        {
            StartSetFallbackValue();
        }

        protected virtual void OnEnable()
        {
            Init();
        }

        protected virtual void OnApplicationQuit()
        {
            isQuitting = true;
        }

        protected virtual void OnDisable()
        {
            if (isQuitting)
                return;

            Dispose();
        }

        protected virtual void OnDestroy()
        {
            isDestroyed = true;
        }

        protected virtual bool StartSetFallbackValue()
        {
            // If it is already set from Editor, do not execute
            if (!string.IsNullOrEmpty(fallbackValue))
                return false;

            return true;
        }

        protected virtual void Init()
        {
            Transform current = transform;
            string objectPath = "";
            while (current != null)
            {
                objectPath = $"{current}/{objectPath}";
                current = current.parent;
            }

            propertyName = BindingUtility.GetPropertyName(path);
            binding = CreateBinding();

            BindingProvider.AddBinding(binding, this, transform, out dataContext);
        }

        protected virtual void Dispose()
        {
            BindingProvider.RemoveBinding(binding, dataContext);

            binding = null;
            dataContext = null;
        }

        protected virtual IBinding CreateBinding()
        {
            return new Binding(Path, this, fallbackValue, Converters, mode: mode);
        }
    }
}

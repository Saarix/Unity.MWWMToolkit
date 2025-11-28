using System.Collections.Generic;
using System.Linq;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.DataBinding.Tests
{
    public class TestBinder : Binder
    {
        private string stringFormat;

        protected override void Awake() { }

        protected override void Init()
        {
            binding = CreateBinding();

            dataContext?.AddBinding(binding);
        }

        protected override IBinding CreateBinding()
        {
            return new Binding(Path, this, fallbackValue, converters, stringFormat);
        }

        public void FormBindingConnection()
        {
            Init();
        }

        public void SetPath(string path)
        {
            this.path = path;
        }

        public void SetDataContext(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void SetConverters(IList<ValueConverter> converters)
        {
            this.converters = converters.ToArray();
        }

        public void SetFallbackValue(string fallbackValue)
        {
            this.fallbackValue = fallbackValue;
        }

        public void SetStringFormat(string stringFormat)
        {
            this.stringFormat = stringFormat;
        }
    }
}

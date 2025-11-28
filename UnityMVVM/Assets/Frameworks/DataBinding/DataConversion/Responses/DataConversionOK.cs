namespace MVVMToolkit.DataBinding.Responses
{
    public readonly struct DataConversionOK : IDataConversionOK
    {
        private readonly object data;

        public object Data => this.data;

        public DataConversionOK(object data)
        {
            this.data = data;
        }
    }
}
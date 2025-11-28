namespace MVVMToolkit.DataBinding.Responses
{
    public readonly struct DataConversionMissing : IDataConversionError
    {
        private readonly object originalValue;
        public string Message => $"Value for {this.originalValue} not found!";

        public DataConversionMissing(object originalValue)
        {
            this.originalValue = originalValue;
        }
    }
}
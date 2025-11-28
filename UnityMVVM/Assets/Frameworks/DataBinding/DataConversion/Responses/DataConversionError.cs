namespace MVVMToolkit.DataBinding.Responses
{
    public readonly struct DataConversionError : IDataConversionError
    {
        private readonly string message;
        public string Message => this.message;

        public DataConversionError(string message)
        {
            this.message = message;
        }
    }
}
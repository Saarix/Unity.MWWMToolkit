
namespace MVVMToolkit.DataBinding.Responses
{
    public interface IDataConversionResponse : IResponse { }
    public interface IDataConversionOK : IResponseOK<object>, IDataConversionResponse { }
    public interface IDataConversionError : IResponseError, IDataConversionResponse { }
}
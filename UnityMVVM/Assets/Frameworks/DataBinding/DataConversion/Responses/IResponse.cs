using UnityEngine;

namespace MVVMToolkit.DataBinding.Responses
{
    public interface IResponse
    {
    }
    public interface IResponseOK<T> : IResponse
    {
        T Data { get; }
    }

    public interface IResponseError : IResponse
    {
        string Message { get; }
    }

    public static class ResponseExtensions
    {
        public static void Log(this IResponseError error)
        {
            Debug.LogError(error.Message);
        }
    }
}
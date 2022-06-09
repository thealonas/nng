using VkNet.Model;

namespace nng.Exceptions;

public class VkFrameworkMethodException : Exception
{
    public VkFrameworkMethodException(string methodName, string content) : base(
        $"Ошибка в методе {methodName}: {content}")
    {
    }

    public VkFrameworkMethodException(string methodName, Exception vkException)
        : base($"Ошибка в методе {methodName}\n{vkException.GetType()}: {vkException.Message}")
    {
    }

    public VkFrameworkMethodException(string methodName, VkError vkException)
        : base($"Ошибка в методе {methodName}\n{vkException.Method}: {vkException.ErrorMessage}")
    {
    }
}

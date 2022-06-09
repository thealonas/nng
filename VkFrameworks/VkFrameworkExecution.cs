using nng.Exceptions;
using VkNet.Exception;

namespace nng.VkFrameworks;

/// <summary>
///     Класс для безопасного выполнения операций с VK API
/// </summary>
public static class VkFrameworkExecution
{
    public static int WaitTime { get; set; }

    /// <summary>
    ///     Выполнить действие с возвращаемым значением
    /// </summary>
    /// <param name="func"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="VkFrameworkMethodException"></exception>
    public static T ExecuteWithReturn<T>(Func<T> func)
    {
        if (func == null) throw new NullReferenceException(nameof(func));
        try
        {
            var obj = func.Invoke();
            return obj;
        }
        catch (TooManyRequestsException)
        {
            Task.Delay(WaitTime).Wait();
            return ExecuteWithReturn(func);
        }
        catch (CaptchaNeededException)
        {
            Task.Delay(WaitTime).Wait();
            VkFramework.InvokeOnCaptchaWait(nameof(func), WaitTime);
            return ExecuteWithReturn(func);
        }
        catch (Exception e)
        {
            throw new VkFrameworkMethodException(nameof(func), e);
        }
    }

    /// <summary>
    ///     Выполнить действие без возвращаемого значения
    /// </summary>
    /// <param name="action">Действие</param>
    /// <exception cref="VkFrameworkMethodException">Ошибка</exception>
    public static void Execute(Action action)
    {
        if (action == null) throw new NullReferenceException(nameof(action));
        try
        {
            action.Invoke();
        }
        catch (TooManyRequestsException)
        {
            Task.Delay(WaitTime).Wait();
            Execute(action);
        }
        catch (CaptchaNeededException)
        {
            Task.Delay(WaitTime).Wait();
            VkFramework.InvokeOnCaptchaWait(nameof(action), WaitTime);
            Execute(action);
        }
        catch (Exception e)
        {
            throw new VkFrameworkMethodException(nameof(action), e);
        }
    }
}

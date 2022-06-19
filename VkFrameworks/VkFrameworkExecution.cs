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
    /// <param name="func">Метод </param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Обобщенный объект, который должен вернуть передаваемый обобщенный метод</returns>
    /// <exception cref="VkApiException">Ошибка при выполнении</exception>
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
    }

    /// <summary>
    ///     Выполнить действие с возвращаемым значением
    /// </summary>
    /// <param name="func">Метод</param>
    /// <param name="captchaWait">Таймаут при капче</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Обобщенный объект, который должен вернуть передаваемый обобщенный метод</returns>
    /// <exception cref="CaptchaNeededException">Каптча</exception>
    /// <exception cref="VkApiException">Ошибка при выполнении</exception>
    public static T ExecuteWithReturn<T>(Func<T> func, bool captchaWait)
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
            if (!captchaWait) throw;
            Task.Delay(WaitTime).Wait();
            VkFramework.InvokeOnCaptchaWait(nameof(func), WaitTime);
            return ExecuteWithReturn(func);
        }
    }

    /// <summary>
    ///     Выполнить действие без возвращаемого значения
    /// </summary>
    /// <param name="action">Действие</param>
    /// <exception cref="VkApiException">Ошибка</exception>
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
    }

    /// <summary>
    ///     Выполнить действие без возвращаемого значения
    /// </summary>
    /// <param name="action">Действие</param>
    /// <param name="captchaWait">Таймаут при капче</param>
    /// <exception cref="CaptchaNeededException">Каптча</exception>
    /// <exception cref="VkApiException">Ошибка</exception>
    public static void Execute(Action action, bool captchaWait)
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
            if (!captchaWait) throw;
            Task.Delay(WaitTime).Wait();
            VkFramework.InvokeOnCaptchaWait(nameof(action), WaitTime);
            Execute(action);
        }
    }
}

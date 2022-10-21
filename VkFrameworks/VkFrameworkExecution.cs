using VkNet.Exception;

namespace nng.VkFrameworks;

/// <summary>
///     Класс для безопасного выполнения операций с VK API
/// </summary>
public static class VkFrameworkExecution
{
    public static TimeSpan WaitTime { get; set; }

    /// <summary>
    ///     Выполнить действие с возвращаемым значением
    /// </summary>
    /// <param name="func">Метод </param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Обобщенный объект, который должен вернуть передаваемый обобщенный метод</returns>
    /// <exception cref="VkApiException">Ошибка при выполнении</exception>
    public static T ExecuteWithReturn<T>(Func<T> func)
    {
        return BaseExecuteWithReturn(func, true);
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
        return BaseExecuteWithReturn(func, captchaWait);
    }

    /// <summary>
    ///     Выполнить действие без возвращаемого значения
    /// </summary>
    /// <param name="action">Действие</param>
    /// <exception cref="VkApiException">Ошибка</exception>
    public static void Execute(Action action)
    {
        BaseExecute(action, true);
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
        BaseExecute(action, captchaWait);
    }

    private static T BaseExecuteWithReturn<T>(Func<T> action, bool captchaWait)
    {
        if (action == null) throw new NullReferenceException(nameof(action));
        try
        {
            var obj = action.Invoke();
            return obj;
        }
        catch (TooManyRequestsException)
        {
            Task.Delay(WaitTime).GetAwaiter().GetResult();
            return BaseExecuteWithReturn(action, captchaWait);
        }
        catch (CaptchaNeededException)
        {
            if (!captchaWait) throw;
            Task.Delay(WaitTime).GetAwaiter().GetResult();
            VkFramework.InvokeOnCaptchaWait(nameof(action), WaitTime);
            return BaseExecuteWithReturn(action, captchaWait);
        }
    }

    private static void BaseExecute(Action action, bool captchaWait)
    {
        if (action == null) throw new NullReferenceException(nameof(action));
        try
        {
            action.Invoke();
        }
        catch (TooManyRequestsException)
        {
            Task.Delay(WaitTime).GetAwaiter().GetResult();
            BaseExecute(action, captchaWait);
        }
        catch (CaptchaNeededException)
        {
            if (!captchaWait) throw;
            Task.Delay(WaitTime).Wait();
            VkFramework.InvokeOnCaptchaWait(nameof(action), WaitTime);
            BaseExecute(action, captchaWait);
        }
    }
}

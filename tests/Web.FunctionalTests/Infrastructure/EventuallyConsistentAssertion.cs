namespace RoboForge.Web.FunctionalTests.Infrastructure;

/// <summary>
/// Waits for an effect that arrives through the outbox (polled every 0.5 s), failing after a timeout
/// instead of sleeping for a fixed time.
/// </summary>
internal static class EventuallyConsistentAssertion
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(200);

    public static async Task<T> WaitUntilAsync<T>(Func<Task<T>> read, Func<T, bool> isExpected, string expectation)
    {
        var deadline = DateTime.UtcNow + Timeout;

        while (true)
        {
            var value = await read();

            if (isExpected(value))
            {
                return value;
            }

            if (DateTime.UtcNow > deadline)
            {
                throw new TimeoutException($"Still waiting after {Timeout.TotalSeconds} s for: {expectation}. Last value: {value}");
            }

            await Task.Delay(RetryDelay, TestContext.Current.CancellationToken);
        }
    }
}

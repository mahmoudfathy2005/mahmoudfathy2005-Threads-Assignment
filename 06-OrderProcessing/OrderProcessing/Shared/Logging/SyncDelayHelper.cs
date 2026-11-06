namespace OrderProcessing.Shared.Logging;

public static class SyncDelayHelper
{
    public static void Wait(
        string source,
        string step,
        int milliseconds)
    {
        Log.Info(source, $"{step}...");

        Thread.Sleep(milliseconds);

        Log.Success(source, $"{step} completed");
    }
}

namespace OrderProcessing.Shared.Logging;

public static class Log
{
    private static readonly object _lock = new();

    public static void Info(string source, string message)
    {
        Write(source, message, ConsoleColor.Gray);
    }

    public static void Success(string source, string message)
    {
        Write(source, message, ConsoleColor.Green);
    }

    public static void Error(string source, string message)
    {
        Write(source, message, ConsoleColor.Red);
    }

    private static void Write(
        string source,
        string message,
        ConsoleColor color)
    {
        lock (_lock)
        {
            var time = DateTime.Now.ToString("HH:mm:ss.fff");
            var threadId = Environment.CurrentManagedThreadId;

            Console.ForegroundColor = color;

            Console.WriteLine(
                $"[{time}] [Thread {threadId}] {source} -> {message}");

            Console.ResetColor();
        }
    }
}

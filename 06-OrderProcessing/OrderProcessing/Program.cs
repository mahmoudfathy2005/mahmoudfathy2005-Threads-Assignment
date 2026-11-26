using OrderProcessing.Phase1.Runners;

namespace OrderProcessing;

public class Program
{
    public static async Task Main(string[] args)
    {
        var phase1 = new Phase1Runner();

        await phase1.RunAsync(CancellationToken.None);
    }
}
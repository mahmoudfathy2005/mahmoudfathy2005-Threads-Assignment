using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessing.Shared.Logging
{

    public static class DelayHelper
    {
        public static async Task WaitAsync(string source, string step,int milliseconds, CancellationToken ct)
        {
            Log.Info(source, $"{step}...");

            await Task.Delay(milliseconds, ct);

            Log.Success(source, $"{step} completed");
        }
    }

}

using System;
using Es.Serilog.Lite;
using Es.Serilog.Lite.Formatting;
using Serilog;

namespace SampleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = LogBuilder.Create();

            for (int i = 0; i < 1; i++)
            {
                Log.Debug("Warning!!!");
            }

            Log.ForContext<Program>().Information("okoko");

            Log.Warning("LogWarning");
            Log.Error("LogError");
            Log.Fatal("LogCritical");

            int a = 10, b = 0;
            try
            {
                Log.Debug("Dividing {A} by {B}", a, b);
                Console.WriteLine(a / b);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }
            Console.Read();
        }
    }
}
﻿using System;
using Serilog;
using Serilog.Formatting.Compact;

namespace SampleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var environmentName = "Development";

            Log.Logger = new LoggerConfiguration()
                .Configue()
                .ConfigueLevel(environmentName)
                .WriteTo.Async(async =>
                {
                    async.ConfigueStd(new CompactJsonFormatter());
                    async.ConfigueRollingFile(new CompactJsonFormatter());
                })
                .CreateLogger();

            for (int i = 0; i < 50; i++)
            {
                Log.Debug("Warning!!!");
            }

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
namespace MunicipalityRegistry.Importer
{
    using System;
    using System.Diagnostics;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing.CommandLine;
    using Be.Vlaanderen.Basisregisters.GrAr.Import.Processing.Serilog;
    using Crab;
    using Serilog;
    using Serilog.Events;

    internal class Program
    {
        private static Stopwatch _stopwatch;
        private static int _commandCounter;

        private static void Main(string[] args)
        {
            var settings = new SettingsBasedConfig();
            try
            {
                var options = new ImportOptions(
                    args,
                    errors => WaitForExit(settings, "Could not parse commandline options."));

                MapLogging.Log = s => _commandCounter++;

                var commandProcessor = new CommandProcessorBuilder<int>(new MunicipalityCommandGenerator())
                    .WithCommandLineOptions(options.ImportArguments)
                    .UseSerilog(cfg => cfg
                        .WriteTo.File(
                            "tracing.log",
                            LogEventLevel.Verbose,
                            retainedFileCountLimit: 20,
                            fileSizeLimitBytes: 104857600,
                            rollOnFileSizeLimit: true,
                            rollingInterval: RollingInterval.Day)
                        .WriteTo.Console(LogEventLevel.Information))
                    .UseApiProxyFactory(logger => new NonBatchedHttpApiProxyFactory(logger, settings))
                    .UseCommandProcessorConfig(settings)
                    .UseDefaultSerializerSettingsForCrabImports()
                    .Build();

                WaitForStart(settings);

                commandProcessor.Run(options, settings);

                WaitForExit(settings);
            }
            catch (Exception exception)
            {
                WaitForExit(settings, "General error occurred", exception);
            }
        }

        private static void WaitForExit(ICommandProcessorConfig settings, string errorMessage = null, Exception exception = null)
        {
            if (!string.IsNullOrEmpty(errorMessage))
                Console.Error.WriteLine(errorMessage);

            if (exception != null)
                Console.Error.WriteLine(exception);

            Console.WriteLine();

            if (_stopwatch != null)
            {
                var avg = _commandCounter / _stopwatch.Elapsed.TotalSeconds;
                var summary = $"Report: generated {_commandCounter} commands in {_stopwatch.Elapsed}ms (={avg}/second).";
                Console.WriteLine(summary);
            }

            if (settings.WaitForUserInput)
            {
                Console.WriteLine("Done! Press ENTER key to exit...");
                ConsoleExtensions.WaitFor(ConsoleKey.Enter);
            }

            if (!string.IsNullOrEmpty(errorMessage))
                Environment.Exit(1);

            Environment.Exit(0);
        }

        private static void WaitForStart(ICommandProcessorConfig settings)
        {
            if (settings.WaitForUserInput)
            {
                Console.WriteLine("Press ENTER key to start the CRAB Import...");
                ConsoleExtensions.WaitFor(ConsoleKey.Enter);
            }

            _stopwatch = Stopwatch.StartNew();
        }
    }
}

using EasyCLI.Console;
using EasyCLI.Shell;
using EasyCLI.Shell.SignalHandling;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCLI.Demo
{
    /// <summary>
    /// Demonstrates the signal handling and cleanup hooks functionality.
    /// Run with signal handling enabled to see graceful shutdown on Ctrl+C.
    /// </summary>
    public class SignalHandlingDemo
    {
        public static async Task<int> Main(string[] args)
        {
            var reader = new ConsoleReader();
            var writer = new ConsoleWriter();

            // Enable signal handling for this demo
            var options = new ShellOptions
            {
                Prompt = "signal-demo>",
                EnableSignalHandling = true
            };

            using var shell = new CliShell(reader, writer, options);

            // Register the demo command
            await shell.RegisterAsync(new DemoCleanupCommand());

            writer.WriteHeadingLine("Signal Handling Demo");
            writer.WriteLine();
            writer.WriteInfoLine("This demo shows EasyCLI's signal handling and cleanup hooks.");
            writer.WriteInfoLine("Type 'demo' to start a long-running task, then press Ctrl+C to interrupt it.");
            writer.WriteInfoLine("Notice how cleanup actions execute before shutdown.");
            writer.WriteLine();
            writer.WriteHintLine("Type 'help' for available commands, 'exit' to quit normally.");
            writer.WriteLine();

            return await shell.RunAsync();
        }

        /// <summary>
        /// Demo command that registers cleanup actions and simulates work.
        /// </summary>
        private class DemoCleanupCommand : ICleanupAwareCommand
        {
            public string Name => "demo";
            public string Description => "Demonstrates cleanup hooks with a long-running task";
            public string Category => "Demo";

            public void RegisterCleanupActions(ICleanupManager cleanupManager, ShellExecutionContext context)
            {
                cleanupManager.RegisterCleanup(() =>
                {
                    context.Writer.WriteWarningLine("🧹 Cleanup: Simulating resource cleanup...");
                    Thread.Sleep(500); // Simulate cleanup work
                }, "demo-cleanup");

                cleanupManager.RegisterCleanup(() =>
                {
                    context.Writer.WriteInfoLine("🔄 Cleanup: Restoring application state...");
                    Thread.Sleep(200);
                }, "state-restoration");
            }

            public async Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken cancellationToken)
            {
                context.Writer.WriteInfoLine("Starting long-running task...");
                context.Writer.WriteHintLine("Press Ctrl+C to interrupt and see cleanup in action!");

                try
                {
                    for (int i = 1; i <= 20; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        context.Writer.Write($"\rProgress: {i}/20");
                        await Task.Delay(1000, cancellationToken);
                    }

                    context.Writer.WriteLine();
                    context.Writer.WriteSuccessLine("Task completed successfully!");
                    return ExitCodes.Success;
                }
                catch (OperationCanceledException)
                {
                    context.Writer.WriteLine();
                    context.Writer.WriteWarningLine("Task interrupted by user");
                    return ExitCodes.UserCancelled;
                }
            }
        }
    }
}
using EasyCLI.Shell;
using EasyCLI.Shell.SignalHandling;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCLI.Demo.Commands
{
    /// <summary>
    /// Demo command that shows signal handling and cleanup hooks functionality.
    /// Creates a temporary file and registers cleanup to delete it on interruption.
    /// </summary>
    public class SignalDemoCommand : ICleanupAwareCommand
    {
        private string? _tempFile;

        /// <inheritdoc />
        public string Name => "signal-demo";

        /// <inheritdoc />
        public string Description => "Demonstrates signal handling and cleanup hooks";

        /// <inheritdoc />
        public string Category => "Demo";

        /// <inheritdoc />
        public void RegisterCleanupActions(ICleanupManager cleanupManager, ShellExecutionContext context)
        {
            // Register cleanup action that will be called on signal/shutdown
            cleanupManager.RegisterCleanup(async (ct) =>
            {
                context.Writer.WriteInfoLine("🧹 Cleanup: Removing temporary files...");
                
                if (_tempFile != null && File.Exists(_tempFile))
                {
                    try
                    {
                        File.Delete(_tempFile);
                        context.Writer.WriteSuccessLine($"✅ Cleaned up: {_tempFile}");
                    }
                    catch (Exception ex)
                    {
                        context.Writer.WriteWarningLine($"⚠️  Could not delete {_tempFile}: {ex.Message}");
                    }
                }
                
                await Task.Delay(100, ct); // Simulate cleanup work
            }, "temp-file-cleanup");

            // Register terminal restoration
            cleanupManager.RegisterCleanup(() =>
            {
                context.Writer.WriteInfoLine("🖥️  Restoring terminal state...");
                // Terminal state restoration is handled automatically by the shell
            }, "terminal-restoration");
        }

        /// <inheritdoc />
        public async Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken cancellationToken)
        {
            context.Writer.WriteHeadingLine("Signal Handling Demo");
            context.Writer.WriteLine();
            context.Writer.WriteInfoLine("This command demonstrates graceful signal handling and cleanup hooks.");
            context.Writer.WriteInfoLine("Try pressing Ctrl+C while it's running to see cleanup in action!");
            context.Writer.WriteLine();

            // Create a temporary file
            _tempFile = Path.GetTempFileName();
            context.Writer.WriteInfoLine($"📄 Created temporary file: {_tempFile}");
            
            await File.WriteAllTextAsync(_tempFile, $"Demo file created at {DateTime.Now}", cancellationToken);

            try
            {
                // Simulate long-running work
                context.Writer.WriteInfoLine("⏳ Simulating long-running work... (Press Ctrl+C to interrupt)");
                
                for (int i = 1; i <= 30; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    context.Writer.Write($"\r⏱️  Working... {i}/30 seconds");
                    await Task.Delay(1000, cancellationToken);
                }
                
                context.Writer.WriteLine();
                context.Writer.WriteSuccessLine("✅ Work completed successfully!");
                
                // Normal cleanup
                if (File.Exists(_tempFile))
                {
                    File.Delete(_tempFile);
                    context.Writer.WriteInfoLine($"🧹 Normal cleanup: Removed {_tempFile}");
                }
                
                return ExitCodes.Success;
            }
            catch (OperationCanceledException)
            {
                context.Writer.WriteLine();
                context.Writer.WriteWarningLine("⚠️  Operation interrupted by user");
                
                // Cleanup will be handled by the registered cleanup actions
                return ExitCodes.UserCancelled;
            }
        }
    }
}
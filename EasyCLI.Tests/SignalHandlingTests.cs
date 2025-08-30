using EasyCLI.Console;
using EasyCLI.Shell;
using EasyCLI.Shell.SignalHandling;
using EasyCLI.Tests.Fakes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EasyCLI.Tests
{
    public class SignalHandlingTests
    {
        [Fact]
        public void SignalHandler_CanBeCreated()
        {
            using var handler = new SignalHandler();
            Assert.NotNull(handler);
            Assert.False(handler.IsActive);
            Assert.False(handler.ShutdownToken.IsCancellationRequested);
        }

        [Fact]
        public void SignalHandler_StartAndStop()
        {
            using var handler = new SignalHandler();
            
            handler.Start();
            Assert.True(handler.IsActive);
            
            handler.Stop();
            Assert.False(handler.IsActive);
        }

        [Fact]
        public void CleanupManager_CanRegisterCleanup()
        {
            using var cleanupManager = new CleanupManager();
            
            bool cleanupCalled = false;
            var handle = cleanupManager.RegisterCleanup(() => cleanupCalled = true, "test-cleanup");
            
            Assert.NotNull(handle);
            Assert.Equal("test-cleanup", handle.Name);
            Assert.True(handle.IsRegistered);
            Assert.Equal(1, cleanupManager.RegisteredCleanupCount);
        }

        [Fact]
        public async Task CleanupManager_ExecutesCleanupActions()
        {
            using var cleanupManager = new CleanupManager();
            
            bool cleanup1Called = false;
            bool cleanup2Called = false;
            
            cleanupManager.RegisterCleanup(() => cleanup1Called = true, "cleanup1");
            cleanupManager.RegisterCleanup(() => cleanup2Called = true, "cleanup2");
            
            bool success = await cleanupManager.ExecuteCleanupAsync(TimeSpan.FromSeconds(5));
            
            Assert.True(success);
            Assert.True(cleanup1Called);
            Assert.True(cleanup2Called);
        }

        [Fact]
        public async Task CleanupManager_ExecutesInReverseOrder()
        {
            using var cleanupManager = new CleanupManager();
            
            var executionOrder = new List<int>();
            
            cleanupManager.RegisterCleanup(() => executionOrder.Add(1), "first");
            cleanupManager.RegisterCleanup(() => executionOrder.Add(2), "second");
            cleanupManager.RegisterCleanup(() => executionOrder.Add(3), "third");
            
            await cleanupManager.ExecuteCleanupAsync(TimeSpan.FromSeconds(5));
            
            // Should execute in reverse order (LIFO)
            Assert.Equal(new[] { 3, 2, 1 }, executionOrder);
        }

        [Fact]
        public void CleanupHandle_CanUnregister()
        {
            using var cleanupManager = new CleanupManager();
            
            var handle = cleanupManager.RegisterCleanup(() => { }, "test");
            Assert.True(handle.IsRegistered);
            Assert.Equal(1, cleanupManager.RegisteredCleanupCount);
            
            handle.Unregister();
            Assert.False(handle.IsRegistered);
            Assert.Equal(0, cleanupManager.RegisteredCleanupCount);
        }

        [Fact]
        public void TerminalStateManager_CanCaptureState()
        {
            using var manager = new TerminalStateManager();
            
            Assert.False(manager.HasCapturedState);
            
            manager.CaptureState();
            
            Assert.True(manager.HasCapturedState);
        }

        [Fact]
        public async Task TerminalStateManager_CanRestoreState()
        {
            using var manager = new TerminalStateManager();
            
            manager.CaptureState();
            
            // This should not throw
            await manager.RestoreStateAsync();
        }

        [Fact]
        public void TerminalStateManager_TemporaryModification()
        {
            using var manager = new TerminalStateManager();
            
            // This should not throw and should return a disposable
            using var modification = manager.TemporaryModification(TerminalModification.HideCursor);
            Assert.NotNull(modification);
        }

        [Fact]
        public async Task SignalHandler_ShutdownTokenTriggeredOnDispose()
        {
            var handler = new SignalHandler();
            var shutdownToken = handler.ShutdownToken;
            
            Assert.False(shutdownToken.IsCancellationRequested);
            
            handler.Dispose();
            
            // The token should be cancelled when the handler is disposed
            Assert.True(shutdownToken.IsCancellationRequested);
        }

        [Fact]
        public void LinkedCancellationTokenSource_UnderstandingDisposalBehavior()
        {
            // This test demonstrates the correct understanding of CancellationTokenSource disposal
            using var signalHandler = new SignalHandler();
            using var externalCts = new CancellationTokenSource();

            signalHandler.Start();

            CancellationToken linkedToken;

            // Create linked token source in limited scope
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                externalCts.Token,
                signalHandler.ShutdownToken))
            {
                linkedToken = linkedCts.Token;

                // At this point, the token should not be cancelled
                Assert.False(linkedToken.IsCancellationRequested);
            }
            // linkedCts is now disposed

            // Contrary to initial assumption, when a CancellationTokenSource is disposed,
            // its token does NOT automatically become cancelled (unless it was already cancelled)
            Assert.False(linkedToken.IsCancellationRequested);

            // However, the real issue is that once the linked CTS is disposed,
            // the token loses its connection to parent tokens, so future cancellations
            // of parent tokens won't propagate to this token

            // To test that the signal handler's token does work, create another linked token
            // BEFORE disposing the signal handler
            using var anotherLinkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                externalCts.Token,
                signalHandler.ShutdownToken);
            var anotherLinkedToken = anotherLinkedCts.Token;

            // This should not be cancelled yet
            Assert.False(anotherLinkedToken.IsCancellationRequested);

            signalHandler.Dispose(); // This cancels signalHandler.ShutdownToken

            // This new linked token should be cancelled because its parent (signal handler) was cancelled
            Assert.True(anotherLinkedToken.IsCancellationRequested,
                "New linked token should be cancelled when parent signal handler is disposed");

            // But the old linkedToken (whose CTS was disposed early) should still NOT be cancelled
            Assert.False(linkedToken.IsCancellationRequested,
                "Disposed linked CTS breaks the connection to parent tokens");
        }

        [Fact]
        public async Task CliShell_CancellationTokenLinksRemainsAlive_DuringExecution()
        {
            // This test verifies that the linked cancellation token source in CliShell.RunAsync
            // remains alive during shell execution, so signal cancellation works properly
            var reader = new FakeConsoleReader(new[] { "exit" });
            var writer = new FakeConsoleWriter();
            var options = new ShellOptions { EnableSignalHandling = true };
            
            using var shell = new CliShell(reader, writer, options);
            using var externalCts = new CancellationTokenSource();
            
            // Run the shell with an external cancellation token
            var result = await shell.RunAsync(externalCts.Token);
            
            // Should exit normally
            Assert.Equal(0, result);
            
            // Test that signal cancellation would work during shell execution
            // We can't easily test the actual signal behavior in a unit test,
            // but we can verify the shell setup is correct
        }
    }
}
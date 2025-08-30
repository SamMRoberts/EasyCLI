using EasyCLI.Shell.SignalHandling;
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
    }
}
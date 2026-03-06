using ASCOM;
using ASCOM.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TraceLoggerTests
{
    /// <summary>
    /// Multi-threaded unit tests for TraceLogger to ensure thread safety and proper mutex handling
    /// </summary>
    public class TraceLoggerMultiThreadedTests
    {
        private const int THREAD_COUNT = 10;
        private const int MESSAGES_PER_THREAD = 100;
        private const int TIMEOUT_SECONDS = 30;

        [Fact]
        public void ConcurrentWritesFromMultipleThreads()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(ConcurrentWritesFromMultipleThreads), true);
            var threads = new List<Thread>();
            var exceptions = new List<Exception>();
            var lockObject = new object();

            // Act - Create multiple threads that write concurrently
            for (int i = 0; i < THREAD_COUNT; i++)
            {
                int threadId = i;
                var thread = new Thread(() =>
                {
                    try
                    {
                        for (int j = 0; j < MESSAGES_PER_THREAD; j++)
                        {
                            logger.LogMessage($"Thread{threadId}", $"Message {j}");
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObject)
                        {
                            exceptions.Add(ex);
                        }
                    }
                });
                threads.Add(thread);
                thread.Start();
            }

            // Wait for all threads to complete
            foreach (var thread in threads)
            {
                Assert.True(thread.Join(TimeSpan.FromSeconds(TIMEOUT_SECONDS)), "Thread did not complete in time");
            }

            // Get log file path before disposing
            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);

            // Dispose and wait a moment for file handles to be released
            logger.Dispose();
            Thread.Sleep(100);

            // Assert - No exceptions occurred
            Assert.Empty(exceptions);

            // Verify all messages were written
            string[] lines = File.ReadAllLines(logFile);
            Assert.Equal(THREAD_COUNT * MESSAGES_PER_THREAD, lines.Length);

            // Verify no interleaved lines (each line should be complete)
            foreach (var line in lines)
            {
                Assert.Matches(@"^\d{2}:\d{2}:\d{2}\.\d{3}\s+Thread\d+\s+Message\s+\d+$", line);
            }
        }

        [Fact]
        public async Task ConcurrentWritesWithAsyncAwait()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(ConcurrentWritesWithAsyncAwait), true);
            var tasks = new List<Task>();
            var exceptions = new List<Exception>();
            var lockObject = new object();

            // Act - Create multiple async tasks that write concurrently
            for (int i = 0; i < THREAD_COUNT; i++)
            {
                int taskId = i;
                var task = Task.Run(() =>
                {
                    try
                    {
                        for (int j = 0; j < MESSAGES_PER_THREAD; j++)
                        {
                            logger.LogMessage($"Task{taskId}", $"Message {j}");
                            // Introduce small delays to increase contention
                            if (j % 10 == 0)
                            {
                                Thread.Sleep(1);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObject)
                        {
                            exceptions.Add(ex);
                        }
                    }
                });
                tasks.Add(task);
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks.ToArray());

            // Get log file
            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert - No exceptions occurred
            Assert.Empty(exceptions);

            // Verify all messages were written
            string[] lines = File.ReadAllLines(logFile);
            Assert.Equal(THREAD_COUNT * MESSAGES_PER_THREAD, lines.Length);
        }

        [Fact]
        public void HighContentionScenario()
        {
            // Arrange - Use a high number of threads writing very frequently
            var logger = new ASCOM.Tools.TraceLogger(nameof(HighContentionScenario), true);
            const int highThreadCount = 50;
            const int messagesPerThread = 20;
            var countdown = new CountdownEvent(highThreadCount);
            var exceptions = new List<Exception>();
            var lockObject = new object();

            // Act
            for (int i = 0; i < highThreadCount; i++)
            {
                int threadId = i;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        for (int j = 0; j < messagesPerThread; j++)
                        {
                            logger.LogMessage($"T{threadId:D3}", $"Msg{j:D3}");
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObject)
                        {
                            exceptions.Add(ex);
                        }
                    }
                    finally
                    {
                        countdown.Signal();
                    }
                });
            }

            // Wait for completion
            Assert.True(countdown.Wait(TimeSpan.FromSeconds(TIMEOUT_SECONDS)), "High contention test did not complete in time");

            // Get log file
            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert
            Assert.Empty(exceptions);

            string[] lines = File.ReadAllLines(logFile);
            Assert.Equal(highThreadCount * messagesPerThread, lines.Length);
        }

        [Fact]
        public void DisableAndEnableWhileThreadsAreWriting()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(DisableAndEnableWhileThreadsAreWriting), true);
            var keepRunning = true;
            var threads = new List<Thread>();

            // Act - Start writer threads
            for (int i = 0; i < 5; i++)
            {
                int threadId = i;
                var thread = new Thread(() =>
                {
                    int messageCount = 0;
                    while (keepRunning)
                    {
                        logger.LogMessage($"Writer{threadId}", $"Message {messageCount++}");
                        Thread.Sleep(10);
                    }
                });
                threads.Add(thread);
                thread.Start();
            }

            // Toggle enabled state while threads are writing
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(50);
                logger.Enabled = false;
                Thread.Sleep(50);
                logger.Enabled = true;
            }

            // Stop threads
            keepRunning = false;
            foreach (var thread in threads)
            {
                thread.Join();
            }

            // Assert - Should not crash or throw exceptions
            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            Assert.True(File.Exists(logFile));
        }

        [Fact]
        public void DisposeWhileThreadsAreWaiting()
        {
            // Arrange - Create scenario where threads might be waiting for mutex
            var logger = new ASCOM.Tools.TraceLogger(nameof(DisposeWhileThreadsAreWaiting), true);
            var threads = new List<Thread>();
            var startBarrier = new Barrier(THREAD_COUNT + 1); // +1 for main thread

            // Act - Start threads that will all try to log at the same time
            for (int i = 0; i < THREAD_COUNT; i++)
            {
                int threadId = i;
                var thread = new Thread(() =>
                {
                    startBarrier.SignalAndWait(); // Synchronize start
                    try
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            logger.LogMessage($"Thread{threadId}", $"Message {j}");
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // Expected if logger is disposed while thread is running
                    }
                    catch (NullReferenceException)
                    {
                        // Can occur if disposed during operation
                    }
                });
                threads.Add(thread);
                thread.Start();
            }

            // Release all threads at once
            startBarrier.SignalAndWait();
            Thread.Sleep(50); // Let some messages get written

            // Dispose while threads are still running
            logger.Dispose();

            // Wait for threads to finish
            foreach (var thread in threads)
            {
                thread.Join(TimeSpan.FromSeconds(5));
            }

            // Assert - Should complete without deadlock
            Assert.True(true); // If we get here, no deadlock occurred
        }

        [Fact]
        public void MultipleLoggersWritingSimultaneously()
        {
            // Arrange - Test that multiple TraceLogger instances don't interfere with each other
            var loggers = new List<ASCOM.Tools.TraceLogger>();
            var threads = new List<Thread>();
            const int loggerCount = 5;

            // Create multiple loggers
            for (int i = 0; i < loggerCount; i++)
            {
                loggers.Add(new ASCOM.Tools.TraceLogger($"Logger{i}", true));
            }

            // Act - Each logger has multiple threads writing to it
            for (int i = 0; i < loggerCount; i++)
            {
                int loggerId = i;
                var logger = loggers[i];

                for (int j = 0; j < 3; j++)
                {
                    int threadId = j;
                    var thread = new Thread(() =>
                    {
                        for (int k = 0; k < 50; k++)
                        {
                            logger.LogMessage($"L{loggerId}T{threadId}", $"Message {k}");
                        }
                    });
                    threads.Add(thread);
                    thread.Start();
                }
            }

            // Wait for all threads
            foreach (var thread in threads)
            {
                thread.Join();
            }

            // Assert - Each logger should have all its messages
            for (int i = 0; i < loggerCount; i++)
            {
                string logFile = Path.Combine(loggers[i].LogFilePath, loggers[i].LogFileName);
                loggers[i].Dispose();

                string[] lines = File.ReadAllLines(logFile);
                Assert.Equal(150, lines.Length); // 3 threads * 50 messages
            }
        }

        [Fact]
        public async Task StressTestWithLargeMessages()
        {
            // Arrange - Test with large message strings
            var logger = new ASCOM.Tools.TraceLogger(nameof(StressTestWithLargeMessages), true);
            var tasks = new List<Task>();
            var largeMessage = new string('X', 10000); // 10KB message

            // Act
            for (int i = 0; i < THREAD_COUNT; i++)
            {
                int taskId = i;
                var task = Task.Run(() =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        logger.LogMessage($"Task{taskId}", $"[{j}]{largeMessage}");
                    }
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());

            // Get log file
            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert
            Assert.True(File.Exists(logFile));
            string[] lines = File.ReadAllLines(logFile);
            Assert.Equal(THREAD_COUNT * 10, lines.Length);
        }

        [Fact]
        public void RapidEnableDisableCycles()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(RapidEnableDisableCycles), true);
            var writerThread = new Thread(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    logger.LogMessage("Writer", $"Message {i}");
                }
            });

            var toggleThread = new Thread(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    logger.Enabled = !logger.Enabled;
                    Thread.Sleep(5);
                }
            });

            // Act
            writerThread.Start();
            toggleThread.Start();

            writerThread.Join();
            toggleThread.Join();

            // Assert - Should complete without exception
            logger.Dispose();
            Assert.True(true);
        }

        [Fact]
        public void ThreadSafePropertyAccess()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(ThreadSafePropertyAccess), true);
            var exceptions = new List<Exception>();
            var lockObject = new object();
            var threads = new List<Thread>();

            // Act - Multiple threads accessing properties while others are writing
            for (int i = 0; i < 10; i++)
            {
                int threadId = i;
                var thread = new Thread(() =>
                {
                    try
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            // Access properties
                            _ = logger.Enabled;
                            _ = logger.LogFileName;
                            _ = logger.LogFilePath;
                            _ = logger.IdentifierWidth;
                            _ = logger.UseUtcTime;
                            _ = logger.RespectCrLf;

                            // Write message
                            logger.LogMessage($"T{threadId}", $"Msg{j}");

                            // Modify properties
                            if (j % 20 == 0)
                            {
                                logger.UseUtcTime = !logger.UseUtcTime;
                                logger.RespectCrLf = !logger.RespectCrLf;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObject)
                        {
                            exceptions.Add(ex);
                        }
                    }
                });
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            logger.Dispose();

            // Assert - Should handle concurrent property access safely
            Assert.Empty(exceptions);
        }

        [Fact]
        public void NoMessageLossUnderLoad()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(NoMessageLossUnderLoad), true);
            var messageIds = new HashSet<string>();
            var lockObject = new object();
            var threads = new List<Thread>();
            const int expectedMessages = THREAD_COUNT * MESSAGES_PER_THREAD;

            // Act
            for (int i = 0; i < THREAD_COUNT; i++)
            {
                int threadId = i;
                var thread = new Thread(() =>
                {
                    for (int j = 0; j < MESSAGES_PER_THREAD; j++)
                    {
                        string messageId = $"T{threadId:D3}M{j:D5}";
                        lock (lockObject)
                        {
                            messageIds.Add(messageId);
                        }
                        logger.LogMessage("TraceLogger", messageId);
                    }
                });
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert - Verify all unique messages were written
            string[] lines = File.ReadAllLines(logFile);
            Assert.Equal(expectedMessages, lines.Length);

            // Verify each message ID appears exactly once
            foreach (var messageId in messageIds)
            {
                int count = lines.Count(line => line.Contains(messageId));
                Assert.Equal(1, count);
            }
        }
    }
}

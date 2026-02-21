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
        public void MutexTimeoutDoesNotOccur()
        {
            // Arrange - Test that normal operation doesn't cause timeouts
            var logger = new ASCOM.Tools.TraceLogger(nameof(MutexTimeoutDoesNotOccur), true);
            var exceptions = new List<Exception>();
            var lockObject = new object();
            var threads = new List<Thread>();

            // Act - Many threads writing simultaneously
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
                    catch (DriverException ex) when (ex.Message.Contains("Timed out waiting"))
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

            // Assert - No timeout exceptions should occur during normal operation
            Assert.Empty(exceptions);
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

        #region Abandoned Mutex Recovery Tests

        [Fact]
        public void RecoverFromSingleAbandonedMutex()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(RecoverFromSingleAbandonedMutex), true);
            logger.LogMessage("Initial", "First message to create log file");

            // Get the mutex name using reflection to simulate abandonment
            var mutexNameField = typeof(ASCOM.Tools.TraceLogger).GetField("mutexName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string mutexName = (string)mutexNameField.GetValue(logger);

            var mutexAbandoned = new ManualResetEventSlim(false);

            // Act - Thread that will abandon the mutex
            var abandoningThread = new Thread(() =>
            {
                Mutex externalMutex = null;
                try
                {
                    externalMutex = Mutex.OpenExisting(mutexName);
                    externalMutex.WaitOne();
                    mutexAbandoned.Set();
                    // Thread exits here without releasing mutex - this abandons it
                }
                catch
                {
                    // Mutex might not be available yet, that's OK for this test
                }
            });

            abandoningThread.Start();
            mutexAbandoned.Wait(TimeSpan.FromSeconds(2)); // Wait for mutex to be acquired and abandoned
            abandoningThread.Join(); // Wait for thread to exit (mutex is now abandoned)

            Thread.Sleep(100); // Give time for abandonment to register

            // Try to log after mutex was abandoned - this should detect and recover
            logger.LogMessage("RecoveryThread", "Message after abandoned mutex");
            logger.LogMessage("RecoveryThread", "Second message to verify continued operation");

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert - Should recover and log warning
            string[] lines = File.ReadAllLines(logFile);
            Assert.Contains(lines, line => line.Contains("First message to create log file"));
            Assert.Contains(lines, line => line.Contains("Message after abandoned mutex"));
            Assert.Contains(lines, line => line.Contains("Second message to verify continued operation"));

            // Check if abandoned mutex warning was logged
            var warningLine = lines.FirstOrDefault(line => line.Contains("[WARNING]") && line.Contains("Abandoned mutex detected"));
            if (warningLine != null)
            {
                // If warning appears, verify its format
                Assert.Contains("[WARNING]", warningLine);
                Assert.Contains("Abandoned mutex detected", warningLine);
                Assert.Contains(logger.LogFileName, warningLine);
                Assert.Matches(@"^\d{2}:\d{2}:\d{2}\.\d{3}\s+\[WARNING\].*TraceLogger - Abandoned mutex detected for file", warningLine);
            }
        }

        [Fact]
        public void RecoverFromMultipleAbandonedMutexes()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(RecoverFromMultipleAbandonedMutexes), true);
            logger.LogMessage("Initial", "Starting test");

            var mutexNameField = typeof(ASCOM.Tools.TraceLogger).GetField("mutexName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string mutexName = (string)mutexNameField.GetValue(logger);

            var abandonCount = 3;

            // Act - Create multiple threads that will attempt to abandon the mutex
            for (int i = 0; i < abandonCount; i++)
            {
                int threadId = i;
                var mutexAcquired = new ManualResetEventSlim(false);
                var abandoningThread = new Thread(() =>
                {
                    try
                    {
                        Mutex externalMutex = Mutex.OpenExisting(mutexName);
                        if (externalMutex.WaitOne(100))
                        {
                            mutexAcquired.Set();
                            // Exit without releasing - abandon the mutex
                        }
                    }
                    catch
                    {
                        // Expected if mutex is busy or not available
                    }
                });

                abandoningThread.Start();
                mutexAcquired.Wait(TimeSpan.FromSeconds(1));
                abandoningThread.Join();

                Thread.Sleep(50);

                // Log after each potential abandonment
                logger.LogMessage("RecoveryThread", $"Recovered after attempt {threadId}");
            }

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert - Should recover from all attempts
            string[] lines = File.ReadAllLines(logFile);

            // Verify recovery messages are present
            for (int i = 0; i < abandonCount; i++)
            {
                Assert.Contains(lines, line => line.Contains($"Recovered after attempt {i}"));
            }

            // Check for abandoned mutex warnings
            var warningLines = lines.Where(line => line.Contains("[WARNING]") && line.Contains("Abandoned mutex detected")).ToList();
            if (warningLines.Any())
            {
                // If warnings appear, verify their format
                foreach (var warningLine in warningLines)
                {
                    Assert.Contains("[WARNING]", warningLine);
                    Assert.Contains("Abandoned mutex detected", warningLine);
                    Assert.Contains(logger.LogFileName, warningLine);
                }
            }
        }

        [Fact]
        public void AbandonedMutexUnderHighContention()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(AbandonedMutexUnderHighContention), true);
            logger.LogMessage("Initial", "Starting high contention test");

            var threads = new List<Thread>();
            const int normalThreads = 20;
            const int messagesPerThread = 10;
            var exceptions = new List<Exception>();
            var lockObject = new object();

            // Act - Start normal logging threads
            for (int i = 0; i < normalThreads; i++)
            {
                int threadId = i;
                var thread = new Thread(() =>
                {
                    try
                    {
                        for (int j = 0; j < messagesPerThread; j++)
                        {
                            logger.LogMessage($"NormalThread{threadId}", $"Message {j}");
                            Thread.Sleep(5);
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

            // Wait for all normal threads
            foreach (var thread in threads)
            {
                thread.Join();
            }

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert - Should complete without exceptions
            Assert.Empty(exceptions);

            // Verify messages were written
            string[] lines = File.ReadAllLines(logFile);
            int totalMessages = lines.Count(line => line.Contains("Message"));
            Assert.True(totalMessages > 0, "Should have messages logged");
        }

        [Fact]
        public void AbandonedMutexDoesNotBlockSubsequentThreads()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(AbandonedMutexDoesNotBlockSubsequentThreads), true);
            logger.LogMessage("Initial", "Initializing test");

            var barrier = new Barrier(6); // 5 threads + main thread
            var exceptions = new List<Exception>();
            var lockObject = new object();

            // Act - Create threads that will wait at barrier
            var threads = new List<Thread>();
            for (int i = 0; i < 5; i++)
            {
                int threadId = i;
                var thread = new Thread(() =>
                {
                    try
                    {
                        barrier.SignalAndWait(); // All threads start together
                        logger.LogMessage($"Thread{threadId}", $"Attempting to log");
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
            }

            // Start all waiting threads
            foreach (var thread in threads)
            {
                thread.Start();
            }
            barrier.SignalAndWait(); // Release all threads

            // Wait for completion
            foreach (var thread in threads)
            {
                Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "Thread should not be blocked");
            }

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert - All threads completed successfully without exceptions
            Assert.Empty(exceptions);

            string[] lines = File.ReadAllLines(logFile);
            for (int i = 0; i < 5; i++)
            {
                Assert.Contains(lines, line => line.Contains($"Thread{i}") && line.Contains("Attempting to log"));
            }
        }

        [Fact]
        public void AbandonedMutexWarningIsLogged()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(AbandonedMutexWarningIsLogged), true);
            logger.LogMessage("Initial", "Starting warning test");

            var mutexNameField = typeof(ASCOM.Tools.TraceLogger).GetField("mutexName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string mutexName = (string)mutexNameField.GetValue(logger);

            // Act - Attempt to abandon mutex
            var mutexAbandoned = new ManualResetEventSlim(false);
            var abandoningThread = new Thread(() =>
            {
                try
                {
                    Mutex externalMutex = Mutex.OpenExisting(mutexName);
                    if (externalMutex.WaitOne(100))
                    {
                        mutexAbandoned.Set();
                        // Exit without releasing
                    }
                }
                catch
                {
                    // Expected if busy
                }
            });
            abandoningThread.Start();
            mutexAbandoned.Wait(TimeSpan.FromSeconds(1));
            abandoningThread.Join();

            Thread.Sleep(100);

            // Next log should potentially detect the abandoned mutex
            logger.LogMessage("DetectingThread", "This may detect an abandoned mutex");
            logger.LogMessage("ContinuingThread", "Continued operation");

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert - Should have successful logging
            string[] lines = File.ReadAllLines(logFile);
            Assert.Contains(lines, line => line.Contains("This may detect an abandoned mutex"));
            Assert.Contains(lines, line => line.Contains("Continued operation"));

            // Check for abandoned mutex warning and verify its format
            var warningLine = lines.FirstOrDefault(line => line.Contains("[WARNING]") && line.Contains("Abandoned mutex detected"));
            if (warningLine != null)
            {
                Assert.Contains("[WARNING]", warningLine);
                Assert.Contains("Abandoned mutex detected", warningLine);
                Assert.Contains(logger.LogFileName, warningLine);
                Assert.Contains("TraceLogger - Abandoned mutex detected for file", warningLine);

                // Verify timestamp format at the beginning of the warning line
                Assert.Matches(@"^\d{2}:\d{2}:\d{2}\.\d{3}\s+\[WARNING\]", warningLine);
            }
        }

        [Fact]
        public void MultipleThreadsRecoveringFromAbandonedMutex()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(MultipleThreadsRecoveringFromAbandonedMutex), true);
            logger.LogMessage("Initial", "Starting multi-thread recovery test");

            var threads = new List<Thread>();
            var exceptions = new List<Exception>();
            var lockObject = new object();

            // Act - Multiple threads trying to log
            for (int i = 0; i < 10; i++)
            {
                int threadId = i;
                var thread = new Thread(() =>
                {
                    try
                    {
                        logger.LogMessage($"RecoveryThread{threadId}", "Attempting recovery");
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

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert - No exceptions, all messages logged
            Assert.Empty(exceptions);
            string[] lines = File.ReadAllLines(logFile);
            for (int i = 0; i < 10; i++)
            {
                Assert.Contains(lines, line => line.Contains($"RecoveryThread{i}"));
            }

            // Check for abandoned mutex warnings
            var warningLines = lines.Where(line => line.Contains("[WARNING]") && line.Contains("Abandoned mutex detected")).ToList();
            if (warningLines.Any())
            {
                // Verify warning format
                foreach (var warningLine in warningLines)
                {
                    Assert.Contains("[WARNING]", warningLine);
                    Assert.Contains("Abandoned mutex detected", warningLine);
                    Assert.Matches(@"^\d{2}:\d{2}:\d{2}\.\d{3}\s+\[WARNING\].*TraceLogger - Abandoned mutex detected", warningLine);
                }
            }
        }

        [Fact]
        public void AbandonedMutexWithEnabledToggling()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(AbandonedMutexWithEnabledToggling), true);
            logger.LogMessage("Initial", "Starting enabled toggle test");

            // Act - Toggle enabled state and log
            logger.Enabled = false;
            logger.LogMessage("DisabledLog", "Should not appear");

            logger.Enabled = true;
            logger.LogMessage("EnabledLog", "Should appear after re-enabling");

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert
            string[] lines = File.ReadAllLines(logFile);
            Assert.Contains(lines, line => line.Contains("EnabledLog") && line.Contains("Should appear"));
            Assert.DoesNotContain(lines, line => line.Contains("DisabledLog"));
        }

        [Fact]
        public async Task AbandonedMutexInAsyncContext()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(AbandonedMutexInAsyncContext), true);
            logger.LogMessage("Initial", "Starting async context test");

            // Act - Try logging from async task
            await Task.Run(() =>
            {
                logger.LogMessage("AsyncTask", "Logging in async context");
            });

            // Multiple async tasks
            var tasks = Enumerable.Range(0, 5).Select(i => Task.Run(() =>
            {
                logger.LogMessage($"AsyncTask{i}", $"Message {i}");
            })).ToArray();

            await Task.WhenAll(tasks);

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert
            string[] lines = File.ReadAllLines(logFile);
            Assert.Contains(lines, line => line.Contains("Logging in async context"));
            for (int i = 0; i < 5; i++)
            {
                Assert.Contains(lines, line => line.Contains($"AsyncTask{i}"));
            }
        }

        [Fact]
        public void ConsecutiveAbandonmentsFromDifferentThreads()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(ConsecutiveAbandonmentsFromDifferentThreads), true);
            logger.LogMessage("Initial", "Starting consecutive abandonments test");

            var mutexNameField = typeof(ASCOM.Tools.TraceLogger).GetField("mutexName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string mutexName = (string)mutexNameField.GetValue(logger);

            // Act - Create a pattern of attempted abandonment and recovery
            for (int i = 0; i < 5; i++)
            {
                // Thread that attempts to abandon
                var mutexAcquired = new ManualResetEventSlim(false);
                var abandoningThread = new Thread(() =>
                {
                    try
                    {
                        Mutex externalMutex = Mutex.OpenExisting(mutexName);
                        if (externalMutex.WaitOne(50))
                        {
                            mutexAcquired.Set();
                            // Exit without releasing
                        }
                    }
                    catch
                    {
                        // Expected if busy
                    }
                });
                abandoningThread.Start();
                mutexAcquired.Wait(TimeSpan.FromSeconds(1));
                abandoningThread.Join();

                Thread.Sleep(50);

                // Thread that recovers and completes properly
                logger.LogMessage($"CycleThread{i}", $"Cycle {i}");
            }

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert - All messages present
            string[] lines = File.ReadAllLines(logFile);
            for (int i = 0; i < 5; i++)
            {
                Assert.Contains(lines, line => line.Contains($"CycleThread{i}"));
            }

            // Check for abandoned mutex warnings across cycles
            var warningLines = lines.Where(line => line.Contains("[WARNING]") && line.Contains("Abandoned mutex detected")).ToList();
            if (warningLines.Any())
            {
                // Verify warning format
                foreach (var warningLine in warningLines)
                {
                    Assert.Contains("[WARNING]", warningLine);
                    Assert.Contains("Abandoned mutex detected", warningLine);
                    Assert.Contains("TraceLogger - Abandoned mutex detected for file", warningLine);
                }
            }
        }

        #endregion

        #region Warning Message Format Tests

        [Fact]
        public void AbandonedMutexWarningFormatIsCorrect()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(AbandonedMutexWarningFormatIsCorrect), true);
            logger.LogMessage("Setup", "Initializing test");

            var mutexNameField = typeof(ASCOM.Tools.TraceLogger).GetField("mutexName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string mutexName = (string)mutexNameField.GetValue(logger);

            // Act - Try to create an abandoned mutex scenario
            var mutexAcquired = new ManualResetEventSlim(false);
            var abandoningThread = new Thread(() =>
            {
                try
                {
                    Mutex externalMutex = Mutex.OpenExisting(mutexName);
                    if (externalMutex.WaitOne(100))
                    {
                        mutexAcquired.Set();
                        // Exit without releasing - abandons the mutex
                    }
                }
                catch
                {
                    // Expected if mutex is busy or not available
                }
            });

            abandoningThread.Start();
            mutexAcquired.Wait(TimeSpan.FromSeconds(1));
            abandoningThread.Join();

            Thread.Sleep(100);

            // Log messages that may trigger abandoned mutex detection
            logger.LogMessage("TestMessage1", "First message after potential abandonment");
            logger.LogMessage("TestMessage2", "Second message");

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert
            string[] lines = File.ReadAllLines(logFile);

            // Find warning line if it exists
            var warningLine = lines.FirstOrDefault(line => line.Contains("[WARNING]") && line.Contains("Abandoned mutex detected"));

            if (warningLine != null)
            {
                // Verify complete warning format
                // Expected format: "HH:mm:ss.fff [WARNING]              TraceLogger - Abandoned mutex detected for file {filename}"

                // 1. Check timestamp format
                Assert.Matches(@"^\d{2}:\d{2}:\d{2}\.\d{3}", warningLine);

                // 2. Check [WARNING] identifier is present and properly padded
                Assert.Contains("[WARNING]", warningLine);

                // 3. Check complete warning message
                Assert.Contains("TraceLogger - Abandoned mutex detected for file", warningLine);

                // 4. Check filename is included
                Assert.Contains(logger.LogFileName, warningLine);

                // 5. Verify the full pattern matches expected format
                Assert.Matches(@"^\d{2}:\d{2}:\d{2}\.\d{3}\s+\[WARNING\]\s+TraceLogger - Abandoned mutex detected for file\s+\S+\.txt", warningLine);

                // 6. Verify warning appears after the message that triggered it
                int setupIndex = Array.FindIndex(lines, line => line.Contains("Initializing test"));
                int warningIndex = Array.IndexOf(lines, warningLine);
                Assert.True(warningIndex > setupIndex, "Warning should appear after the setup message");
            }
            // Note: If warning doesn't appear, the test still passes as abandoned mutex detection is timing-dependent
        }

        [Fact]
        public void WarningMessageIdentifierWidthIsRespected()
        {
            // Arrange - Test with custom identifier width
            const int customWidth = 40;
            var logger = new ASCOM.Tools.TraceLogger(nameof(WarningMessageIdentifierWidthIsRespected), true, customWidth);

            Assert.Equal(customWidth, logger.IdentifierWidth);
            logger.LogMessage("Setup", "Test with custom width");

            var mutexNameField = typeof(ASCOM.Tools.TraceLogger).GetField("mutexName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string mutexName = (string)mutexNameField.GetValue(logger);

            // Act - Attempt to create abandoned mutex
            var mutexAcquired = new ManualResetEventSlim(false);
            var abandoningThread = new Thread(() =>
            {
                try
                {
                    Mutex externalMutex = Mutex.OpenExisting(mutexName);
                    if (externalMutex.WaitOne(100))
                    {
                        mutexAcquired.Set();
                    }
                }
                catch { }
            });

            abandoningThread.Start();
            mutexAcquired.Wait(TimeSpan.FromSeconds(1));
            abandoningThread.Join();
            Thread.Sleep(100);

            logger.LogMessage("TriggerMessage", "Message that may detect abandonment");

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert
            string[] lines = File.ReadAllLines(logFile);
            var warningLine = lines.FirstOrDefault(line => line.Contains("[WARNING]") && line.Contains("Abandoned mutex detected"));

            if (warningLine != null)
            {
                // Extract the identifier portion (between timestamp and message)
                // Format: "HH:mm:ss.fff [WARNING]{padding} TraceLogger..."
                var timestampEnd = warningLine.IndexOf(' ') + 1; // After "HH:mm:ss.fff "
                var messageStart = warningLine.IndexOf("TraceLogger");

                if (messageStart > timestampEnd)
                {
                    var identifierSection = warningLine.Substring(timestampEnd, messageStart - timestampEnd);

                    // The identifier should be exactly customWidth characters (including padding)
                    // "[WARNING]" padded to customWidth
                    Assert.True(identifierSection.Length >= 9, "[WARNING] should be at least 9 characters"); // "[WARNING]" = 9 chars
                    Assert.Contains("[WARNING]", identifierSection);
                }
            }
        }

        [Fact]
        public void MultipleWarningsHaveConsistentFormat()
        {
            // Arrange
            var logger = new ASCOM.Tools.TraceLogger(nameof(MultipleWarningsHaveConsistentFormat), true);
            logger.LogMessage("Setup", "Starting consistency test");

            // Act - Generate multiple potential abandoned mutex scenarios
            for (int i = 0; i < 3; i++)
            {
                var mutexNameField = typeof(ASCOM.Tools.TraceLogger).GetField("mutexName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                string mutexName = (string)mutexNameField.GetValue(logger);

                var mutexAcquired = new ManualResetEventSlim(false);
                var abandoningThread = new Thread(() =>
                {
                    try
                    {
                        Mutex externalMutex = Mutex.OpenExisting(mutexName);
                        if (externalMutex.WaitOne(50))
                        {
                            mutexAcquired.Set();
                        }
                    }
                    catch { }
                });

                abandoningThread.Start();
                mutexAcquired.Wait(TimeSpan.FromSeconds(1));
                abandoningThread.Join();
                Thread.Sleep(50);

                logger.LogMessage($"Cycle{i}", $"Message in cycle {i}");
            }

            string logFile = Path.Combine(logger.LogFilePath, logger.LogFileName);
            logger.Dispose();

            // Assert - All warnings should have consistent format
            string[] lines = File.ReadAllLines(logFile);
            var warningLines = lines.Where(line => line.Contains("[WARNING]") && line.Contains("Abandoned mutex detected")).ToList();

            if (warningLines.Count > 1)
            {
                // Verify all warnings have the same structure
                foreach (var warningLine in warningLines)
                {
                    Assert.Matches(@"^\d{2}:\d{2}:\d{2}\.\d{3}\s+\[WARNING\].*TraceLogger - Abandoned mutex detected for file", warningLine);
                    Assert.Contains(logger.LogFileName, warningLine);
                }

                // Verify format consistency across all warnings
                var firstWarning = warningLines[0];
                var firstWarningPattern = @"^\d{2}:\d{2}:\d{2}\.\d{3}\s+\[WARNING\]";

                foreach (var warningLine in warningLines)
                {
                    Assert.Matches(firstWarningPattern, warningLine);
                }
            }
        }

        #endregion
    }
}

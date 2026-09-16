using ASCOM.Alpaca.Discovery;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReliabilityTests
{
    public class FinderResponderReliabilityTests
    {
        private const int FirstAlpacaPort = 41001;
        private const int SecondAlpacaPort = 41002;

        [Fact]
        public void FinderIPv6LoopbackTest()
        {
            RunLoopbackDiscoveryTest();
        }

        /// <summary>
        /// This test runs the FinderIPv6LoopbackTestLinuxChild test under WSL on a Linux x64 environment. It is intended to be run by the Windows test runner, and will skip if not running on Windows.
        /// </summary>
        [Fact]
        public void FinderIPv6LoopbackTestLinux()
        {
            Assert.SkipUnless(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), "The WSL bridge test must be run by the Windows test runner.");

            string repositoryRoot = FindRepositoryRoot();
            ProcessResult pathResult = RunProcess(
                "wsl.exe",
                $"-- wslpath -a -u {QuoteProcessArgument(repositoryRoot.Replace('\\', '/'))}",
                TimeSpan.FromSeconds(30));

            Assert.True(
                pathResult.ExitCode == 0,
                $"Unable to convert the repository path for WSL.{Environment.NewLine}{FormatProcessResult(pathResult)}");

            string wslRepositoryRoot = pathResult.StandardOutput.Trim();
            string wslProjectPath = $"{wslRepositoryRoot.TrimEnd('/')}/UnitTests/UnitTests.csproj";
            string linuxTestName = $"ReliabilityTests.FinderResponderReliabilityTests.{nameof(FinderIPv6LoopbackTestLinuxChild)}";
            string wslBuildRoot = $"/tmp/ascomlibrary-wsl-{Guid.NewGuid():N}";
            string command =
                $"set +e; " +
                $"cd {QuoteBashArgument(wslRepositoryRoot)}; " +
                $"dotnet test {QuoteBashArgument(wslProjectPath)} -c Debug -f net10.0 " +
                $"--filter {QuoteBashArgument($"FullyQualifiedName~{linuxTestName}")} " +
                $"--artifacts-path {QuoteBashArgument(wslBuildRoot)} -v minimal; " +
                $"exitCode=$?; rm -rf {QuoteBashArgument(wslBuildRoot)}; exit $exitCode";

            ProcessResult linuxResult = RunProcess(
                "wsl.exe",
                $"-- bash -lc {QuoteProcessArgument(command)}",
                TimeSpan.FromMinutes(3));

            Assert.True(
                linuxResult.ExitCode == 0,
                $"The Linux x64 discovery test failed under WSL.{Environment.NewLine}{FormatProcessResult(linuxResult)}");
        }

        [Fact]
        public void FinderIPv6LoopbackTestLinuxChild()
        {
            // This test is only run by the WSL Linux bridge test, and is not intended to be run directly on Windows.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) // Not Linux so skip the test and return a green OK result.
            {
                return;
            }

            Assert.Equal(Architecture.X64, RuntimeInformation.ProcessArchitecture);
            RunLoopbackDiscoveryTest();
        }

        private static void RunLoopbackDiscoveryTest()
        {
            int discoveryPort = GetAvailableIPv6LoopbackPort();
            var discoveredEndpoints = new ConcurrentBag<IPEndPoint>();
            bool useMulticastLoopbackDiscovery = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            int expectedResponderCount = useMulticastLoopbackDiscovery ? 2 : 1;

            using (var firstResponder = new Responder(FirstAlpacaPort, discoveryPort, false, true))
            using (var secondResponder = useMulticastLoopbackDiscovery ? new Responder(SecondAlpacaPort, discoveryPort, false, true) : null)
            using (var finder = new Finder())
            using (var responsesReceived = new ManualResetEventSlim())
            {
                finder.ResponseReceivedEvent += (_, endpoint) =>
                {
                    discoveredEndpoints.Add(endpoint);
                    if (discoveredEndpoints.Count >= expectedResponderCount)
                    {
                        responsesReceived.Set();
                    }
                };

                finder.Search(discoveryPort, IPv4: false, IPv6: true);

                Assert.True(
                    responsesReceived.Wait(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken),
                    $"Expected {expectedResponderCount} response(s), but received {discoveredEndpoints.Count}.");
            }

            int[] discoveredPorts = discoveredEndpoints
                .Select(endpoint => endpoint.Port)
                .OrderBy(port => port)
                .ToArray();

            int[] expectedPorts = useMulticastLoopbackDiscovery
                ? new[] { FirstAlpacaPort, SecondAlpacaPort }
                : new[] { FirstAlpacaPort };

            Assert.Equal(expectedPorts, discoveredPorts);
            Assert.All(discoveredEndpoints, endpoint => Assert.True(IPAddress.IsLoopback(endpoint.Address), $"Expected a loopback response address, received {endpoint.Address}."));
        }

        private static string FindRepositoryRoot()
        {
            DirectoryInfo directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "ASCOMLibrary.sln")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException($"Could not locate ASCOMLibrary.sln above {AppContext.BaseDirectory}.");
        }

        private static ProcessResult RunProcess(string fileName, string arguments, TimeSpan timeout)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                if (!process.Start())
                {
                    throw new InvalidOperationException($"Unable to start process '{fileName}'.");
                }

                Task<string> standardOutput = process.StandardOutput.ReadToEndAsync();
                Task<string> standardError = process.StandardError.ReadToEndAsync();

                if (!process.WaitForExit((int)timeout.TotalMilliseconds))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (InvalidOperationException)
                    {
                    }

                    process.WaitForExit();
                    Task.WaitAll(standardOutput, standardError);
                    throw new TimeoutException($"Process '{fileName}' did not exit within {timeout}.");
                }

                Task.WaitAll(standardOutput, standardError);
                return new ProcessResult(process.ExitCode, standardOutput.Result, standardError.Result);
            }
        }

        private static string FormatProcessResult(ProcessResult result)
        {
            var output = new StringBuilder();
            output.AppendLine($"Exit code: {result.ExitCode}");
            output.AppendLine("Standard output:");
            output.AppendLine(result.StandardOutput);
            output.AppendLine("Standard error:");
            output.AppendLine(result.StandardError);
            return output.ToString();
        }

        private static string QuoteProcessArgument(string argument)
        {
            return $"\"{argument.Replace("\"", "\\\"")}\"";
        }

        private static string QuoteBashArgument(string argument)
        {
            return $"'{argument.Replace("'", "'\"'\"'")}'";
        }

        private static int GetAvailableIPv6LoopbackPort()
        {
            using (var socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(IPAddress.IPv6Loopback, 0));
                return ((IPEndPoint)socket.LocalEndPoint).Port;
            }
        }

        private sealed class ProcessResult
        {
            public ProcessResult(int exitCode, string standardOutput, string standardError)
            {
                ExitCode = exitCode;
                StandardOutput = standardOutput;
                StandardError = standardError;
            }

            public int ExitCode
            {
                get;
            }

            public string StandardOutput
            {
                get;
            }

            public string StandardError
            {
                get;
            }
        }
    }
}

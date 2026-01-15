using ASCOM.Tools;
using Xunit;

namespace TransformTests
{
    public class TransformLoggerTests
    {
        readonly TraceLogger TL = new TraceLogger("TransformTest1", true);
        Transform transform;

        [Fact]
        public void CanCreateInstanc()
        {
            transform = new Transform(null);
            Assert.NotNull(transform);

            Assert.Equal(0.0, transform.DeltaUT1);
            transform.Dispose();
            transform = null;



            ConsoleLogger consLogger = new ConsoleLogger();
            consLogger.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Debug);
            consLogger.Log(ASCOM.Common.Interfaces.LogLevel.Debug, "Console logger created");
            transform = new Transform(consLogger);
            Assert.NotNull(transform);

            transform = new Transform(TL);
            Assert.NotNull(transform);

            Assert.Equal(0.0, transform.DeltaUT1);

            //Assert.True(TL.Enabled);

            //TL.LogMessage("CreateLog", FIRST_LOG_LINE);
            //TL.Enabled = false;

            //Assert.False(TL.Enabled);

            //TL.LogMessage("CreateLog", FIRST_LOG_LINE);

            //string logFile = Path.Combine(TL.LogFilePath, TL.LogFileName);

            //TL.Enabled = false;
            //TL.Dispose();

            //string[] lines = File.ReadAllLines(logFile);

            //Assert.Single<string>(lines);

        }
    }
}

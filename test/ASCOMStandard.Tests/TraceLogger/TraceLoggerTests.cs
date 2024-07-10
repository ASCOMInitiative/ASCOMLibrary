using ASCOM;
using System;
using System.IO;
using Xunit;

namespace TraceLoggerTests
{
    public class TraceLoggerTests
    {
        private const int IDENTIFIER_OFFSET = 14;
        private const int IDENTIFIER_WIDTH_DEFAULT = 25;
        private const int IDENTIFIER_WIDTH_TEST_VALUE = 40;

        private const string FIRST_LOG_LINE = "Initial message";

        private const string SECOND_LOG_LINE_PART1 = "Second message";
        private const string SECOND_LOG_LINE_PART2 = "After CrLf";
        private const string SECOND_LOG_LINE = SECOND_LOG_LINE_PART1 + "\r\n" + SECOND_LOG_LINE_PART2;
        private const string SECOND_LOG_LINE_DONT_RESPECT_OUTPUT = SECOND_LOG_LINE_PART1 + "[0D][0A]" + SECOND_LOG_LINE_PART2;

        private const string UNPRINTABLE_LOG_LINE_START = "Unprintable characters 00, 17, 11, 31, 127: ";
        private const string UNPRINTABLE_LOG_LINE = UNPRINTABLE_LOG_LINE_START + "\u0000\u0017\u0011\u001F\u007F";
        private const string UNPRINTABLE_LOG_LINE_OUTPUT = UNPRINTABLE_LOG_LINE_START + "[00][17][11][1F][7F]";

        [Fact]
        public void CantWriteWhenDisabled()
        {
            ASCOM.Tools.TraceLogger TL = new(nameof(CantWriteWhenDisabled), true);
            Assert.True(TL.Enabled);

            TL.LogMessage("CreateLog", FIRST_LOG_LINE);
            TL.Enabled = false;

            Assert.False(TL.Enabled);

            TL.LogMessage("CreateLog", SECOND_LOG_LINE_PART1);

            string logFile = Path.Combine(TL.LogFilePath, TL.LogFileName);

            TL.Enabled = false;
            TL.Dispose();

            string[] lines = File.ReadAllLines(logFile);

            Assert.Single<string>(lines);
        }

        [Fact]
        public void DefaultidentifierWidth()
        {
            ASCOM.Tools.TraceLogger TL = new(nameof(DefaultidentifierWidth), true);
            Assert.Equal(IDENTIFIER_WIDTH_DEFAULT, TL.IdentifierWidth);

            TL.LogMessage("CreateLog", FIRST_LOG_LINE);

            string logFile = Path.Combine(TL.LogFilePath, TL.LogFileName);

            TL.Enabled = false;
            TL.Dispose();

            string[] lines = File.ReadAllLines(logFile);

            Assert.Equal("I", lines[0].Substring(IDENTIFIER_WIDTH_DEFAULT + IDENTIFIER_OFFSET, 1)); // Test that the identifier width changes
        }

        [Fact]
        public void GoodIdentifierWidth()
        {
            ASCOM.Tools.TraceLogger TL = new(nameof(GoodIdentifierWidth), true)
            {
                IdentifierWidth = IDENTIFIER_WIDTH_TEST_VALUE
            };
            Assert.Equal(IDENTIFIER_WIDTH_TEST_VALUE, TL.IdentifierWidth);

            TL.LogMessage("CreateLog", FIRST_LOG_LINE);

            string logFile = Path.Combine(TL.LogFilePath, TL.LogFileName);

            TL.Enabled = false;
            TL.Dispose();

            string[] lines = File.ReadAllLines(logFile);

            Assert.Equal("I", lines[0].Substring(IDENTIFIER_WIDTH_TEST_VALUE + IDENTIFIER_OFFSET, 1)); // Test that the identifier width changes
        }

        [Fact]
        public void BadIdentifierWidth()
        {
            ASCOM.Tools.TraceLogger TL = new(nameof(BadIdentifierWidth), true);
            Assert.Equal(IDENTIFIER_WIDTH_DEFAULT, TL.IdentifierWidth);

            Exception ex = Assert.Throws<InvalidValueException>(() => TL.IdentifierWidth = -1);
            Assert.Equal("IdentifierWidth - '-1' is an invalid value. The valid range is: 0 to 2,147,483,647.", ex.Message);

            TL.Enabled = false;
            TL.Dispose();
        }

        [Fact]
        public void DontRespectCrLf()
        {
            ASCOM.Tools.TraceLogger TL = new(nameof(DontRespectCrLf), true);

            Assert.True(TL.RespectCrLf);
            TL.RespectCrLf = false;
            Assert.False(TL.RespectCrLf);

            TL.LogMessage("UnprintableTest", SECOND_LOG_LINE);

            string logFile = Path.Combine(TL.LogFilePath, TL.LogFileName);

            TL.Enabled = false;
            TL.Dispose();

            string[] lines = File.ReadAllLines(logFile);

            Assert.Contains(SECOND_LOG_LINE_DONT_RESPECT_OUTPUT, lines[0]);
        }

        [Fact]
        public void AutoPathAutoName()
        {
            ASCOM.Tools.TraceLogger TL = new(nameof(AutoPathAutoName), true);
            string originalLogFileName = TL.LogFileName;
            string originalLogFilePath = TL.LogFilePath;

            Assert.Equal(IDENTIFIER_WIDTH_DEFAULT, TL.IdentifierWidth);

            TL.LogMessage("CreateLog", FIRST_LOG_LINE);
            TL.LogMessage("CrLfTest", SECOND_LOG_LINE);
            TL.LogMessage("UnprintableTest", UNPRINTABLE_LOG_LINE);
            TL.LogMessage("LogFileName", TL.LogFileName);
            TL.LogMessage("LogFilePath", TL.LogFilePath);
            TL.LogMessage("Original LogFileName", originalLogFileName.ToString());
            TL.LogMessage("Original LogFilePath", originalLogFilePath.ToString());

            string logFile = Path.Combine(TL.LogFilePath, TL.LogFileName);

            Assert.Contains($"ASCOM.{nameof(AutoPathAutoName)}", TL.LogFileName);
            // Assert.Contains(@"ASCOM\Logs", TL.LogFilePath);

            TL.Enabled = false;
            TL.Dispose();

            string[] lines = File.ReadAllLines(logFile);

            Assert.Contains(FIRST_LOG_LINE, lines[0]);
            Assert.Contains(SECOND_LOG_LINE_PART1, lines[1]);
            Assert.Contains(SECOND_LOG_LINE_PART2, lines[2]);
            Assert.Contains(UNPRINTABLE_LOG_LINE_OUTPUT, lines[3]);
        }

        [Fact]
        public void AutoPathAutoNameUtc()
        {
            ASCOM.Tools.TraceLogger TL = new(nameof(AutoPathAutoNameUtc), true)
            {
                UseUtcTime = true
            };

            string originalLogFileName = TL.LogFileName;
            string originalLogFilePath = TL.LogFilePath;

            Assert.Equal(IDENTIFIER_WIDTH_DEFAULT, TL.IdentifierWidth);

            TL.LogMessage("CreateLog", FIRST_LOG_LINE);
            TL.LogMessage("CrLfTest", SECOND_LOG_LINE);
            TL.LogMessage("UnprintableTest", UNPRINTABLE_LOG_LINE);
            TL.LogMessage("LogFileName", TL.LogFileName);
            TL.LogMessage("LogFilePath", TL.LogFilePath);
            TL.LogMessage("Original LogFileName", originalLogFileName.ToString());
            TL.LogMessage("Original LogFilePath", originalLogFilePath.ToString());

            string logFile = Path.Combine(TL.LogFilePath, TL.LogFileName);

            Assert.Contains($"ASCOM.{nameof(AutoPathAutoNameUtc)}", TL.LogFileName);
            // Assert.Contains(@"ASCOM\Logs", TL.LogFilePath);

            TL.Enabled = false;
            TL.Dispose();

            string[] lines = File.ReadAllLines(logFile);

            Assert.Contains(FIRST_LOG_LINE, lines[0]);
            Assert.Contains(SECOND_LOG_LINE_PART1, lines[1]);
            Assert.Contains(SECOND_LOG_LINE_PART2, lines[2]);
            Assert.Contains(UNPRINTABLE_LOG_LINE_OUTPUT, lines[3]);
        }

        [Fact]
        public void AutoPathManualName()
        {
            const string TEST_FILE_NAME = "AutoPathManualName.txt";

            ASCOM.Tools.TraceLogger TL = new(TEST_FILE_NAME, "", nameof(AutoPathManualName), true)
            {
                Enabled = true
            };
            string originalLogFileName = TL.LogFileName;
            string originalLogFilePath = TL.LogFilePath;

            TL.LogMessage("CreateLog", FIRST_LOG_LINE);
            TL.LogMessage("CrLfTest", SECOND_LOG_LINE);
            TL.LogMessage("UnprintableTest", UNPRINTABLE_LOG_LINE);
            TL.LogMessage("LogFileName", TL.LogFileName);
            TL.LogMessage("LogFilePath", TL.LogFilePath);
            TL.LogMessage("Original LogFileName", originalLogFileName.ToString());
            TL.LogMessage("Original LogFilePath", originalLogFilePath.ToString());

            string logFile = Path.Combine(TL.LogFilePath, TL.LogFileName);

            Assert.Equal(TEST_FILE_NAME, TL.LogFileName);
            // Assert.Contains(@"ASCOM\Logs", TL.LogFilePath);

            TL.Enabled = false;
            TL.Dispose();

            string[] lines = File.ReadAllLines(logFile);

            Assert.Contains(FIRST_LOG_LINE, lines[0]);
            Assert.Contains(SECOND_LOG_LINE_PART1, lines[1]);
            Assert.Contains(SECOND_LOG_LINE_PART2, lines[2]);
            Assert.Contains(UNPRINTABLE_LOG_LINE_OUTPUT, lines[3]);
        }

        [Fact]
        public void ManualPathAutolName()
        {
            ASCOM.Tools.TraceLogger TL = new("", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ASCOM"), nameof(ManualPathAutolName), true)
            {
                Enabled = true
            };
            string originalLogFileName = TL.LogFileName;
            string originalLogFilePath = TL.LogFilePath;

            TL.LogMessage("CreateLog", FIRST_LOG_LINE);
            TL.LogMessage("CrLfTest", SECOND_LOG_LINE);
            TL.LogMessage("UnprintableTest", UNPRINTABLE_LOG_LINE);
            TL.LogMessage("LogFileName", TL.LogFileName);
            TL.LogMessage("LogFilePath", TL.LogFilePath);
            TL.LogMessage("Original LogFileName", originalLogFileName.ToString());
            TL.LogMessage("Original LogFilePath", originalLogFilePath.ToString());

            string logFile = Path.Combine(TL.LogFilePath, TL.LogFileName);

            Assert.Contains($"ASCOM.{nameof(ManualPathAutolName)}", TL.LogFileName);
            Assert.Contains("ASCOM", TL.LogFilePath);

            TL.Enabled = false;
            TL.Dispose();

            string[] lines = File.ReadAllLines(logFile);

            Assert.Contains(FIRST_LOG_LINE, lines[0]);
            Assert.Contains(SECOND_LOG_LINE_PART1, lines[1]);
            Assert.Contains(SECOND_LOG_LINE_PART2, lines[2]);
            Assert.Contains(UNPRINTABLE_LOG_LINE_OUTPUT, lines[3]);
        }

        [Fact]
        public void ManualPathManuallName()
        {
            const string TEST_FILE_NAME = "ManualPathManualName.txt";

            ASCOM.Tools.TraceLogger TL = new(TEST_FILE_NAME, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ASCOM"), nameof(ManualPathManuallName), true)
            {
                Enabled = true
            };
            string originalLogFileName = TL.LogFileName;
            string originalLogFilePath = TL.LogFilePath;

            TL.LogMessage("CreateLog", FIRST_LOG_LINE);
            TL.LogMessage("CrLfTest", SECOND_LOG_LINE);
            TL.LogMessage("UnprintableTest", UNPRINTABLE_LOG_LINE);
            TL.LogMessage("LogFileName", TL.LogFileName);
            TL.LogMessage("LogFilePath", TL.LogFilePath);
            TL.LogMessage("Original LogFileName", originalLogFileName.ToString());
            TL.LogMessage("Original LogFilePath", originalLogFilePath.ToString());

            string logFile = Path.Combine(TL.LogFilePath, TL.LogFileName);

            Assert.Contains(TEST_FILE_NAME, TL.LogFileName);
            Assert.Contains(@"ASCOM", TL.LogFilePath);

            TL.Enabled = false;
            TL.Dispose();

            string[] lines = File.ReadAllLines(logFile);

            Assert.Contains(FIRST_LOG_LINE, lines[0]);
            Assert.Contains(SECOND_LOG_LINE_PART1, lines[1]);
            Assert.Contains(SECOND_LOG_LINE_PART2, lines[2]);
            Assert.Contains(UNPRINTABLE_LOG_LINE_OUTPUT, lines[3]);
        }

    }
}

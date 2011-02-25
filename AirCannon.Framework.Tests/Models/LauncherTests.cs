using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using AirCannon.Framework.Models;
using AirCannon.Framework.Tests.Utilities;
using MbUnit.Framework;

namespace AirCannon.Framework.Tests.Models
{
    /// <summary>
    ///   Verifies that <see cref = "Launcher" /> works correctly.
    /// </summary>
    [TestFixture]
    public class LauncherTests
    {
        private string mTempDir;

        /// <summary>
        ///   Verifies that <see cref = "Launcher.AggregateEnvironmentVariables" /> works correctly.
        /// </summary>
        /// <remarks>
        ///   <see cref = "Launcher.AggregateEnvironmentVariables" /> should start with the topmost
        ///   <see cref = "LaunchGroup" /> and path down, overriding/adding existing/missing variables
        ///   down to the <see cref = "Launcher" />.
        /// </remarks>
        [Test]
        public void AggregateEnvironmentVariablesTest()
        {
            var launcher = new Launcher();
            LaunchGroup group =
                new LaunchGroup(null,
                                new[]
                                    {
                                        new LaunchGroup(null,
                                                        null,
                                                        new[]
                                                            {
                                                                launcher
                                                            }),
                                    });

            group.EnvironmentVariables.UpdateWith(
                new KeyValuePair<string, string>("A", "A"),
                new KeyValuePair<string, string>("B", "B"),
                new KeyValuePair<string, string>("D", "D"));

            group.Groups[0].EnvironmentVariables.UpdateWith(
                new KeyValuePair<string, string>("C", "C"),
                new KeyValuePair<string, string>("B", "B2"));

            launcher.EnvironmentVariables.UpdateWith(
                new KeyValuePair<string, string>("D", "D2"),
                new KeyValuePair<string, string>("E", "E"));

            var envVars = launcher.AggregateEnvironmentVariables();

            Assert2.ContainsKeyAndValue(envVars, "A", "A");
            Assert2.ContainsKeyAndValue(envVars, "B", "B2");
            Assert2.ContainsKeyAndValue(envVars, "C", "C");
            Assert2.ContainsKeyAndValue(envVars, "D", "D2");
            Assert2.ContainsKeyAndValue(envVars, "E", "E");
        }

        /// <summary>
        ///   Verifies the Launcher runs a simple script correctly.
        /// </summary>
        [Test, Timeout(60)]
        public void LaunchTest()
        {
            const string SCRIPT_NAME = "test.bat";
            string scriptPath = Path.Combine(mTempDir, SCRIPT_NAME);
            string testFilePath = Path.Combine(mTempDir, "test.txt");
            string script = @"@echo test>""" + testFilePath + @"""";


            File.WriteAllText(scriptPath, script);

            Launcher launcher = new Launcher();
            launcher.File = scriptPath;

            Process process = launcher.Launch();
            process.WaitForExit();

            Assert.IsTrue(process.HasExited, "Process should have exited");
            Assert.IsTrue(File.Exists(testFilePath), "Process should have created the file '{0}'", testFilePath);
        }

        /// <summary>
        ///   Creates a temporary folder for tests.
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
            mTempDir = Path.GetTempFileName();
            File.Delete(mTempDir);
            Directory.CreateDirectory(mTempDir);

            Assert.IsTrue(Directory.Exists(mTempDir),
                          "Temporary directory '{0}' could not be created", mTempDir);
        }

        /// <summary>
        ///   Deletes the temporary folder.
        /// </summary>
        [TearDown]
        public void TestTeardown()
        {
            //Give some time for files to unlock
            Thread.Yield();

            Directory.Delete(mTempDir, true);

            Assert.IsFalse(Directory.Exists(mTempDir),
                           "Temporary directory '{0}' could not be deleted", mTempDir);

            mTempDir = string.Empty;
        }
    }
}
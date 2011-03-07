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
                new EnvironmentVariable("A", "A"),
                new EnvironmentVariable("B", "B"),
                new EnvironmentVariable("D", "D"));

            group.Groups[0].EnvironmentVariables.UpdateWith(
                new EnvironmentVariable("C", "C"),
                new EnvironmentVariable("B", "B2"));

            launcher.EnvironmentVariables.UpdateWith(
                new EnvironmentVariable("D", "D2"),
                new EnvironmentVariable("E", "E"));

            var envVars = launcher.AggregateEnvironmentVariables();

            Assert.Contains(envVars, new EnvironmentVariable("A", "A"));
            Assert.Contains(envVars, new EnvironmentVariable("B", "B2"));
            Assert.Contains(envVars, new EnvironmentVariable("C", "C"));
            Assert.Contains(envVars, new EnvironmentVariable("D", "D2"));
            Assert.Contains(envVars, new EnvironmentVariable("E", "E"));
        }

        /// <summary>
        ///   Verifies that two launchers are equal.
        /// </summary>
        [Test]
        public void AreEqualTest()
        {
            var launcher = new Launcher();
            var other = new Launcher();

            Assert.AreEqual(launcher, other, "Blank launchers should be equal");

            launcher.Arguments = "a";
            other.Arguments = "a";
            Assert.AreEqual(launcher, other, "Launchers with the same arguments should be equal");

            launcher.File = "file";
            other.File = "file";
            Assert.AreEqual(launcher, other, "Launchers with the same file should be equal");

            launcher.Name = "test";
            other.Name = "test";
            Assert.AreEqual(launcher, other, "Launchers with the same name should be equal");

            launcher.WorkingDirectory = "wd";
            other.WorkingDirectory = "wd";
            Assert.AreEqual(launcher, other, "Launchers with the same working directory should be equal");

            launcher.EnvironmentVariables["a"] = "a";
            other.EnvironmentVariables["a"] = "a";
            launcher.EnvironmentVariables["b"] = "b";
            other.EnvironmentVariables["b"] = "b";
            Assert.AreEqual(launcher, other, "Launchers with the same environment variables should be equal");
        }

        /// <summary>
        ///   Verifies that two launchers are not equal.
        /// </summary>
        [Test]
        public void AreNotEqualTest()
        {
            var launcher = new Launcher {Arguments = "a"};
            var other = new Launcher {Arguments = "b"};
            Assert.AreNotEqual(launcher, other, "Launchers with different arguments should not be equal");


            launcher = new Launcher {File = "a"};
            other = new Launcher {File = "b"};
            Assert.AreNotEqual(launcher, other, "Launchers with different files should not be equal");

            launcher = new Launcher {Name = "a"};
            other = new Launcher {Name = "b"};
            Assert.AreNotEqual(launcher, other, "Launchers with different names should not be equal");

            launcher = new Launcher {WorkingDirectory = "a"};
            other = new Launcher {WorkingDirectory = "b"};
            Assert.AreNotEqual(launcher, other, "Launchers with different working directories should not be equal");

            launcher = new Launcher();
            other = new Launcher();
            launcher.EnvironmentVariables["a"] = "a";
            other.EnvironmentVariables["b"] = "b";
            Assert.AreNotEqual(launcher, other, "Launchers with different environment variables should not be equal");
        }

        /// <summary>
        ///   Verifies the <see cref = "Launcher.HasChanges" /> property works correctly.
        /// </summary>
        [Test]
        public void HasChangesTest()
        {
            Launcher launcher = new Launcher();
            Assert.IsFalse(launcher.HasChanges);

            launcher.File = "a";
            Assert.IsTrue(launcher.HasChanges, "Updating File should cause HasChanges to be true");

            launcher.HasChanges = false;
            launcher.Name = "a";
            Assert.IsTrue(launcher.HasChanges, "Updating Name should cause HasChanges to be true");

            launcher.HasChanges = false;
            launcher.Parent = new LaunchGroup();
            Assert.IsTrue(launcher.HasChanges, "Updating Parent should cause HasChanges to be true");

            launcher.HasChanges = false;
            launcher.WorkingDirectory = "a";
            Assert.IsTrue(launcher.HasChanges, "Updating WorkingDirectory should cause HasChanges to be true");

            launcher.HasChanges = false;
            launcher.EnvironmentVariables.Add(new EnvironmentVariable("a", "b"));
            Assert.IsTrue(launcher.HasChanges, "New environment variables should cause HasChanges to be true");

            launcher.HasChanges = false;
            launcher.EnvironmentVariables["a"] = "c";
            Assert.IsTrue(launcher.HasChanges, "Updating environment variables should cause HasChanges to be true");
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
            Thread.Sleep(500);

            Directory.Delete(mTempDir, true);

            Assert.IsFalse(Directory.Exists(mTempDir),
                           "Test cleanup - Temporary directory '{0}' could not be deleted", mTempDir);

            mTempDir = string.Empty;
        }
    }
}
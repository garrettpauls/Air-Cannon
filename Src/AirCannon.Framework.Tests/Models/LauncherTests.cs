using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using AirCannon.Framework.Models;
using AirCannon.Framework.Utilities;
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
        ///   Verifies the File property validates correctly.
        /// </summary>
        [Test]
        public void FileValidationTest()
        {
            Launcher launcher = new Launcher();
            launcher.File = string.Empty;

            var fileError = launcher[Property<Launcher>.Name(p => p.File)];

            Assert.IsFalse(string.IsNullOrEmpty(fileError), "The File property should cause an error when empty");

            string nonexistentFile;
            do
            {
                nonexistentFile = Guid.NewGuid().ToString();
            } while (File.Exists(nonexistentFile));

            launcher.File = nonexistentFile;
            fileError = launcher[Property<Launcher>.Name(p => p.File)];
            Assert.IsFalse(string.IsNullOrEmpty(fileError),
                           "The File property should cause an error when the file doesn't exist");

            launcher.File = _GetExistingFilePath();
            fileError = launcher[Property<Launcher>.Name(p => p.File)];
            Assert.IsTrue(File.Exists(launcher.File),
                          "The file {0} must exist for this test to complete successfully",
                          launcher.File);
            Assert.IsTrue(string.IsNullOrEmpty(fileError),
                          "The File property should not cause an error when the file exists");
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
        ///   Verifies the Launcher runs a script with the correct environment variables.
        /// </summary>
        [Test, Timeout(60)]
        public void LaunchWithEnvironmentVariablesTest()
        {
            const string SCRIPT_NAME = "test.bat";
            string scriptPath = Path.Combine(mTempDir, SCRIPT_NAME);
            string testFilePath = Path.Combine(mTempDir, "test.txt");
            string script = @"
@echo %v1%>>""" + testFilePath + @"""
@echo %v2%>>""" + testFilePath +
                            @"""
@echo %v3%>>""" + testFilePath + @"""";

            var v1 = new EnvironmentVariable("v1", "{E16620DD-D150-4F2C-B4AE-DBC54E72373C}");
            var v2 = new EnvironmentVariable("v2", "{4EB5E514-BF79-43B1-A535-0BB5AAD4D60F}");
            var v3 = new EnvironmentVariable("v3", "{9DD27426-93AF-46D0-81DE-4073EDBC7664}");

            File.WriteAllText(scriptPath, script);

            Launcher launcher = new Launcher();
            launcher.File = scriptPath;
            launcher.EnvironmentVariables.Add(v1);
            launcher.EnvironmentVariables.Add(v2);
            launcher.EnvironmentVariables.Add(v3);

            Process process = launcher.Launch();
            process.WaitForExit();

            Assert.IsTrue(process.HasExited, "Process should have exited");
            Assert.IsTrue(File.Exists(testFilePath), "Process should have created the file '{0}'", testFilePath);

            string[] contents = File.ReadAllLines(testFilePath);
            Assert.Count(3, contents, "There should have been three lines in the output file");
            Assert.AreEqual(v1.Value, contents[0], "The first environment variable value was wrong");
            Assert.AreEqual(v2.Value, contents[1], "The second environment variable value was wrong");
            Assert.AreEqual(v3.Value, contents[2], "The third environment variable value was wrong");
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

        /// <summary>
        ///   Verifies the WorkingDirectory property validates correctly.
        /// </summary>
        [Test]
        public void WorkingDirectoryValidationTest()
        {
            Launcher launcher = new Launcher();
            launcher.WorkingDirectory = string.Empty;

            var wdError = launcher[Property<Launcher>.Name(p => p.WorkingDirectory)];

            Assert.IsTrue(string.IsNullOrEmpty(wdError),
                          "The WorkingDirectory property should not cause an error if it is empty");

            string nonexistentDirectory;
            do
            {
                nonexistentDirectory = Guid.NewGuid().ToString();
            } while (Directory.Exists(nonexistentDirectory));

            launcher.WorkingDirectory = nonexistentDirectory;
            wdError = launcher[Property<Launcher>.Name(p => p.WorkingDirectory)];
            Assert.IsFalse(string.IsNullOrEmpty(wdError),
                           "The WorkingDirectory property should cause an error when the directory doesn't exist");

            launcher.WorkingDirectory = Path.GetDirectoryName(_GetExistingFilePath());
            wdError = launcher[Property<Launcher>.Name(p => p.WorkingDirectory)];
            Assert.IsTrue(Directory.Exists(launcher.WorkingDirectory),
                          "The directory {0} must exist for this test to complete successfully",
                          launcher.WorkingDirectory);
            Assert.IsTrue(string.IsNullOrEmpty(wdError),
                          "The WorkingDirectory property should not cause an error when the directory exists");
        }

        private string _GetExistingFilePath()
        {
            return new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
        }
    }
}
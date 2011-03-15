using System.IO;
using AirCannon.Framework.Models;
using NUnit.Framework;

namespace AirCannon.Framework.Tests.Models
{
    /// <summary>
    ///   Tests for <see cref = "LaunchGroup" />.
    /// </summary>
    [TestFixture]
    public class LaunchGroupTests
    {
        /// <summary>
        ///   Verifies the <see cref = "LaunchGroup.ClearAllHasChanges" /> method sets all child
        ///   <see cref = "LaunchGroup.HasChanges" /> and all <see cref = "Launcher.HasChanges" />
        ///   properties to false.
        /// </summary>
        [Test]
        public void ClearAllHasChangesTest()
        {
            var group = new LaunchGroup(null, new[]
                                                  {
                                                      new LaunchGroup(null, new[]
                                                                                {
                                                                                    new LaunchGroup {HasChanges = true},
                                                                                },
                                                                      new[]
                                                                          {
                                                                              new Launcher {HasChanges = true},
                                                                          }) {HasChanges = true},
                                                  },
                                        new[]
                                            {
                                                new Launcher {HasChanges = true},
                                            }) {HasChanges = true};

            Assert.IsTrue(group.HasChanges, "All Groups and Launchers should have changes");
            Assert.IsTrue(group.Groups[0].HasChanges, "All Groups and Launchers should have changes");
            Assert.IsTrue(group.Groups[0].Groups[0].HasChanges, "All Groups and Launchers should have changes");
            Assert.IsTrue(group.Groups[0].Launchers[0].HasChanges, "All Groups and Launchers should have changes");
            Assert.IsTrue(group.Launchers[0].HasChanges, "All Groups and Launchers should have changes");

            group.ClearAllHasChanges();

            Assert.IsFalse(group.HasChanges, "All Groups and Launchers should not have changes");
            Assert.IsFalse(group.Groups[0].HasChanges, "All Groups and Launchers should not have changes");
            Assert.IsFalse(group.Groups[0].Groups[0].HasChanges, "All Groups and Launchers should not have changes");
            Assert.IsFalse(group.Groups[0].Launchers[0].HasChanges, "All Groups and Launchers should not have changes");
            Assert.IsFalse(group.Launchers[0].HasChanges, "All Groups and Launchers should not have changes");
        }

        /// <summary>
        ///   Verifies the <see cref = "LaunchGroup.HasChanges" /> property works correctly.
        /// </summary>
        [Test]
        public void HasChangesTest()
        {
            LaunchGroup group = new LaunchGroup();
            LaunchGroup childGroup = new LaunchGroup();
            Launcher childLauncher = new Launcher();

            Assert.IsFalse(group.HasChanges);

            group.Name = "a";
            Assert.IsTrue(group.HasChanges, "Changing Name should cause HasChanges to be true");

            group.ClearAllHasChanges();
            group.EnvironmentVariables.Add(new EnvironmentVariable("A", "A"));
            Assert.IsTrue(group.HasChanges, "New environment variables should cause HasChanges to be true");

            group.ClearAllHasChanges();
            group.EnvironmentVariables["A"] = "B";
            Assert.IsTrue(group.HasChanges, "Updating environment variables should cause HasChanges to be true");

            group.ClearAllHasChanges();
            group.Groups.Add(childGroup);
            Assert.IsTrue(group.HasChanges, "New child groups should cause HasChages to be true");

            group.ClearAllHasChanges();
            childGroup.Name = "a";
            Assert.IsTrue(group.HasChanges, "Changes to child groups should cause HasChanges to be true");

            group.ClearAllHasChanges();
            group.Launchers.Add(childLauncher);
            Assert.IsTrue(group.HasChanges, "New child launchers should cause HasChanges to be true");

            group.ClearAllHasChanges();
            childLauncher.Name = "A";
            Assert.IsTrue(group.HasChanges, "Changes to child launchers should cause HasChanges to be true");
        }

        /// <summary>
        ///   Verifies that a saved <see cref = "LaunchGroup" /> can be loaded to create an equivalent one.
        /// </summary>
        [Test]
        public void SaveLoadTest()
        {
            var tempFile = Path.GetTempFileName();

            var launchGroup = new LaunchGroup
                                  {
                                      Name = "Root",
                                      EnvironmentVariables =
                                          {
                                              {"Var1", "Val1"},
                                              {"Var2", "Val2"}
                                          }
                                  };
            launchGroup.Groups.Add(new LaunchGroup
                                       {
                                           Name = "LG1",
                                           EnvironmentVariables =
                                               {
                                                   {"Var3", "Val3"}
                                               }
                                       });
            launchGroup.Launchers.Add(new Launcher
                                          {
                                              Arguments = "abc",
                                              File = "somefile",
                                              Name = "test",
                                              WorkingDirectory = "aaaa",
                                              EnvironmentVariables =
                                                  {
                                                      {"Var4", "Val4"}
                                                  }
                                          });

            launchGroup.SaveTo(tempFile);

            var newGroup = LaunchGroup.LoadFrom(tempFile);

            Assert.AreNotSame(launchGroup, newGroup, "LoadFrom should have created a new group");

            Assert.AreEqual(launchGroup, newGroup, "LoadFrom should have loaded a group equal to the saved one");
        }
    }
}
using System;
using System.IO;
using System.Linq;
using AirCannon.Framework.Models;
using AirCannon.Framework.Utilities;
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
        ///   Determines if two <see cref = "LaunchGroup" />s are semantically equal.
        /// </summary>
        private void _Equal(LaunchGroup left, LaunchGroup right)
        {
            Assert.AreEqual(left.Name, right.Name, "Names must be equal");
            Assert.AreEqual(left.EnvironmentVariables, right.EnvironmentVariables,
                            "Environment variables must be equal");

            Assert.AreEqual(left.LaunchGroups.Count, right.LaunchGroups.Count,
                            "Group collections must have the same count");

            foreach (var comp in left.LaunchGroups.Zip(right.LaunchGroups, LinqEx.ToTuple))
            {
                _Equal(comp.Item1, comp.Item2);
            }

            Assert.AreEqual(left.Launchers.Count, right.Launchers.Count,
                            "Launcher collections must have the same count");

            foreach (var comp in left.Launchers.Zip(right.Launchers, LinqEx.ToTuple))
            {
                _Equal(comp.Item1, comp.Item2);
            }
        }

        /// <summary>
        ///   Determines if two <see cref = "Launcher" />s are semantically equal.
        /// </summary>
        private void _Equal(Launcher left, Launcher right)
        {
            Assert.AreEqual(left.Arguments, right.Arguments, "Launcher arguments must be equal");
            Assert.AreEqual(left.EnvironmentVariables, right.EnvironmentVariables, "EnvironmentVariables must be equal");
            Assert.AreEqual(left.File, right.File, "File must be equal");
            Assert.AreEqual(left.Name, right.Name, "Name must be equal");
            Assert.AreEqual(left.WorkingDirectory, right.WorkingDirectory, "WorkingDirectories must be equal");
        }

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
            Assert.IsTrue(group.LaunchGroups[0].HasChanges, "All Groups and Launchers should have changes");
            Assert.IsTrue(group.LaunchGroups[0].LaunchGroups[0].HasChanges, "All Groups and Launchers should have changes");
            Assert.IsTrue(group.LaunchGroups[0].Launchers[0].HasChanges, "All Groups and Launchers should have changes");
            Assert.IsTrue(group.Launchers[0].HasChanges, "All Groups and Launchers should have changes");

            group.ClearAllHasChanges();

            Assert.IsFalse(group.HasChanges, "All Groups and Launchers should not have changes");
            Assert.IsFalse(group.LaunchGroups[0].HasChanges, "All Groups and Launchers should not have changes");
            Assert.IsFalse(group.LaunchGroups[0].LaunchGroups[0].HasChanges, "All Groups and Launchers should not have changes");
            Assert.IsFalse(group.LaunchGroups[0].Launchers[0].HasChanges, "All Groups and Launchers should not have changes");
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
            group.LaunchGroups.Add(childGroup);
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
            launchGroup.LaunchGroups.Add(new LaunchGroup
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

            _Equal(launchGroup, newGroup);
        }
    }
}
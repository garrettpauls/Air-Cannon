using System.IO;
using AirCannon.Framework.Models;
using MbUnit.Framework;

namespace AirCannon.Framework.Tests.Models
{
    /// <summary>
    ///   Tests for <see cref = "LaunchGroup" />.
    /// </summary>
    [TestFixture]
    public class LaunchGroupTests
    {
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
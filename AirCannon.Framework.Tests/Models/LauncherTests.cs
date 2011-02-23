using System.Collections.Generic;
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
    }
}
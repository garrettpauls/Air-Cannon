using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AirCannon.Framework.Services;
using MbUnit.Framework;

namespace AirCannon.Framework.Tests.Services
{
    [TestFixture]
    public class ServiceTests
    {
        /// <summary>
        ///   Verifies that service instances can be switched with <see cref = "Service{TService}.Register" />.
        /// </summary>
        [Test]
        public void ServiceSwitchTest()
        {
            TestService first = new TestService();
            TestService second = new TestService();

            Assert.AreNotSame(first, second, "This test requires two different instances of ITestService");

            Service<ITestService>.Register(first);
            Assert.AreSame(first, Service<ITestService>.Instance, "First service instance expected");

            Service<ITestService>.Register(second);
            Assert.AreSame(second, Service<ITestService>.Instance, "Service was not switched");
        }

        /// <summary>
        ///   Verifies that <see cref = "Service{TService}.Register" /> and 
        ///   <see cref = "Service{TService}.Instance" /> work correctly.
        /// </summary>
        [Test]
        public void ServiceTest()
        {
            Service<ITestService>.Register(new TestService());

            Assert.IsNotNull(Service<ITestService>.Instance, "Instance should not be null");
            Assert.IsInstanceOfType<TestService>(Service<ITestService>.Instance, "Instance should be a TestService");
        }

        /// <summary>
        ///   Verifies that an unregistered service is null.
        /// </summary>
        [Test]
        public void UnregisteredServiceTest()
        {
            Assert.IsNull(Service<INeverImplementedService>.Instance,
                          "An Instance for an unregistered service should be null");
        }

        #region Nested type: INeverImplementedService

        private interface INeverImplementedService
        {
        }

        #endregion

        #region Nested type: ITestService

        private interface ITestService
        {
        }

        #endregion

        #region Nested type: TestService

        private class TestService : ITestService
        {
        }

        #endregion
    }
}
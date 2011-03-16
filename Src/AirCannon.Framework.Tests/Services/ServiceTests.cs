using AirCannon.Framework.Services;
using NUnit.Framework;

namespace AirCannon.Framework.Tests.Services
{
    [TestFixture]
    public class ServiceTests
    {
        private interface INeverImplementedService
        {
        }

        private interface ITestService
        {
        }

        private class TestService : ITestService
        {
        }

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
            Assert.That(Service<ITestService>.Instance, Is.TypeOf<TestService>(),
                        "Instance should be a TestService");
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
    }
}
using System;
using System.Windows.Input;
using AirCannon.Framework.WPF;
using NUnit.Framework;

namespace AirCannon.Framework.Tests.WPF
{
    /// <summary>
    ///   Verifies that <see cref = "DelegateCommand" /> works correctly.
    /// </summary>
    [TestFixture]
    public class DelegateCommandTests
    {
        #region Setup/Teardown

        /// <summary>
        ///   Resets the command.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            mCommand = CreateCommand();
            mCanExecuteCalled = false;
            mExecuteCalled = false;
            mCanExecuteResult = true;
        }

        #endregion

        protected bool mCanExecuteCalled = false;
        protected bool mCanExecuteResult = true;
        protected ICommand mCommand;
        protected bool mExecuteCalled = false;

        protected bool CanExecute()
        {
            mCanExecuteCalled = true;
            return mCanExecuteResult;
        }

        protected virtual ICommand CreateCommand()
        {
            return new DelegateCommand(Execute, CanExecute);
        }

        protected void Execute()
        {
            mExecuteCalled = true;
        }

        /// <summary>
        ///   Verifies that CanExecute is called correctly.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            mCanExecuteResult = true;
            Assert.IsTrue(mCommand.CanExecute(null));
            Assert.IsTrue(mCanExecuteCalled);

            mCanExecuteResult = false;
            Assert.IsFalse(mCommand.CanExecute(null));
            Assert.IsTrue(mCanExecuteCalled);
        }

        /// <summary>
        ///   Verifies the Execute delegate is only called if CanExecute is true.
        /// </summary>
        [Test]
        public void ExecuteTest()
        {
            mCanExecuteResult = true;
            mCommand.Execute(null);
            Assert.IsTrue(mExecuteCalled);

            mCanExecuteResult = false;
            mExecuteCalled = false;
            mCommand.Execute(null);
            Assert.IsFalse(mExecuteCalled);
        }
    }

    [TestFixture]
    public class GenericDelegateCommandTests : DelegateCommandTests
    {
        private int? mCanExecuteCalledWith;
        private int? mExecuteCalledWith;

        protected override ICommand CreateCommand()
        {
            return new DelegateCommand<int?>(_Execute, _CanExecute);
        }

        private bool _CanExecute(int? parameter)
        {
            mCanExecuteCalledWith = parameter;
            return CanExecute();
        }

        private void _Execute(int? parameter)
        {
            mExecuteCalledWith = parameter;
            Execute();
        }

        /// <summary>
        ///   Verifies that CanExecute gets called with the right parameter.
        /// </summary>
        [Test]
        public void CanExecuteParameterTest()
        {
            const int PARAMETER = -4;

            mCanExecuteResult = true;
            mCanExecuteCalledWith = 0;
            Assert.IsTrue(mCommand.CanExecute(PARAMETER));
            Assert.IsTrue(mCanExecuteCalled);
            Assert.AreEqual(PARAMETER, mCanExecuteCalledWith);

            mCanExecuteResult = false;
            mCanExecuteCalledWith = 0;
            Assert.IsFalse(mCommand.CanExecute(PARAMETER));
            Assert.IsTrue(mCanExecuteCalled);
            Assert.AreEqual(PARAMETER, mCanExecuteCalledWith);
        }

        /// <summary>
        ///   Verifies that calling CanExecute with a parameter of the wrong type causes an
        ///   InvalidCastException to be thrown.
        /// </summary>
        [Test, ExpectedException(typeof (InvalidCastException))]
        public void CanExecuteWrongParameterTypeTest()
        {
            mCommand.CanExecute("Blah");
        }

        /// <summary>
        ///   Verifies that Execute gets called with the correct parameter.
        /// </summary>
        [Test]
        public void ExecuteParameterTest()
        {
            const int PARAMETER = 9;
            mCanExecuteResult = true;
            mExecuteCalledWith = 0;
            mCommand.Execute(PARAMETER);
            Assert.IsTrue(mExecuteCalled);
            Assert.AreEqual(PARAMETER, mExecuteCalledWith);

            mCanExecuteResult = false;
            mExecuteCalled = false;
            mExecuteCalledWith = 0;
            mCommand.Execute(PARAMETER);
            Assert.IsFalse(mExecuteCalled);
            Assert.AreEqual(0, mExecuteCalledWith);
        }

        /// <summary>
        ///   Verifies that calling Execute with a parameter of the wrong type causes an
        ///   InvalidCastException to be thrown.
        /// </summary>
        [Test, ExpectedException(typeof (InvalidCastException))]
        public void ExecuteWrongParameterTypeTest()
        {
            mCommand.Execute("Blah");
        }
    }
}
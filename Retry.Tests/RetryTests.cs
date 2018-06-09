using NUnit.Framework;
using RS.Retry;
using System;

namespace RetryUnit.Tests
{
    [TestFixture]
    public class RetryTests
    {
        [Test]
        public void CanCreate()
        {
            var retry = new Retry();
        }

        [Test]
        public void CanUseRunDirectly()
        {
            var retry = new Retry();
            bool check = false;
            retry.Run(() =>
            {
                check = true;
            }, 1);

            Assert.That(check, Is.True);
        }

        [Test]
        public void EnsureDoesNotThrowHandledExption()
        {
            var retry = new Retry();
            retry.HandleException<NotImplementedException>()
                 .Run(() =>
                 {
                     throw new NotImplementedException();
                 }, 1);
        }

        [Test]
        public void EnsureDoesThrowUnhandledException()
        {
            var retry = new Retry();
            Assert.Throws<OutOfMemoryException>(() =>
            retry.HandleException<NotImplementedException>()
                   .Run(() =>
                   {
                       throw new OutOfMemoryException();
                   }, 1));
        }

        [Test]
        public void EnsureSuccessCheckerCalled()
        {
            var successCounter = 0;
            var retry = new Retry();

            retry.SuccessChecker(() =>
                    {
                        successCounter++;
                        return false;
                    })
                   .Run(() =>
                   {
                       //NoOp
                   }, 5);

            Assert.That(successCounter, Is.EqualTo(5));

        }

        [Test]
        public void EnsureAttemptsExhaustedCalled()
        {
            var successCounter = 0;
            var retry = new Retry();

            retry.AttemptsExhausted(() =>
            {
                successCounter++;

            })
            .SuccessChecker(() => false)
            .Run(() =>
            {
                //NoOp
            }, 5);

            Assert.That(successCounter, Is.EqualTo(1));

        }

        [Test]
        public void EnsureAttemptsExhaustedNotCalled()
        {
            var successCounter = 0;
            var retry = new Retry();

            retry.AttemptsExhausted(() =>
            {
                successCounter++;

            })
            .SuccessChecker(() => true)
            .Run(() =>
            {
                //NoOp
            }, 5);

            Assert.That(successCounter, Is.EqualTo(0));

        }

        [Test]
        public void EnsureSuccessCheckerWorks()
        {
            var successCounter = 0;
            var retry = new Retry();
            var success = true;
            retry
            .SuccessChecker(() =>
                {
                    successCounter++;
                    return success = !success;
                }
            )
            .Run(() =>
            {
                //NoOp
            }, 5);

            Assert.That(successCounter, Is.EqualTo(2));

        }
    }
}

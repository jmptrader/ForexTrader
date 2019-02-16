using ForexTrader.Cryptography;
using ForexTrader.Cryptography.Interfaces;
using NUnit.Framework;
using System.Collections.Concurrent;

namespace ForexTrader.Cryptography.Tests
{
    [TestFixture]
    public class DecryptTests
    {
        ConcurrentQueue<object> _loggerQueue;

        [SetUp]
        public void Setup()
        {
            _loggerQueue = new ConcurrentQueue<object>();
        }

        [Test]
        public void AesDecrpytTest()
        {
            
        }
    }
}
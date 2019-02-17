using ForexTrader.Cryptography;
using ForexTrader.Cryptography.Interfaces;
using ForexTrader.Logging.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using NSubstitute;

namespace ForexTrader.Cryptography.Tests
{
    [TestFixture]
    public class DecryptTests
    {
        private ILogger _logger;
        private string _input, _pass;
        private byte[] _iv;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger>();
            _input = "jvdvJIpbtPs6eLIur/xGoXf6wfZCafCcA/D4LZXHoCE=";
            _pass = "SuperSecurePassword";
            _iv = Convert.FromBase64String("MT/RPPJ8HgsvnD49qRIcyA==");
        }

        [Test]
        public void AesDecrpytIvProvidedTest()
        {
            var decryptor = new Decrypt(_logger);

            var res = decryptor.AesDecrypt(_input, _pass, _iv);
        }
    }
}
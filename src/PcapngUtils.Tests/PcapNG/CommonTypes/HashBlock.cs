using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haukcode.PcapngUtils.PcapNG.CommonTypes
{
    [TestFixture]
    public static class HashBlock_Test
    {
        [Test]
        public static void HashBlock_InvalidEnum_Test()
        {
            string md5Hash = "f59b7efafd800e27b47a488d30615c73";
            byte[] utf8Hash = System.Text.Encoding.UTF8.GetBytes(md5Hash);
            byte[] test = { 5 };
            test = test.Concat(utf8Hash).ToArray();

            HashBlock hashBlock = new HashBlock(test);
            Assert.AreEqual(hashBlock.Algorithm, HashBlock.HashAlgorithm.Invalid);
        }

        [Test]
        public static void HashBlock_Md5Test_Test()
        {
            string md5Hash = "f59b7efafd800e27b47a488d30615c73";
            byte[] utf8Hash = System.Text.Encoding.UTF8.GetBytes(md5Hash);
            byte[] test = { (byte)HashBlock.HashAlgorithm.Md5 };
            test = test.Concat(utf8Hash).ToArray();

            HashBlock hashBlock = new HashBlock(test);
            Assert.AreEqual(hashBlock.Algorithm, HashBlock.HashAlgorithm.Md5);
            Assert.AreEqual(hashBlock.StringValue, md5Hash);
        }
    }
}

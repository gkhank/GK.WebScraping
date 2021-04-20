using GK.WebScraping.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GK.WebScraping.Test
{
    [TestClass]
    public class EncryptionTest
    {

        [TestMethod]
        public void TestEncryption()
        {

            EncryptionUtility encryptionUtility = new EncryptionUtility();

            String value = "*lZ[}0mB]*)00(o;";

            string encrypted = encryptionUtility.Encrypt(value);

            string decrprted = encryptionUtility.Decrypt(encrypted);


            Assert.AreEqual(value, decrprted);
        }
    }
}

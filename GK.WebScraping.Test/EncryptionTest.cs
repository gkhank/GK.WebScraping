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

            String value = "*lZ[}0mB]*)00(o";

            string encrypted = encryptionUtility.Encrypt(value);

            string decrprted = encryptionUtility.Decrypt(encrypted);


            Assert.AreEqual(value, decrprted);
        }


        [TestMethod]
        public void TestDecryption()
        {

            EncryptionUtility encryptionUtility = new EncryptionUtility();

            String encrypted = "l9S5dtLibDGvrj2MPM3bvg==";

            string decrprted = encryptionUtility.Decrypt(encrypted);

            string value = "*lZ[}0mB]*)00(o";

            Assert.AreEqual(value, decrprted);
        }
    }
}

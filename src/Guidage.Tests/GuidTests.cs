using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Guidage.Tests
{
    [TestClass]
    public class GuidTests
    {
        public static Guid ToGuid(Int64 value)
        {
            byte[] guidData = new byte[16];
            Array.Copy(BitConverter.GetBytes(value), guidData, 8);
            return new Guid(guidData);
        }

        [TestMethod]
        public void TestMethod1()
        {
            Int64 valueMin = Int64.MinValue;
            Int64 valueMax = Int64.MaxValue;

            var guidMin= ToGuid(valueMin);
            var guidMax = ToGuid(valueMax);
        }
        [TestMethod]
        public void Test_RijndaelManaged_GenerateKey()
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {

                myRijndael.GenerateKey();
                myRijndael.GenerateIV();

                string Key = System.Text.Encoding.UTF8.GetString(myRijndael.Key);
                string IV = System.Text.Encoding.UTF8.GetString(myRijndael.IV);
                
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kasca.Common.Extention;

namespace Kasca.Common.Tests
{
    [TestClass]
    public class ExtentionTests
    {
        [TestMethod]
        public void Base64Test()
        {
            var str = "m";
            var base64Str = str.ToBase64(Encoding.ASCII);
            //base64Str = base64Str.Replace("=", "");
            var fromBase64Str = base64Str.FromBase64(Encoding.ASCII);
        }


        [TestMethod]
        public void DirTest()
        {
            var dir = new Dictionary<string, string>();
            dir["sss"] = "mmmm";
            dir["sss"] = "vvvv";
        }



    }
}

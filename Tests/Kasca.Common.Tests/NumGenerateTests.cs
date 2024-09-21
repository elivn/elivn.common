using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kasca.Common.ComUtils;

namespace Kasca.Common.Tests
{
    [TestClass]
    public class NumGenerateTests:BaseTests
    {
        [TestMethod]
        public async Task PostTest()
        {

            // 12 4095
            // 11 2047
            // 10 1023
            // 9  511
            // 53    9007199254740991


            var big= NumUtil.SnowNumBig();
            var small = NumUtil.SnowNum();
            var jsNum = NumUtil.SnowNumJs16();
            //var jsNumG=new JsNum16LenGenerator(0);
            //var id = jsNumG.NextNum();
        }
    }
    
}

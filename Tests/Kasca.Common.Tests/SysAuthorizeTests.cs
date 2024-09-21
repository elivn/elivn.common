using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kasca.Common.Authrization;

namespace Kasca.Common.Tests
{
    [TestClass]
    public class SysAuthorizeTests
    {
        [TestMethod]
        public void SignTest()
        {
            var key = Guid.NewGuid().ToString().Replace("-", "");

            var sys = new AppAuthorizeInfo();
            sys.DeviceId = "DeviceId";
            sys.IpAddress = "127.0.0.1";
            sys.LogSerialNum = "TraceCode";
         

            var headers = sys.CompleteDicSign("Test", "1.0",  key);
            sys.Sign = headers["Z-Sign"];//"J65cN94mnL2RUuRgrqoEYJHoEnE=";
            
            sys.AppCode = "Test";
            sys.AppVersion = "1.0";

            var checkRes = sys.CheckSign(key);
        }
    }
}

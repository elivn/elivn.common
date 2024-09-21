using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kasca.Common.Authrization;
using Kasca.Common.ComModels;
using Kasca.Common.ComUtils;

namespace Kasca.Common.Tests
{
    [TestClass]
    public class ApiUtilTests:BaseTests
    {
        [TestMethod]
        public async Task PostTest()
        {
            var res = await ApiUtil.PostApi<ResultMo>("http://localhost:27260/api/Contract/PreparedContract", new
            {
                LoanSheetId = "ab9a9676-6183-4d1b-9195-ea558b7688b2"
            });
            Assert.IsTrue(res.IsSuccess());
        }

        [TestMethod]
        public async Task SignTest()
        {
            var sysinfo = new AppAuthorizeInfo
            {
                AppCode = "Test",
                AppVersion = "1.0.0",
                LogSerialNum = "c2beb669-2bdd-4a8a-88c3-27e4f7a696db"
            };
            var headers = sysinfo.CompleteDicSign(sysinfo.AppCode, sysinfo.AppVersion,"630e57b8cf034eb0967cad9511d21ee3");
        }

        [TestMethod]
        public async Task TOutTest()
        {
            var s = new SubC();
            s.name = "sss";
            s.nick_name = "sssvbbbb";
         
            //var sRes =
            IResultMo<ParentC> pRes = new ResultMo<SubC>(s);
        }
    }




    public class ParentC
    {
        public string name { get; set; }
    }
    public class SubC : ParentC
    {
        public string nick_name { get; set; }
    }
}

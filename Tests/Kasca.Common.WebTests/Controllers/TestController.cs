using System;
using Microsoft.AspNetCore.Mvc;
using Kasca.Common.Authrization;
using Kasca.Common.ComModels;
using Kasca.Common.Web.Filters;

namespace Kasca.Common.WebTests.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class TestController : Controller
    {
        [HttpPost]
        public ResultMo Receive([FromBody]UserTestInfo info)
        {
            //throw new ArgumentException();
            return new ResultMo();
        }
    }



    [CallApi]
    [Route("callapi/[controller]/[action]")]
    public class TestCallController : Controller
    {
        [HttpGet]
        public ActionResult test()
        {
            var appcode = MemberShiper.AppAuthorize.AppCode;

           return Content("test"+ appcode);
        }
    }

    public class UserTestInfo
    {
        public string name { get; set; }
    }
}
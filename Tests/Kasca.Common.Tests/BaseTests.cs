using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kasca.Common.Authrization;
using Kasca.Common.ComUtils;
using Kasca.CachePlug.Redis;

namespace Kasca.Common.Tests
{
    [TestClass]
    public class BaseTests
    {

        public BaseTests()
        {
            SetConfig();
            MemberShiper.SetAppAuthrizeInfo(new AppAuthorizeInfo());
        }

        private static void SetConfig()
        {
            var basePat = Directory.GetCurrentDirectory();
            var configPath = basePat.Substring(0, basePat.IndexOf("bin"));
            var config = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .Add(new JsonConfigurationSource
                {
                    Path = "appsettings.json",
                    ReloadOnChange = true
                }).Build();

            ConfigUtil.Configuration = config;
            
        }

    }
}

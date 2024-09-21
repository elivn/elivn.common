using System.Buffers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Kasca.Common.ComUtils;
using Kasca.Common.Web.Filters;
using Kasca.Common.Web.Formatters;

namespace Kasca.Common.WebTests
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            ConfigUtil.Configuration = Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opt =>
            {
                var jsonSetting = new JsonSerializerSettings();
                SetJsonOptionSettings(jsonSetting);

                opt.OutputFormatters.RemoveType<JsonOutputFormatter>();
                opt.OutputFormatters.Insert(0, new JsonOutputWithLogFormatter(jsonSetting, ArrayPool<char>.Shared));
            }).AddJsonOptions(opt =>  SetJsonOptionSettings(opt.SerializerSettings));
        }

        private static void SetJsonOptionSettings(JsonSerializerSettings options)
        {
            //忽略循环引用
            options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //不使用驼峰样式的key
            options.ContractResolver = new DefaultContractResolver();
            //设置时间格式
            options.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseBrowserLink();
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //    app.UseExceptionHandler("/Error");

            app.UseStaticFiles();
            app.UseExceptionMiddleware();

            app.UseReqLogMiddleware();
            app.UseAppStartMiddleware();

            app.UseMvc();
        }
    }
}

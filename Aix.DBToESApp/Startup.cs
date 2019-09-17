using Aix.DBToES;
using Aix.DBToES.Common;
using Aix.DBToESApp.Hosts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.DBToESApp
{
    public class Startup
    {
        internal static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var esOptions = context.Configuration.GetSection("es").Get<ESOptions>();
            var dbOptions = context.Configuration.GetSection("db").Get<DBOptions>();

            services.AddSingleton(esOptions);
            services.AddSingleton(dbOptions);

            services.AddSingleton<ESSearchConfiguration>();
            services.AddSingleton<DBQuery>();
            services.AddSingleton<SyncToES>();
            services.AddHostedService<StartHostedService>();
        }
    }
}

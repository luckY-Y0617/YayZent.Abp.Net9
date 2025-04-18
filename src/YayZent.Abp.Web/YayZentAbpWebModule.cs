using System.Configuration;
using Microsoft.AspNetCore.Cors;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using YayZent.Abp.Application;
using YayZent.Abp.SqlSugarCore;
using YayZent.Framework.Blog.Application;

namespace YayZent.Abp.Web;

[DependsOn(typeof(YayZentAbpSqlSugarCoreModule),
    typeof(YayZentAbpApplicationModule),
    
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAutofacModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAuditingModule))]
public class YayZentAbpWebModule: AbpModule
{
    private const string DefaultCorsPolicyName = "Default";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var configuration = services.GetConfiguration();
        var host = services.GetHostingEnvironment();
        
        
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(YayZentFrameworkBlogApplicationModule).Assembly,
                option => option.RemoteServiceName = "Blog");
            options.ConventionalControllers.Create(typeof(YayZentAbpApplicationModule).Assembly, 
                option => option.RemoteServiceName = "Test");
        });
        
        Configure<AbpAuditingOptions>(options =>
        {
            options.IsEnabled = true;
        });

        services.AddCors(options =>
        {
            options.AddPolicy(DefaultCorsPolicyName, builder =>
            {
                var origins = configuration["App:CorsOrigins"]?
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(orgin => orgin.RemovePostFix("/"))
                    .ToArray();

                builder.WithOrigins(origins!)
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        app.UseRouting();
        app.UseCors(DefaultCorsPolicyName);

        app.UseAuthorization();


    }
}
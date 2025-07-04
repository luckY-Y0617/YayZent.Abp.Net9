using System.Globalization;
using System.Text;
using System.Threading.RateLimiting;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Hangfire;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Swashbuckle;
using YayZent.Abp.Application;
using YayZent.Abp.Infrastructure;
using YayZent.Abp.SqlSugarCore;
using YayZent.Framework.AspNetCore.Extensions;
using YayZent.Framework.Auth.Application;
using Yayzent.Framework.BackgroundWorkers.Hangfire;
using YayZent.Framework.Bbs.Application;
using YayZent.Framework.Bbs.Infrastructure.MiddleWares;
using YayZent.Framework.Blog.Application;
using YayZent.Framework.Rbac.Application;
using YayZent.Framework.TenantManagement.Application;

namespace YayZent.Abp.Web;

[DependsOn(typeof(YayZentAbpSqlSugarCoreModule),
    typeof(YayZentAbpApplicationModule),
    typeof(YayZentAbpInfrastructureModule),
    
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpHangfireModule),
    typeof(AbpBackgroundJobsHangfireModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpAuditingModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAutofacModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAuditingModule))]
public class YayZentAbpWebModule: AbpModule
{
    private const string DefaultCorsPolicyName = "Default";

    public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
    {
        PreConfigure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(YayZentFrameworkBlogApplicationModule).Assembly,
                option => option.RemoteServiceName = "Blog");
            options.ConventionalControllers.Create(typeof(YayZentFrameworkTenantManagementApplicationModule).Assembly,
                option => option.RemoteServiceName = "TenantManagement");
            options.ConventionalControllers.Create(typeof(YayZentFrameworkRbacApplicationModule).Assembly,
                option => option.RemoteServiceName = "Rbac");
            options.ConventionalControllers.Create(typeof(YayZentFrameworkAuthApplicationModule).Assembly,
                option => option.RemoteServiceName = "Auth");
            options.ConventionalControllers.Create(typeof(YayZentFrameworkBbsApplicationModule).Assembly,
                option => option.RemoteServiceName = "Bbs");
        });
        
        return Task.CompletedTask;
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var configuration = services.GetConfiguration();
        
        Configure<AbpTenantResolveOptions>(options =>
        {
            options.TenantResolvers.Clear();
            options.TenantResolvers.Add(new HeaderTenantResolveContributor());
        });
        
        Configure<AbpAuditingOptions>(options =>
        {
            //默认关闭，开启会有大量的审计日志
            options.IsEnabled = false;
            options.IsEnabledForGetRequests = false;
        });
        
        Configure<AbpDistributedCacheOptions>(configuration.GetSection("AbpDistributedCacheOptions"));
        
        // 注册 IConnectionMultiplexer 到依赖注入容器
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConfig = configuration["Redis:Configuration"]!;
                return ConnectionMultiplexer.Connect(redisConfig);
            });
            
            //配置Hangfire定时任务存储，开启redis后，优先使用redis
            context.Services.AddHangfire((provider, config)=>
            {
                //Hangfire 会将待执行的任务（Job）排入 Redis 队列（如 list、set 等结构）；
                //Redis 充当一个高性能分布式任务队列，允许多个 Worker 并发读取、处理任务
                var redisEnabled=configuration.GetSection("Redis").GetValue<bool>("IsEnabled");
                var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>(); 
                if (redisEnabled)
                {
                    var jobDb = configuration.GetSection("Redis").GetValue<int>("JobDb");
                    config.UseRedisStorage(
                        // 创建 Redis 连接复用器（ConnectionMultiplexer）
                        // 它是 Redis 客户端的核心类，用于管理与 Redis 的多个连接实例，并提供线程安全的方式获取数据库（IDatabase）对象。
                        multiplexer,
                        new RedisStorageOptions()
                        {
                            Db =jobDb,
                            //当一个 Worker 从队列中取出一个任务开始执行时，Hangfire 会将这个任务设置为“不可见”，以避免多个 Worker 重复执行该任务。
                            //InvisibilityTimeout 控制这个“不可见”的持续时间。如果超过这个时间任务还没执行完，Hangfire 会认为它失败了，可以被别的 Worker 重新取出执行。
                            InvisibilityTimeout = TimeSpan.FromHours(1), //JOB允许执行1小时
                            Prefix = "Yayzent:HangfireJob:"
                        }).WithJobExpirationTimeout(TimeSpan.FromHours(1)); // 任务结果的保留时间，过期会自动清理
                }
                else
                {
                    config.UseMemoryStorage();
                }
            });
        

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "YayZent",
                    ValidAudience = "YayZent",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("892u4j1803qj23jroadajdoahjdao92834u23jdf923jrnmvasbceqwt347562tgdhdnsv9wevbnop"))
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = failedContext =>
                    {
                        Console.WriteLine($"❌ Token 认证失败：{failedContext.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });
        
        context.Services.AddCustomSwaggerGen<YayZentAbpWebModule>(options =>
        {
            options.SwaggerDoc("default",
                new OpenApiInfo { Title = "Doro.Framework.Abp", Version = "v1", Description = "集大成者" });
        });

        services.AddCors(options =>
        {
            options.AddPolicy(DefaultCorsPolicyName, builder =>
            {
                var origins = configuration["App:CorsOrigins"]?
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(orgin => orgin.RemovePostFix("/"))
                    .ToArray();

                builder.WithOrigins(origins ?? Array.Empty<string>())
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        //速率限制
        //每60秒限制100个请求，滑块添加，分6段
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = (rejectionContext, _) =>
            {
                if (rejectionContext.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    rejectionContext.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                rejectionContext.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                rejectionContext.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken: CancellationToken.None);

                return new ValueTask();
            };

            // 全局限流策略：按 User-Agent 维度设置滑动窗口限流
            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var userAgent = httpContext.Request.Headers.UserAgent.ToString();

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        userAgent,
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 1000,                      // 每 60 秒允许最多 1000 次
                            Window = TimeSpan.FromSeconds(60),
                            SegmentsPerWindow = 6,                   // 分成 6 个 10 秒段
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                }));
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

        app.UseMultiTenancy();
        
        app.UseAuthentication();

        app.UseAccessLog();
        
        app.UseStaticFiles(new StaticFileOptions
        {
            RequestPath = "/api/app/wwwroot",
            // 可以在这里添加或修改MIME类型映射  
            ContentTypeProvider = new FileExtensionContentTypeProvider
            {
                Mappings =
                {
                    [".wxss"] = "text/css"
                }
            }
        });
        app.UseDefaultFiles();
        app.UseDirectoryBrowser("/api/app/wwwroot");
        
        app.UseUnitOfWork();
        
        app.UseAuthorization();
        
        app.UseAuditing();
        
        //日志记录
        app.UseAbpSerilogEnrichers();

        app.UseYiSwagger();
        //流量访问统计,需redis支持，否则不生效
        
        //Hangfire定时任务面板，可配置授权
        app.UseAbpHangfireDashboard("/hangfire",
            options =>
            {
                options.AsyncAuthorization = [new YayZentTokenAuthorizationFilter(app.ApplicationServices)];
            });

        //终节点
        app.UseConfiguredEndpoints();
    }
}
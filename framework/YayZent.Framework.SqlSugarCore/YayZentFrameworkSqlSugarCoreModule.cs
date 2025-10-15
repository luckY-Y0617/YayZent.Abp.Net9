using System.Data;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.MultiTenancy.ConfigurationStore;
using YayZent.Framework.Core.Attributes;
using YayZent.Framework.Core.Extensions;
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.SqlSugarCore.Extensions;
using YayZent.Framework.SqlSugarCore.Factories;
using YayZent.Framework.SqlSugarCore.Interceptors;
using YayZent.Framework.SqlSugarCore.Repositories;
using YayZent.Framework.SqlSugarCore.Uow;
using DbType = SqlSugar.DbType;

namespace YayZent.Framework.SqlSugarCore;

[DependsOn(typeof(YayZentFrameworkSqlSugarCoreAbstrationsModule),
    typeof(AbpDddDomainModule),
    typeof(AbpMultiTenancyModule))]
public class YayZentFrameworkSqlSugarCoreModule: AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var configuration = services.GetConfiguration();
        var dbConnOptionsSection = configuration.GetSection("DbConnOptions");
    
        // 绑定 DbConnOptions 配置
        Configure<DbConnOptions>(dbConnOptionsSection);
        var dbConnOptions = dbConnOptionsSection.Get<DbConnOptions>();

        // 根据不同的数据库类型选择对应的 SequentialGuidType
        SequentialGuidType guidType = dbConnOptions.DbType switch
        {
            DbType.MySql or DbType.PostgreSQL => SequentialGuidType.SequentialAsString,
            DbType.SqlServer => SequentialGuidType.SequentialAtEnd,
            DbType.Oracle => SequentialGuidType.SequentialAsBinary,
            _ => SequentialGuidType.SequentialAtEnd,
        };

        // 配置默认的 GUID 生成选项
        Configure<AbpSequentialGuidGeneratorOptions>(options =>
        {
            options.DefaultSequentialGuidType = guidType;
        });

        // 注册 SqlSugar 相关服务
        services.TryAddScoped<ICurrentDbContextAccessor, AsyncLocalCurrentDbContextAccessor>();
        services.TryAddScoped<ISqlSugarDbClientFactory, SqlSugarDbClientFactory>();
        services.AddTransient<ISqlSugarDbContext, DefaultSqlSugarDbContext>();
        services.AddTransient(typeof(IRepository<>), typeof(SqlSugarRepository<>));
        services.AddTransient(typeof(IRepository<,>), typeof(SqlSugarRepository<,>));
        services.AddTransient(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
        services.AddTransient(typeof(ISqlSugarRepository<,>), typeof(SqlSugarRepository<,>));
        services.AddTransient(typeof(ISugarDbContextProvider<>), typeof(UowSqlSugarDbContextProvider<>));
        
        context.Services.AddSingleton<ISerializeService, SqlSugarNonPublicSerializer>();

        // 配置数据库连接默认字符串
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = dbConnOptions.Url;
        });

        if (dbConnOptions.EnabledSaasMultiTenancy)
        {
            Configure<AbpDefaultTenantStoreOptions>(options => 
            {
                var tenants = options.Tenants.ToList();
                
                tenants.ForEach(tenant => tenant.NormalizedName = tenant.Name.GetNormalizedName());
                
                // 添加默认租户
                tenants.Insert(0, new TenantConfiguration
                {
                    Id = Guid.Empty,
                    Name = ConnectionStrings.DefaultConnectionStringName + '@' + dbConnOptions.DbType.ToString(),
                    NormalizedName = ConnectionStrings.DefaultConnectionStringName,
                    ConnectionStrings = new ConnectionStrings 
                    { 
                        { ConnectionStrings.DefaultConnectionStringName, dbConnOptions.Url } 
                    },
                    IsActive = true
                });

                options.Tenants = tenants.ToArray();
            });
        }
    
        services.AddDbContext<DefaultSqlSugarDataInterceptor>();

        return Task.CompletedTask;
    }

    public override async Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var services = context.ServiceProvider;
        var options = services.GetRequiredService<IOptions<DbConnOptions>>().Value;

        var logger = services.GetRequiredService<ILogger<YayZentFrameworkSqlSugarCoreModule>>();
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"数据库连接字符串：{options.Url}");
        sb.AppendLine($"数据库类型：{options.DbType.ToString()}");
        sb.AppendLine($"是否开启种子数据：{options.EnableDbSeed}");
        sb.AppendLine($"是否开启CodeFirst：{options.EnableCodeFirst}");
        sb.AppendLine("===============================");

        logger.LogInformation(sb.ToString());

        if (options.EnableCodeFirst)
        {
            await CodeFirstSystemTables(services);
        }

        if (options.EnableDbSeed)
        {
            await InitializeDataSeed(services);
        }
    }
    
    private async Task CodeFirstSystemTables(IServiceProvider sp)
    {
        var moduleContainer = sp.GetRequiredService<IModuleContainer>();
        var db = sp.GetRequiredService<ISqlSugarDbContext>().SqlSugarClient;

        await Task.Run(() => db.DbMaintenance.CreateDatabase());
        List<Type> types = new List<Type>();
        foreach (var module in moduleContainer.Modules)
        {
            types.AddRange(module.Assembly.GetTypes()
                    .Where(x => x.GetCustomAttribute<SplitTableAttribute>() is null
                                && x.GetCustomAttribute<IgnoreCodeFirstAttribute>() is null
                                && x.GetCustomAttribute<SugarTable>() is not null));
        }

        if (types.Count > 0)
        {
            await Task.Run(() => db.CodeFirst.InitTables(types.ToArray()));
        }
    }

    private async Task InitializeDataSeed(IServiceProvider serviceProvider)
    {
        var dataSeeder = serviceProvider.GetRequiredService<IDataSeeder>();
        await dataSeeder.SeedAsync();
    }
}
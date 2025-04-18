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
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.SqlSugarCore.Repositories;
using YayZent.Framework.SqlSugarCore.Uow;
using DbType = SqlSugar.DbType;

namespace YayZent.Framework.SqlSugarCore;

[DependsOn(typeof(YayZentFrameworkSqlSugarCoreAbstrationsModule),
    typeof(AbpDddDomainModule))]
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
        services.TryAddScoped<ISqlSugarDbContext, SqlSugarDbContextFactory>();
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

        if (options.EnableDbSeed)
        {
            CodeFirst(services);
        }
    }
    

    private void CodeFirst(IServiceProvider sp)
    {
        var moduleContainer = sp.GetRequiredService<IModuleContainer>();
        var db = (sp.GetRequiredService<ISqlSugarDbContext>()).SqlSugarClient;

        db.DbMaintenance.CreateDatabase();

        List<Type> types = new List<Type>();
        foreach (var module in moduleContainer.Modules)
        {
            types.AddRange(module.Assembly.GetTypes().Where(type => type.GetCustomAttribute<IgnoreCodeFirstAttribute>() is null 
                                                                    && type.GetCustomAttribute<SugarTable>() is not null 
                                                                    && type.GetCustomAttribute<SplitTableAttribute>() is null));
        }

        if (types.Count > 0)
        {
            db.CodeFirst.InitTables(types.ToArray());
        }
    }
}
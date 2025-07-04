using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Volo.Abp.AspNetCore.Mvc;
using YayZent.Framework.AspNetCore.Filters;

namespace YayZent.Framework.AspNetCore.Extensions;

public static class SwaggerAddExtensions
{
    public static IServiceCollection AddCustomSwaggerGen<TProgram>(
        this IServiceCollection services,
        Action<SwaggerGenOptions>? configure = null)
    {
        var mvcOptions = services.GetPreConfigureActions<AbpAspNetCoreMvcOptions>().Configure();
        var controllerSettings = mvcOptions.ConventionalControllers
            .ConventionalControllerSettings
            .DistinctBy(x => x.RemoteServiceName)
            .OrderBy(x => x.RemoteServiceName)
            .ToList();

        services.AddAbpSwaggerGen(options =>
        {
            // 外部自定义配置
            configure?.Invoke(options);

            // 添加分组文档
            foreach (var setting in controllerSettings)
            {
                if (!options.SwaggerGeneratorOptions.SwaggerDocs.ContainsKey(setting.RemoteServiceName))
                {
                    options.SwaggerDoc(setting.RemoteServiceName, new OpenApiInfo
                    {
                        Title = setting.RemoteServiceName,
                        Version = "v1"
                    });
                }
            }

            // 根据控制器所在程序集，按分组过滤文档
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (apiDesc.ActionDescriptor is ControllerActionDescriptor descriptor)
                {
                    var setting = controllerSettings.FirstOrDefault(
                        s => s.Assembly == descriptor.ControllerTypeInfo.Assembly);
                    return setting?.RemoteServiceName == docName;
                }

                return false;
            });

            // 使用完整类型名避免 Schema 冲突
            options.CustomSchemaIds(type => type.FullName);

            // 加载 XML 注释
            AddXmlComments(typeof(TProgram).Assembly.Location, options);

            // JWT 鉴权支持
            AddJwtSecurityDefinition(options);

            // 枚举增强显示
            options.SchemaFilter<EnumSchemaFilter>();
        });

        return services;
    }

    private static void AddXmlComments(string assemblyPath, SwaggerGenOptions options)
    {
        var basePath = Path.GetDirectoryName(assemblyPath);
        if (basePath == null) return;

        foreach (var file in Directory.GetFiles(basePath, "*.xml"))
        {
            options.IncludeXmlComments(file, includeControllerXmlComments: true);
        }
    }

    private static void AddJwtSecurityDefinition(SwaggerGenOptions options)
    {
        const string schemeName = "JwtBearer";
        options.AddSecurityDefinition(schemeName, new OpenApiSecurityScheme
        {
            Description = "直接输入 Token 即可",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = schemeName
                }
            }] = Array.Empty<string>()
        });
    }
}

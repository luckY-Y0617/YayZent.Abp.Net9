using Microsoft.OpenApi.Models;

namespace YayZent.Abp.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Abp", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });

        builder.Host.UseAutofac();
        await builder.Services.AddApplicationAsync<YayZentAbpWebModule>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();    // 启用 Swagger
            app.UseSwaggerUI(); // 启用 Swagger UI
        }


        await app.InitializeApplicationAsync();
        
        app.MapControllers();

        
        await app.RunAsync();
    }
}
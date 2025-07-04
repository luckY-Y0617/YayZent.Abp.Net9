using Microsoft.OpenApi.Models;
using YayZent.Framework.AspNetCore.Filters;

namespace YayZent.Abp.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddAuthorization();

        builder.Host.UseAutofac();
        await builder.Services.AddApplicationAsync<YayZentAbpWebModule>();

        var app = builder.Build();

        await app.InitializeApplicationAsync();
        
        app.MapControllers();
        
        await app.RunAsync();
    }
}
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace YayZent.Framework.AspNetCore.Filters;

public class TenantHeaderOperationFilter: IOperationFilter
{
    private readonly string _headerKey = "__tenant";
    
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter()
        {
            Name = _headerKey,
            In = ParameterLocation.Header,
            Required = false,
            AllowEmptyValue = true,
            Description = "租户id或者租户名称（可空为默认租户）"
        });
    }
}
using System.ComponentModel;
using System.Reflection;
using System.Text;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace YayZent.Framework.AspNetCore.Filters;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
            return;

        schema.Enum.Clear();
        schema.Type = "string";
        schema.Format = null;

        var descriptionBuilder = new StringBuilder();

        foreach (var name in Enum.GetNames(context.Type))
        {
            var value = (Enum)Enum.Parse(context.Type, name);
            var description = GetEnumDescription(value);
            var intValue = Convert.ToInt64(value);

            schema.Enum.Add(new OpenApiString(name));
            descriptionBuilder.Append($"【{name}{(description != null ? $"({description})" : "")}={intValue}】<br />");
        }

        schema.Description = descriptionBuilder.ToString();
    }

    private static string? GetEnumDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        return field?.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }
}

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Saas_Micro_Finance.API.Swagger
{
    public class TenantHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "__tenant__",
                In = ParameterLocation.Header,
                Required = false,
                Description = "Tenant identifier (e.g. firstbank)",
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}

namespace MunicipalityRegistry.Api.Oslo.Infrastructure
{
    using System.Collections.Generic;
    using CloudNative.CloudEvents;
    using Microsoft.OpenApi;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class CloudEventSchemaFilter : ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type != typeof(CloudEvent))
                return;

            if (schema is not OpenApiSchema cloudEventSchema)
                return;

            cloudEventSchema.Type = JsonSchemaType.Object;
            cloudEventSchema.Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["specversion"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["id"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["type"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["source"] = new OpenApiSchema { Type = JsonSchemaType.String, Format = "uri" },
                ["time"] = new OpenApiSchema { Type = JsonSchemaType.String, Format = "date-time" },
                ["datacontenttype"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["dataschema"] = new OpenApiSchema { Type = JsonSchemaType.String, Format = "uri" },
                ["data"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    AdditionalPropertiesAllowed = true
                },
                ["basisregisterseventtype"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["basisregisterscausationid"] = new OpenApiSchema { Type = JsonSchemaType.String },
            };

            cloudEventSchema.AdditionalPropertiesAllowed = true;
        }
    }

}

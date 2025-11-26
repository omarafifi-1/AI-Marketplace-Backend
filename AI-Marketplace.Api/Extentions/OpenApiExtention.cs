using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

public static class OpenApiExtensions
{
    public static OpenApiOptions AddJwtBearerAuth(this OpenApiOptions options)
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your valid token in the text input below.\r\n\r\nExample: \"12345abcdef\""
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes.Add("Bearer", securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            };

            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
            {
                operation.Value.Security.Add(securityRequirement);
            }

            return Task.CompletedTask;
        });
        return options;
    }
}
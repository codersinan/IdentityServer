using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IdentityServer.Api.Helpers
{
    public class SwaggerFluentValidationSchemaFilter : ISchemaFilter
    {
        private readonly IServiceProvider _provider;

        public SwaggerFluentValidationSchemaFilter(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var validator = _provider.GetService(typeof(IValidator<>).MakeGenericType(context.Type)) as IValidator;
            if (validator == null) return;

            if (schema.Required == null)
            {
                schema.Required = new HashSet<string>();
            }

            var validatorDescriptor = validator.CreateDescriptor();
            foreach (var key in schema.Properties.Keys)
            {
                foreach (var propertyValidator in validatorDescriptor.GetValidatorsForMember(ToPascalCase(key)))
                {
                    switch (propertyValidator)
                    {
                        case NotNullValidator _:
                            schema.Properties[key].Nullable = false;
                            schema.Required.Add(key);
                            break;
                        case NotEmptyValidator _:
                            schema.Required.Add(key);
                            break;
                        case LengthValidator lengthValidator:
                        {
                            if (lengthValidator.Max > 0)
                                schema.Properties[key].MaxLength = lengthValidator.Max;
                            schema.Properties[key].MinLength = lengthValidator.Min;
                            break;
                        }
                        case RegularExpressionValidator expressionValidator:
                            schema.Properties[key].Pattern = expressionValidator.Expression;
                            break;
                        case EmailValidator emailValidator:
                            schema.Properties[key].Example=new OpenApiString("mail@domain.com");
                            schema.Properties[key].Description = emailValidator.Expression;
                            break;
                        case EnumValidator enumValidator:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        private string ToPascalCase(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length < 2)
            {
                return null;
            }

            return key.Substring(0, 1).ToUpper() + key.Substring(1);
        }
    }
}
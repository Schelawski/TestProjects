using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

namespace SwaggerDocsRazorViews.Infrastructure
{
    public class SwaggerUtils
    {
        public static string GetXmlCommentsPath()
        {
            return System.String.Format(@"{0}bin\SwaggerDocsRazorViews.XML", System.AppDomain.CurrentDomain.BaseDirectory);

            //return System.String.Format(@"{0}bin\DomainServices.XML", System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }

    public class SwaggerHeaderParameter : IOperationFilter
    {
        public string Description { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string DefaultValue { get; set; }

        public void Apply(SwaggerDocsConfig c)
        {
            c.ApiKey(Key).Name(Name).Description(Description).In("header");
            c.OperationFilter(() => this);
        }


        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            operation.parameters = operation.parameters ?? new List<Parameter>();
            operation.parameters.Add(new Parameter
            {
                name = Name,
                description = Description,
                @in = "header",
                required = true,
                type = "string",
                @default = DefaultValue
            });
        }

        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            var security = new List<IDictionary<string, IEnumerable<string>>>
            {
                new Dictionary<string, IEnumerable<string>> { {Key, new string[0] } }
            };
            swaggerDoc.security = security;
        }
    }

    public class SupportFlaggedEnums : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null) return;

            var queryEnumParams = operation.parameters
                .Where(param => param.@in == "query" && param.@enum != null)
                .ToArray();

            foreach (var param in queryEnumParams)
            {
                param.items = new PartialSchema { type = param.type, @enum = param.@enum };
                param.type = "array";
                param.collectionFormat = "csv";
            }
        }
    }

    public class SupportDropDownEnums : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null) return;

            var queryEnumParams = operation.parameters
                .Where(param => param.@in == "query" && param.@enum != null)
                .ToArray();

            foreach (var param in queryEnumParams)
            {
                param.items = new PartialSchema { type = param.type, @enum = param.@enum };
            }
        }
    }

    //http://mattfrear.com/2015/04/21/generating-swagger-example-responses-with-swashbuckle/
    #region Example responses

    public class SwaggerResponseExamplesAttribute : Attribute
    {
        public SwaggerResponseExamplesAttribute(Type responseType, Type examplesProviderType)
        {
            ResponseType = responseType;
            ExamplesProviderType = examplesProviderType;
        }

        public Type ExamplesProviderType { get; set; }
        public Type ResponseType { get; set; }
        
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SwaggerRequestExamplesAttribute : Attribute
    {
        public SwaggerRequestExamplesAttribute(Type responseType, Type examplesProviderType)
        {
            ResponseType = responseType;
            ExamplesProviderType = examplesProviderType;
        }

        public Type ExamplesProviderType { get; private set; }

        public Type ResponseType { get; private set; }
    }

    public class ExamplesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            SetRequestModelExamples(operation, schemaRegistry, apiDescription);
            SetResponseModelExamples(operation, schemaRegistry, apiDescription);
        }

        private static void SetRequestModelExamples(
            Operation operation, 
            SchemaRegistry schemaRegistry, 
            ApiDescription apiDescription)
        {
            var requestAttributes = 
                apiDescription.GetControllerAndActionAttributes<SwaggerRequestExamplesAttribute>();

            foreach (var attr in requestAttributes)
            {
                var schema = schemaRegistry.GetOrRegister(attr.ResponseType);

                var request = operation.parameters.FirstOrDefault(p => p.@in == "body" && p.schema.@ref == schema.@ref);

                if (request != null)
                {
                    var provider = (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType);

                    var parts = schema.@ref.Split('/');
                    var name = parts.Last();

                    var definitionToUpdate = schemaRegistry.Definitions[name];

                    if (definitionToUpdate != null)
                    {
                        definitionToUpdate.example = ((dynamic)FormatAsJson(provider))["application/json"];
                    }
                }
            }
        }

        private static void SetResponseModelExamples(Operation operation, SchemaRegistry schemaRegistry, 
            ApiDescription apiDescription)
        {
            var responseAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerResponseExamplesAttribute>();

            foreach (var attr in responseAttributes)
            {
                var schema = schemaRegistry.GetOrRegister(attr.ResponseType);

                var response =
                    operation.responses.FirstOrDefault(
                        x => x.Value != null && x.Value.schema != null && x.Value.schema.@ref == schema.@ref);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false)
                {
                    if (response.Value != null)
                    {
                        var provider = (IExamplesProvider)Activator.CreateInstance(attr.ExamplesProviderType);
                        response.Value.examples = FormatAsJson(provider);
                    }
                }
            }
        }

        private static object ConvertToCamelCase(Dictionary<string, object> examples)
        {
            var jsonString = JsonConvert.SerializeObject(examples, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return JsonConvert.DeserializeObject(jsonString);
        }

        private static object FormatAsJson(IExamplesProvider provider)
        {
            var examples = new Dictionary<string, object>
    {
        {
            "application/json", provider.GetExamples()
        }
    };

            return ConvertToCamelCase(examples);
        }
    }

    public interface IExamplesProvider
    {
        object GetExamples();
    }
       

    #endregion

    #region SwaggerDefaultValue

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerDefaultValueAttribute : Attribute
    {
        public SwaggerDefaultValueAttribute(string parameterName, object value)
        {
            this.ParameterName = parameterName;
            this.Value = value;
        }

        public string ParameterName { get; private set; }

        public object Value { get; set; }
    }

    public class AddDefaultValues : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                return;

            var actionParams = apiDescription.ActionDescriptor.GetParameters();
            var customAttributes = apiDescription.ActionDescriptor.GetCustomAttributes<SwaggerDefaultValueAttribute>();
            foreach (var param in operation.parameters)
            {
                var actionParam = actionParams.FirstOrDefault(p => p.ParameterName == param.name);
                if (actionParam != null)
                {
                    if (actionParam.DefaultValue != null)
                    {
                        param.@default = actionParam.DefaultValue;
                    }
                    else
                    {
                        var customAttribute = customAttributes.FirstOrDefault(p => p.ParameterName == param.name);
                        if (customAttribute != null)
                        {
                            param.@default = customAttribute.Value;
                        }
                    }
                }
            }
        }
    }

    #endregion
}
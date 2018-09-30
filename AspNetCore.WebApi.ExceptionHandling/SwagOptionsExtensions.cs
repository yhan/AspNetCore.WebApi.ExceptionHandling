namespace AspNetCore.WebApi.ExceptionHandling
{
    using System;

    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public static class SwagOptionsExtensions
    {
        public static void SetDefaultOptionsWithXmlDocumentation(this SwaggerGenOptions options, string processName, string contactTeamName, string contactTeamEmail, string description)
        {
            options.IgnoreObsoleteActions();
            options.IgnoreObsoleteProperties();
            options.DescribeAllParametersInCamelCase();
            options.DescribeAllEnumsAsStrings();
            options.DescribeStringEnumsInCamelCase();
            options.SwaggerDoc(
                "v1",
                new Info()
                    {
                        Title = processName,
                        Version = "v1",
                        Contact = new Contact() { Email = contactTeamEmail, Name = contactTeamName },
                        TermsOfService = "Copyright, " + (object)DateTime.Now.Year + " La Française Asset Management, Inc. All Rights Reserved. For authorized use only.",
                        Description = description
                    });
            options.IncludeXmlComments(GetXmlCommentsPath(processName), false);

            // options.DocumentFilter<RemoveContextAttributesDocumentFilter>(Array.Empty<object>());
            // options.DocumentFilter<RemoveAuthorizationControllerPathDocumentFilter>(Array.Empty<object>());
            // options.OperationFilter<AddContextHeadersOperationFilter>(Array.Empty<object>());
        }

        private static string GetXmlCommentsPath(string processName)
        {
            return string.Format("{0}\\{1}.XML", (object)AppDomain.CurrentDomain.BaseDirectory, (object)processName);
        }
    }
}
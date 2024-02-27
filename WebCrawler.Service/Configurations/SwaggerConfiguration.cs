using Microsoft.OpenApi.Models;
using System.Reflection;

namespace WebCrawler.Service.Configurations
{
    public class SwaggerConfiguration
    {
        public static void AddSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "APi WEBCRAWLER - API DE CONSULTA DE PROXY",
                    Description = "API REST desenvolvida em AspNet 8 com EntityFramework",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Ágatha Santos",
                        Email = "agathasatos145@gmail.com",
                        Url = new Uri("https://github.com/AgathaDevBr")
                    }
                });


                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                //configurando o swagger para incluir os comentarios XML do código
                options.IncludeXmlComments(xmlPath);
            });
        }
    }
}

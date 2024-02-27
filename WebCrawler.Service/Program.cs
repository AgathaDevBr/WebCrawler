using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebCrawler.Data.Context;
using WebCrawler.Service.Configurations;
using WebCrawler.Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Adicionando as configura��es do Swagger
SwaggerConfiguration.AddSwagger(builder);

// Registra o servi�o HttpClient
builder.Services.AddHttpClient();

// Configura o DbContext para usar um banco de dados em mem�ria
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("BDWebCrawler");
});

// Registra o servi�o WebService
builder.Services.AddScoped<WebService>();

var app = builder.Build();

// Configura o pipeline de requisi��es HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseRouting(); 
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebCrawler.Data.Context;
using WebCrawler.Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra o servi�o HttpClient
builder.Services.AddHttpClient();

// Configura o DbContext para usar um banco de dados em mem�ria
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("BDWebCraw");
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
app.UseRouting(); // Adiciona o middleware de roteamento
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

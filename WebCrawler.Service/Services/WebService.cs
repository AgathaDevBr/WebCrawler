using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WebCrawler.Data.Context;
using WebCrawler.Data.Entities;
using WebCrawler.Service.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebCrawler.Service.Services
{
    public class WebService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;
        public WebService(HttpClient httpClient, AppDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<CrawlResult> CrawlWebsite(HttpClient httpClient)
        {
            string url = "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc";
            string jsonFilePath = "proxies.json";
            string htmlDirectory = "html_pages";

            // Iniciar o rastreamento
            DateTime startTime = DateTime.Now;

            // Listas para armazenar os proxies e os resultados de rastreamento
            List<ProxyInfo> proxies = new List<ProxyInfo>();
            List<string> htmlFiles = new List<string>();

            try
            {
                // Desativar a validação do certificado SSL
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                // Adiciona o User-Agent ao HttpClient
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

                // Obter o conteúdo da página
                string content = await httpClient.GetStringAsync(url);

                CriaArquivoHTML(content, htmlDirectory, htmlFiles, proxies);
                CriaArquivoJson(jsonFilePath, proxies);

                // Finalizar o rastreamento
                DateTime endTime = DateTime.Now;
                int totalPages = 1;
                int totalLines = proxies.Count;

                // Salvar os resultados de rastreamento
                CrawlResult crawlResult = new CrawlResult
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    TotalPages = totalPages,
                    TotalLines = totalLines,
                    JsonFilePath = jsonFilePath
                };

                _dbContext.Add(crawlResult);
                await _dbContext.SaveChangesAsync();

                return crawlResult;
            }
            catch (HttpRequestException ex)
            {
                // Lidar com exceções de requisição HTTP
                throw new HttpRequestException("Erro durante a requisição HTTP.", ex);
            }
            catch (Exception ex)
            {
                // Lidar com outras exceções
                throw new Exception("Ocorreu um erro durante o rastreamento da web.", ex);
            }
        }

        private void CriaArquivoJson(string jsonFilePath, List<ProxyInfo> proxies)
        {
            string json = JsonSerializer.Serialize(proxies, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonFilePath, json);
        }

        private void CriaArquivoHTML(string content, string htmlDirectory, List<string> htmlFiles, List<ProxyInfo> proxies)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);

            // Salvar o conteúdo em um arquivo .html
            string htmlFileName = Path.Combine(htmlDirectory, $"page_1.html");
            File.WriteAllText(htmlFileName, content);
            htmlFiles.Add(htmlFileName);

            // Encontre a tabela usando XPath
            var table = doc.DocumentNode.SelectSingleNode("//table[@id='proxy_list_table']");

            if (table != null)
            {
                // Encontre todas as linhas da tabela
                var rows = table.SelectNodes("tbody/tr");

                // Verifique se as linhas foram encontradas
                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        // Selecione os elementos dentro da linha
                        var cells = row.SelectNodes("td");

                        if (cells != null && cells.Count >= 4)

                            proxies.Add(new ProxyInfo
                            {
                                IpAddress = cells[0].InnerText.Trim(),
                                Port = cells[1].InnerText.Trim(),
                                Country = cells[2].InnerText.Trim(),
                                Protocol = cells[3].InnerText.Trim()
                            });

                    }
                }
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}

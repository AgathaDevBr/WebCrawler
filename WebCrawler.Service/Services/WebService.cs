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
            string url = "https://proxyservers.pro/?ts=fENsZWFuUGVwcGVybWludEJsYWNrfHw1Y2U4NHxidWNrZXQwMTF8fHx8fHw2NWRkNDk5NGM4NmM5fHx8MTcwOTAwMTEwOC44NHw1NTgxYTZmYzIyYzIxNTM3NWViODI3MWUwM2I5ZGEyOTRmMDUwYzkwfHNrZW56by50cGx8fHx8MXx8MHwwfHx8fDF8fHx8fDB8MHx8fHx8fHx8fHwwfDB8fDB8fHwwfDB8VzEwPXx8MXxXMTA9fDM3MzYxMGMzOGEzNDczMjU1YjdkZGRiNmUyYjdmN2NkOTAzMDVkOGR8MHxkcC10ZWFtaW50ZXJuZXQwOV8zcGh8MHwwfHw%3D&skrghlp=u4HryDXg02keRopJlWJWd2EkKcvS2SpvWwBFFSlwcqm%2BPcdd61giz1u0ARI7BOj4&query=Servidor+Proxy&afdToken=ChMI0aKqoL3KhAMVKqaVAh076AEZEmYBlLqpj3HhjhXs-rVYP7HjkuNtTeIP-QFFOb2TXQOtPZezt4MP4ADSEWjjTOjNrZsEUDFDWw56chzI4VBPo6tOizSrYmh_dQKjjjlOz26lXLmLkbu0vLtTw32xKOLUdKsq3FWAyPQ&pcsa=false&nb=0&nm=3&nx=300&ny=88&is=530x496&clkt=96";
            string jsonFilePath = "proxies.json";
            string htmlDirectory = "html_pages";

            DateTime startTime = DateTime.Now;

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

                DateTime endTime = DateTime.Now;
                int totalPages = 1;
                int totalLines = proxies.Count;

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
            catch (Exception ex)
            {
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

            string htmlFileName = Path.Combine(htmlDirectory, $"page_1.html");
            File.WriteAllText(htmlFileName, content);
            htmlFiles.Add(htmlFileName);

            var table = doc.DocumentNode.SelectSingleNode("//div[@class='wrapper3']//table[@id='proxy_list_table']");

            if (table != null)
            {
                var rows = table.SelectNodes("tbody/tr");

                if (rows != null)
                {
                    foreach (var row in rows)
                    {
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
            else
            {
                throw new Exception("Nenhuma linha encontrada na tabela de proxies ou proxies expirados.");
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WebCrawler.Service.Services;

namespace WebCrawler.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebCrawlerController : ControllerBase
    {
        private readonly WebService _crawlService;

        public WebCrawlerController(WebService crawlService)
        {
            _crawlService = crawlService;
        }

        [HttpGet("crawl")]
        public async Task<IActionResult> Crawl()
        {
            try
            {
                // Ignorar a validação do nome do certificado
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                var httpClient = new HttpClient(handler);

                var result = await _crawlService.CrawlWebsite(httpClient);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                // Lidar com exceção de requisição HTTP
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro na requisição HTTP: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Lidar com outras exceções
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao rastrear o website: {ex.Message}");
            }
        }

    }
}

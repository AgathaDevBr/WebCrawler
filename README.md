WebCrawler para Extrair Proxies
1. Introdução
Este é um aplicativo de console C# para extrair informações sobre proxies de um site e salvá-las em um arquivo JSON e em um banco de dados.

2. Passos
2.1. Itens
2.1.1. Acessar o site "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc"
O WebCrawler acessará o site especificado para extrair informações sobre os proxies.

2.1.2. Extrair os campos "IP Adress", "Port", "Country" e "Protocol" de todas as linhas, de todas as páginas disponíveis na execução.
O WebCrawler extrairá os campos especificados de todas as linhas de todas as páginas disponíveis no site durante a execução.

2.1.3. Necessário salvar o resultado da extração em arquivo JSON, que deverá ser salvo na máquina.
Os dados extraídos serão armazenados em um arquivo JSON na máquina local.

2.1.4. Necessário salvar em banco de dados a data início execução, data termino execução, quantidade de páginas, quantidade linhas extraídas em todas as páginas e arquivo JSON gerado.
As informações sobre a execução, como data de início e término, quantidade de páginas e linhas extraídas, e o arquivo JSON gerado serão armazenadas em um banco de dados.

2.1.5. Necessário print (arquivo .html) de cada página.
Será gerado um arquivo HTML para cada página acessada durante a extração.

2.1.6. Necessário que o WebCrawler seja multithread, com máximo de 3 execuções simultâneas.
O WebCrawler será projetado para executar em múltiplas threads, com um máximo de 3 execuções simultâneas, para melhorar a eficiência da extração de dados.

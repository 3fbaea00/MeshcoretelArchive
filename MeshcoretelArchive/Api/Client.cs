#pragma warning disable CS8603 // Possible null reference return.
using System.Net;
using System.Text.Json;
using System.Threading.Channels;

static class Client
{    
    static Client()
    {
        var cookieContainer = new CookieContainer();

        var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
            UseCookies = true
        };

        client = new HttpClient(handler);

        client.DefaultRequestHeaders.TryAddWithoutValidation(
            "User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/154.0.0.0 Safari/537.36");

        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br, zstd");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9,ru;q=0.8");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Pragma", "no-cache");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Site", "same-origin");
        client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Chromium\";v=\"154\", \"Google Chrome\";v=\"154\", \"Not A(Brand\";v=\"99\"");
        client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
        client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
    }

    static HttpClient client;

    static async Task<string> GetMethod(string url)
    {
        var identity = Random.Shared.Next();
        while (true)
        {
            Thread.Sleep(500);
            try
            {
                var response = await client.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{identity:8} {ex.GetType().Name}");
            }
        }
    }

    static async Task<string> Get(string path) 
        => await GetMethod($"https://meshcoretel.ru/api/{path}");

    public static async Task<CatalogRootJson> GetCatalog() 
        => Deserialize<CatalogRootJson>(await Get("regions/catalog"));

    public static async Task<ChannelsPageJson> GetChannels(string regionCode, int offset = 0, bool summaryOnly = true)
        => Deserialize<ChannelsPageJson>(await Get($"channels/page?region_code={regionCode}&limit=500&offset={offset}&summary_only={(summaryOnly ? "true" : "false")}"));

    public static async Task<MessagesPageJson> GetLastMessages(string regionCode, int channelId)
        => Deserialize<MessagesPageJson>(await Get($"channels/{channelId}/messages?region_code={regionCode}&limit=200"));

    public static async Task<MessagesPageJson> GetMessagesBefore(string regionCode, int channelId, int beforeId)
        => Deserialize<MessagesPageJson>(await Get($"channels/{channelId}/messages/window?region_code={regionCode}&limit=200&before={beforeId}"));

    public static T Deserialize<T>(string json)
    {
        if (json[0] == '<')
        {
            Console.WriteLine("IP ban detected");
            Console.ReadLine();
        }    

        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex) 
        {

        }

        return default;
    }
}
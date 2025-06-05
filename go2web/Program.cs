using System;
using System.Net.Sockets;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

class Program
{
    static void PrintHelp()
    {
        Console.WriteLine("Использование:");
        Console.WriteLine("  go2web -u <URL>        Сделать HTTP запрос к URL и вывести ответ");
        Console.WriteLine("  go2web -s <search-term> Сделать поиск и вывести топ 10 результатов");
        Console.WriteLine("  go2web -h              Показать эту справку");
    }

    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Ошибка: отсутствуют аргументы");
            PrintHelp();
            return;
        }

        switch (args[0])
        {
            case "-h":
                PrintHelp();
                break;

            case "-u":
                if (args.Length < 2)
                {
                    Console.WriteLine("Ошибка: не указан URL");
                    return;
                }
                string url = args[1];
                Console.WriteLine($"Будем делать запрос к URL: {url}");
                MakeHttpRequest(url);
                break;

            case "-s":
                if (args.Length < 2)
                {
                    Console.WriteLine("Ошибка: не указан поисковый запрос");
                    return;
                }
                string searchTerm = string.Join(' ', args, 1, args.Length - 1);
                Console.WriteLine($"Будем искать: {searchTerm}");
                // Пока заглушка для поиска
                break;

            default:
                Console.WriteLine("Ошибка: неизвестный параметр");
                PrintHelp();
                break;
        }
    }

    static void MakeHttpRequest(string url)
    {
        try
        {
            var uri = new Uri(url);
            string host = uri.Host;
            string path = string.IsNullOrEmpty(uri.PathAndQuery) ? "/" : uri.PathAndQuery;
            int port = uri.Port == -1 ? (uri.Scheme == "https" ? 443 : 80) : uri.Port;

            using (var client = new TcpClient(host, port))
            using (var stream = client.GetStream())
            {
                Stream finalStream = stream;
                if (uri.Scheme == "https")
                {
                    var sslStream = new SslStream(stream, false, (sender, certificate, chain, errors) => true);
                    sslStream.AuthenticateAsClient(host);
                    finalStream = sslStream;
                }

                string request = $"GET {path} HTTP/1.1\r\nHost: {host}\r\nConnection: close\r\n\r\n";
                byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                finalStream.Write(requestBytes, 0, requestBytes.Length);

                byte[] buffer = new byte[4096];
                int bytesRead;
                var responseBuilder = new StringBuilder();

                while ((bytesRead = finalStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    responseBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                }

                string response = responseBuilder.ToString();

                // Отделяем заголовки и тело по двойному \r\n
                string[] parts = response.Split(new string[] { "\r\n\r\n" }, 2, StringSplitOptions.None);
                if (parts.Length < 2)
                {
                    Console.WriteLine("Ошибка: не удалось получить тело ответа");
                    return;
                }

                string body = parts[1];

                // Убираем HTML теги простым regex
                string textOnly = Regex.Replace(body, "<.*?>", String.Empty);

                Console.WriteLine("=== Тело ответа (без HTML тегов) ===");
                Console.WriteLine(textOnly.Trim());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при запросе: {ex.Message}");
        }
    }
}

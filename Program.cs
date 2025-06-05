using System;
using System.Net.Sockets;
using System.Text;

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
            // Парсим URL, чтобы получить хост и путь
            var uri = new Uri(url);
            string host = uri.Host;
            string path = string.IsNullOrEmpty(uri.PathAndQuery) ? "/" : uri.PathAndQuery;
            int port = uri.Port == -1 ? 80 : uri.Port;

            using (var client = new TcpClient(host, port))
            using (var stream = client.GetStream())
            {
                // Формируем HTTP GET запрос
                string request = $"GET {path} HTTP/1.1\r\nHost: {host}\r\nConnection: close\r\n\r\n";
                byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                stream.Write(requestBytes, 0, requestBytes.Length);

                // Читаем ответ
                byte[] buffer = new byte[4096];
                int bytesRead;
                var responseBuilder = new StringBuilder();

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    responseBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                }

                Console.WriteLine("=== Ответ сервера ===");
                Console.WriteLine(responseBuilder.ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при запросе: {ex.Message}");
        }
    }
}

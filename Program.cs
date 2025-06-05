using System;

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
                Console.WriteLine($"Будем делать запрос к URL: {args[1]}");
                break;

            case "-s":
                if (args.Length < 2)
                {
                    Console.WriteLine("Ошибка: не указан поисковый запрос");
                    return;
                }
                string searchTerm = string.Join(' ', args, 1, args.Length - 1);
                Console.WriteLine($"Будем искать: {searchTerm}");
                break;

            default:
                Console.WriteLine("Ошибка: неизвестный параметр");
                PrintHelp();
                break;
        }
    }
}
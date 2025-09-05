using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;

class Program
{
    const int q = 101;
    const int baseVal = 256;

    static int Hash(string str, int start, int end)
    {
        int h = 0;
        for (int i = start; i < end; i++)
        {
            h = (h * baseVal + str[i]) % q;
        }
        return h;
    }

    static (List<int> positions, int comparisons) RabinKarp(string haystack, string needle)
    {
        int n = haystack.Length;
        int m = needle.Length;
        int comparisons = 0;
        var positions = new List<int>();

        if (m == 0 || n < m) return (positions, comparisons);

        int hashNeedle = Hash(needle, 0, m);
        int hashWindow = Hash(haystack, 0, m);

        int h = 1;
        for (int i = 0; i < m - 1; i++)
        {
            h = (h * baseVal) % q;
        }

        for (int i = 0; i <= n - m; i++)
        {
            if (hashWindow == hashNeedle)
            {
                int j;
                for (j = 0; j < m; j++)
                {
                    comparisons++;
                    if (haystack[i + j] != needle[j]) break;
                }
                if (j == m) positions.Add(i);
            }

            if (i < n - m)
            {
                hashWindow = (baseVal * (hashWindow - haystack[i] * h) + haystack[i + m]) % q;
                if (hashWindow < 0) hashWindow += q;
            }
        }

        return (positions, comparisons);
    }

    static (List<int> positions, int comparisons) DirectSearch(string haystack, string needle)
    {
        int n = haystack.Length;
        int m = needle.Length;
        int comparisons = 0;
        var positions = new List<int>();

        if (m == 0 || needle == null) return (positions, comparisons);

        for (int i = 0; i <= n - m; i++)
        {
            int j;
            for (j = 0; j < m; j++)
            {
                comparisons++;
                if (haystack[i + j] != needle[j]) break;
            }
            if (j == m) positions.Add(i);
        }

        return (positions, comparisons);
    }

    static void Main()
    {
        string text =
@"My name is Bob. Each day I drive my kids to school. 
My daughter goes to a school that’s far from our house. 
It takes 30 minutes to get there. Then I drive my son to his school. 
It’s close to my job. My daughter is in the sixth grade and my son 
is in the second. They are both good students. My daughter usually 
sings her favorite songs while I drive. My son usually sleeps.
I arrive at the office at 8:30 AM. I say good morning to all my 
workmates then I get a big cup of hot coffee. I turn on my computer 
and read my email. Some days I have a lot to read. Soon I need another 
cup of coffee. ";

        Console.Write("Введите подстроку для поиска: ");
        string? search = Console.ReadLine();

        // Проверка на null или пустую строку
        if (string.IsNullOrEmpty(search))
        {
            Console.WriteLine("Ошибка: введена пустая строка!");
            return;
        }

        var (dPositions, dComparisons) = DirectSearch(text, search);
        var (rPositions, rComparisons) = RabinKarp(text, search);

        Console.WriteLine("\nПрямой перебор:");
        if (dPositions.Count > 0)
            Console.WriteLine("Подстрока найдена в позициях: " + string.Join(", ", dPositions));
        else
            Console.WriteLine("Подстрока не найдена");
        Console.WriteLine("Количество сравнений: " + dComparisons);

        Console.WriteLine("\nМетод Рабина-Карпа:");
        if (rPositions.Count > 0)
            Console.WriteLine("Подстрока найдена в позициях: " + string.Join(", ", rPositions));
        else
            Console.WriteLine("Подстрока не найдена");
        Console.WriteLine("Количество сравнений: " + rComparisons);

        if (rComparisons < dComparisons && rComparisons > 0)
        {
            Console.WriteLine($"Метод Рабина-Карпа быстрее чем метод прямого поиска в {(dComparisons / (float)rComparisons):F2} раз.");
        }
        else if (dComparisons < rComparisons && dComparisons > 0)
        {
            Console.WriteLine($"Метод прямого поиска быстрее чем метод Рабина-Карпа в {(rComparisons / (float)dComparisons):F2} раз.");
        }
        else if (dComparisons == 0 && rComparisons == 0)
        {
            Console.WriteLine("Сравнений нет.");
        }
        else
        {
            Console.WriteLine("Методы сделали одинаковое количество сравнений.");
        }

        Console.WriteLine("\nАнализ зависимости трудоёмкости Рабина-Карпа от длины подстроки:");
        var values = new List<int>();
        int maxLen = Math.Min(50, text.Length);
        
        for (int m = 1; m <= maxLen; m++)
        {
            string sub = text.Substring(0, m);
            var (_, rComparisonsLen) = RabinKarp(text, sub);
            values.Add(rComparisonsLen);
            Console.WriteLine($"Длина подстроки {m}: сравнений = {rComparisonsLen}");
        }
        var series = new LineSeries<int> 
        { 
            Values = values,
            Name = "Сравнения Рабина-Карпа"
        };

        var chart = new SKCartesianChart 
        { 
            Width = 1000,
            Height = 600,
            Series = new[] { series },
            XAxes = new[] 
            { 
                new Axis 
                { 
                    Name = "Длина подстроки (символы)",
                    NameTextSize = 14,
                    TextSize = 12
                } 
            },
            YAxes = new[] 
            { 
                new Axis 
                { 
                    Name = "Количество сравнений",
                    NameTextSize = 14,
                    TextSize = 12
                } 
            }
        };

        chart.SaveImage("rabin_karp_analysis.png");
        Process.Start("open", "rabin_karp_analysis.png");
    }
}
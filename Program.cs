using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    // Вершины: A = 0, B = 1, C = 2, D = 3, E = 4.
    // Строка массива: откуда, куда, вес.
    static int[,] edges = {
        {0, 1, 4}, {0, 2, 5}, {1, 2, -2},
        {1, 3, 6}, {2, 3, 3}, {3, 4, 2}, {2, 4, 7}
    };
    static bool[] active = {true, true, true, true, true, true, true};

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        while (true)
        {
            Console.WriteLine("\nРёбра графа (движение только по стрелке):");
            for (int i = 0; i < active.Length; i++)
                Console.WriteLine($"{i + 1}: {(char)('A' + edges[i, 0])} -> " +
                    $"{(char)('A' + edges[i, 1])}, вес {edges[i, 2]} " +
                    (active[i] ? "[есть]" : "[разорвано]"));

            Console.WriteLine("1 — найти путь; 2 — разорвать/вернуть ребро; 0 — выход");
            string command = Console.ReadLine();
            if (command == null || command == "0") return;
            if (command == "1")
            {
                Console.Write("Начальная вершина (A–E, латиница): ");
                string from = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
                Console.Write("Конечная вершина (A–E, латиница): ");
                string to = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
                if (from.Length != 1 || to.Length != 1 ||
                    from[0] < 'A' || from[0] > 'E' || to[0] < 'A' || to[0] > 'E')
                    Console.WriteLine("Введите латинскую букву от A до E.");
                else
                    FloydWarshall(from[0] - 'A', to[0] - 'A');
            }
            else if (command == "2")
            {
                Console.Write("Номер ребра (1–7): ");
                if (int.TryParse(Console.ReadLine(), out int number) &&
                    number >= 1 && number <= active.Length)
                    active[number - 1] = !active[number - 1];
                else
                    Console.WriteLine("Неверный номер.");
            }
            else Console.WriteLine("Неизвестная команда.");
        }
    }

    static void FloydWarshall(int start, int finish)
    {
        const int n = 5;
        const long infinity = long.MaxValue;
        long[,] distance = new long[n, n];
        int[,] next = new int[n, n];

        // В себя — 0, до остальных пока нет пути.
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            {
                distance[i, j] = i == j ? 0 : infinity;
                next[i, j] = -1;
            }

        // Записываем только включённые рёбра.
        for (int e = 0; e < active.Length; e++)
            if (active[e])
            {
                int a = edges[e, 0], b = edges[e, 1], weight = edges[e, 2];
                if (weight < distance[a, b])
                {
                    distance[a, b] = weight;
                    next[a, b] = b;
                }
            }

        // k — промежуточная вершина. Пробуем путь i -> k -> j.
        for (int k = 0; k < n; k++)
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (distance[i, k] != infinity && distance[k, j] != infinity &&
                        distance[i, k] + distance[k, j] < distance[i, j])
                    {
                        distance[i, j] = distance[i, k] + distance[k, j];
                        next[i, j] = next[i, k];
                    }

        // Отрицательное число на диагонали означает отрицательный цикл.
        for (int k = 0; k < n; k++)
            if (distance[k, k] < 0)
            {
                Console.WriteLine("В графе есть отрицательный цикл. Расчёт остановлен.");
                return;
            }

        Console.WriteLine("Матрица кратчайших расстояний (нет = пути нет):");
        Console.Write("     ");
        for (int j = 0; j < n; j++) Console.Write($"{(char)('A' + j),6}");
        Console.WriteLine();
        for (int i = 0; i < n; i++)
        {
            Console.Write($"{(char)('A' + i),5}");
            for (int j = 0; j < n; j++)
                Console.Write($"{(distance[i, j] == infinity ? "нет" : distance[i, j].ToString()),6}");
            Console.WriteLine();
        }

        if (distance[start, finish] == infinity)
        {
            Console.WriteLine("Пути нет.");
            return;
        }
        List<char> path = new List<char>();
        int current = start;
        path.Add((char)('A' + current));
        while (current != finish)
        {
            current = next[current, finish];
            path.Add((char)('A' + current));
        }
        Console.WriteLine("Путь: " + string.Join(" -> ", path));
        Console.WriteLine("Суммарный вес: " + distance[start, finish]);
    }
}

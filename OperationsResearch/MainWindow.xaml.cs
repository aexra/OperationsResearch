using CommunityToolkit.WinUI.UI.Controls.Primitives;
using Microsoft.UI.Xaml;
using OperationsResearch.Services;
using OperationsResearch.Structures;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace OperationsResearch;
public sealed partial class MainWindow : Window
{
    TransportProblemTable InitialValues = new();

    public MainWindow()
    {
        this.InitializeComponent();

        LogService.Log("Application launched");

        FillInitialTable();

        ShowLabInfoButton_Click(null, null);
    }

    // WEIRD STUFF FOR LAB
    private void FillInitialTable()
    {
        // Мой вариант
        InitialValues.Rows.Add(new List<object>() { 1, 4, 5, 3, 1, 200 });
        InitialValues.Rows.Add(new List<object>() { 2, 3, 1, 4, 2, 350 });
        InitialValues.Rows.Add(new List<object>() { 2, 1, 3, 1, 2, 150 });
        InitialValues.Rows.Add(new List<object>() { 100, 100, 80, 90, 70 });

        // Пример с capacity < requests
        //InitialValues.Rows.Add(new List<object>() { 1, 3, 10 });
        //InitialValues.Rows.Add(new List<object>() { 1, 2, 10 });
        //InitialValues.Rows.Add(new List<object>() { 50, 100 });
    }

    // BUTTON CLICK EVENTS
    private void ShowInitialTableButton_Click(object sender, RoutedEventArgs e)
    {
        LogService.Log(InitialValues.ToLongString());
    }
    private void ShowLabInfoButton_Click(object sender, RoutedEventArgs e)
    {
        LogService.Log("Лабораторная работа по Исследованию операций №1\nФомин Н. А.\nВПР22");
    }
    private void SolveButton_Click(object sender, RoutedEventArgs e)
    {
        // Получим копию нормализованной исходной таблицы
        var Values = InitialValues.Normalized();

        // Найдем опорный план
        LogService.Log("Найдём опорный план");

        List<Vector2> path = new();

        var x = 0;
        var y = 0;

        while (true)
        {
            LogService.Log($"Рассматриваемая точка: ({x}, {y})");

            var min = Math.Min(Values.Capacity[y], Values.Requests[x]);

            LogService.Log($"Минимальное: {min}");

            Values.Rows[y][^1] = Values.Capacity[y] - min;
            Values.Rows[^1][x] = Values.Requests[x] - min;

            if ((int)Values.Rows[y][^1] == 0)
            {
                for (var i = x + 1; i < Values.Requests.Count; i++)
                {
                    Values.Rows[y][i] = 'x';
                }
            }
            else if ((int)Values.Rows[^1][x] == 0)
            {
                for (var i = y + 1; i < Values.Capacity.Count; i++)
                {
                    Values.Rows[i][x] = 'x';
                }
            }

            LogService.Log("Результат:\n" + Values.ToLongString());

            path.Add(new(x, y));

            x++;
            if (x >= Values.Rows[^1].Count) { x--; y++; }
            if (y >= Values.Rows.Count - 1) break;

            var toForceBreak = false;
            var v = Values.Rows.Count;
            while (Values.Capacity[y] <= 0) { y++; LogService.Log($"{y}"); if (y >= Values.Rows.Count - 1) { toForceBreak = true; break; } }
            if (toForceBreak) break;
        }

        var s = "Найденный опорный план:";
        path.ForEach(x => s += $"\n({x.Y+1}, {x.X+1})");
        LogService.Log(s);
    }
}

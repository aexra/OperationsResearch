using ABI.System.Collections.Generic;
using CommunityToolkit.WinUI.UI.Controls.Primitives;
using Microsoft.UI.Xaml;
using OperationsResearch.Classes.TransportProblem;
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

    TransportProblem Problem;

    public MainWindow()
    {
        this.InitializeComponent();

        LogService.Log("Application launched");

        //FillInitialMyTable();

        //ShowLabInfoButton_Click(null, null);

        /// HERE GOES PONOS

        FillVariantMine();

        LogService.Log(Problem.GetTableString());
    }

    /// SOME VARIANTS OF TRANSPORT PROBLEM
    // MY PROBLEM
    private void FillVariantMine()
    {
        Problem = new(
            new int[] { 200, 350, 150 }, 
            new int[] { 100, 100, 80, 90, 70 }, 
            new int[][] { 
                new int[] { 1, 4, 5, 3, 1 }, 
                new int[] { 2, 3, 1, 4, 2 }, 
                new int[] { 2, 1, 3, 1, 2 } 
            }
        );
    }

    // WEIRD STUFF FOR LAB
    private void FillInitialMyTable()
    {
        // Мой вариант
        InitialValues.Rows.Add(new List<object>() { 1, 4, 5, 3, 1, 200 });
        InitialValues.Rows.Add(new List<object>() { 2, 3, 1, 4, 2, 350 });
        InitialValues.Rows.Add(new List<object>() { 2, 1, 3, 1, 2, 150 });
        InitialValues.Rows.Add(new List<object>() { 100, 100, 80, 90, 70 });
    }
    private void FillInitialTable2()
    {
        // Пример с capacity < requests
        InitialValues.Rows.Add(new List<object>() { 1, 1, 1, 14 });
        InitialValues.Rows.Add(new List<object>() { 1, 1, 1, 20 });
        InitialValues.Rows.Add(new List<object>() { 1, 1, 1, 33 });
        InitialValues.Rows.Add(new List<object>() { 50, 100, 200 });
    }
    private void FillInitialMaksTable()
    {
        // Вариант Фоминцева М.
        InitialValues.Rows.Add(new List<object>() { 1, 2, 4, 1, 5, 200 });
        InitialValues.Rows.Add(new List<object>() { 1, 2, 1, 3, 1, 120 });
        InitialValues.Rows.Add(new List<object>() { 2, 1, 3, 3, 1, 150 });
        InitialValues.Rows.Add(new List<object>() { 100, 90, 200, 30, 80 });
    }
    private List<Vector4> GetPivotPlan(TransportProblemTable Values)
    {
        List<Vector4> path = new();

        var x = 0;
        var y = 0;

        while (true)
        {
            LogService.Log($"Рассматриваемая точка: ({y}, {x})");
            if (Values.Rows[y][x] is char)
            {
                y++;
                if (y >= Values.Rows.Count - 1) break;
                continue;
            }

            var min = Math.Min(Values.Capacity[y], Values.Requests[x]);

            LogService.Log($"Минимальное: {min}");

            Values.Rows[y][^1] = Values.Capacity[y] - min;
            Values.Rows[^1][x] = Values.Requests[x] - min;

            // х всё что справа
            if ((int)Values.Rows[y][^1] == 0)
            {
                for (var i = x + 1; i < Values.Requests.Count; i++)
                {
                    Values.Rows[y][i] = 'x';
                }
            }
            // х всё что снизу
            else if ((int)Values.Rows[^1][x] == 0)
            {
                for (var i = y + 1; i < Values.Capacity.Count; i++)
                {
                    Values.Rows[i][x] = 'x';
                }
            }

            LogService.Log("Результат:\n" + Values.ToLongString());

            path.Add(new(x, y, min, (int)Values.Rows[y][x]));

            
            if ((int)Values.Rows[y][^1] == 0) { y++; }
            else if ((int)Values.Rows[^1][x] == 0) { x++; }

            if (x >= Values.Rows[^1].Count) { x = 0; y++; }
            if (y >= Values.Rows.Count - 1) { y--; }

            if (x == Values.Rows[^1].Count - 1 && y == Values.Rows.Count - 2) break;
        }

        return path;
    }
    private string GetV4ListStack(List<Vector4> list, string title)
    {
        var s = title;
        list.ForEach(x => s += $"\n({x.Y + 1}, {x.X + 1}, {x.Z} * {x.W})");
        return s;
    }
    private int GetTargetFuncValue(TransportProblemTable table, List<Vector4> path)
    {
        var sum = 0;
        foreach (var vec in path)
        {
            sum += (int)vec.Z * (int)vec.W;
        }
        return sum;
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
        var table = InitialValues.Normalized();

        // Найдем опорный план
        LogService.Log("Найдём опорный план");
        var path = GetPivotPlan(table);
        LogService.Log(GetV4ListStack(path, "Найденный опорный план:"));

        // Значение целевой функции
        var targetFuncVal = GetTargetFuncValue(table, path);
        LogService.Log($"Стоимость перевозок: {targetFuncVal}");
    }
}

using ABI.System.Collections.Generic;
using CommunityToolkit.WinUI.UI.Controls.Primitives;
using Microsoft.UI.Xaml;
using OperationsResearch.Classes.TransportProblem;
using OperationsResearch.Extensions;
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
    public MainWindow()
    {
        this.InitializeComponent();

        LogService.Log("Application launched");   
    }

    /// SOME VARIANTS OF TRANSPORT PROBLEM
    // MY PROBLEM
    private TransportProblem GetVariantMine()
    {
        return new(
            new int[] { 200, 350, 150 }, 
            new int[] { 100, 100, 80, 90, 70 }, 
            new int[][] { 
                new int[] { 1, 4, 5, 3, 1 }, 
                new int[] { 2, 3, 1, 4, 2 }, 
                new int[] { 2, 1, 3, 1, 2 } 
            }
        );
    }
    // MASKIM FOMINTSEV'S PROBLEM
    private TransportProblem GetVariantMaks()
    {
        return new(
            new int[] { 200, 120, 150 },
            new int[] { 100, 90, 200, 30, 80 },
            new int[][] {
                new int[] { 1, 2, 4, 1, 5 },
                new int[] { 1, 2, 1, 3, 1 },
                new int[] { 2, 1, 3, 3, 1 }
            }
        );
    }
    // WEIRD PROBLEM
    private TransportProblem GetVariantWeird()
    {
        return new(
            new int[] { 14, 20, 33 },
            new int[] { 50, 100, 200 },
            new int[][] {
                new int[] { 1, 1, 1 },
                new int[] { 1, 1, 1 },
                new int[] { 1, 1, 1 }
            }
        );
    }

    /// SOME WEIRD STUFF GOES HERE
    // BASE METHOD TO OUTPUT SOLUTION
    private void Solve(TransportProblem problem)
    {
        LogService.Log("\n===============================\n");

        LogService.Log(problem.GetTableString());

        switch(problem.Normalize())
        {
            case Enums.TransportProblemNormalizationResult.Match:
                LogService.Log("Как видно, суммарная потребность груза в пунктах назначения совпадает с запасами груза на базах, а значит необходимое и достаточное условие условие разрешимости задачи выполняется");
                break;
            case Enums.TransportProblemNormalizationResult.Deficiency:
                LogService.Log("Как видно, суммарная потребность груза в пунктах назначения превышает запасы груза на базах. Следовательно, модель исходной транспортной задачи является открытой. Чтобы получить закрытую модель, введем дополнительную (фиктивную) базу");
                break;
            case Enums.TransportProblemNormalizationResult.Excess:
                LogService.Log("Как видно, суммарная потребность груза в пунктах назначения меньше запасов груза на базах. Следовательно, модель исходной транспортной задачи является открытой. Чтобы получить закрытую модель, введем дополнительную (фиктивную) потребность");
                break;
        }
        LogService.Log(problem.GetTableString());

        var plan = problem.GetInitialPlan();

        LogService.Log(plan.Mask);
        LogService.Log($"Путь алгоритма:\n{string.Join("\n", plan.Path.Select(x => $"(i,j)=({x.Y}, {x.X}), min={x.Z}, Cij={x.W}"))}");
        LogService.Log($"Начальное целевое значение: {plan.GetTargetValue()}");

        plan.GetUVPotentials(out var us, out var vs);
        LogService.Log($"Потенциалы Ui: {string.Join(", ", us)}\nПотенциалы Vi: {string.Join(", ", vs)}");

        var ic = plan.GetIndirectCosts();
        LogService.Log("Dij:\n" + ic.ToLongString());

        if (!plan.Improve(1))
        {
            LogService.Warning("Не удалось улучшить план методом потенциалов");
        }
        else
        {
            LogService.Log("План успешно улучшен методом потенциалов");
            LogService.Log(plan.Mask);
            LogService.Log($"Улучшенное целевое значение: {plan.GetTargetValue()}");
        }
    }

    // BUTTON CLICK EVENTS
    private void ShowLabInfoButton_Click(object sender, RoutedEventArgs e)
    {
        LogService.Log("Лабораторная работа по Исследованию операций №1\nФомин Н. А.\nВПР22");
    }
    private void SolveMyButton_Click(object sender, RoutedEventArgs e)
    {
        Solve(GetVariantMine());
    }
    private void SolveMaksButton_Click(object sender, RoutedEventArgs e)
    {
        Solve(GetVariantMaks());
    }
    private void SolveWeirdButton_Click(object sender, RoutedEventArgs e)
    {
        Solve(GetVariantWeird());
    }
}

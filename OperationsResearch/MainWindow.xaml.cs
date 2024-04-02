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

    // BUTTON CLICK EVENTS
    private void ShowLabInfoButton_Click(object sender, RoutedEventArgs e)
    {
        LogService.Log("������������ ������ �� ������������ �������� �1\n����� �. �.\n���22");
    }
    private void SolveMyButton_Click(object sender, RoutedEventArgs e)
    {
        var problem = GetVariantMine();
        LogService.Log(problem.GetTableString());

        problem.Normalize();
        LogService.Log(problem.GetTableString());

        var path = problem.GetInitialPlanMask(out var mask);
        path.ForEach(x => LogService.Log(x.ToLongString()));

        LogService.Log($"��������� ������� ��������: {problem.GetInitialTargetValue()}");
    }
}

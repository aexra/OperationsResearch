using Microsoft.UI.Xaml;
using OperationsResearch.Classes.TransportProblem;
using OperationsResearch.Services;
using System.Data;
using System.Linq;

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
    // PROBLEM 1
    private TransportProblem GetVariant1()
    {
        return new(
            new int[] { 200, 150, 350 },
            new int[] { 120, 120, 200, 180, 110 },
            new int[][]
            {
                new int[] { 1, 2, 3, 5, 2 },
                new int[] { 4, 6, 7, 3, 1 },
                new int[] { 2, 2, 3, 4, 5 }
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
    // WEIRD PROBLEM
    private TransportProblem GetVariantKirill()
    {
        return new(
            new int[] { 300, 120, 300 },
            new int[] { 100, 120, 130, 100, 90 },
            new int[][] {
                new int[] { 1, 4, 5, 3, 1 },
                new int[] { 2, 1, 2, 1, 2 },
                new int[] { 3, 1, 4, 2, 1 }
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
                LogService.Log("��� �����, ��������� ����������� ����� � ������� ���������� ��������� � �������� ����� �� �����, � ������ ����������� � ����������� ������� ������� ������������ ������ �����������");
                break;
            case Enums.TransportProblemNormalizationResult.Deficiency:
                LogService.Log("��� �����, ��������� ����������� ����� � ������� ���������� ��������� ������ ����� �� �����. �������������, ������ �������� ������������ ������ �������� ��������. ����� �������� �������� ������, ������ �������������� (���������) ����");
                break;
            case Enums.TransportProblemNormalizationResult.Excess:
                LogService.Log("��� �����, ��������� ����������� ����� � ������� ���������� ������ ������� ����� �� �����. �������������, ������ �������� ������������ ������ �������� ��������. ����� �������� �������� ������, ������ �������������� (���������) �����������");
                break;
        }
        LogService.Log(problem.GetTableString());

        var plan = problem.GetInitialPlan();

        LogService.Log(plan.Mask);
        LogService.Log($"���� ���������:\n{string.Join("\n", plan.Path.Select(x => $"(i,j)=({x.Y}, {x.X}), min={x.Z}, Cij={x.W}"))}");
        LogService.Log($"��������� ������� ��������: {plan.GetTargetValue()}");

        if (!plan.Improve(10))
        {
            LogService.Warning("�� ������� �������� ���� ������� �����������");
        }
        else
        {
            LogService.Log("���� ������� ������� ������� �����������");
            LogService.Log(plan.Mask);
            LogService.Log($"���������� ������� ��������: {plan.GetTargetValue()}");
        }
    }

    // BUTTON CLICK EVENTS
    private void ShowLabInfoButton_Click(object sender, RoutedEventArgs e)
    {
        LogService.Log("������������ ������ �� ������������ �������� �1\n����� �. �.\n���22");
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
    private void SolveKirillButton_Click(object sender, RoutedEventArgs e)
    {
        Solve(GetVariantKirill());
    }
    private void SolveV1Button_Click(object sender, RoutedEventArgs e)
    {
        Solve(GetVariant1());
    }
}

using Microsoft.UI.Xaml;
using OperationsResearch.Services;

namespace OperationsResearch;
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        LogService.Log("Application launched");
    }
}

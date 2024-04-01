using Microsoft.UI.Xaml;
using OperationsResearch.Services;
using OperationsResearch.Structures;
using System;
using System.Collections.Generic;
using System.Data;

namespace OperationsResearch;
public sealed partial class MainWindow : Window
{
    Table Values = new();

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
        Values.Rows.Add(new List<object>(){ "1", "4", "5", "3", "1" });
        Values.Rows.Add(new List<object>() { "2", "3", "1", "4", "2" });
        Values.Rows.Add(new List<object>() { "2", "1", "3", "1", "2" });
        Values.Headers = new List<string>() { "B1=100", "B2=100", "B3=80", "B4=90", "B5=70" };
        Values.Names = new List<string>() { "A1=200", "A2=350", "A3=150" };
    }

    // BUTTON CLICK EVENTS
    private void ShowInitialTableButton_Click(object sender, RoutedEventArgs e)
    {
        LogService.Log(Values.ToLongString());
    }
    private void ShowLabInfoButton_Click(object sender, RoutedEventArgs e)
    {
        LogService.Log("Лабораторная работа по Исследованию операций №1\nФомин Н. А.\nВПР22");
    }
}

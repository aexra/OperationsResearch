using Microsoft.UI.Xaml;
using OperationsResearch.Services;
using OperationsResearch.Structures;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OperationsResearch;
public sealed partial class MainWindow : Window
{
    TransportProblemTable InitialValues = new();
    TransportProblemTable Values = new();

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
        InitialValues.Rows.Add(new List<object>() { 200, 1, 4, 5, 3, 1 });
        InitialValues.Rows.Add(new List<object>() { 350, 2, 3, 1, 4, 2 });
        InitialValues.Rows.Add(new List<object>() { 150, 2, 1, 3, 1, 2 });
        InitialValues.Rows.Add(new List<object>() { 100, 100, 80, 90, 70 });

        // Пример с capacity < requests
        //InitialValues.Rows.Add(new List<object>() { 10, 1, 3 });
        //InitialValues.Rows.Add(new List<object>() { 10, 2, 1 });
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
        Values = InitialValues.Normalized();

        


    }
}

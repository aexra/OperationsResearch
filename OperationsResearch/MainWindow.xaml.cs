using Microsoft.UI.Xaml;
using OperationsResearch.Services;
using OperationsResearch.Structures;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OperationsResearch;
public sealed partial class MainWindow : Window
{
    TransportProblemTable InitialValues = new();
    TransportProblemTable Values = new();
    List<int> Capacity => GetCapacity();
    List<int> Requests => GetRequests();

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
        //Values.Rows.Add(new List<object>(){ 1, 4, 5, 3, 1 });
        //Values.Rows.Add(new List<object>() { 2, 3, 1, 4, 2 });
        //Values.Rows.Add(new List<object>() { 2, 1, 3, 1, 2 });
        //Values.Headers = new List<string>() { "B1=100", "B2=100", "B3=80", "B4=90", "B5=70"};
        //Values.Names = new List<string>() { "A1=200", "A2=350", "A3=150" };
        InitialValues.Rows.Add(new List<object>() { 10, 1, 3 });
        InitialValues.Rows.Add(new List<object>() { 10, 2, 1 });
        InitialValues.Rows.Add(new List<object>() { 50, 100 });
    }
    private List<int> GetCapacity()
    {
        var list = new List<int>();
        for (var i = 0; i < Values.Rows.Count - 1; i++)
        {
            list.Add((int)Values.Rows[i][0]);
        }
        return list;
    }
    private List<int> GetRequests()
    {
        return Values.Rows[^1].Cast<int>().ToList();
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
        // Копируем исходную таблицу
        Values = (TransportProblemTable)InitialValues.Clone();

        // Проверим необходимое и достаточное условие разрешимости задачи
        LogService.Log("Проверим необходимое и достаточное условие разрешимости задачи:");

        var totalcap = Capacity.Sum();
        var totalreq = Requests.Sum();
        LogService.Log($"Запасы: {totalcap}\nПотребности: {totalreq}");

        // Оценим знаения
        if (totalcap == totalreq)
        {
            LogService.Log("Значения равны, следовательно необходимое и достаточное условие разрешимости задачи выполняется");
        }
        else
        {
            var diff = Math.Abs(totalcap - totalreq);
            if (totalcap > totalreq)
            {
                LogService.Log($"Как видно, суммарная потребность груза в пунктах назначения меньше запасов груза на базах. Следовательно, модель исходной транспортной задачи является открытой. Чтобы получить закрытую модель, введем дополнительную (фиктивную) потребность, равной {totalcap} - {totalreq} = {diff}. Тарифы перевозки единицы груза к этому потребителю полагаем равны нулю.\r\nЗанесем исходные данные в распределительную таблицу.");
                for (var i = 0; i < Values.Rows.Count - 1; i++)
                {
                    Values.Rows[i].Add(0);
                }
                Values.Rows[^1].Add(diff);
            }
            else
            {
                LogService.Log($"Как видно, суммарная потребность груза в пунктах назначения превышает запасы груза на базах. Следовательно, модель исходной транспортной задачи является открытой. Чтобы получить закрытую модель, введем дополнительную (фиктивную) базу с запасом груза, равным {totalreq} - {totalcap} = {diff}. Тарифы перевозки единицы груза из базы ко всем потребителям полагаем равны нулю.\r\nЗанесем исходные данные в распределительную таблицу.");
                Values.Rows.Insert(Values.Rows.Count - 1, new List<object>() { diff, 0, 0 });
            }
            LogService.Log(Values.ToLongString());
        }
    }
}

﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using MyApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

public class GoogleApi
{
    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static readonly string ApplicationName = "SeaBattle";
    static readonly string SpreadSheetId = "1eb2vaC45-prUvId_fjTEPqHbbeXYbooVciDVkXQBmkU";
    static SheetsService service;
    static public object[,] array = new object[10, 10];
    static public string[] sheetsName = new string[4] { "1", "2", "3", "4" };

    public GoogleApi()
    {
        Acsess();
        array = ReadEntries(Form2.sheetsArray[0]);
        CreateEntry();
        Form2 myForm = new Form2();
        Form2.sheetsArray = myForm.WriteToArray();
    }

    public static void Acsess()
    {
        GoogleCredential credential;
        using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(Scopes);
        }

        service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    public static object[,] ReadEntries(string sheet)
    {
        var range = $"{sheet}!A1:J10"; //задаем диапазоны
        var request = service.Spreadsheets.Values.Get(SpreadSheetId, range); //объект запроса

        var response = request.Execute(); //объект ответа
        var values = response.Values; //доступ к значниям
        object[,] mass = new object[10, 10];
        var raw1 = new List<object>();
        //if (values != null && values.Count > 0)
        //{
        //int i = 0;
        int j = 0;
        while (j < 10)
        {
            while (values.Count < 10)
            {
                while (raw1.Count < 10)
                {
                    raw1.Add("");
                }
                values.Add(raw1);
            }
            
            foreach (var raw in values)
            {
                while (raw.Count < 10)
                {
                    raw.Add("");
                }
                
                for (int i = 0; i < 10; i++)
                {
                    mass[j, i] = raw[i];
                }
                j++;
            }
        }
        return mass;
    }

    public static void CreateEntry()
    {
        object valuePlayer, valueEnemy;
        valuePlayer = Form2.sheetsArray[0];
        valueEnemy = Form2.sheetsArray[1];

        var range = $"{valuePlayer}!L1:M1";
        var valueRange = new ValueRange();

        var objectList = new List<object>() { valuePlayer, valueEnemy };
        valueRange.Values = new List<IList<object>> { objectList };

        var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadSheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        var appendResponse = appendRequest.Execute();
    }

    public static void WrtiteTern(string sheet, int tern)
    {
        var range = $"{sheet}!O3:O100";
        var valueRange = new ValueRange();

        var objectList = new List<object>() { tern };
        valueRange.Values = new List<IList<object>> { objectList };

        var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadSheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        var appendResponse = appendRequest.Execute();
    }
    public static int[] ReadTern(string sheet)
    {
        var range = $"{sheet}!O3:O100"; //задаем диапазоны
        var request = service.Spreadsheets.Values.Get(SpreadSheetId, range); //объект запроса

        var response = request.Execute(); //объект ответа
        var values = response.Values; //доступ к значниям
        int index = values.Count;
        int[] mass = new int[index];
        int i = 0;
        while (i < index)
        {
            foreach (var raw in values)
            {
                mass[i] = Convert.ToInt32(raw[0]);
                i++;
            }
        }
        
        return mass;
    }

}

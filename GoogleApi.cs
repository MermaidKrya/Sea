using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using MyApp;
using System;
using System.IO;

public class GoogleApi
{
    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static readonly string ApplicationName = "SeaBattle";
    static readonly string SpreadSheetId = "1eb2vaC45-prUvId_fjTEPqHbbeXYbooVciDVkXQBmkU";
    static readonly string sheet = "Number1";
    static SheetsService service;
    static public object[,] array = new object[10, 10];

    public GoogleApi()
	{
        Acsess();
        array = ReadEntries();
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

    public static object[,] ReadEntries()
    {
        var range = $"{sheet}!A1:J10"; //задаем диапазоны
        var request = service.Spreadsheets.Values.Get(SpreadSheetId, range); //объект запроса

        var response = request.Execute(); //объект ответа
        var values = response.Values; //доступ к значниям
        object[,] mass = new object[10, 10];
        //if (values != null && values.Count > 0)
        //{
            int j = 0;
            while (j < 10)
            {            
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
}

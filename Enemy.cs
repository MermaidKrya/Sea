using MyApp;
using System;
using System.Drawing;
using System.Windows.Forms;

public class Enemy
{
	public int[,] myMap = new int[Form1.mapSize, Form1.mapSize];
	public int[,] enemyMap = new int[Form1.mapSize, Form1.mapSize];

	public Button[,] myButtons = new Button[Form1.mapSize, Form1.mapSize];
	public Button[,] enemyButtons = new Button[Form1.mapSize, Form1.mapSize];

	public Enemy(int[,] myMap, int[,] enemyMap, Button[,] myButtons, Button[,] enemyButtons)
	{
		this.myMap = myMap;
		this.enemyMap = enemyMap;
		this.enemyButtons = enemyButtons;
		this.myButtons = myButtons;
	}


    public int [,] ConfigureShips()
    {
        GoogleApi.array = GoogleApi.ReadEntries(Form2.sheetsArray[1]);
        for (int i = 0; i < Form1.mapSize; i++)
        {
            for (int j = 0; j < Form1.mapSize; j++)
            {
                if (GoogleApi.array[i, j] != "")
                {
                    myMap[i, j] = 1;
                }
                else
                {
                    myMap[i, j] = 0;
                }
                
            }
        }
        return myMap;
    }

}

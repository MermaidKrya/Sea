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

   /* public bool Shoot()
    {
        bool hit = false;
        int posX = (pressedButton.Location.X - delta) / Form1.cellSize];
        int posY = pressedButton.Location.Y / Form1.cellSize;
        if (enemyMap[posX, posY] != 0)
        {
            hit = true;
            enemyMap[posX, posY] = 0;
            enemyButtons[posX, posY].BackColor = Color.Blue;
        }
        else
        {
            hit = false;
            enemyButtons[posX, posY].BackColor = Color.Black;
        }
        if (hit)
            Shoot();
        return hit;
    }*/


}

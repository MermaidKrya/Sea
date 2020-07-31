﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyApp
{
    public partial class Form1 : Form
    {
        public const int mapSize = 10;
        public const int cellSize = 90;

        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];

        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public static bool isPlaying = false;
        public Enemy enemy;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Морской бой";
            Init();
        }

        public void Init()
        {
            isPlaying = false;
            GoogleApi.Acsess();
            GoogleApi.array = GoogleApi.ReadEntries();
            CreateMap(myMap, myButtons, 0, "Игрок");
            ButtonStart();
            enemy = new Enemy(enemyMap, myMap, enemyButtons, myButtons);
            enemyMap = enemy.ConfigureShips();
        }

        public void CreateMap(int[,] map, Button[,] buttons, int position, string text)
        {
            this.Width = mapSize * 2 * cellSize + 50;
            this.Height = (mapSize + 3) * cellSize + 20;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    map[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(position + j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;
                    if (position > 0)
                    {
                        button.Click += new EventHandler(PlayerShoot);
                    }
                    buttons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            Label mapLabel = new Label();
            mapLabel.Text = text;
            mapLabel.Location = new Point(position + mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(mapLabel);
        }

        public void ButtonStart()
        {
            Button startButton = new Button();
            startButton.Text = "Начать";
            startButton.Click += new EventHandler(Start);
            startButton.Location = new Point(0, mapSize * cellSize + 20);
            this.Controls.Add(startButton);
        }

        public void Start(object sender, EventArgs e)
        {
            isPlaying = true;
            ClearPole();
            ConfigureShips();
            CreateMap(enemyMap, enemyButtons, 950, "Противник");
        }

        public void ClearPole()
        {
            this.Controls.Clear();
        }

        public void ConfigureShips()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.Blue;
                    if (GoogleApi.array[i, j] != "")
                    {
                        button.BackColor = Color.Gray;
                        button.Text = GoogleApi.array[i, j].ToString();
                        myMap[i, j] = 1;
                    }
                    else
                    {
                        button.BackColor = Color.White;
                        myMap[i, j] = 0;
                    }
                    myButtons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            Label mapLabel = new Label();
            mapLabel.Text = "Игрок";
            mapLabel.Location = new Point(mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(mapLabel);
        }

        public void PlayerShoot(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;
            bool playerTern = Shoot(enemyMap, pressedButton);
           /* if (!playerTern)
                enemy.Shoot();*/

            if (!CheckIfMapIsNotEmpty())
            {
                ClearPole();
                Init();
            }
        }

        public bool CheckIfMapIsNotEmpty()
        {
            bool isEmpty1 = true;
            bool isEmpty2 = true;
            for (int i = 1; i < mapSize; i++)
            {
                for (int j = 1; j < mapSize; j++)
                {
                    if (myMap[i, j] != 0)
                        isEmpty1 = false;
                    if (enemyMap[i, j] != 0)
                        isEmpty2 = false;
                }
            }
            if (isEmpty1 || isEmpty2)
                return false;
            else return true;
        }

        public bool Shoot(int[,] map, Button pressedButton)
        {
            bool hit = false;
            if (isPlaying)
            {
                int delta = 0;
                if (pressedButton.Location.X >= 950)
                    delta = 950;
                if (map[(pressedButton.Location.Y) / cellSize, (pressedButton.Location.X - delta) / cellSize] != 0)
                {
                    hit = true;
                    map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] = 0;
                    pressedButton.BackColor = Color.Blue;
                    pressedButton.Text = GoogleApi.array[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize].ToString();
                }
                else
                {
                    hit = false;
                    pressedButton.BackColor = Color.Black;
                }
            }
            return hit;
        }

    }
}

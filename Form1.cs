﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisioForge.Shared.Accord.Math.Decompositions;
using VisioForge.Shared.AForge.Imaging;
using VisioForge.Shared.Decklink.SDK;

namespace MyApp
{
    public partial class Form1 : Form
    {
        public const int mapSize = 10;
        public const int cellSize = 60;

        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];

        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public static bool isPlaying = false;
        public static int tern;
        int kol = 0;
        int i = 0;

        Label whoseMove = new Label();
        Label amount = new Label();
        Label myRemainder = new Label();
        Label enemyRemainder = new Label();
        Label time = new Label();
        GroupBox groupBox = new GroupBox();

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
            GoogleApi.array = GoogleApi.ReadEntries(Form2.sheetsArray[0]);
            ConfigureShips();
            ButtonStart();
            enemy = new Enemy(enemyMap, myMap, enemyButtons, myButtons);
        }

        public void CreateMap()
        {
            this.Width = mapSize * 2 * cellSize + 50;
            this.Height = (mapSize + 3) * cellSize + 20;
        
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    enemyMap[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;
                    button.Click += new EventHandler(PlayerShoot);
                    enemyButtons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            Label mapLabel = new Label();
            mapLabel.Text = "Противник";
            mapLabel.Location = new Point(mapSize * cellSize / 2, mapSize * cellSize + 10);
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
            CreateMap();
            enemyMap = enemy.ConfigureShips();

            Timer timer = new Timer();
            timer.Interval = (2000); // 2 sec
            timer.Start();
            timer.Tick += new EventHandler(TimerTick);

            Timer mainTimer = new Timer();
            mainTimer.Interval = (1000); // 1 sec
            mainTimer.Start();
            mainTimer.Tick += new EventHandler(MainTimer_Tick);

            CreateGroupBox();
            AddToGroupBox("Ход", 25, 50, 200, 50);
            AddToGroupBox("Осталось клеток противника", 25, 100, 300, 50);
            AddToGroupBox("Осталось клеток своих", 25, 150, 300, 50);
            AddToGroupBox("Время", 25, 200, 200, 50);

            Balance(enemyRemainder, 400, 100, 100, 50);
            Balance(myRemainder, 400, 150, 100, 50);
            Balance(amount, 400, 50, 100, 50);
            Balance(time, 400, 200, 100, 50);
            myRemainder.Text = RemainderOfCells(myMap).ToString();
            enemyRemainder.Text = RemainderOfCells(enemyMap).ToString();
            amount.Text = kol.ToString();
            time.Text = DateTime.Now.ToString();

            tern = ValueOfTern(0);
            string text;
            Information();
            if (tern == 1)
            {
                text = "Ваш ход";
                whoseMove.BackColor = Color.Green;
            }
            else
            {
                text = "Ход противника";
                whoseMove.BackColor = Color.Red;
            }
            whoseMove.Text = text;
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            i++;
            time.Text = i.ToString() + " сек";
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
            kol++;
            WhoMove(pressedButton);
            int remainder1, remainder2;
            tern = ValueOfTern(0);
            myRemainder.Text = RemainderOfCells(myMap).ToString();
            enemyRemainder.Text = RemainderOfCells(enemyMap).ToString();
            amount.Text = kol.ToString();

            string text;
            if (tern == 1)
            {
                text = "Ваш ход";
                whoseMove.BackColor = Color.Green;
            }
            else
            {
                text = "Ход противника";
                whoseMove.BackColor = Color.Red;
            }
            whoseMove.Text = text;

           /* remainder1 = RemainderOfCells(enemyMap);
            remainder2 = RemainderOfCells(myMap);
            if (remainder1 == 0)
            {
                Label winFinish = new Label();
                winFinish.Font = new Font(winFinish.Font.Name, 20, winFinish.Font.Style);
                winFinish.Location = new Point(mapSize * cellSize + 20, 500);
                winFinish.Width = 300;
                winFinish.Height = 100;
                text = "Вы выиграли";
                winFinish.BackColor = Color.Green;
            }
            if (remainder2 == 0)
            {
                Label loseFnish = new Label();
                loseFnish.Font = new Font(loseFnish.Font.Name, 20, loseFnish.Font.Style);
                loseFnish.Location = new Point(mapSize * cellSize + 20, 500);
                loseFnish.Width = 300;
                loseFnish.Height = 100;
                text = "Вы проиграли";
                loseFnish.BackColor = Color.Red;
            }*/

            if (!CheckIfMapIsNotEmpty())
            {
                ClearPole();
                Form2 myForm = new Form2();
                myForm.StartGame();
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
                if (map[(pressedButton.Location.Y) / cellSize, pressedButton.Location.X / cellSize] != 0)
                {
                    hit = true;
                    map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] = 0;
                    pressedButton.BackColor = Color.Blue;
                    pressedButton.Text = GoogleApi.array[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize].ToString();
                }
                else
                {
                    hit = false;
                    pressedButton.BackColor = Color.Black;
                }
            }
            return hit;
        }

        public void WhoMove(Button pressedButton)
        {
            int tern1, tern2;
            tern1 = ValueOfTern(0);
            bool shootData = Shoot(enemyMap, pressedButton);
            if (tern1 == 1)
            {
                if (shootData) 
                {
                    tern1 = 1;
                    tern2 = 0;
                    GoogleApi.WrtiteTern(Form2.sheetsArray[0], tern1);
                    GoogleApi.WrtiteTern(Form2.sheetsArray[1], tern2);
                }
                else
                {
                    tern1 = 0;
                    tern2 = 1;
                    GoogleApi.WrtiteTern(Form2.sheetsArray[0], tern1);
                    GoogleApi.WrtiteTern(Form2.sheetsArray[1], tern2);
                }
           
            }
        }

        public static int StartMove() 
        {
            int whoMove;
            if (Convert.ToInt32(Form2.sheetsArray[0]) < Convert.ToInt32(Form2.sheetsArray[1]))
                whoMove = 1;
            else whoMove = 0;
            return whoMove;
        }

        public void UpdatePole()
        {
            tern = ValueOfTern(0);
            if (tern == 1)
                EnableOfButtons(true);
            else
                EnableOfButtons(false);
        }

        public void TimerTick(object sender, EventArgs e)
        {
            tern = ValueOfTern(0);
            if (tern == 1)
            {
                EnableOfButtons(true);
            }
                
            else
            {
                EnableOfButtons(false);
            }
                              
        }

        public int ValueOfTern(int nameOfSheet)
        {
            int tern;
            int[] mass = new int[100];
            int index = 0;
            mass = GoogleApi.ReadTern(Form2.sheetsArray[nameOfSheet]); //чтение с моей страницы
            index = mass.Length - 1;
            tern = mass[index];
            return tern;
        }

        public void EnableOfButtons(bool flag)
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    enemyButtons[i, j].Enabled = flag;
                }
            }
        }

        public void Information()
        {
            whoseMove.TextAlign = ContentAlignment.MiddleCenter;
            whoseMove.Font = new Font(whoseMove.Font.Name, 20, whoseMove.Font.Style);
            whoseMove.Location = new Point(mapSize * cellSize + 20, 0);
            whoseMove.Width = 300;
            whoseMove.Height = 100;
            whoseMove.BackColor = Color.FromArgb(255, 0, 255);
            this.Controls.Add(whoseMove);
        }

        public void Balance(Label name, int x, int y, int width, int height)
        {
            name.Location = new Point(x, y);
            name.Size = new Size(width, height);
            groupBox.Controls.Add(name);
        }

        public void AddToGroupBox(string text, int x, int y, int width, int height)
        {
            Label label = new Label { Text = text };
            label.Location = new Point(x, y);
            label.Size = new Size(width, height);
            groupBox.Controls.Add(label);
        }

        public void CreateGroupBox()
        {
            groupBox.Location = new Point(mapSize * cellSize + 20, 200);
            groupBox.Text = "Ход игры";
            groupBox.Width = 500;
            groupBox.Height = 250;
            this.Controls.Add(groupBox);
        }

        public int RemainderOfCells(int[,] map)
        {
            int remainder = 0;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == 1)
                    {
                        remainder++;
                    }
                }
            }
            return remainder;
        }
    }

}

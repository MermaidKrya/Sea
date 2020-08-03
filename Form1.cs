using System;
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

        public static int[] shootCoord = new int[2];
        public static bool isPlaying = false;
        public static int tern = 0;
        
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
            CreateMap(myMap, myButtons, 0, "Игрок");
            ButtonStart();
            enemy = new Enemy(enemyMap, myMap, enemyButtons, myButtons);
            
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
            enemyMap = enemy.ConfigureShips();
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
            bool playerTern;
            shootCoord = WriteToArrayCoord(pressedButton);
            tern = WhoMove();
                playerTern = Shoot(enemyMap, pressedButton);
            if (!playerTern)
                tern = 0;
                             
            
            //int turn;
            //GoogleApi.WriteCoord();
            //ChangeMyMapAfterShoot();
                         

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
                shootCoord = WriteToArrayCoord(pressedButton);
            }
            return hit;
        }

        public static int[] WriteToArrayCoord(Button pressedButton)
        {
            int[] mass = new int[2];
            int delta = 0;
            if (pressedButton.Location.X >= 950)
                delta = 950;
            mass[0] = (pressedButton.Location.X - delta) / cellSize;
            mass[1] = (pressedButton.Location.Y) / cellSize;
            return mass;
        }

        public void ChangeMyMapAfterShoot()
        {
            int[,] mass = new int[100, 2];
            int xCoord, yCoord;
            int index = 0;
            mass = GoogleApi.ReadCoord(Form2.sheetsArray[0]);
            index = mass.Length / 2 - 1;
            xCoord = mass[index, 1];
            yCoord = mass[index, 0];

            if (myMap[xCoord, yCoord] != 0)
            {
                myMap[xCoord, yCoord] = 0;
                myButtons[xCoord, yCoord].BackColor = Color.Blue;
            }
            else
            {
                myButtons[xCoord, yCoord].BackColor = Color.Black;
            }

        }

        public int WhoMove()
        {
            int whoMove;
            if (tern == 1)
            {
                for (int i = 0; i < mapSize; i++)
                {
                    for (int j = 0; j < mapSize; j++)
                    {
                        enemyButtons[i, j].Enabled = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < mapSize; i++)
                {
                    for (int j = 0; j < mapSize; j++)
                    {
                        enemyButtons[i, j].Enabled = false;
                    }
                }
            }
            int[] mass = new int[100];
            int index = 0;
            mass = GoogleApi.ReadTern(Form2.sheetsArray[1]);
            index = mass.Length;
            if (mass[index] == 0)
                whoMove = 1;
            else whoMove = 0;
            return whoMove;
        }

        public static int StartMove() {
            int whoMove;
            if (Convert.ToInt32(Form2.sheetsArray[0]) < Convert.ToInt32(Form2.sheetsArray[1]))
            {
                whoMove = 1;
            }
            else whoMove = 0;
            return whoMove;
        }
    }
}

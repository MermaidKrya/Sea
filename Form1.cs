using System;
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
        public int cellSize = 30;

        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];

        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public static bool isPlaying = false;
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
            CreateMyMap();
            ButtonStart();
        }

        public void CreateMyMap()
        {
            this.Width = mapSize * 2 * cellSize + 50;
            this.Height = (mapSize + 3) * cellSize + 20;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    myMap[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;
                    myButtons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            Label map1 = new Label();
            map1.Text = "Игрок";
            map1.Location = new Point(mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(map1);
        }//записать в одну функцию

        public void CreateEnemyMap()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    enemyMap[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(320 + j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;
                    button.Click += new EventHandler(PlayerShoot);
                    enemyButtons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            Label map2 = new Label();
            map2.Text = "Противник";
            map2.Location = new Point(mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(map2);
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
            CreateEnemyMap();
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
                    if (GoogleApi.array[i, j] == 1)
                    {
                        button.BackColor = Color.Red;
                        myMap[i, j] = 1;
                    }
                    else
                    {
                        button.BackColor = Color.Gray;
                        myMap[i, j] = 0;
                    }
                    myButtons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            Label map = new Label();
            map.Text = "Игрок";
            map.Location = new Point(mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(map);
        }

        public void PlayerShoot(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;
            Shoot(enemyMap, pressedButton);
        }

        public bool Shoot(int[,] map, Button pressedButton)
        {
            bool hit = false;
            if (isPlaying)
            {
                int delta = 0;
                if (pressedButton.Location.X >= 320)
                    delta = 320;
                if (map[(pressedButton.Location.Y) / cellSize, (pressedButton.Location.X - delta) / cellSize] != 0)
                {
                    hit = true;
                    map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] = 0;
                    pressedButton.BackColor = Color.Blue;
                    pressedButton.Text = "X";
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

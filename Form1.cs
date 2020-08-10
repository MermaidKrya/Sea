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
        public const int cellSize = 60;

        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];

        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public static bool isPlaying = false;
        public static int tern;

                
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
            timer.Interval = (5000); // 5 sec
            timer.Start();
            timer.Tick += new EventHandler(TimerTick);
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
            WhoMove(pressedButton);
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
            tern2 = ValueOfTern(1);
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

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        public void TimerTick(object sender, EventArgs e)
        {
            tern = ValueOfTern(0);
            if (tern == 1) 
                EnableOfButtons(true);
            else
                EnableOfButtons(false);                
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

    }

}

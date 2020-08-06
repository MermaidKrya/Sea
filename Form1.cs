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

        public static int[] shootCoord = new int[2];
        public static bool isPlaying = false;
        public static int tern;

        Timer timer = new Timer();
        public int ticks = 0;

        
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

        public void TimerTick(object sender, EventArgs e)
        {
            ticks++;
            if (ticks == 5)
            {
                int tern1;
                int[] mass1 = new int[100];
                int index1 = 0;
                mass1 = GoogleApi.ReadTern(Form2.sheetsArray[0]); //чтение с моей страницы
                index1 = mass1.Length - 1;
                tern1 = mass1[index1];

                if (tern1 == 1)
                {
                    for (int i = 0; i < mapSize; i++)
                    {
                        for (int j = 0; j < mapSize; j++)
                        {
                            enemyButtons[i, j].Click += new EventHandler(PlayerShoot);
                        }
                    }

                }
                
            }
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
            CreateMap(enemyMap, enemyButtons, 650, "Противник");
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
            
            WhoMove(pressedButton);
            //UpdatePole();
           
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
                if (pressedButton.Location.X >= 650)
                    delta = 650;
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

        public static int[] WriteToArrayCoord(Button pressedButton)
        {
            int[] mass = new int[2];
            int delta = 0;
            if (pressedButton.Location.X >= 650)
                delta = 650;
            mass[0] = (pressedButton.Location.X - delta) / cellSize;
            mass[1] = (pressedButton.Location.Y) / cellSize;
            return mass;
        }

        public void ChangeMyMapAfterShoot(string sheet)
        {
            int[,] mass = new int[100, 2];
            int xCoord, yCoord;
            int index = 0;
            mass = GoogleApi.ReadCoord(sheet);
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

        public void WhoMove(Button pressedButton)
        {
            int tern1, tern2;
            int[] mass1 = new int[100];
            int[] mass2 = new int[100];
            int index1 = 0;
            int index2 = 0;
            bool shootData;
            mass1 = GoogleApi.ReadTern(Form2.sheetsArray[0]); //чтение с моей страницы
            mass2 = GoogleApi.ReadTern(Form2.sheetsArray[1]); //чтение со страницы врага
            index1 = mass1.Length - 1;
            tern1 = mass1[index1];
            shootData = Shoot(enemyMap, pressedButton);

            shootCoord = WriteToArrayCoord(pressedButton);
            GoogleApi.WriteCoord();


            if (tern1 == 1)
            {
                for (int i = 0; i < mapSize; i++)
                {
                    for (int j = 0; j < mapSize; j++)
                    {
                        enemyButtons[i, j].Enabled = true;
                    }
                }
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
            else
            {
                for (int i = 0; i < mapSize; i++)
                {
                    for (int j = 0; j < mapSize; j++)
                    {
                        enemyButtons[i, j].Enabled = false;
                    }
                }
                ChangeMyMapAfterShoot(Form2.sheetsArray[0]);
                //ChangeMyMapAfterShoot(Form2.sheetsArray[1]);
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
            int index1, index2;
            int tern1, tern2;
            int[] mass1 = new int[100];
            int[] mass2 = new int[100];

            mass1 = GoogleApi.ReadTern(Form2.sheetsArray[0]); //чтение с моей страницы
            mass2 = GoogleApi.ReadTern(Form2.sheetsArray[1]);

            index1 = mass1.Length - 1;
            index2 = mass1.Length - 1;

            tern1 = mass1[index1];
            tern2 = mass1[index2];

            Button button = new Button();
            Form2 myForm = new Form2();
            if (tern1 == 1)
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
            if (tern1 == 1)
            {
                for (int i = 0; i < mapSize; i++)
                {
                    for (int j = 0; j < mapSize; j++)
                    {
                        enemyButtons[i, j].Enabled = true;
                    }
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Interval = (10 * 1000); // 10 secs
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
        }

    }
}

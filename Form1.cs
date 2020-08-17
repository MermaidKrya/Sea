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
using VisioForge.Shared.AForge.Imaging;
using VisioForge.Shared.Decklink.SDK;

namespace MyApp
{
    public partial class Form1 : Form
    {
        public const int mapSize = 10; //размер массива для кораблей
        public const int cellSize = 60; //размер отдельной клетки

        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];

        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public static bool isPlaying = false;   //показывает началась игра или нет
        public static int tern; // 0/1 в зависимости от того,чей сейчас ход
        int kol = 0; //подсчитывет количество ходов
        int i = 0; // подсчитывает время всей игры

        GroupBox groupBox = new GroupBox(); //в нем записаны все последующие пункты
        Label whoseMove = new Label();  //выводит на экран, чей сейчас ход
        Label amount = new Label(); //количество ходов
        Label myRemainder = new Label();    //остаток моих палуб
        Label enemyRemainder = new Label(); //остаток палуб противника
        Label time = new Label();   //общее время игры
        
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

        public void CreateMap() //создаем карту противника
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
                    button.Click += new EventHandler(PlayerShoot);  //при клике на поле противника вызывается этот обработчик
                    enemyButtons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            Label mapLabel = new Label();
            mapLabel.Text = "Противник";
            mapLabel.Location = new Point(mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(mapLabel);
        }

        public void ButtonStart()   //описание кнопки "Начать"
        {
            Button startButton = new Button();
            startButton.Text = "Начать";
            startButton.Click += new EventHandler(Start);   //при нажатии на кнопку Начать вызывается этот обработчик
            startButton.Location = new Point(0, mapSize * cellSize + 20);
            this.Controls.Add(startButton);
        }

        public void Start(object sender, EventArgs e)   //обработчик на кнопку старт
        {
            isPlaying = true;
            ClearPole();    //очищаем форму от кораблей игрока
            CreateMap();    //создаем поле врага
            enemyMap = enemy.ConfigureShips();

            Timer timer = new Timer();  //создаем таймер, которые каждые 2 секунды обращается к таблице
            timer.Interval = (2000); // 2 sec
            timer.Start();
            timer.Tick += new EventHandler(TimerTick);  

            Timer mainTimer = new Timer();  //таймер, который ведет отсчет с начала игры
            mainTimer.Interval = (1000); // 1 sec
            mainTimer.Start();
            mainTimer.Tick += new EventHandler(MainTimer_Tick);

            CreateGroupBox();   
            AddToGroupBox("Ход", 25, 50, 200, 50);
            AddToGroupBox("Осталось клеток противника", 25, 100, 300, 50);
            AddToGroupBox("Осталось клеток своих", 25, 150, 300, 50);
            AddToGroupBox("Время", 25, 200, 200, 50);

            Balance(enemyRemainder, 400, 100, 100, 50); //остаток палуб у противника
            Balance(myRemainder, 400, 150, 100, 50);    //остаток моих палуб
            Balance(amount, 400, 50, 100, 50);  //количество ходов
            Balance(time, 400, 200, 100, 50);   //время с начала игры
            myRemainder.Text = RemainderOfCells(myMap).ToString();
            enemyRemainder.Text = RemainderOfCells(enemyMap).ToString();
            amount.Text = kol.ToString();
            time.Text = DateTime.Now.ToString();

            tern = ValueOfTern(0);  //получаем значение из таблицы, чей сейчас ход
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

        private void MainTimer_Tick(object sender, EventArgs e) //подсчитывает время с начала игры и выводит на экран
        {
            i++;
            time.Text = i.ToString() + " сек";
        }

        public void ClearPole() //очищает поле
        {
            this.Controls.Clear();
        }

        public void ConfigureShips()    //расставляет корабли на поле игрока 
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

        public void PlayerShoot(object sender, EventArgs e) //обработчик на нажатие кнопок на поле противника
         {
            Button pressedButton = sender as Button;
            kol++;  //подсчитывает количество ходов
            WhoMove(pressedButton);

            if (!CheckIfMapIsNotEmpty())    //если игра закончилась, то снова открывается первоначальная форма
            {
                ClearPole();
                Hide();
                Form2 myForm = new Form2();
                myForm.ShowDialog();

            }
        }

        public bool CheckIfMapIsNotEmpty()  //проверяет остались ли еще корабли на поле
        {
            bool isEmpty1 = true;
            bool isEmpty2 = true;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
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

        public bool Shoot(int[,] map, Button pressedButton) //выстрел, true - попадание, false - промах
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

        public void WhoMove(Button pressedButton)   //совершает выстрел и в зависимости от промаха/попадания записывает в таблицу игрока и в таблицу противника значения
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

        public static int StartMove() //определяет кто будет ходить первым
        {
            int whoMove;
            if (Convert.ToInt32(Form2.sheetsArray[0]) < Convert.ToInt32(Form2.sheetsArray[1]))
                whoMove = 1;
            else whoMove = 0;
            return whoMove;
        }

        public void UpdatePole() //для первоначального определения, чей сейчас ход. Если игрока, то кнопки разблокированы, если нет, то кнопки заблокированы
        {
            tern = ValueOfTern(0);
            if (tern == 1)
                EnableOfButtons(true);
            else
                EnableOfButtons(false);
        }

        public void TimerTick(object sender, EventArgs e)   //обработчик на таймер, который смотрит в таблицу каждые 2 сек
        {
            string text;
            tern = ValueOfTern(0);
            if (tern == 1)  //если сейчас ход игрока, то кнопки активны
            {
                EnableOfButtons(true);
                text = "Ваш ход";
                whoseMove.BackColor = Color.Green;
            }
            else //если не ход игрока, то кнопки недоступны
            {
                EnableOfButtons(false);
                text = "Ход противника";
                whoseMove.BackColor = Color.Red;
            }
            whoseMove.Text = text;

            myRemainder.Text = RemainderOfCells(myMap).ToString();
            enemyRemainder.Text = RemainderOfCells(enemyMap).ToString();
            amount.Text = kol.ToString();
        }

        public int ValueOfTern(int nameOfSheet) //на вход получаем массив всех 0/1(ход) и берем только последнее значение
        {
            int tern;
            int[] mass = new int[100];
            int index = 0;
            mass = GoogleApi.ReadTern(Form2.sheetsArray[nameOfSheet]); //чтение со страницы игрока
            index = mass.Length - 1;
            tern = mass[index];
            return tern;
        }

        public void EnableOfButtons(bool flag) //открывает и закрывает доступ к кнопкам
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    enemyButtons[i, j].Enabled = flag;
                }
            }
        }

        public void Information()   //отдельно для вывода текстов ("Мой ход", "Ход противника"
        {
            whoseMove.TextAlign = ContentAlignment.MiddleCenter;
            whoseMove.Font = new Font(whoseMove.Font.Name, 20, whoseMove.Font.Style);
            whoseMove.Location = new Point(mapSize * cellSize + 20, 0);
            whoseMove.Width = 300;
            whoseMove.Height = 100;
            whoseMove.BackColor = Color.FromArgb(255, 0, 255);
            this.Controls.Add(whoseMove);
        }

        public void Balance(Label name, int x, int y, int width, int height)    //добавляем в groupBox labels(значения)
        {
            name.Location = new Point(x, y);
            name.Size = new Size(width, height);
            groupBox.Controls.Add(name);
        }

        public void AddToGroupBox(string text, int x, int y, int width, int height) //добавляем в groupBox labels(описания)
        {
            Label label = new Label { Text = text };
            label.Location = new Point(x, y);
            label.Size = new Size(width, height);
            groupBox.Controls.Add(label);
        }

        public void CreateGroupBox()    //создание groupBox
        {
            groupBox.Location = new Point(mapSize * cellSize + 20, 200);
            groupBox.Text = "Ход игры";
            groupBox.Width = 500;
            groupBox.Height = 250;
            this.Controls.Add(groupBox);
        }

        public int RemainderOfCells(int[,] map) //подсчитывает остаток палуб
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

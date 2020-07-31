using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VisioForge.Shared.MediaFoundation.OPM;

namespace MyApp
{
    public partial class Form2 : Form
    {
        string[] sheetsName = new string[2] { "1", "2" };

        TextBox sheetNumberPlayer = new TextBox();
        TextBox sheetNumberEnemy = new TextBox();

        string sheetNumber1 = ""; 
        string sheetNumber2 = "";

        public Form2()
        {
            InitializeComponent();
            this.Text = "Морской бой";
            StartGame();
        }

        public void StartGame()
        {
            sheetNumberPlayer.Location = new Point(100, 100);
            this.Controls.Add(sheetNumberPlayer);
            
            sheetNumberEnemy.Location = new Point(300, 100);
            this.Controls.Add(sheetNumberEnemy);

            Button startButton = new Button();
            startButton.Text = "Ввод";
            startButton.Width = 100;
            startButton.Height = 40;
            startButton.Location = new Point(200, 200);
            startButton.Click += new EventHandler(Start);
            this.Controls.Add(startButton);
        }

        public void Start(object sender, EventArgs e)
        {
            TakeText();
            if (sheetNumber1 == sheetNumber2)
            {
                WarningText("Необходимо ввести разные таблицы для игрока и противника");
            }
            else
            {
                if (ValidData(sheetNumber1) && ValidData(sheetNumber2))
                {
                    Hide();
                    Form1 myForm = new Form1();
                    myForm.ShowDialog();
                }
                else
                {
                    WarningText("Проверьте правильность ввода листов таблицы");
                }
            }                   
        }

        public void WarningText(string text)
        {
            Label warningText = new Label();
            warningText.Text = text;
            warningText.Width = 700;
            warningText.Height = 40;
            warningText.Location = new Point(300, 200);
            this.Controls.Add(warningText);
        }

        public void TakeText()
        {
            sheetNumber1 = sheetNumberPlayer.Text;
            sheetNumber2 = sheetNumberEnemy.Text;
        }


        public bool ValidData(string nameOfSheet)
        {
            bool flag = false;

            if (nameOfSheet.Length == 0)
                return false;

            foreach (string str in sheetsName)
            {
                if (nameOfSheet == str)
                    flag = true;
            }

            if (flag)
                return true;
            return false;
        }

    }
}

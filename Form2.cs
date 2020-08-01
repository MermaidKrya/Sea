﻿using System;
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
        public TextBox sheetNumberPlayer = new TextBox();
        public TextBox sheetNumberEnemy = new TextBox();

        public static string[] sheetsArray = new string[2];

        public Form2()
        {
            InitializeComponent();
            this.Text = "Морской бой";
            GoogleApi.Acsess();
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
            sheetsArray = WriteToArray();
            if (sheetsArray[0] == sheetsArray[1])
            {
                WarningText("Необходимо ввести разные таблицы для игрока и противника");
            }
            else
            {
                if (ValidData(sheetsArray[0]) && ValidData(sheetsArray[1]))
                {
                    GoogleApi.CreateEntry();
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

        public void WarningText(object text)
        {
            Label warningText = new Label();
            warningText.Text = text.ToString();
            warningText.Width = 700;
            warningText.Height = 40;
            warningText.Location = new Point(300, 200);
            this.Controls.Add(warningText);
        }

        public string[] WriteToArray()
        {
            string sheetNumber1 = sheetNumberPlayer.Text;
            string sheetNumber2 = sheetNumberEnemy.Text;
            string[] mass = new string[2];
            mass[0] = sheetNumber1.ToString();
            mass[1] = sheetNumber2.ToString();
            return mass;
        }

        public bool ValidData(object nameOfSheet)
        {
            bool flag = false;

            if (nameOfSheet.ToString().Length == 0)
                return false;

            foreach (string str in GoogleApi.sheetsName)
            {
                if (nameOfSheet.ToString() == str)
                    flag = true;
            }

            if (flag)
                return true;
            return false;
        }

    }
}

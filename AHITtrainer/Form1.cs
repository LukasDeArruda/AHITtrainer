using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Memory;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AHITtrainer
{
    public partial class Form1 : Form
    {
        public int ticks = 0;
        Mem memory = new Mem();
        readonly StreamWriter logger = new StreamWriter("logs.txt");

        List<string> lines = new List<string>();
        List<string> pointerNames = new List<string>();
        List<string> pointerAddresses = new List<string>();

        public float xPos;
        public float yPos;
        public float zPos;

        public float xSpd;
        public float ySpd;
        public float zSpd;

        public int health;
        public int money;

        public int isPaused = 0;

        public Form1()
        {
            InitializeComponent();
            ReadPointers();
        }

        private void timerTick(object sender, EventArgs e)
        {
            bool gameOpen = memory.OpenProcess("HatinTimeGame");

            GameStatus(gameOpen, isPaused);
            ticks++;
            Label label = (Label)Controls.Find("TickLabel", true)[0];
            label.Text = ticks.ToString();

            int isLoading = memory.ReadInt(lines[10]);
            if(isLoading == 1)
            {
                lockControls();
            }
            else
            {
                unlockControls();
            }

            UpdatePos();
            UpdateSpeed();
            UpdateMoney();
            UpdateHealth();
            CheckGrounded();

        }

        private void ReadPointers()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            StreamReader sr = new StreamReader(asm.GetManifestResourceStream("AHITtrainer.Resources.pointers.txt"));



            while (!sr.EndOfStream)
            {
                lines.Add(sr.ReadLine());
                //Log(sr.ReadLine());
            }
            for (int i = 0; i > lines.Count; i++)
            {
                Log(lines[i]);
            }
        }



        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString();
            logger.WriteLine(timestamp + ": " + message);
            richTextBox1.SelectedText += message + "\n";
        }

        private void GameStatus(bool isOpen, int isPaused)
        {
            isPaused = memory.ReadInt(lines[8]);
            if (isOpen && isPaused == 0)
            {
                gameRunLabel.Text = "Game Is Running";
            }
            else if (isOpen && isPaused == 1)
            {
                gameRunLabel.Text = "Game Is Paused";
            }
            else
            {
                gameRunLabel.Text = "Game Is Closed";
            }
        }

        private void UpdatePos()
        {
            xPos = memory.ReadFloat(lines[0]);
            yPos = memory.ReadFloat(lines[1]);
            zPos = memory.ReadFloat(lines[2]);
            xPosVal.Text = xPos.ToString();
            yPosVal.Text = yPos.ToString();
            zPosVal.Text = zPos.ToString();
        }

        private void UpdateSpeed()
        {
            xSpd = memory.ReadFloat(lines[3]);
            ySpd = memory.ReadFloat(lines[4]);
            zSpd = memory.ReadFloat(lines[5]);
            xSpdVal.Text = xSpd.ToString();
            ySpdVal.Text = ySpd.ToString();
            zSpdVal.Text = zSpd.ToString();
        }


        private void UpdateMoney()
        {
            money = memory.ReadInt(lines[7]);
            moneyVal.Text = money.ToString();
        }

        private void UpdateHealth()
        {
            health = memory.ReadInt(lines[6]);
            healthVal.Text = health.ToString();
        }


        private void UpdateMemory(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox snd = sender as TextBox;
                switch (snd.Name)
                {
                    case "xPosBox":
                        xPos = Int32.Parse(xPosBox.Text);
                        Log("X position changed to " + xPos);
                        memory.WriteMemory(lines[0], "float", xPos.ToString());
                        break;

                    case "yPosBox":
                        yPos = Int32.Parse(yPosBox.Text);
                        Log("Y position changed to " + yPos);
                        memory.WriteMemory(lines[1], "float", yPos.ToString());
                        break;

                    case "zPosBox":
                        zPos = Int32.Parse(zPosBox.Text);
                        Log("Z position changed to " + zPos);
                        memory.WriteMemory(lines[2], "float", zPos.ToString());
                        break;

                    case "xSpdBox":
                        xSpd = Int32.Parse(xSpdBox.Text);
                        Log("X speed changed to " + xSpd);
                        memory.WriteMemory(lines[3], "float", xSpd.ToString());
                        break;

                    case "ySpdBox":
                        ySpd = Int32.Parse(ySpdBox.Text);
                        Log("Y speed changed to " + ySpd);
                        memory.WriteMemory(lines[4], "float", ySpd.ToString());
                        break;

                    case "zSpdBox":
                        zSpd = Int32.Parse(zSpdBox.Text);
                        Log("Z speed changed to " + zSpd);
                        memory.WriteMemory(lines[5], "float", zSpd.ToString());
                        break;

                    case "healthBox":
                        health = Int32.Parse(healthBox.Text);
                        Log("Health Updated to " + health);
                        memory.WriteMemory(lines[6], "int", health.ToString());
                        break;

                    case "moneyBox":
                        money = Int32.Parse(moneyBox.Text);
                        Log("Money Updated to " + money);
                        memory.WriteMemory(lines[7], "int", money.ToString());
                        break;

                    default:
                        Log(snd.Name);
                        break;
                }
                snd.Text = "";

            }
        }

        private void Freeze(object sender, EventArgs e)
        {
            CheckBox check = sender as CheckBox;
            string[] splitName = check.Name.Split('F');
            string boxVal = splitName[0] + "Val";
            var valueToFreezeAt = Controls.Find(boxVal, true);

            if (check.Checked)
            {
                switch (boxVal)
                {
                    case "xPosVal":

                        memory.FreezeValue(lines[0], "float", valueToFreezeAt[0].Text.ToString());
                        Log(splitName[0] + " Frozen");
                        break;

                    case "yPosVal":
                        memory.FreezeValue(lines[1], "float", valueToFreezeAt[0].Text.ToString());
                        Log(splitName[0] + " Frozen");
                        break;

                    case "zPosVal":
                        memory.FreezeValue(lines[2], "float", valueToFreezeAt[0].Text.ToString());
                        Log(splitName[0] + " Frozen");
                        break;

                    case "xSpdVal":
                        memory.FreezeValue(lines[3], "float", valueToFreezeAt[0].Text.ToString());
                        Log(splitName[0] + " Frozen");
                        break;

                    case "ySpdVal":
                        memory.FreezeValue(lines[4], "float", valueToFreezeAt[0].Text.ToString());
                        Log(splitName[0] + " Frozen");
                        break;

                    case "zSpdVal":
                        memory.FreezeValue(lines[5], "float", valueToFreezeAt[0].Text.ToString());
                        Log(splitName[0] + " Frozen");
                        break;

                    case "healthVal":
                        memory.FreezeValue(lines[6], "int", valueToFreezeAt[0].Text.ToString());
                        Log(splitName[0] + " Frozen");
                        break;

                    case "moveyVal":
                        memory.FreezeValue(lines[7], "int", valueToFreezeAt[0].Text.ToString());
                        Log(splitName[0] + " Frozen");
                        break;

                    default:
                        break;

                }


            }
            else
            {
                switch (boxVal)
                {
                    case "xPosVal":
                        memory.UnfreezeValue(lines[0]);
                        Log(splitName[0] + " Unfrozen");
                        break;

                    case "yPosVal":
                        memory.UnfreezeValue(lines[1]);
                        Log(splitName[0] + " Unfrozen");
                        break;

                    case "zPosVal":
                        memory.UnfreezeValue(lines[2]);
                        Log(splitName[0] + " Unfrozen");
                        break;

                    case "xSpdVal":
                        memory.UnfreezeValue(lines[3]);
                        Log(splitName[0] + " Unfrozen");
                        break;

                    case "ySpdVal":
                        memory.UnfreezeValue(lines[4]);
                        Log(splitName[0] + " Unfrozen");
                        break;

                    case "zSpdVal":
                        memory.UnfreezeValue(lines[5]);
                        Log(splitName[0] + " Unfrozen");
                        break;

                    case "healthVal":
                        memory.UnfreezeValue(lines[6]);
                        Log(splitName[0] + " Unfrozen");
                        break;

                    case "moveyVal":
                        memory.UnfreezeValue(lines[7]);
                        Log(splitName[0] + " Unfrozen");
                        break;

                    default:
                        break;

                }
            }
        }

        private void CheckGrounded()
        {
            int isGrounded = memory.ReadInt(lines[9]);
            if (isGrounded == 1)
            {
                inAirLabel.Text = "In air";
            }
            else
            {
                inAirLabel.Text = "On Ground";
            }
        }

        private void lockControls()
        {
            xPosBox.Visible = false;
            yPosBox.Visible = false;
            zPosBox.Visible = false;
            xSpdBox.Visible = false;
            ySpdBox.Visible = false;
            zSpdBox.Visible = false;
            moneyBox.Visible = false;
            healthBox.Visible = false;

            xPosFreeze.Visible = false;
            yPosFreeze.Visible = false;
            zPosFreeze.Visible = false;
            xSpdFreeze.Visible = false;
            ySpdFreeze.Visible = false;
            zSpdFreeze.Visible = false;
            moneyFreeze.Visible = false;
            healthFreeze.Visible = false;
        }

        private void unlockControls()
        {
            xPosBox.Visible = true;
            yPosBox.Visible = true;
            zPosBox.Visible = true;
            xSpdBox.Visible = true;
            ySpdBox.Visible = true;
            zSpdBox.Visible = true;
            zPosBox.Visible = true;
            moneyBox.Visible = true;
            healthBox.Visible = true;

            xPosFreeze.Visible = true;
            yPosFreeze.Visible = true;
            zPosFreeze.Visible = true;
            xSpdFreeze.Visible = true;
            ySpdFreeze.Visible = true;
            zSpdFreeze.Visible = true;
            moneyFreeze.Visible = true;
            healthFreeze.Visible = true;
        }
    }
}

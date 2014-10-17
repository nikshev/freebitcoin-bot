using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Awesomium.Core;
using System.Threading;
using Awesomium.Core;
using System.Collections.Generic;

namespace freecoin
{
    public partial class Form1 : Form
    {
        private bool work_flag=false;
        private string login = "";
        private string password = "";
        private double begin_bet = 0.0;
        private double bet = 0.0;
        private string proxy = "";
        private string message = "";
        private bool finishedLoading = false;
        private bool login_ok = false;
        private int win_count = 0;
        private int loose_count = 0;
 
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Start")
            {
                this.message += DateTime.Now.ToString() + ": Start!\r\n";
                if (textBox1.Text != "" && textBox1.Text.Length>2)
                {
                    if (textBox2.Text != "" && textBox1.Text.Length > 2)
                    {
                        if (textBox4.Text != "" && textBox1.Text.Length > 2)
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            this.login = textBox1.Text;
                            this.password = textBox2.Text;
                            this.bet = Double.Parse(textBox4.Text);
                            this.begin_bet = Double.Parse(textBox4.Text);
                            this.proxy = textBox3.Text;
                            if (!this.login_ok)
                             this.login_ok=this.CheckLogin();
                            if (this.login_ok)
                            {
                                button1.Text = "Stop";
                                this.work_flag = true;
                                textBox1.Enabled = false;
                                textBox2.Enabled = false;
                                textBox3.Enabled = false;
                                textBox4.Enabled = false;
                                Cursor.Current = Cursors.Default;
                            }
                            else
                            {
                                MessageBox.Show("Error! Can't login!");
                                this.message += DateTime.Now.ToString() + ": Can't login!\r\n";
                                button1.Text = "Start";
                                this.work_flag = false;
                                textBox1.Enabled = true;
                                textBox2.Enabled = true;
                                textBox3.Enabled = true;
                                textBox4.Enabled = true;
                                Cursor.Current = Cursors.Default;
                            }
                        }
                        else 
                            MessageBox.Show("Error! Check the bet!");
                    }
                    else
                        MessageBox.Show("Error! Check the password!");
                }
                else
                    MessageBox.Show("Error! Check the login!");
            }
            else
            {
                button1.Text = "Start";
                this.work_flag = false;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                Cursor.Current = Cursors.Default;
                this.message += DateTime.Now.ToString() + ": Stop!\r\n";
                
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.message != "")
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(System.Environment.CurrentDirectory + "\\log.txt", true))
                {
                    file.WriteLine(this.message);
                }
                this.message = "";
            }

            if (this.work_flag)
            {
                this.work_flag = false;
                this.Play();
                this.work_flag = true;
            }
        }

        private bool CheckLogin()
        {
            bool ret_val = false;
            this.finishedLoading = false;
            try
            {
                webControl1.Source = new Uri("https://freebitco.in/?op=home");

                while (!this.finishedLoading)
                {
                    Thread.Sleep(100);
                    WebCore.Update();
                }

                dynamic document = (JSObject)webControl1.ExecuteJavascriptWithResult("document");

                while (true)
                {
                    if (document != null && document.ToString().Length > 0)
                        break;

                    document = (JSObject)webControl1.ExecuteJavascriptWithResult("document");
                }

                if (document != null)
                {
                    using (document)
                    {
                        dynamic switch_to_login = document.getElementById("switch_to_login_button");
                        if (switch_to_login != null)
                        {
                            switch_to_login.click();

                            dynamic login_field = document.getElementById("login_form_btc_address");
                            if (login_field != null)
                            {
                                login_field.value = this.login;
                                dynamic pass_field = document.getElementById("login_form_password");
                                if (pass_field != null)
                                {
                                    pass_field.value = this.password;
                                    dynamic submit_btn = document.getElementById("login_button");
                                    if (submit_btn != null)
                                    {
                                        submit_btn.click();
                                        Uri uriPlay = new Uri("https://freebitco.in/?op=home#");
                                        this.finishedLoading = false;
                                        webControl1.Source = uriPlay;
                                        while (!this.finishedLoading)
                                        {
                                            Thread.Sleep(100);
                                            WebCore.Update();
                                        }
                                        ret_val = true;
                                    }
                                    else
                                        this.message += DateTime.Now.ToString() + ": Error! Can't find submit button!\r\n"; 
                                }
                                else
                                    this.message += DateTime.Now.ToString() + ": Error! Can't find password field!\r\n";
                            }
                            else
                                this.message += DateTime.Now.ToString() + ": Error! Can't find login field!\r\n";
                        }
                        else
                            this.message += DateTime.Now.ToString() + ": Error! Can't find switch button!\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                this.message += DateTime.Now.ToString() + ": Error! Exception in CheckLogin:"+ex.Message+"!\r\n";
            }
            return ret_val;
        }

        private void Play()
        {
            bool win = false;
            
            try
            {
                dynamic document = (JSObject)webControl1.ExecuteJavascriptWithResult("document");

                while (true)
                {
                    if (document != null && document.ToString().Length > 0)
                        break;

                    document = (JSObject)webControl1.ExecuteJavascriptWithResult("document");
                }

                if (document != null)
                {
                    using (document)
                    {
                        int i = 0;
                        dynamic balance_field_pre = document.getElementById("balance");
                        double balance_old = Double.Parse(balance_field_pre.innerText); ;
                        dynamic btc_stake = document.getElementById("double_your_btc_stake");
                        if (btc_stake != null)
                            {

                                btc_stake.value = this.bet.ToString("0.000000000");

                                dynamic submit_btn = document.getElementById("double_your_btc_bet_hi_button");
                                if (submit_btn != null)
                                 {
                                   submit_btn.click();
                                  
                                  
                                    Thread.Sleep(6000);
                                    WebCore.Update();
                                    
                                    dynamic win_bet = document.getElementById("double_your_btc_bet_win");
                                    dynamic loose_bet = document.getElementById("double_your_btc_bet_lose");
                                    string win_bet_value = win_bet.innerText;
                                    string loose_bet_value = loose_bet.innerText;
                                    dynamic balance_field = document.getElementById("balance");
                                    double balance = Double.Parse(balance_field.innerText);
                                                                    
                                    if ((win_bet_value != null&&win_bet_value.Length>5))
                                    {
                                        this.message += DateTime.Now.ToString() + ": Win with bet:" + this.bet.ToString() + "!\r\n";
                                        this.bet = this.begin_bet;
                                        win = true;
                                        balance_old = balance;
                                        this.win_count++;
                                    }

                                    if ((loose_bet_value != null && loose_bet_value.Length > 5))
                                    {
                                        this.bet *= 2;
                                      
                                        this.loose_count++;
                                        if (this.loose_count > 5)
                                        {
                                            this.loose_count = 0;
                                            webControl1.Reload(true);
                                        }

                                        this.message += DateTime.Now.ToString() + ": Loose! Last bet:" + this.bet.ToString() + "!\r\n";
                                        balance_old = balance;
                                    }
                                }
                                else
                                   this.message += DateTime.Now.ToString() + ": Error! Can't find submit button!\r\n";
                            }
                            else
                                this.message += DateTime.Now.ToString() + ": Error! Can't find btc stake field\r\n";
                        }

                    }
            }
            catch (Exception ex)
            {
                this.message += DateTime.Now.ToString() + ": Error! Exception in Play:" + ex.Message + "!\r\n";
            }
        }

        private void Awesomium_Windows_Forms_WebControl_LoadingFrameComplete(object sender, Awesomium.Core.FrameEventArgs e)
        {
            if (e.IsMainFrame)
            {
                this.finishedLoading = true;
            }
            Cursor.Current = Cursors.Default;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }
    }
}

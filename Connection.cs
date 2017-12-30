using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExchangeRates
{
    public partial class Connection : Form
    {
        public Connection()
        {
            InitializeComponent();
        }

        //Сохранение введеных данных
        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxServer.Text))
            {
                Properties.Settings.Default.Server = textBoxServer.Text;
            }
            if (!string.IsNullOrEmpty(textBoxDB.Text))
            {
                Properties.Settings.Default.DBName = textBoxDB.Text;
            }
            if (!string.IsNullOrEmpty(textBoxDB_NAME.Text))
            {
                Properties.Settings.Default.SUN_DBName = textBoxDB_NAME.Text;
            }
            if (!string.IsNullOrEmpty(textBoxLogin.Text))
            {
                Properties.Settings.Default.Login = textBoxLogin.Text;
            }
            if (!string.IsNullOrEmpty(textBoxPassword.Text))
            {
                if (checkBox1.Checked) //Если нажат чек бокс что не хотим сохранять пароль
                {
                    PasswordClass.Value = textBoxPassword.Text;
                    Properties.Settings.Default.Password = "";
                }
                else
                {
                    PasswordClass.Value = "";
                    Properties.Settings.Default.Password = textBoxPassword.Text;
                }
                
            }
            if (checkBox2.Checked)
            {
                Properties.Settings.Default.WindowsAutification = "yes";
            }
            else 
            {
                Properties.Settings.Default.WindowsAutification = "no";
            }
            Properties.Settings.Default.Save();
            this.Close();
        }

        //Заполнение текстовых полей сохраннеными данными
        private void Connection_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.WindowsAutification == "yes")
            {
                checkBox2.Checked = true;                
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Server))
            {
                textBoxServer.Text = Properties.Settings.Default.Server.ToString();
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.DBName))
            {
                textBoxDB.Text = Properties.Settings.Default.DBName.ToString();
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.SUN_DBName))
            {
                textBoxDB_NAME.Text = Properties.Settings.Default.SUN_DBName.ToString();
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Login))
            {
                textBoxLogin.Text = Properties.Settings.Default.Login.ToString();
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Password) || !string.IsNullOrEmpty(PasswordClass.Value))
            {
                textBoxPassword.Text = Properties.Settings.Default.Password.ToString() + PasswordClass.Value;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) //Програмирование интерфейса, закрываем текст боксы если используем аутонтефикацию виндовс
        {
            if (checkBox2.Checked == true)
            {
                textBoxLogin.Enabled = false;
                textBoxPassword.Enabled = false;
                checkBox1.Enabled = false;
            }
            if (checkBox2.Checked == false)
            {
                textBoxLogin.Enabled = true;
                textBoxPassword.Enabled = true;
                checkBox1.Enabled = true;
            }   
        }
    }
}

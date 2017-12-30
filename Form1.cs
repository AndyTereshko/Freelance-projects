using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Data.SqlClient;

namespace ExchangeRates
{  

    public partial class ExchangeRates : Form
    {
        LogClass applog = new LogClass(); //вызов конструктора класса log
        public ExchangeRates()
        {
            checkLocalization();            
            InitializeComponent();
            label2.Text = getSystemDate();//Вводим системную дату в текст бокс
            label4.Text = Properties.Settings.Default.lastDate; 
                     
        }
        public void checkLocalization()  //Функция локализации
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
        }

        private string getSystemDate() //Функция получения системной даты в виде строки
        {
            string tmp1 = "";
            string tmp2 = "";
            //Если день или месяц меньше 10, нужно добавить 0 перед ними в строку
            if (System.DateTime.Now.Day < 10)
                tmp1 = "0";
            if (System.DateTime.Now.Month < 10)
                tmp2 = "0";
            return (tmp1 + System.DateTime.Now.Day + "." + tmp2 + System.DateTime.Now.Month + "." + System.DateTime.Now.Year);
        }

        private string getSystemDateDB() //Функция получения системной даты в виде строки в формате БД
        {
            string tmp1 = "";
            string tmp2 = "";
            //Если день или месяц меньше 10, нужно добавить 0 перед ними в строку
            if (System.DateTime.Now.Day < 10)
                tmp1 = "0";
            if (System.DateTime.Now.Month < 10)
                tmp2 = "0";
            return (System.DateTime.Now.Year + tmp2 + System.DateTime.Now.Month + tmp1 + System.DateTime.Now.Day);
        }

        private string converFloatDB(double float1) //Функция конвертирующая число курса валют в формате дабл в строку
        { 
            string text=float1.ToString();
            string temp = text;
            text=text.Replace(',','.');
            string[] split = temp.Split(new Char[] {','});
            while (split[1].Length < 9)
            {
                split[1] += "0";
                text += "0";
            }
            return(text);
        }

        private void Form1_Resize(object sender, EventArgs e) //Сворачиваем в систем трей
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.notifyIcon1.BalloonTipText = MyStrings.trayText;
                this.notifyIcon1.BalloonTipTitle = MyStrings.trayTitle;
                this.notifyIcon1.Text = MyStrings.trauHover;
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(800);
                this.Hide();
                this.ShowInTaskbar = false;
            }
                       
        }    

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) //Разворачиваем из трея
        {
            this.Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            notifyIcon1.Visible = false;
                       
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            About about = new About();            
            about.ShowDialog();
        }

        private void настройкаСоединенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Connection connection = new Connection();
            connection.ShowDialog();
        }

        private void localize()
        {
            //После выхода с диалогового окна загружаем новые ресурсы в области форм
            var resources = new ComponentResourceManager(typeof(ExchangeRates));
            resources.ApplyResources(this, "$this");
            resources.ApplyResources(this.файлToolStripMenuItem, "файлToolStripMenuItem");
            resources.ApplyResources(this.настройкиToolStripMenuItem, "настройкиToolStripMenuItem");
            resources.ApplyResources(this.оПрограммеToolStripMenuItem, "оПрограммеToolStripMenuItem");
            resources.ApplyResources(this.помощьToolStripMenuItem, "помощьToolStripMenuItem");
            resources.ApplyResources(this.локализацияToolStripMenuItem, "локализацияToolStripMenuItem");
            resources.ApplyResources(this.выходToolStripMenuItem, "выходToolStripMenuItem");
            resources.ApplyResources(this.настройкаСоединенияToolStripMenuItem, "настройкаСоединенияToolStripMenuItem");
            resources.ApplyResources(this.label1, "label1");
            resources.ApplyResources(this.label3, "label3");
            resources.ApplyResources(this.label5, "label5");
            resources.ApplyResources(this.label6, "label6");
            resources.ApplyResources(this.label7, "label7");
            resources.ApplyResources(this.button3, "button3");
            resources.ApplyResources(this.button1, "button1");
            resources.ApplyResources(this.dateTimePicker1, "dateTimePicker1");
            resources.ApplyResources(this.dateTimePicker2, "dateTimePicker2");
        }

        private void локализацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Localization localization = new Localization();
            localization.ShowDialog();
            localize();
            
        }

        private bool textContainsNumber(string text) //Проверка состоит ли строка только из цифр
        {
            foreach (char ch in text)
                if (Char.IsDigit(ch))
                { 
                    return true;
                }                    
            return false;
        }

        private String GetAvrgRateUA(String text)//функция для средневзвешеного к гривне
        {
            var webGet = new HtmlWeb();
            //Загружаем страничку национального банка Украины для среднего значения
            var document1 = webGet.Load("http://www.bank.gov.ua/control/uk/index");
            var table = document1.DocumentNode.SelectNodes("//table[@class='classicTable']");
            int flag = 0; //флаг для вывода текста
            String temp = "";
            String temp2 = "";
            //Парсим елементы таблицы
            foreach (var table_element in table)
            {

                if (table_element.ChildNodes != null)
                {
                    //Парсим строки
                    foreach (var tr_element in table_element.ChildNodes)
                    {
                        //Парсим столбцы
                        foreach (var td_element in tr_element.ChildNodes)
                        {
                            temp = td_element.InnerText;
                            temp = temp.Replace("&nbsp;", "");
                            temp = temp.Replace("\r", "");
                            temp = temp.Replace("\n", "");
                            temp = temp.Replace(" ", "");
                            //Вывод значения курса
                            if (flag == 2 && temp != "")
                            {
                                temp2 += temp;
                                temp2 += ";";
                                flag = 0;
                            }
                            //Вывод даты
                            if (flag == 1 && temp != "")
                            {
                                temp = temp.Remove(10);
                                temp2 += temp;
                                temp2 += ";";
                                flag = 2;
                            }
                            //Ищем строку для вывода даты и курса после нее  
                            if (temp == "Середньозваженийкурснаміжбанківськомуринку(начасвстановленняофіційногокурсугривні)")
                            {
                                temp = "9999 Середньозважений;";
                                temp2 += temp;
                                flag = 1;
                            }
                        }
                    }
                }

            }
            return(temp2);
        }

        private String getRateUA(String text)//Функция получение украинских курсов валют
        {
            //Загружаем страничку национального банка Украины для курсов валют
            var webGet = new HtmlWeb();
            var document = webGet.Load("http://www.bank.gov.ua/control/uk/curmetal/currency/search?formType=searchFormDate&time_step=daily&date=" + getSystemDate() + "&execute=%D0%92%D0%B8%D0%BA%D0%BE%D0%BD%D0%B0%D1%82%D0%B8");

            //string temp = "";
            string temp2 = "";
            int intTemp = 0;
            bool Flag = false;

            var trList = document.DocumentNode.SelectNodes("//tr");
            //Теперь для каждой строки tr, получаем все столбцы td
            foreach (var tr in trList)
            {
                //Получаем список столбцов i-ой строки
                var tdList = tr.ChildNodes.Where(x => x.Name == "td");
                foreach (var td in tdList)
                {
                    //Проверяем класс td на соответсвие
                    if (td.Attributes["class"] != null && (td.Attributes["class"].Value == "cell_c" || td.Attributes["class"].Value == "cell"))
                    {
        
                        if(intTemp==0 && td.InnerText==text)
                        {                           
                            Flag = true;                             
                        }               
                        if (intTemp == 5)
                        {
                            intTemp = 0;
                            Flag = false;
                        }
                        if (Flag)
                        {
                            intTemp++;
                            temp2 += td.InnerText;
                            temp2 += ";";
                             
                        }
                    }
                }
            }
            //Console.WriteLine(temp2);            
            return (temp2);    
        }

        private String getRateRU(String text)//Функция для русских курсов
        {
            //Загружаем страничку центрального банка России
            var webGet = new HtmlWeb();
            var document = webGet.Load("http://www.cbr.ru/currency_base/daily.aspx?date_req=" + getSystemDate());
            string temp2 = "";
            int intTemp = 0;
            bool Flag = false;
            var trList = document.DocumentNode.SelectNodes("//tr");

            //Теперь для каждой строки tr, получаем все столбцы td
            foreach (var tr in trList)
            {
                //Получаем список столбцов i-ой строки
                var tdList = tr.ChildNodes.Where(x => x.Name == "td");
                foreach (var td in tdList)
                {
                    //Проверяем класс td на соответсвие
                    if (td.Attributes["class"] == null)
                    {                        
                        if (intTemp == 0 && td.InnerText == text)
                        {
                            Flag = true;
                        }
                        if (intTemp == 5)
                        {
                            intTemp = 0;
                            Flag = false;
                        }
                        if (Flag)
                        {
                            intTemp++;
                            temp2 += td.InnerText;
                            temp2 += ";";

                        }
                    }
                }
            }
            //Console.WriteLine(temp2); 
            return (temp2);
        }

        private String getRate(String text)//Функция выбора между курсами Украины и России
        {
            if (Properties.Settings.Default.Country.ToString() == "ua")
            {
                return(getRateUA(text));
            }
            if (Properties.Settings.Default.Country.ToString() == "ru")
            {
                return(getRateRU(text));
            }
            return(null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (Properties.Settings.Default.lastDate == getSystemDate()) //Проверяем Былы ли уже записаны сегодня курсы в БД 
                {
                    DialogResult result1 = MessageBox.Show(MyStrings.warningTodayIsDone + Properties.Settings.Default.lastDate +
                                                        MyStrings.warningTodayIsDone2 + "\n" + Properties.Settings.Default.lastCodes + "\n" + MyStrings.warningTodayIsDone3,
                                                        MyStrings.warning + "!",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Warning);
                    if (result1 != DialogResult.Yes)
                    {
                        return;
                    }                
                 }
            }
            catch (Exception ex)
            {
                applog.write(ex.ToString());
                return;
            }
            try
            {
                Properties.Settings.Default.lastDate = Properties.Settings.Default.lastDate; //костыль для проверки доступа к фалу параметров
                Properties.Settings.Default.Save();
            }
            catch (Exception ex1)
            {
                MessageBox.Show(MyStrings.ConfigFileOpenException);
                applog.write(ex1.ToString());
                return;
            }
            DataTable tableSSRFCVD = new DataTable(); //Таблица для копии dbo.SSRFCVD
            bool exception = false;
            String tempCode = ""; //Строка для кодов валют
            String lastCodes = "";//Строка для хранение последних записаных кодов
            String insertString = "";
            String passwordString = Properties.Settings.Default.Password + PasswordClass.Value;
            double float3 = 0;
            System.Data.SqlClient.SqlConnectionStringBuilder builder =
                                                new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder["server"] = Properties.Settings.Default.Server;
            builder["Trusted_Connection"] = Properties.Settings.Default.WindowsAutification;
            builder["user id"] = Properties.Settings.Default.Login;
            builder["password"] = passwordString;
            builder["database"] = Properties.Settings.Default.DBName;
            SqlConnection myConnection = new SqlConnection(builder.ConnectionString);
            try
            {                
                myConnection.Open();
                String cmd = "SELECT * FROM dbo.SSRFCVD_TEST WHERE SUN_DB=@SUNDBname AND USE_DAILY='Y'";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd, myConnection);
                adapter.SelectCommand.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@SUNDBname",
                    Value = Properties.Settings.Default.SUN_DBName,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 2000  // Assuming a 2000 char size of the field annotation (-1 for MAX)
                });
                adapter.Fill(tableSSRFCVD); //Копируем в таблицу tableSSRFCVD таблицу из БД dbo.SSRFCVD
                myConnection.Close();
                
            }            
            catch (Exception ex)
            {
               exception = true;
               applog.write(ex.ToString());
            }
            if (exception)
            {
                return;
            }

            // Получение кодов валют из сохраненной таблицы и формирование инсертов
            foreach (DataRow row in tableSSRFCVD.Rows) 
            {
                foreach (DataColumn column in tableSSRFCVD.Columns)
                {
                    if(column.ColumnName=="CONV_NAME")
                    {
                        tempCode = row[column].ToString();
                        tempCode = tempCode.Remove(4);
                        tempCode = tempCode.Replace(" ","");
                        if (textContainsNumber(tempCode))
                        {
                            lastCodes += tempCode + " ";
                        }                        
                        if (textContainsNumber(tempCode) && tempCode!="9999")
                        {
                            string [] split = getRate(tempCode).Split(new Char [] {';'});
                            float3 = Convert.ToDouble(split[4].Replace('.',',')) / Convert.ToDouble(split[2]);
                            insertString = "INSERT INTO dbo.SSRFCND_TEST Values(@SUNDBname, @Code, @Date,'', @Lookup, @Date,'', @ConvName,"+
                                " '*', @Rates, @DecPlaces, @NetAcctReal, @LossAcctReal,@GainAcctReal,'0.000000000');";
                            SqlConnection myConnection1 = new SqlConnection(builder.ConnectionString);
                            try
                            {
                                myConnection1.Open();
                                SqlCommand cmd = new SqlCommand(insertString, myConnection1);
                                cmd.Parameters.AddWithValue("@SUNDBname", Properties.Settings.Default.SUN_DBName);
                                cmd.Parameters.AddWithValue("@Code", row["CODE"]);
                                cmd.Parameters.AddWithValue("@Date", getSystemDateDB());
                                cmd.Parameters.AddWithValue("@Lookup", row["LOOKUP"]);
                                cmd.Parameters.AddWithValue("@ConvName", row["CONV_NAME"]);
                                cmd.Parameters.AddWithValue("@Rates", converFloatDB(float3));
                                cmd.Parameters.AddWithValue("@DecPlaces", row["DEC_PLACES"]);
                                cmd.Parameters.AddWithValue("@NetAcctReal", row["NET_ACCT_REAL"]);
                                cmd.Parameters.AddWithValue("@LossAcctReal", row["LOSS_ACCT_REAL"]);
                                cmd.Parameters.AddWithValue("@GainAcctReal", row["GAIN_ACCT_REAL"]);
                                cmd.ExecuteNonQuery();                               
                                myConnection1.Close();
                            }
                            catch (Exception ex)
                            {
                                exception = true;
                                applog.write(ex.ToString());
                            }
                           
                        }
                        if(tempCode=="9999" && Properties.Settings.Default.Country=="ua") // Для средневзвешеного курса гривны
                        {
                            string[] split0 = GetAvrgRateUA(tempCode).Split(new Char[] { ';' ,'А'});
                            string[] split1 = split0[2].Split(new Char[] {'а','д'});
                            string[] split2 = split0[1].Split(new Char[]{'.'});
                            float3 = Convert.ToDouble(split0[3].Replace('.', ',')) / Convert.ToDouble(split1[1]);
                            insertString = "INSERT INTO dbo.SSRFCND_TEST Values(@SUNDBname, @Code, @Date,'', @Lookup, @SystemDate,'', @ConvName," +
                                " '*', @Rates, @DecPlaces, @NetAcctReal, @LossAcctReal,@GainAcctReal,'0.000000000');";
                            SqlConnection myConnection1 = new SqlConnection(builder.ConnectionString);
                            try
                            {
                                myConnection1.Open();
                                SqlCommand cmd = new SqlCommand(insertString, myConnection1);
                                cmd.Parameters.AddWithValue("@SUNDBname", Properties.Settings.Default.SUN_DBName);
                                cmd.Parameters.AddWithValue("@Code", row["CODE"]);
                                cmd.Parameters.AddWithValue("@Date", split2[2] + split2[1]  + split2[0]);                                
                                cmd.Parameters.AddWithValue("@Lookup", row["LOOKUP"]);
                                cmd.Parameters.AddWithValue("@SystemDate", getSystemDateDB());
                                cmd.Parameters.AddWithValue("@ConvName", row["CONV_NAME"]);
                                cmd.Parameters.AddWithValue("@Rates", converFloatDB(float3));
                                cmd.Parameters.AddWithValue("@DecPlaces", row["DEC_PLACES"]);
                                cmd.Parameters.AddWithValue("@NetAcctReal", row["NET_ACCT_REAL"]);
                                cmd.Parameters.AddWithValue("@LossAcctReal", row["LOSS_ACCT_REAL"]);
                                cmd.Parameters.AddWithValue("@GainAcctReal", row["GAIN_ACCT_REAL"]);
                                cmd.ExecuteNonQuery();
                                myConnection1.Close();
                            }
                            catch (Exception ex)
                            {
                                exception = true;
                                applog.write(ex.ToString());
                            }
                        }                        
                    }                    
                }
            }
            if (!exception)
            {
                Properties.Settings.Default.lastDate = getSystemDate(); //Записываем в юзер конфиг что мы уже сегодня записали такие то курсы
                Properties.Settings.Default.lastCodes = lastCodes;
                try
                {
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(MyStrings.ConfigFileOpenException);
                    applog.write(ex.ToString());
                }
                label4.Text = Properties.Settings.Default.lastDate;
                DialogResult result2 = MessageBox.Show(MyStrings.Success +" " + Properties.Settings.Default.lastCodes);
            }
            applog.write(insertString + "\n" + builder.ConnectionString  + "\n");
       }

        private void ExchangeRates_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Language.ToString() == "" || Properties.Settings.Default.Country.ToString() == "")
            {
                Localization localization = new Localization();
                localization.ShowDialog();
                localize();
            }
            if (Properties.Settings.Default.Server.ToString() == "" || Properties.Settings.Default.SUN_DBName.ToString() == "" || Properties.Settings.Default.DBName.ToString() == "")
            {
                Connection connection = new Connection();
                connection.ShowDialog();

            }
            String passwordString = Properties.Settings.Default.Password + PasswordClass.Value;
            DataTable TempTable = new DataTable();
            System.Data.SqlClient.SqlConnectionStringBuilder builder =
                                                new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder["server"] = Properties.Settings.Default.Server;
            builder["Trusted_Connection"] = Properties.Settings.Default.WindowsAutification;
            builder["user id"] = Properties.Settings.Default.Login;
            builder["password"] = passwordString;
            builder["database"] = Properties.Settings.Default.DBName;
            SqlConnection myConnection = new SqlConnection(builder.ConnectionString);
            try
            {
                myConnection.Open();
                String Sql = "SELECT DISTINCT CODE FROM dbo.SSRFCND_TEST";
                SqlCommand cmd = new SqlCommand(Sql, myConnection);
                SqlDataReader DR = cmd.ExecuteReader();

                while (DR.Read())
                {
                    comboBox1.Items.Add(DR[0]);

                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                applog.write(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
                String StartDate="",EndDate="",temp="";
                temp = dateTimePicker1.Text.Replace(".", "");
                StartDate = temp.Substring(4, 4) + temp.Substring(2, 2) + temp.Substring(0, 2);
                temp = dateTimePicker2.Text.Replace(".", "");
                EndDate = temp.Substring(4, 4) + temp.Substring(2, 2) + temp.Substring(0, 2);
                String passwordString = Properties.Settings.Default.Password + PasswordClass.Value;
                DataTable TempTable = new DataTable();
                System.Data.SqlClient.SqlConnectionStringBuilder builder =
                                                    new System.Data.SqlClient.SqlConnectionStringBuilder();
                builder["server"] = Properties.Settings.Default.Server;
                builder["Trusted_Connection"] = Properties.Settings.Default.WindowsAutification;
                builder["user id"] = Properties.Settings.Default.Login;
                builder["password"] = passwordString;
                builder["database"] = Properties.Settings.Default.DBName;
                SqlConnection myConnection = new SqlConnection(builder.ConnectionString);
                try
                {
                    myConnection.Open();
                    String cmd = "";
                    comboBox1.DisplayMember = "NativeName";
                    comboBox1.ValueMember = "Name";
                    cmd = "SELECT CODE, CONV_DATE, RATE FROM dbo.SSRFCND_TEST WHERE CONV_DATE>=@StartDate AND CONV_DATE<=@EndDate  AND CODE=@Code ORDER BY CONV_DATE";
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd, myConnection);
                    adapter.SelectCommand.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@StartDate",
                        Value = StartDate,
                        SqlDbType = SqlDbType.Int,
                    });
                    adapter.SelectCommand.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@EndDate",
                        Value = EndDate,
                        SqlDbType = SqlDbType.Int,
                    });
                    adapter.SelectCommand.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@Code",
                        Value = comboBox1.SelectedItem.ToString(),
                        SqlDbType = SqlDbType.NVarChar,
                        Size = 2000  // Assuming a 2000 char size of the field annotation (-1 for MAX)
                    });
                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
                    // tableSSRFCND.Locale = System.Globalization.CultureInfo.InvariantCulture;
                    adapter.Fill(TempTable);
                    dataGridView1.DataSource = TempTable;
                    dataGridView1.Refresh();
                    //SBind.DataSource = tableSSRFCND;
                    myConnection.Close();

                }
                catch (Exception ex)
                {
                    applog.write(ex.ToString());
                }
            
            
        }        
    }
}

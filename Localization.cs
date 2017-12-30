using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ExchangeRates
{
    public partial class Localization : Form
    {
        public Localization()
        {
            checkLocalization();
            InitializeComponent();            
        }
        
        public void checkLocalization()  //Функция локализации
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
        }

        //По нажатию ОК сохраняем страну локализации
        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Language = comboBox1.SelectedValue.ToString();
            Properties.Settings.Default.Save();
            checkLocalization();
           if (comboBox2.SelectedItem.ToString() == MyStrings.countrySiteNameUkraine)
           {
                Properties.Settings.Default.Country = "ua";
                Properties.Settings.Default.Save();
           }
           if (comboBox2.SelectedItem.ToString() == MyStrings.countrySiteNameRussia)
           {
                Properties.Settings.Default.Country = "ru";
                Properties.Settings.Default.Save();
           }           
           this.Close();
        }



        private void localizationCheck() //Подставляем в комбобокс значение сохранненой локализации
        {
            if (Properties.Settings.Default.Country.ToString() == "ua")
            {
                comboBox2.SelectedItem = MyStrings.countrySiteNameUkraine;
            }
            if (Properties.Settings.Default.Country.ToString() == "ru")
            {
                comboBox2.SelectedItem = MyStrings.countrySiteNameRussia;
            }
        }

        //При загрузке формы инифиализируем комбобоксы
        private void Localization_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = new System.Globalization.CultureInfo[]{
                System.Globalization.CultureInfo.GetCultureInfo("en-US"),
                System.Globalization.CultureInfo.GetCultureInfo("ru-RU"),
                System.Globalization.CultureInfo.GetCultureInfo("uk-UA")
            };
            comboBox1.DisplayMember = "NativeName";
            comboBox1.ValueMember = "Name";
            if (string.IsNullOrEmpty(Properties.Settings.Default.Language))
            {
                Properties.Settings.Default.Language = "en-US";
                Properties.Settings.Default.Save();
                checkLocalization();
            }
            //Показываем в комбобоксе уже выбраный язык
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Language))
            {
                comboBox1.SelectedValue = Properties.Settings.Default.Language;
            }

            //Если нету установненой страны локализацию, то присваеваем базовое значение Украине
            if (string.IsNullOrEmpty(Properties.Settings.Default.Country))
            {
                Properties.Settings.Default.Country = "ua";
                Properties.Settings.Default.Save();
            }
            comboBox2.Items.Add(MyStrings.countrySiteNameUkraine);
            comboBox2.Items.Add(MyStrings.countrySiteNameRussia);
            //Подставляем в комбобокс значение сохранненой локализации
            localizationCheck();
        }
        
        //Изминение языка при выборе опции из комбобокса
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Properties.Settings.Default.Language = comboBox1.SelectedValue.ToString();
            Properties.Settings.Default.Save();
            checkLocalization();
            //Перезаполняем области форм
            var resources = new ComponentResourceManager(typeof(Localization));             
            resources.ApplyResources(this, "$this");
            resources.ApplyResources(this.label2, "label2");
            //Перезаписываем комбококс с страной локализации
            comboBox2.Items.Clear();            
            comboBox2.Items.Add(MyStrings.countrySiteNameUkraine);
            comboBox2.Items.Add(MyStrings.countrySiteNameRussia);
            //Подставляем в комбобокс значение сохранненой локализации
            localizationCheck();
        }

    }
}

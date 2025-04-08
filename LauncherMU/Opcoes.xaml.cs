using Microsoft.Win32;
using System.Windows;

namespace LauncherMU
{
    public partial class Opcoes : Window
    {
        public static int Res = 0;
        public static int Som = 0;
        public static int Music = 0;
        public static int Lang = 0;
        public static int WindowMode = 0;
        public static string login = "";
        public static string senha = "";
        public Opcoes()
        {
            InitializeComponent();

            comboBoxIdioma.Items.Add("POR");
            comboBoxIdioma.Items.Add("ENG");
            //comboBoxIdioma.Items.Add("ESP");

            var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Webzen\Mu\Config", true);

            if (Convert.ToInt32(reg.GetValue("SoundOnOFF", RegistryValueKind.DWord)) == 1)
            {
                checkBoxSom.IsChecked = true;
                Som = 1;
            }

            if (Convert.ToInt32(reg.GetValue("MusicOnOFF", RegistryValueKind.DWord)) == 1)
            {
                checkBoxMusic.IsChecked = true;
                Music = 1;
            }
            if (Convert.ToInt32(reg.GetValue("Resolution", RegistryValueKind.DWord)) == 0)
            {
                radioButton1.IsChecked = true;
                Res = 0;
            }
            if (Convert.ToInt32(reg.GetValue("Resolution", RegistryValueKind.DWord)) == 1)
            {
                radioButton2.IsChecked = true;
                Res = 1;
            }
            if (Convert.ToInt32(reg.GetValue("Resolution", RegistryValueKind.DWord)) == 2)
            {
                radioButton3.IsChecked = true;
                Res = 2;
            }
            if (Convert.ToInt32(reg.GetValue("Resolution", RegistryValueKind.DWord)) == 3)
            {
                radioButton4.IsChecked = true;
                Res = 3;
            }
            if (Convert.ToInt32(reg.GetValue("Resolution", RegistryValueKind.DWord)) == 4)
            {
                radioButton5.IsChecked = true;
                Res = 4;
            }
            if (Convert.ToInt32(reg.GetValue("Resolution", RegistryValueKind.DWord)) == 7)
            {
                radioButton6.IsChecked = true;
                Res = 7;
            }
            if (Convert.ToInt32(reg.GetValue("WindowMode", RegistryValueKind.DWord)) == 1)
            {
                checkBoxModoJanela.IsChecked = true;
                WindowMode = 1;
            }
            if (String.Compare(Convert.ToString(reg.GetValue("LangSelection", RegistryValueKind.String)), "Por", true) == 0)
            {
                comboBoxIdioma.SelectedIndex = 0;
                Lang = 1;
            }
            if (String.Compare(Convert.ToString(reg.GetValue("LangSelection", RegistryValueKind.String)), "Spn", true) == 0)
            {
                comboBoxIdioma.SelectedIndex = 2;
                Lang = 2;
            }
            if (String.Compare(Convert.ToString(reg.GetValue("LangSelection", RegistryValueKind.String)), "Eng", true) == 0)
            {
                comboBoxIdioma.SelectedIndex = 1;
                Lang = 3;
            }
            if (String.Compare(Convert.ToString(reg.GetValue("ID", RegistryValueKind.String)), "String", true) != 0)
            {
                textBoxLogin.Text = Convert.ToString(reg.GetValue("ID", RegistryValueKind.String));
            }
            if (String.Compare(Convert.ToString(reg.GetValue("PW", RegistryValueKind.String)), "String", true) != 0)
            {
                textBoxSenha.Password = Convert.ToString(reg.GetValue("PW", RegistryValueKind.String));
            }
        }

        private void btnAplicar_Click(object sender, RoutedEventArgs e)
        {
            if (radioButton1.IsChecked == true)
                Res = 0;
            if (radioButton2.IsChecked == true)
                Res = 1;
            if (radioButton3.IsChecked == true)
                Res = 2;
            if (radioButton4.IsChecked == true)
                Res = 3;
            if (radioButton5.IsChecked == true)
                Res = 4;
            if (radioButton6.IsChecked == true)
                Res = 7;

            var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Webzen\Mu\Config", true);

            if (reg == null)
                reg = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Webzen\Mu\Config");
            if (reg.GetValue("MusicOnOFF") == null)
                reg.SetValue("MusicOnOFF", "0", RegistryValueKind.DWord);
            if (reg.GetValue("SoundOnOFF") == null)
                reg.SetValue("SoundOnOFF", "0", RegistryValueKind.DWord);
            if (reg.GetValue("Resolution") == null)
                reg.SetValue("Resolution", "0", RegistryValueKind.DWord);
            if (reg.GetValue("WindowMode") == null)
                reg.SetValue("WindowMode", "0", RegistryValueKind.DWord);
            if (reg.GetValue("LangSelection") == null)
                reg.SetValue("LangSelection", "Eng", RegistryValueKind.String);

            if (comboBoxIdioma.SelectedIndex == 0)
                reg.SetValue("LangSelection", "Por", RegistryValueKind.String);
            else if (comboBoxIdioma.SelectedIndex == 1)
                reg.SetValue("LangSelection", "Eng", RegistryValueKind.String);
            else
                reg.SetValue("LangSelection", "Esp", RegistryValueKind.String);

            reg.SetValue("MusicOnOFF", Music, RegistryValueKind.DWord);
            reg.SetValue("SoundOnOFF", Som, RegistryValueKind.DWord);
            reg.SetValue("Resolution", Res, RegistryValueKind.DWord);
            reg.SetValue("WindowMode", WindowMode, RegistryValueKind.DWord);
            reg.SetValue("ID", textBoxLogin.Text, RegistryValueKind.String);
            reg.SetValue("PW", textBoxSenha.Password, RegistryValueKind.String);

            reg.Close();

            this.DialogResult = true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void checkBoxSom_Checked(object sender, RoutedEventArgs e)
        {
            Som = 1;
        }

        private void checkBoxSom_Unchecked(object sender, RoutedEventArgs e)
        {
            Som = 0;
        }

        private void checkBoxModoJanela_Checked(object sender, RoutedEventArgs e)
        {
            WindowMode = 1;
        }

        private void checkBoxModoJanela_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowMode = 0;
        }

        private void checkBoxMusic_Checked(object sender, RoutedEventArgs e)
        {
            Music = 1;
        }

        private void checkBoxMusic_Unchecked(object sender, RoutedEventArgs e)
        {
            Music = 0;
        }

    }
}

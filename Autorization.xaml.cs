using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using EasyCaptcha.Wpf;
using Microsoft.IdentityModel.Protocols;

namespace UchebPraktika
{
    public partial class Autorization : Window
    {
        private readonly bbbbbEntities bd = new bbbbbEntities();
        public string CaptchaOtvet;
        private int logAttempt = 0;
        private DispatcherTimer timer;
         

        public Autorization()
        {
            InitializeComponent();


            InitializeTimer();
            GenerateCaptcha();
            LoadCredentials();
        }


        private void LoadCredentials()
        {
            string savedUserId = ConfigurationManager.AppSettings["UserID"];
            string savedPassword = ConfigurationManager.AppSettings["UserPassword"];

            if (!string.IsNullOrEmpty(savedUserId) && !string.IsNullOrEmpty(savedPassword))
            {
                Login.Text = savedUserId;
                Password.Password = savedPassword;
            }
        }

        private void SaveCredentials(string userId, string password)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["UserID"].Value = userId;
            config.AppSettings.Settings["UserPassword"].Value = password;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10) 
            };
            timer.Tick += UnblockSystem; 
        }

        private void GenerateCaptcha()
        {
            CaptchaTextBox.CreateCaptcha(Captcha.LetterOption.Alphanumeric, 4);
            CaptchaOtvet = CaptchaTextBox.CaptchaText; 
        }

        private void BlockSystem()
        {
            Login.IsEnabled = false;
            Password.IsEnabled = false;
            enter.IsEnabled = false;
            MessageBox.Show("Система заблокирована на 10 секунд.");
            timer.Start();
        }

        private void UnblockSystem(object sender, EventArgs e)
        {
            Login.IsEnabled = true;
            Password.IsEnabled = true;
            enter.IsEnabled = true;
            logAttempt = 0; // Сброс счетчика попыток
            timer.Stop(); 
            GenerateCaptcha();
            CaptchaTextBox.Visibility = Visibility.Visible;
            TextBoxCap.Visibility = Visibility.Visible;
        }
        private void CheckAttempts()
        {
            if (logAttempt >= 3)
            {
                BlockSystem();

            }
        }

        public int ValidatePassword(string x)
        {
            bool hasDigit = false, hasLower = false, hasUpper = false, hasSpecialChar = false;
            foreach (char c in x)
            {
                if (char.IsDigit(c)) hasDigit = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsUpper(c)) hasUpper = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecialChar = true;
            }

            if (x.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов.");
                return 1; 
            }
            if (!hasUpper)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну заглавную букву.");
                return 2;
            }
            if (!hasLower)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну строчную букву.");
                return 3; 
            }
            if (!hasDigit)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру.");
                return 4; 
            }
            if (!hasSpecialChar)
            {
                MessageBox.Show("Пароль должен содержать хотя бы один специальный символ.");
                return 5; 
            }

            return 0; 
        }


        public bool AuthorizeUser(string inputLogin, string inputPassword)
        {
            if (!int.TryParse(inputLogin.Trim(), out int id))
            {
                ErrorText.Content = "Некорректный ID пользователя. Введите числовое значение.";
                ErrorText.Visibility = Visibility.Visible;
                return false;
            }

            var user = bd.Polzovateli.FirstOrDefault(u => u.Id_polzovatelya == id);
            if (user == null)
            {
                ErrorText.Content = "Пользователь с таким ID не найден.";
                ErrorText.Visibility = Visibility.Visible;
                return false;
            }

            if (user.Parol != inputPassword)
            {
                ErrorText.Content = "Неверный пароль. В пароле " +
                    "не менее 6 символов;\r\n• заглавные и строчные буквы;\r\n• не менее одного спецсимвола;\r\n• не менее одной цифры.";
                ErrorText.Visibility = Visibility.Visible;
                return false;
            }

            int validationResult = ValidatePassword(inputPassword);
            if (validationResult != 0) // Если не 0, пароль не соответствует требованиям
            {
                return false;
            }

            switch (user.Roli.Nazvanie)
            {
                case "участник": new UchastnikWindows().Show(); break;
                case "модератор": new ModeratorWindow().Show(); break;
                case "организатор": new OrganizatorWindow(user).Show(); break;
                case "жюри": new ZhuriWindow().Show(); break;
                default:
                    ErrorText.Content = "Роль пользователя не распознана.";
                    ErrorText.Visibility = Visibility.Visible;
                    return false;
            }

            ErrorText.Visibility = Visibility.Collapsed;
            MessageBox.Show("Авторизация успешна!");
            this.Close();
            return true;
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string inputLogin = Login.Text.Trim();
                string inputPassword = Password.Password.Trim();
                

                if (string.IsNullOrWhiteSpace(inputLogin) || string.IsNullOrWhiteSpace(inputPassword))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.");
                    return;
                }

                if (CaptchaTextBox.Visibility == Visibility.Visible && TextBoxCap.Text.Trim() != CaptchaOtvet)
                {
                    MessageBox.Show("Неверная CAPTCHA. Попробуйте снова.");
                    GenerateCaptcha();
                    logAttempt++;
                    CheckAttempts();
                    return;
                }
                if (AuthorizeUser(inputLogin, inputPassword))
                {
                    logAttempt = 0;
                    CaptchaTextBox.Visibility = Visibility.Collapsed;
                    TextBoxCap.Visibility = Visibility.Collapsed;
                    SaveCredentials(inputLogin, inputPassword);
                }
                else
                {
                    logAttempt++;
                    CheckAttempts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
                logAttempt++;
                CheckAttempts();
            }
        }
      

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}

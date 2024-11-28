using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using EasyCaptcha.Wpf;

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

        public bool ValidatePassword(string x)
        {
            bool hasDigit = false, hasLower = false, hasUpper = false, hasSpecialChar = false;
            foreach (char c in x)
            {
                if (char.IsDigit(c)) hasDigit = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsUpper(c)) hasUpper = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecialChar = true;
            }

            if (x.Length < 6) { 
                MessageBox.Show("Пароль должен содержать не менее 6 символов."); return false;
            }
            if (!hasUpper) {
                MessageBox.Show("В пароле не хваатает заглавной буквы"); return false;
            }
            if (!hasLower) {
                MessageBox.Show("В пароле не хватает строчной буквы"); return false; 
            }
            if (!hasDigit) {
                MessageBox.Show("В пароле не хватает цифры"); return false; 
            }
            if (!hasSpecialChar) {
                MessageBox.Show("Осталось только понять, какой символ ты забыл прописать." +
                    " Возможно, дальше ты сможешь войти в свой аккаунт"); return false;
            }

            return true; 
        }

        public bool AuthorizeUser(string inputLogin, string inputPassword)
        {
            if (!int.TryParse(inputLogin.Trim(), out int id))
            {
                MessageBox.Show("Некорректный ID пользователя. Введите числовое значение.");
                return false;
            }

            if (!ValidatePassword(inputPassword)) return false;

            var user = bd.Polzovateli.FirstOrDefault(u => u.Id_polzovatelya == id && u.Parol == inputPassword);
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.");
                return false;
            }

            switch (user.Roli.Nazvanie)
            {
                case "участник": new UchastnikWindows().Show(); break;
                case "модератор": new ModeratorWindow().Show(); break;
                case "организатор": new OrganizatorWindow().Show(); break;
                case "жюри": new ZhuriWindow().Show(); break;
                default: MessageBox.Show("Роль пользователя не распознана."); return false;
            }

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
        private void CheckAttempts()
        {
            if (logAttempt >= 3)
            {
                BlockSystem();
               
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}

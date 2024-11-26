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
        private string CaptchaOtvet;
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
                Interval = TimeSpan.FromSeconds(10) // Таймер блокировки 10 секунд
            };
            timer.Tick += UnblockSystem;
        }

        private void GenerateCaptcha()
        {
            CaptchaTextBox.CreateCaptcha(Captcha.LetterOption.Alphanumeric, 4); // Генерация CAPTCHA
            CaptchaOtvet = CaptchaTextBox.CaptchaText; // Сохранение правильного ответа
        }

        private void BlockSystem()
        {
            // Отключение элементов интерфейса
            Login.IsEnabled = false;
            Password.IsEnabled = false;
            enter.IsEnabled = false;
            MessageBox.Show("Система заблокирована на 10 секунд.");

            timer.Start(); // Запуск таймера
        }

        private void UnblockSystem(object sender, EventArgs e)
        {
            // Включение элементов интерфейса
            Login.IsEnabled = true;
            Password.IsEnabled = true;
            enter.IsEnabled = true;
            logAttempt = 0; // Сброс счетчика попыток

            timer.Stop(); // Остановка таймера
        }

        private void SaveUserCredentials()
        {
            try
            {
                if (save.IsChecked == true)
                {
                    if (string.IsNullOrWhiteSpace(Login.Text) || string.IsNullOrWhiteSpace(Password.Password))
                    {
                        MessageBox.Show("Невозможно сохранить пустые учетные данные.");
                        return;
                    }
                    // Сохранение данных в настройках приложения
                    Properties.Settings.Default.DefaultLogin = Login.Text;
                    Properties.Settings.Default.DefaultPass = Password.Password;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения учетных данных: {ex.Message}");
            }
        }

        private void AuthorizeUser()
        {
            try
            {
                if (!int.TryParse(Login.Text.Trim(), out int id))
                {
                    MessageBox.Show("Некорректный ID пользователя. Введите числовое значение.");
                    return;
                }

                string pass = Password.Password.Trim();
                if (string.IsNullOrWhiteSpace(pass))
                {
                    MessageBox.Show("Пароль не может быть пустым.");
                    return;
                }

                var polzovatel = bd.Polzovateli.FirstOrDefault(x => x.Id_polzovatelya == id && x.Parol == pass);

                if (polzovatel == null)
                {
                    MessageBox.Show("Неверный ID или пароль.");
                    return;
                }

                if (TextBoxCap.Text.Trim() != CaptchaOtvet)
                {
                    MessageBox.Show("Неверная CAPTCHA. Попробуйте снова.");
                    GenerateCaptcha();
                    return;
                }

                // Проверка ролей пользователя
                string rol = polzovatel.Roli.Nazvanie;
                switch (rol)
                {
                    case "участник":
                        new UchastnikWindows().Show();
                        break;
                    case "модератор":
                        new ModeratorWindow().Show();
                        break;
                    case "организатор":
                        new OrganizatorWindow().Show();
                        break;
                    case "жюри":
                        new ZhuriWindow().Show();
                        break;
                    default:
                        MessageBox.Show("Роль пользователя не распознана.");
                        return;
                }

                this.Close();
            }
            catch (System.Data.EntityException ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (logAttempt >= 3)
                {
                    BlockSystem();
                    return;
                }

                string inputLogin = Login.Text.Trim();
                string inputPassword = Password.Password.Trim();

                // Проверка на пустые поля
                if (string.IsNullOrWhiteSpace(inputLogin) || string.IsNullOrWhiteSpace(inputPassword))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.");
                    return;
                }

                // Проверка длины пароля
                if (inputPassword.Length < 6)
                {
                    MessageBox.Show("Пароль должен содержать не менее 6 символов.");
                    return;
                }

                // Проверка CAPTCHA
                if (TextBoxCap.Text.Trim() != CaptchaOtvet)
                {
                    MessageBox.Show("Неверная CAPTCHA. Попробуйте снова.");
                    GenerateCaptcha();
                    return;
                }

                SaveUserCredentials(); // Сохранение данных
                AuthorizeUser(); // Авторизация
            }
            catch (Exception ex)
            {
                logAttempt++; // Увеличение счетчика попыток
                MessageBox.Show($"Произошла ошибка: {ex.Message}. Осталось попыток: {3 - logAttempt}");

                if (logAttempt >= 3)
                {
                    BlockSystem(); // Блокировка после 3 попыток
                }
            }
        }
    }
}

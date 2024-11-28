using System;
using System.Windows;

namespace UchebPraktika
{
    public partial class OrganizatorWindow : Window
    {
        private Polzovateli currentUser;

        public OrganizatorWindow(Polzovateli user)
        {
            InitializeComponent();
            currentUser = user;
            LoadUserData();
        }

        private string GetTimePeriod()
        {
            int currentHour = DateTime.Now.Hour;

            if (currentHour >= 9 && currentHour <= 11) return "Доброе утро";
            if (currentHour >= 11 && currentHour <= 18) return "Добрый день";
           else return "Добрый Вечер";
        }

        private void LoadUserData()
        {
            if (string.IsNullOrWhiteSpace(currentUser?.FIO))
            {
                WelcomeText.Text = "Добро пожаловать, пользователь!";
                return;
            }

            string[] fioParts = currentUser.FIO.Split(' ');
            string lastName = fioParts.Length > 0 ? fioParts[0] : "Пользователь";
            string name = fioParts.Length > 1 ? fioParts[1] : "";
            string middleName = fioParts.Length > 2 ? fioParts[2] : "";

            string timePeriod = GetTimePeriod();
            string greeting = $"{timePeriod}, {lastName} {name} {middleName}";

            WelcomeText.Text = greeting;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            orgRegWin orgRegWin = new orgRegWin();
            orgRegWin.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MeropriyatiyaWin meropriyatiyaWin = new MeropriyatiyaWin();
            meropriyatiyaWin.Show();
            this.Close();
        }
    }
}

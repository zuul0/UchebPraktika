using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UchebPraktika
{
    /// <summary>
    /// Логика взаимодействия для OrganizatorWindow.xaml
    /// </summary>
    public partial class OrganizatorWindow : Window
    {
        public OrganizatorWindow()
        {
            InitializeComponent();
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

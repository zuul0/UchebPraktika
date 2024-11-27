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
    /// Логика взаимодействия для orgRegWin.xaml
    /// </summary>
    public partial class orgRegWin : Window
    {
        public orgRegWin()
        {
            InitializeComponent();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            OrganizatorWindow organizatorWindow = new OrganizatorWindow();
            organizatorWindow.Show();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            OrganizatorWindow organizatorWindow = new OrganizatorWindow();
            organizatorWindow.Show();

        }
    }
} 

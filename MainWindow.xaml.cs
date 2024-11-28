using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace UchebPraktika
{
    public partial class MainWindow : Window
    {
        bbbbbEntities context = new bbbbbEntities();
        List<MeropriyatiyaMainW> allMeropriyatiya;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            InitializeFilters();
        }

        private void LoadData()
        {
            try
            {
                allMeropriyatiya = new List<MeropriyatiyaMainW>(context.MeropriyatiyaMainW);
                MeropriyatiyaMainW.ItemsSource = allMeropriyatiya;
            }
            catch (System.Data.EntityException ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeFilters()
        {
            List<string> directions = new List<string>();
            directions.Add("Все направления");

            foreach (var napravlenie in context.Napravlenie)
            {
                if (!directions.Contains(napravlenie.Nazvanie))
                {
                    directions.Add(napravlenie.Nazvanie);
                }
            }

            FilterDirectionComboBox.ItemsSource = directions;
            FilterDirectionComboBox.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            List<MeropriyatiyaMainW> filteredMeropriyatiya = new List<MeropriyatiyaMainW>();

            foreach (var meropriyatiye in allMeropriyatiya)
            {
                bool matchesDirection = FilterDirectionComboBox.SelectedIndex == 0 ||
                                        meropriyatiye.Expr1 == FilterDirectionComboBox.SelectedItem.ToString();

                bool matchesDate = !FilterDatePicker.SelectedDate.HasValue ||
                                   meropriyatiye.Data_nachala.Date == FilterDatePicker.SelectedDate.Value.Date;

                if (matchesDirection && matchesDate)
                {
                    filteredMeropriyatiya.Add(meropriyatiye);
                }
            }

            MeropriyatiyaMainW.ItemsSource = filteredMeropriyatiya;
        }

        private void FilterDirectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void FilterDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void AutorizationButton_Click(object sender, RoutedEventArgs e)
        {
            Autorization autorization = new Autorization();
            autorization.Show();
            this.Close();
        }

        private void MeropriyatiyaMainW_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Обработка изменения выбора в DataGrid
        }

        private void FilterDirectionComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();

        }
    }
}
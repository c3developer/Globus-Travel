using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для GuestWindow.xaml
    /// </summary>
    public partial class GuestWindow : Window
    {

        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TourAgency;Integrated Security=True";
        public GuestWindow()
        {
            InitializeComponent();

        }
        private void LoadTours()
        {
            try
            {
                // 1. Создаем подключение
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // 2. Создаем команду
                    string query = @"
                        SELECT 
                            t.Id,
                            t.Title,
                            c.Name as Country,
                            t.Duration,
                            t.StartDate,
                            t.BasePrice,
                            t.Discount,
                            t.BusType,
                            t.Capacity,
                            t.FreeSeats
                        FROM Tours t
                        LEFT JOIN Countries c ON t.CountryId = c.Id
                        ORDER BY t.StartDate";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // 3. Создаем DataTable вручную
                        DataTable dataTable = new DataTable();

                        // 4. Создаем колонки
                        dataTable.Columns.Add("Id", typeof(int));
                        dataTable.Columns.Add("Название тура", typeof(string));
                        dataTable.Columns.Add("Страна", typeof(string));
                        dataTable.Columns.Add("Длительность", typeof(int));
                        dataTable.Columns.Add("Дата начала", typeof(DateTime));
                        dataTable.Columns.Add("Стоимость", typeof(decimal));
                        dataTable.Columns.Add("Скидка", typeof(decimal));
                        dataTable.Columns.Add("Тип автобуса", typeof(string));
                        dataTable.Columns.Add("Вместимость", typeof(int));
                        dataTable.Columns.Add("Свободно", typeof(int));

                        // 5. Читаем данные через reader и заполняем DataTable
                        while (reader.Read())
                        {
                            DataRow row = dataTable.NewRow();

                            row["Id"] = reader.GetInt32(0);
                            row["Название тура"] = reader.GetString(1);
                            row["Страна"] = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            row["Длительность"] = reader.GetInt32(3);
                            row["Дата начала"] = reader.GetDateTime(4);
                            row["Стоимость"] = reader.GetDecimal(5);
                            row["Скидка"] = reader.GetDecimal(6);
                            row["Тип автобуса"] = reader.GetString(7);
                            row["Вместимость"] = reader.GetInt32(8);
                            row["Свободно"] = reader.GetInt32(9);

                            dataTable.Rows.Add(row);

                        }

                        dataGrid.ItemsSource = dataTable.DefaultView;
                        //ApplyHighlighting();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки туров: {ex.Message}\n\nПроверь:\n1. Запущен ли LocalDB\n2. Существует ли база TourAgency\n3. Есть ли таблица Tours");
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTours();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно авторизации
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //d
        }
        // ПОДСВЕТКА СТРОК ПО ЗАДАНИЮ
        //private void ApplyHighlighting()
        //{
        //    foreach (System.Windows.Controls.DataGridRow row in DataGridRow(dataGrid))
        //    {
        //        if (row.Item is DataRowView rowView)
        //        {
        //            decimal discount = Convert.ToDecimal(rowView["Скидка"]);
        //            int capacity = Convert.ToInt32(rowView["Вместимость"]);
        //            int freeSeats = Convert.ToInt32(rowView["Свободно"]);
        //            DateTime startDate = Convert.ToDateTime(rowView["Дата начала"]);

        //            // 1. Спецпредложение (скидка > 15%)
        //            if (discount > 15)
        //            {
        //                row.Background = new SolidColorBrush(Color.FromRgb(255, 215, 0)); // #FFD700
        //            }

        //            // 2. Мало мест (< 10% от вместимости)
        //            if (capacity > 0 && freeSeats < capacity * 0.1)
        //            {
        //                row.Background = new SolidColorBrush(Color.FromRgb(255, 182, 193)); // #FFB6C1
        //            }

        //            // 3. Тур скоро начнется (< 7 дней)
        //            if ((startDate - DateTime.Now).Days < 7)
        //            {
        //                row.FontWeight = System.Windows.FontWeights.Bold;
        //            }
        //        }
    }
}

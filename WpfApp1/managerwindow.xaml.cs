using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace WpfApp1
{
    public partial class ManagerWindow : Window
    {
        private int userId;
        private string fullName;
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TourAgency;Integrated Security=True";

        public ManagerWindow(int userId, string fullName)
        {
            InitializeComponent();
            this.userId = userId;
            this.fullName = fullName;
            this.Title = $"Менеджер: {fullName}";

            LoadTours();
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

                        // 6. Привязываем DataTable к DataGrid
                        dataGrid.ItemsSource = dataTable.DefaultView;

                        // 7. Настраиваем форматирование
                        
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

        private void dataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Можно добавить логику при выборе строки
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Простой способ добавить тур через MessageBox
            string tourName = Microsoft.VisualBasic.Interaction.InputBox("Введите название тура:", "Добавить тур", "");

            if (!string.IsNullOrEmpty(tourName))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Простой INSERT запрос
                        string query = @"INSERT INTO Tours (Title, CountryId, Duration, StartDate, BasePrice, Discount, BusType, Capacity, FreeSeats)
                                         VALUES (@Title, 1, 7, GETDATE(), 50000, 0, 'Стандарт', 45, 45)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Title", tourName);
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Тур добавлен!");
                    LoadTours(); // Обновляем список
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрана ли строка
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите тур для удаления");
                return;
            }

            try
            {
                // Получаем выбранную строку
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                int tourId = (int)selectedRow["Id"];
                string tourName = selectedRow["Название тура"].ToString();

                // Подтверждение удаления
                if (MessageBox.Show($"Удалить тур '{tourName}'?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "DELETE FROM Tours WHERE Id = @Id";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", tourId);
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Тур удален");
                                LoadTours(); // Обновляем список
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
            }
        }
    }
}
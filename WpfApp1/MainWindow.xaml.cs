using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Строка подключения - ЗАМЕНИ НА СВОЮ!
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TourAgency;Integrated Security=True";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string password = txtPassword.Password;

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            try
            {
                // ПОДКЛЮЧЕНИЕ К БАЗЕ ДАННЫХ
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Проверяем пользователя в базе
                    string query = "SELECT Id, FullName, Role FROM Users WHERE Login = @login AND Password = @password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Защита от SQL-инъекций
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);

                        // Используем SqlDataReader для чтения результата
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Пользователь найден
                                int userId = reader.GetInt32(0); // Id
                                string fullName = reader.GetString(1); // FullName
                                string role = reader.GetString(2); // Role

                                // Закрываем окно авторизации
                                this.Hide();

                                // Открываем окно в зависимости от роли
                                OpenUserWindow(role, userId, fullName);
                            }
                            else
                            {
                                MessageBox.Show("Неверный логин или пароль");
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void OpenUserWindow(string role, int userId, string fullName)
        {
            switch (role)
            {
                case "Администратор":
                    Adminwindow adminWindow = new Adminwindow(userId, fullName);
                    adminWindow.Show();
                    break;

                case "Менеджер":
                    ManagerWindow managerWindow = new ManagerWindow(userId, fullName);
                    managerWindow.Show();
                    break;

                case "Авторизованный клиент":
                    Clientwindow clientWindow = new Clientwindow(userId, fullName);
                    clientWindow.Show();
                    break;

            }
        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно для гостя
            GuestWindow toursWindow = new GuestWindow();
            toursWindow.Show();
            this.Close();
        }
    }
}

     
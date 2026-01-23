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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Adminwindow.xaml
    /// </summary>
    public partial class Adminwindow : Window
    {
        private int userId;
        private string fullName;
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB.;Initial Catalog=TourAgency;Integrated Security=True";
        public Adminwindow (int userId, string fullName)
        {
            
            InitializeComponent();
        }

    }
}

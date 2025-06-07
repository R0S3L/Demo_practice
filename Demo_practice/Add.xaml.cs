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

namespace Demo_practice
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Window
    {
        Connect con = new Connect();
        MainWindow window = new MainWindow();
        public Add()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object[] values = new object[] { name_box.Text, login_box.Text, pass_box.Text, Convert.ToInt32(age_box.Text) };
            string[] columns = new string[] { "client_name", "client_login", "client_pass", "client_age" };

            Connect con = new Connect();
            con.InsertRecord("client", columns, values);
            this.Close();
        }
        

    }
}

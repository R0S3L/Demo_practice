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
    /// Логика взаимодействия для update.xaml
    /// </summary>
    public partial class update : Window
    {
        Connect con = new Connect();
        int id_appointment = 0;
        public update(int id_client, int id_master, int id_service, DateTime date, int id_app)
        {
            InitializeComponent();
            client.Text = id_client.ToString();
            master.Text = id_master.ToString();
            service.Text = id_service.ToString();
            date_tb.Text = date.ToString();
            id_appointment = id_app;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object[] values = new object[] { client.Text, master.Text, service.Text, Convert.ToDateTime(date_tb.Text) };
            string[] columns = new string[] { "id_client", "id_master", "id_service", "Date" };

            con.UpdateRecord("appointments", columns, values, "id_appointment", id_appointment);
            this.Close();
        }
    }
}

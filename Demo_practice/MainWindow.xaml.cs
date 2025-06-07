using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo_practice
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Connect con = new Connect();
        public int Selectid = 0;
        public MainWindow()
        {
            InitializeComponent();
            FillGrid();
        }
        public void FillGrid()
        {
            con.LoadData("appointments", DataGrid);
        }
        public void Delete()
        {
            con.DeleteRecord("appointments", "id_appointment", Selectid);
            FillGrid();
                
        }
        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            Add ad = new Add();
            this.Hide();
            ad.ShowDialog();
            this.Show();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedItem is DataRowView row)
            {
                Selectid = Convert.ToInt32(row["id_appointment"]);
            }

        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void update_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem is DataRowView row)
            {
                // Получаем значения из выбранной строки
                int id_appointment = Convert.ToInt32(row["id_appointment"]);
                int id_client = Convert.ToInt32(row["id_client"]);
                int id_master = Convert.ToInt32(row["id_master"]);
                int id_service = Convert.ToInt32(row["id_service"]);
                DateTime date = (DateTime)row["Date"];

                // Передаём их во второе окно
                var detailsWindow = new update(id_client, id_master, id_service, date, id_appointment);
                this.Hide();
                detailsWindow.ShowDialog();
                FillGrid();
                this.Show();
            }
            else
            {
                System.Windows.MessageBox.Show("Пожалуйста, выберите строку.");
            }
        }
    }
    
}

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Demo_practice
{
    public class Connect
    {
        private readonly string _connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=beauty_salon;Integrated Security=True";

        public void LoadData(string table, DataGrid grid)
        {
            string query = $"SELECT * FROM [{table}]";
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable tableResult = new DataTable();
                        adapter.Fill(tableResult);
                        grid.ItemsSource = tableResult.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
            }
        }
        public void LoadFilteredData(string table, string column, string value, DataGrid grid)
        {
            string query = $"SELECT * FROM [{table}] WHERE [{column}] = @value";
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@value", value);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable tableResult = new DataTable();
                        adapter.Fill(tableResult);
                        grid.ItemsSource = tableResult.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
            }
        }

        public void ExecuteNonQuery(string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText)) return;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка SQL: {ex.Message}");
            }
        }

        public void LoadDataSet(string table, DataGrid grid, string query)
        {
            if (string.IsNullOrWhiteSpace(table) || grid == null || string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Некорректные параметры для загрузки данных.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, table);
                    grid.ItemsSource = dataSet.Tables[0].DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        // Добавление записи
        public void InsertRecord(string table, string[] columns, object[] values)
        {
            if (columns.Length != values.Length)
            {
                MessageBox.Show("Количество колонок и значений должно совпадать.");
                return;
            }

            string columnList = string.Join(", ", columns);
            string paramList = string.Join(", ", columns.Select(c => "@" + c));

            string query = $"INSERT INTO [{table}] ({columnList}) VALUES ({paramList})";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        command.Parameters.AddWithValue("@" + columns[i], values[i] ?? DBNull.Value);
                    }

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
            }
        }

        // Обновление записи
        public void UpdateRecord(string table, string[] columns, object[] values, string keyColumn, object keyValue)
        {
            if (columns.Length != values.Length)
            {
                MessageBox.Show("Количество колонок и значений должно совпадать.");
                return;
            }

            string setClause = string.Join(", ", columns.Select(c => $"[{c}] = @{c}"));
            string query = $"UPDATE [{table}] SET {setClause} WHERE [{keyColumn}] = @keyValue";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        command.Parameters.AddWithValue("@" + columns[i], values[i] ?? DBNull.Value);
                    }

                    command.Parameters.AddWithValue("@keyValue", keyValue);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении записи: {ex.Message}");
            }
        }

        // Удаление записи
        public void DeleteRecord(string table, string keyColumn, object keyValue)
        {
            string query = $"DELETE FROM [{table}] WHERE [{keyColumn}] = @keyValue";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@keyValue", keyValue);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении записи: {ex.Message}");
            }
        }
    }
}

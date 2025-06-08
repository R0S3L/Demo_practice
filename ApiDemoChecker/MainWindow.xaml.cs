using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;

namespace ApiDemoChecker
{
    public partial class MainWindow : Window
    {
        private string fullName = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = "http://localhost:4444/TransferSimulator/fullName";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string json = reader.ReadToEnd();

                    // Попытка найти значение по ключу "value"
                    Match match = Regex.Match(json, "\"value\"\\s*:\\s*\"([^\"]+)\"");
                    if (match.Success)
                    {
                        fullName = match.Groups[1].Value;
                        FullNameTextBlock.Text = fullName;
                    }
                    else
                    {
                        FullNameTextBlock.Text = "Ошибка парсинга JSON";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message);
            }
        }


        private async void ValidateData_Click(object sender, RoutedEventArgs e)
        {
            string result;
            bool isValid = ValidateFullName(fullName, out result);
            ResultTextBlock.Text = result;

            await Task.Run(() =>
            {
                SaveToWord("Получить данные", fullName);
                SaveToWord("Проверка ФИО", result);
            });
        }

        private bool ValidateFullName(string name, out string message)
        {
            if (Regex.IsMatch(name, @"[^а-яА-ЯёЁ\s\-]"))
            {
                message = "ФИО содержит запрещённые символы";
                return false;
            }

            if (Regex.IsMatch(name, @"\d"))
            {
                message = "ФИО содержит цифры";
                return false;
            }

            message = "ФИО корректно";
            return true;
        }

        private void SaveToWord(string action, string result)
        {
            string filePath = System.IO.Path.Combine(Environment.CurrentDirectory, "ТестКейс.docx");
            Word.Application wordApp = null;
            Word.Document doc = null;

            try
            {
                wordApp = new Word.Application();
                wordApp.Visible = false;

                if (File.Exists(filePath))
                {
                    doc = wordApp.Documents.Open(filePath);
                }
                else
                {
                    doc = wordApp.Documents.Add();

                    // Вставка заголовка таблицы с объединением ячеек
                    Word.Table table = doc.Tables.Add(doc.Range(0, 0), 3, 3);
                    table.Borders.Enable = 1;

                    // Объединение заголовка (1-я строка всех 3 ячеек)
                    table.Cell(1, 1).Merge(table.Cell(1, 3));
                    table.Cell(1, 1).Range.Text = "Проверка ФИО на запрещенные символы";
                    table.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorRed;
                    table.Cell(1, 1).Range.Font.Bold = 1;
                    table.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    // Вторая строка – названия столбцов
                    table.Cell(2, 1).Range.Text = "Действие";
                    table.Cell(2, 2).Range.Text = "Ожидаемый результат";
                    table.Cell(2, 3).Range.Text = "Результат\n(Успешно/Не успешно)";
                }

                Word.Table existingTable = doc.Tables[1];
                int rowIndex = existingTable.Rows.Count + 1;
                existingTable.Rows.Add();
                existingTable.Cell(rowIndex, 1).Range.Text = action;

                // Определяем ожидаемый результат и финальный результат
                string expected;
                string finalResult;

                if (action.Contains("Получить"))
                {
                    expected = $"ФИО успешно получено: {result}";
                    finalResult = string.IsNullOrWhiteSpace(result) ? "Не успешно" : "Успешно";
                }
                else if (action.Contains("Проверка"))
                {
                    expected = $"ФИО не содержит запрещенные символы\nПроверка: {result}";
                    finalResult = result.Contains("ФИО корректно") ? "Успешно" : "Не успешно";
                }
                else
                {
                    expected = result;
                    finalResult = "—";
                }

                existingTable.Cell(rowIndex, 2).Range.Text = expected;
                existingTable.Cell(rowIndex, 3).Range.Text = finalResult;

                doc.SaveAs2(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении Word: " + ex.Message);
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                }

                if (wordApp != null)
                {
                    wordApp.Quit(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }



    }
}

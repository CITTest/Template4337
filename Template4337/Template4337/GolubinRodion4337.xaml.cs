using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Template4337
{
    /// <summary>
    /// Логика взаимодействия для GolubinRodion4337.xaml
    /// </summary>
    public partial class GolubinRodion4337 : Window
    {
        public GolubinRodion4337()
        {
            InitializeComponent();
        }
        private void ImportExcel(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                DefaultExt = "*.xls; *xlsx",
                Title = "Выберите файлы excel для импорта в базу данных",
            };

            var result = openFileDialog.ShowDialog();

            if (!result.HasValue || !result.Value)
                return;

            var excelWork = new Excel.Application();
            var bookWork = excelWork.Workbooks.Open(openFileDialog.FileName);

            var bookWorkSheet = (Excel.Worksheet)bookWork.Sheets[1];
            var lastCell = bookWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);
            var columns = lastCell.Column;
            var rows = bookWorkSheet.Cells[bookWorkSheet.Rows.Count, 1].End(-4162).Row;

            var list = new string[rows, columns];

            for (var i = 0; i < columns; i++)
                for (var j = 0; j < rows; j++)
                    list[j, i] = bookWorkSheet.Cells[j + 1, i + 1].Text;

            var orders = new List<Order>();
            MessageBox.Show($"{rows}");

            for (var i = 1; i < rows; i++)
            {
                var tempOrder = new Order();

                tempOrder.OrderCode = list[i, 1];

                var date = list[i, 2].Split(new char[] { '.' });

                if (date.Length != 3)
                {

                    MessageBox.Show($"Длина даты: {date.Length}, {list[i, 2]}, {i}");
                    return;
                }

                int day, month, year;

                if (!int.TryParse(date[0], out day) || !int.TryParse(date[1], out month) || !int.TryParse(date[2], out year))
                {
                    MessageBox.Show("Ошибка парсинга для даты создания");
                    return;
                }


                tempOrder.DateCreate = new System.DateTime(year, month, day);

                var time = list[i, 3].Split(new char[] { ':' });

                if (time.Length != 2)
                {
                    MessageBox.Show($"Длина времени: {time.Length}");
                    return;

                }

                int hour, minute;

                if (!int.TryParse(time[0], out hour) || !int.TryParse(time[1], out minute))
                {
                    MessageBox.Show("ошибка парсинга времени");
                    return;

                }

                tempOrder.TimeCreate = new System.TimeSpan(hour, minute, 0);

                int clientCode;

                if (!int.TryParse(list[i, 4], out clientCode))
                {
                    MessageBox.Show("ошибка парсинга кода клиента");
                    return;
                }


                tempOrder.ClentCode = clientCode;
                tempOrder.Uslugi = list[i, 5];
                tempOrder.Status = list[i, 6];

                if (!string.IsNullOrEmpty(list[i, 7]))
                {
                    var dateEnd = list[i, 7].Split(new char[] { '.' });

                    int dayEnd, monthEnd, yearEnd;

                    if (!int.TryParse(dateEnd[0], out dayEnd) || !int.TryParse(dateEnd[1], out monthEnd)
                        || !int.TryParse(dateEnd[2], out yearEnd))
                    {
                        MessageBox.Show("Ошибка парсинга времени окончания");
                        return;
                    }


                    tempOrder.DateOfEnd = new System.DateTime(yearEnd, monthEnd, dayEnd);
                }
                else
                {
                    tempOrder.DateOfEnd = null;
                }

                tempOrder.TimeOfProcat = list[i, 8];

                orders.Add(tempOrder);
            }

            System.GC.Collect();

            try
            {
                using (var context = new isrpo3Context())
                {
                    context.Order.AddRange(orders);
                    context.SaveChanges();
                }

                MessageBox.Show($"Добавление в базу данных прошло успешно {orders.Count}");
            }
            catch
            {
                MessageBox.Show("Ошибка базы данных");
            }
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            const int idCol = 1;
            const int codeOrderCol = 2;
            const int dateOfCreateCol = 3;
            const int clientCode = 4;
            const int uslugiCol = 5;


            using (var context = new isrpo3Context())
            {
                var status = context.Order.GroupBy(p => p.Status).Select(p => p.Key).ToList();

                var app = new Excel.Application();
                app.SheetsInNewWorkbook = status.Count;
                var workbook = app.Workbooks.Add(Type.Missing);

                for (var i = 0; i < status.Count; i++)
                {
                    var worksheet = app.Worksheets.Item[i + 1];
                    worksheet.Name = status[i];

                    var startIndexRow = 2;

                    worksheet.Cells[idCol][1] = "Id";
                    worksheet.Cells[codeOrderCol][1] = "Код заказа";
                    worksheet.Cells[dateOfCreateCol][1] = "Дата создания";
                    worksheet.Cells[clientCode][1] = "Код клиента";
                    worksheet.Cells[uslugiCol][1] = "Услуги";

                    var orderThisStatus = context.Order.Where(p => p.Status == status[i]);
                    foreach (var item in orderThisStatus)
                    {
                        worksheet.Cells[idCol][startIndexRow] = item.Id;
                        worksheet.Cells[codeOrderCol][startIndexRow] = item.OrderCode;
                        worksheet.Cells[dateOfCreateCol][startIndexRow] = item.DateCreate.GetValueOrDefault().ToString();
                        worksheet.Cells[clientCode][startIndexRow] = item.ClentCode;
                        worksheet.Cells[uslugiCol][startIndexRow] = item.Uslugi;

                        startIndexRow++;
                    }
                }

                app.Visible = true;
            }
        }

        private async void ImportJson(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                DefaultExt = "*.json",
                Title = "Выберите файлы json для импорта в базу данных",
            };

            var result = openFileDialog.ShowDialog();

            if (!result.HasValue || !result.Value)
                return;

            var orders = new List<Order>();

            using (var fs = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate))
            {
                orders = await JsonSerializer.DeserializeAsync<List<Order>>(fs);
            }


            try
            {
                using (var context = new isrpo3Context())
                {
                    await context.Order.AddRangeAsync(orders);
                    await context.SaveChangesAsync();
                    MessageBox.Show("Импортировано в базу данных");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в доблавение в базу данных {ex.Message}");
            }
        }

        private void ExportWord(object sender, RoutedEventArgs e)
        {
            const int idCol = 1;
            const int codeOrderCol = 2;
            const int dateOfCreateCol = 3;
            const int clientCode = 4;
            const int uslugiCol = 5;

            using (var context = new isrpo3Context())
            {
                var status = context.Order.GroupBy(p => p.Status).Select(p => p.Key).ToList();

                var app = new Word.Application();
                var document = app.Documents.Add();

                foreach (var stat in status)
                {
                    var orderThisStatus = context.Order.Where(p => p.Status == stat);

                    var startIndexRow = 2;

                    var paragraph = document.Paragraphs.Add();
                    var range = paragraph.Range;
                    range.Text = stat;
                    range.InsertParagraphAfter();

                    var talbe = document.Paragraphs.Add();
                    var tableRange = talbe.Range;
                    var table = document.Tables.Add(tableRange, orderThisStatus.Count() + 1, 5);
                    table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    table.Cell(1, idCol).Range.Text = "ID";
                    table.Cell(1, codeOrderCol).Range.Text = "Код заказа";
                    table.Cell(1, dateOfCreateCol).Range.Text = "Дата создания";
                    table.Cell(1, clientCode).Range.Text = "Код клиета";
                    table.Cell(1, uslugiCol).Range.Text = "Услуги";

                    foreach (var item in orderThisStatus)
                    {
                        table.Cell(startIndexRow, idCol).Range.Text = Convert.ToString(item.Id);
                        table.Cell(startIndexRow, codeOrderCol).Range.Text = item.OrderCode;
                        table.Cell(startIndexRow, dateOfCreateCol).Range.Text = item.DateCreate.ToString();
                        table.Cell(startIndexRow, clientCode).Range.Text = Convert.ToString(item.ClentCode);
                        table.Cell(startIndexRow, uslugiCol).Range.Text = item.Uslugi;

                        startIndexRow++;
                    }

                    table.AllowAutoFit = true;
                    tableRange.InsertParagraphAfter();
                }

                app.Visible = true;
            }
        }
    }
}

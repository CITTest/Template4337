using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Template4337
{
    /// <summary>
    /// Логика взаимодействия для Yakovlev_4337.xaml
    /// </summary>
    public partial class Yakovlev_4337 : Window
    {
        public Yakovlev_4337()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Import

            var openFileDialog = new OpenFileDialog()
            {
                DefaultExt = "*.xls; *xlsx",
                Title = "Выберите файлы excel для импорта в базу данных",
            };

            var result = openFileDialog.ShowDialog();

            if (!result.HasValue || !result.Value)
                return;

            var excelWork = new Excel.Application();
            var ServWork = excelWork.Workbooks.Open(openFileDialog.FileName);

            var ServWorkSheet = (Excel.Worksheet)ServWork.Sheets[1];
            var lastCell = ServWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);
            var columns = lastCell.Column;
            var rows = ServWorkSheet.Cells[ServWorkSheet.Rows.Count, 1].End(-4162).Row;

            var list = new string[rows, columns];

            for (var i = 0; i < columns; i++)
                for (var j = 0; j < rows; j++)
                    list[j, i] = ServWorkSheet.Cells[j + 1, i + 1].Text;

            var Services = new List<class1>();
            MessageBox.Show($"{rows}");

            for (var i = 2; i < rows; i++)
            {
                var temp = new class1(list[i, 1], list[i, 2], list[i, 3], Convert.ToInt32(list[i, 4]));

               

                Services.Add(temp);
            }
            try
            {
                using (var context = new Context())
                {
                    context.Class1s.AddRange(Services);
                    context.SaveChanges();
                }
                MessageBox.Show($"Добавление в базу данных прошло успешно {Services.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка базы данных {ex.Message}");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            const int idCol = 1;
            const int Name = 2;
            const int View = 3;
            const int Code = 4;
            const int Price = 5;

            using (var context = new Context())
            {
                

                var status = context.Class1s.GroupBy(p => p.Group).Select(p => p.Key).ToList();
                int countsheet = status.Count;

                var app = new Excel.Application();
                app.SheetsInNewWorkbook = countsheet;
                var workbook = app.Workbooks.Add(Type.Missing);

                for (var i = 0; i < countsheet; i++)
                {
                    var worksheet = app.Worksheets.Item[i + 1];
                    worksheet.Name = "Page - " + i;

                    var startIndexRow = 2;

                    worksheet.Cells[idCol][1] = "Id";
                    worksheet.Cells[Name][1] = "Название";
                    worksheet.Cells[View][1] = "Вид услуги";
                    worksheet.Cells[Code][1] = "Код услуги";
                    worksheet.Cells[Price][1] = "Цена";

                    var orderThisStatus = context.Class1s.Where(p => p.Group == i);
                    foreach (var item in orderThisStatus)
                    {
                        worksheet.Cells[idCol][startIndexRow] = item.Id;
                        worksheet.Cells[Name][startIndexRow] = item.Name;
                        worksheet.Cells[View][startIndexRow] = item.View;
                        worksheet.Cells[Code][startIndexRow] = item.Code;
                        worksheet.Cells[Price][startIndexRow] = item.Price;

                        startIndexRow++;
                    }
                }

                app.Visible = true;
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                DefaultExt = "*.json",
                Title = "Выберите файлы json для импорта в базу данных",
            };

            var result = openFileDialog.ShowDialog();

            if (!result.HasValue || !result.Value)
                return;

            var serv = new List<class1>();

            using (var fs = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate))
            {

                serv = await JsonSerializer.DeserializeAsync<List<class1>>(fs);
            }

            using (var context = new Context())
            {
                for(int i = 0 ; i < serv.Count; i++)
                {
                    serv[i].checkGroup();
                }
                await context.Class1s.AddRangeAsync(serv);
                await context.SaveChangesAsync();
                MessageBox.Show("Импортировано в базу данных");
            }
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в доблавение в базу данных {ex.Message}");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            const int idCol = 1;
            const int Name = 2;
            const int View = 3;
            const int Code = 4;
            const int Price = 5;

            using (var context = new Context())
            {
                var status = context.Class1s.GroupBy(p => p.Group).Select(p => p.Key).ToList();

                var app = new Word.Application();
                var document = app.Documents.Add();

                foreach (var stat in status)
                {
                    var orderThisStatus = context.Class1s.Where(p => p.Group == stat);

                    var startIndexRow = 2;

                    var paragraph = document.Paragraphs.Add();
                    var range = paragraph.Range;
                    range.Text = Convert.ToString(stat);
                    range.InsertParagraphAfter();

                    var talbe = document.Paragraphs.Add();
                    var tableRange = talbe.Range;
                    var table = document.Tables.Add(tableRange, orderThisStatus.Count() + 1, 5);
                    table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    table.Cell(1, idCol).Range.Text = "ID";
                    table.Cell(1, Name).Range.Text = "Название";
                    table.Cell(1, View).Range.Text = "Вид";
                    table.Cell(1, Code).Range.Text = "Код";
                    table.Cell(1, Price).Range.Text = "Цена";

                    foreach (var item in orderThisStatus)
                    {
                        table.Cell(startIndexRow, idCol).Range.Text = Convert.ToString(item.Id);
                        table.Cell(startIndexRow, Name).Range.Text = item.Name;
                        table.Cell(startIndexRow, View).Range.Text = item.View;
                        table.Cell(startIndexRow, Code).Range.Text = item.Code;
                        table.Cell(startIndexRow, Price).Range.Text = Convert.ToString(item.Price);

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

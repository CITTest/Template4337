using Microsoft.Win32;
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
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.AxHost;

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

                var app = new Excel.Application();
                app.SheetsInNewWorkbook = 3;
                var workbook = app.Workbooks.Add(Type.Missing);

                for (var i = 0; i < 4; i++)
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }
    }
}

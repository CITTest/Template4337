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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;
using Xceed.Words.NET;
using Xceed.Document.NET;
using System.IO;
using System.Text.Json;
using Microsoft.Office.Interop.Access.Dao;


namespace Template4337
{
    /// <summary>
    /// Логика взаимодействия для Kuzmina_4337.xaml
    /// </summary>
    public partial class Kuzmina_4337 : Window
    {
        public Kuzmina_4337()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                DefaultExt = "*.xls;*.xlsx",
                Filter = "файл Excel (Spisok.xlsx)|*.xlsx",
                Title = "Выберите файл базы данных"
            };
            if (!(ofd.ShowDialog() == true))
                return;
            string[,] list;
            Excel.Application ObjWorkExcel = new Excel.Application();
            
            Excel.Workbook ObjWorkBook = ObjWorkExcel.Workbooks.Open(ofd.FileName); //
            Excel.Worksheet ObjWorkSheet = (Excel.Worksheet)ObjWorkBook.Sheets[1];
            var lastCell = ObjWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);
            int _columns = (int)lastCell.Column;
            int _rows = (int)lastCell.Row;
            list = new string[_rows, _columns];
            for (int j = 0; j < _columns; j++)
            {
                for (int i = 0; i < _rows; i++)
                {
                    list[i, j] = ObjWorkSheet.Cells[i + 1, j + 1].Text;
                }
            }
            ObjWorkBook.Close(false, Type.Missing, Type.Missing);
            ObjWorkExcel.Quit();
            GC.Collect();

            using (ISRPO1Entities isrpoEntities = new ISRPO1Entities())
            {
                for (int i = 0; i < _rows; i++)
                {
                    DateTime birthDate;
                    if (DateTime.TryParse(list[i, 2], out birthDate))
                    {
                        isrpoEntities.Users.Add(new Users()
                        {
                            FullName = list[i, 0],
                            ClientCode = list[i, 1],
                            ClientIndex = list[i, 3],
                            City = list[i, 4],
                            Street = list[i, 5],
                            House = list[i, 6],
                            Flat = list[i, 7],
                            Email = list[i, 8],
                            DataBirth = birthDate // Используем преобразованное значение
                        });
                    }
                    else
                    {
                        // Обработка ошибки, если значение не может быть преобразовано в дату
                        continue;
                    }
                }

                isrpoEntities.SaveChanges();
                this.Close();
                MessageBox.Show("Импорт завершен", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<Users> users;

            using (ISRPO1Entities isrpoEntities = new ISRPO1Entities())
            {
                users = isrpoEntities.Users.ToList().OrderBy(s => s.ClientCode).ToList();
            }
            var app = new Excel.Application();
            app.SheetsInNewWorkbook = 3; //
            Excel.Workbook workbook = app.Workbooks.Add(Type.Missing);

            Excel.Worksheet age20_29Sheet = app.Worksheets.Item[1];
            age20_29Sheet.Name = "20-29";

            Excel.Worksheet age30_39Sheet = app.Worksheets.Item[2];
            age30_39Sheet.Name = "30-39";

            Excel.Worksheet age40Sheet = app.Worksheets.Item[3];
            age40Sheet.Name = "40+";

            var groupedByAge = users.GroupBy(user =>
            {
                int age = DateTime.Today.Year - user.DataBirth.Year;
                // Если пользователь еще не имел дня рождения в текущем году, вычитаем единицу
                if (user.DataBirth.Date > DateTime.Today.AddYears(-age)) age--;

                if (age >= 20 && age <= 29)
                    return "20-29";
                else if (age >= 30 && age <= 39)
                    return "30-39";
                else if (age >= 40)
                    return "40+";
                else
                    return "Unknown";
            });

            foreach (var group in groupedByAge)
            {
                Excel.Worksheet worksheet = null;

                if (group.Key == "20-29")
                    worksheet = age20_29Sheet;
                else if (group.Key == "30-39")
                    worksheet = age30_39Sheet;
                else if (group.Key == "40+")
                    worksheet = age40Sheet;
                else
                    continue;

                int startRowIndex = 1;
                worksheet.Cells[1][startRowIndex] = "Код клиента";
                worksheet.Cells[2][startRowIndex] = "ФИО";
                worksheet.Cells[3][startRowIndex] = "Логин";
                startRowIndex++;
                foreach (Users user in group)
                {
                    worksheet.Cells[1][startRowIndex] = user.ClientCode;
                    worksheet.Cells[2][startRowIndex] = user.FullName;
                    worksheet.Cells[3][startRowIndex] = user.Email;
                    startRowIndex++;
                }

                worksheet.Columns.AutoFit();
            }

            app.Visible = true;
            this.Close();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                DefaultExt = "*.json",
                Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*",
                Title = "Выберите файл JSON для добавления в базу данных"
            };
            if (!(openFileDialog.ShowDialog() == true))
                return;
            {
                string jsonFilePath = openFileDialog.FileName;
                List<Users2> users2 = JsonSerializer.Deserialize<List<Users2>>(File.ReadAllText(jsonFilePath));
                using (ISRPOJSONEntities isrpoEntities = new ISRPOJSONEntities())
                {
                    foreach (var Users2 in users2)
                    {
                        isrpoEntities.Users2.Add(new Users2()
                        {
                            IdClient = Users2.IdClient,
                            FullName = Users2.FullName,
                            ClientCode = Users2.ClientCode,
                            ClientIndex = Users2.ClientIndex,
                            City = Users2.City,
                            Street = Users2.Street,
                            House = Users2.House,
                            Flat = Users2.Flat,
                            Email = Users2.Email,
                            DataBirth = Users2.DataBirth
                        });
                    }
                    isrpoEntities.SaveChanges(); //
                    this.Close();
                    MessageBox.Show("Импорт завершен", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = "*.docx",
                Filter = "Документ Word (*.docx)|*.docx|Все файлы (*.*)|*.*",
                Title = "Выберите место сохранения файла Word"
            };

            if (sfd.ShowDialog() == true)
            {
                string outputFilePath = sfd.FileName;

                using (DocX document = DocX.Create(outputFilePath))
                {
                    using (ISRPOJSONEntities isrpoEntities = new ISRPOJSONEntities())
                    {
                        var allUsers = isrpoEntities.Users2.OrderBy(x => x.DataBirth).ToList();

                        var age20_29Users = allUsers.Where(x => CalculateAge(x.DataBirth) >= 20 && CalculateAge(x.DataBirth) <= 29).ToList();
                        var age30_39Users = allUsers.Where(x => CalculateAge(x.DataBirth) >= 30 && CalculateAge(x.DataBirth) <= 39).ToList();
                        var age40PlusUsers = allUsers.Where(x => CalculateAge(x.DataBirth) >= 40).ToList();

                        InsertDataIntoWordSheet(document, age20_29Users, "Возраст 20-29");
                        InsertDataIntoWordSheet(document, age30_39Users, "Возраст 30-39");
                        InsertDataIntoWordSheet(document, age40PlusUsers, "Возраст 40+");
                    }

                    document.Save();
                }
                this.Close();
                MessageBox.Show("Данные успешно сохранены в файл Word.");
            }

           
        }
        private int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                age--;
            return age;
        }

        private void InsertDataIntoWordSheet(DocX document, List<Users2> data, string sheetTitle)
        {
            if (data.Count == 0)
                return;

            document.InsertParagraph($"{sheetTitle}").FontSize(14).Bold().Alignment = Alignment.center;

            foreach (var item in data)
            {
                document.InsertParagraph($"Код клиента: {item.ClientCode}, ФИО: {item.FullName}, Email: {item.Email}")
                        .FontSize(12).Alignment = Alignment.left;
            }
            document.InsertSectionPageBreak();
        }
    } 
}


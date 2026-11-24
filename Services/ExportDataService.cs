using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace college_events_desktop.Services
{
    public class ExportDataService
    {
        /// <summary>
        /// Экспортирует данные в Excel
        /// </summary>
        /// <param name="filePath">Путь сохранения (например, C:\temp\report.xlsx)</param>
        /// <param name="headerData">Данные "ключ-значение" (шапка документа)</param>
        /// <param name="table">Таблица с данными</param>
        public bool Export(string filePath, List<KeyValuePair<string, string>> headerData, ExportTable table)
        {
            //проверка аргументов
            if (headerData == null && table == null)
                throw new ArgumentException("Нет данных для экспорта.");

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Не указан путь к файлу.");

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Отчет");
                    int currentRow = 1;

                    //запись шапки (Список ключ-значение)
                    if (headerData != null && headerData.Count > 0)
                    {
                        foreach (var item in headerData)
                        {
                            worksheet.Cell(currentRow, 1).Value = item.Key;
                            worksheet.Cell(currentRow, 1).Style.Font.Bold = true; // Ключ жирным
                            worksheet.Cell(currentRow, 2).Value = item.Value;
                            currentRow++;
                        }
                        currentRow++; //пустая строка между шапкой и таблицей
                    }

                    //запись таблицы
                    if (table != null && table.Columns != null && table.Columns.Count > 0)
                    {
                        int startTableRow = currentRow;

                        //заголовки таблицы
                        for (int col = 0; col < table.Columns.Count; col++)
                        {
                            var cell = worksheet.Cell(currentRow, col + 1);
                            cell.Value = table.Columns[col].Header;
                            cell.Style.Font.Bold = true;
                            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        currentRow++;

                        //данные таблицы
                        if (table.Rows != null)
                        {
                            foreach (var rowObj in table.Rows)
                            {
                                for (int col = 0; col < table.Columns.Count; col++)
                                {
                                    var cell = worksheet.Cell(currentRow, col + 1);
                                    //безопасно вызываем селектор
                                    try
                                    {
                                        cell.Value = table.Columns[col].ValueSelector(rowObj) ?? "";
                                    }
                                    catch
                                    {
                                        cell.Value = "Ошибка";
                                    }

                                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                }
                                currentRow++;
                            }
                        }

                        //автоширина колонок
                        worksheet.Columns().AdjustToContents();
                    }

                    workbook.SaveAs(filePath);
                }
                return true;
            }
            catch (Exception)
            {
                return false; // Или пробрасываем исключение наверх для логирования
            }
        }
    }

    // Вспомогательные классы для описания таблицы
    public class ExportColumn
    {
        public string Header { get; set; }
        public Func<object, string> ValueSelector { get; set; }
    }

    public class ExportTable
    {
        public List<ExportColumn> Columns { get; set; } = new List<ExportColumn>();
        public List<object> Rows { get; set; } = new List<object>();
    }
}
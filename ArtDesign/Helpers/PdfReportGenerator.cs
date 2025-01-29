using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArtDesign.Model;
using ArtDesign.ViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ArtDesign.Services
{
    public class PdfReportGenerator
    {
        public void GenerateReport(
            DateTime? dateFrom,
            DateTime? dateTo,
            List<Project> projects,
            List<Employee> employees,
            List<Client> clients,
            List<Status> statuses,
            List<WorkStage> workStages,
            string filePath,
            ReportTypeEnum reportType) // New parameter
        {
            // Создаем директорию, если не существует
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Инициализируем документ
            Document document = new Document(PageSize.A4, 50, 50, 50, 50);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                // Указываем путь к файлу шрифта
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
                if (!File.Exists(fontPath))
                {
                    throw new FileNotFoundException("Шрифт Arial не найден по пути: " + fontPath);
                }

                // Создаем BaseFont с поддержкой кириллицы
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                // Создаем Font объекты с нужным размером
                Font headerFont = new Font(baseFont, 16, Font.BOLD);
                Font subHeaderFont = new Font(baseFont, 14, Font.BOLD);
                Font tableHeaderFont = new Font(baseFont, 12, Font.BOLD);
                Font tableFont = new Font(baseFont, 12, Font.NORMAL);

                // Заголовок отчета
                Paragraph reportTitle = new Paragraph("Отчет о деятельности компании", headerFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(reportTitle);

                // Генерация разделов в зависимости от типа отчета
                switch (reportType)
                {
                    case ReportTypeEnum.CompletedProjects:
                        ComposeCompletedProjectsSection(document, dateFrom, dateTo, projects, statuses, workStages, tableHeaderFont, tableFont);
                        break;

                    case ReportTypeEnum.EmployeeWorkload:
                        ComposeEmployeeWorkloadSection(document, employees, workStages, tableHeaderFont, tableFont);
                        break;

                    case ReportTypeEnum.ClientOrderHistory:
                        ComposeClientOrderHistorySection(document, clients, projects, statuses, tableHeaderFont, tableFont);
                        break;

                    default:
                        throw new ArgumentException("Неизвестный тип отчета.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при генерации отчета: " + ex.Message);
            }
            finally
            {
                document.Close();
            }
        }

        private void ComposeCompletedProjectsSection(Document document, DateTime? dateFrom, DateTime? dateTo, List<Project> projects, List<Status> statuses, List<WorkStage> workStages, Font tableHeaderFont, Font tableFont)
        {
            // Фильтруем проекты по дате завершения
            var completedProjects = projects.FindAll(p =>
                (!dateFrom.HasValue || p.EndDate >= dateFrom.Value) &&
                (!dateTo.HasValue || p.EndDate <= dateTo.Value));

            int count = completedProjects.Count;
            string periodText = "за весь период";
            if (dateFrom.HasValue && dateTo.HasValue)
                periodText = $"с {dateFrom.Value:dd.MM.yyyy} по {dateTo.Value:dd.MM.yyyy}";
            else if (dateFrom.HasValue)
                periodText = $"с {dateFrom.Value:dd.MM.yyyy}";
            else if (dateTo.HasValue)
                periodText = $"до {dateTo.Value:dd.MM.yyyy}";

            // Заголовок раздела
            Paragraph sectionHeader = new Paragraph($"Количество выполненных проектов {periodText}: {count}", tableHeaderFont)
            {
                SpacingAfter = 10
            };
            document.Add(sectionHeader);

            if (count > 0)
            {
                // Создаем таблицу с 4 столбцами
                PdfPTable table = new PdfPTable(4)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10,
                    SpacingAfter = 20
                };
                table.SetWidths(new float[] { 3f, 2f, 2f, 2f });

                // Добавляем заголовки таблицы
                AddCellToHeader(table, "Название проекта", tableHeaderFont);
                AddCellToHeader(table, "Сроки", tableHeaderFont);
                AddCellToHeader(table, "Статус", tableHeaderFont);
                AddCellToHeader(table, "Завершенные этапы", tableHeaderFont);

                // Добавляем данные
                foreach (var project in completedProjects)
                {
                    AddCellToBody(table, project.ProjectName, tableFont);
                    string period = $"{project.StartDate:dd.MM.yyyy} - {project.EndDate:dd.MM.yyyy}";
                    AddCellToBody(table, period, tableFont);

                    var status = statuses.Find(s => s.ID == project.StatusId)?.StatusName ?? "Неизвестен";
                    AddCellToBody(table, status, tableFont);

                    int completedStages = workStages.FindAll(ws => ws.ProjectId == project.ID && ws.EndDate <= DateTime.Now).Count;
                    AddCellToBody(table, completedStages.ToString(), tableFont);
                }

                document.Add(table);
            }
        }

        private void ComposeEmployeeWorkloadSection(Document document, List<Employee> employees, List<WorkStage> workStages, Font tableHeaderFont, Font tableFont)
        {
            // Заголовок раздела
            Paragraph sectionHeader = new Paragraph("Загруженность сотрудников", tableHeaderFont)
            {
                SpacingAfter = 10
            };
            document.Add(sectionHeader);

            if (employees.Count > 0)
            {
                // Создаем таблицу с 3 столбцами
                PdfPTable table = new PdfPTable(3)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10,
                    SpacingAfter = 20
                };
                table.SetWidths(new float[] { 3f, 2f, 2f });

                // Добавляем заголовки таблицы
                AddCellToHeader(table, "Сотрудник", tableHeaderFont);
                AddCellToHeader(table, "Текущие проекты", tableHeaderFont);
                AddCellToHeader(table, "Текущие этапы", tableHeaderFont);

                // Добавляем данные
                foreach (var employee in employees)
                {
                    string fullName = $"{employee.Name} {employee.Surname}";
                    AddCellToBody(table, fullName, tableFont);

                    var ongoingStages = workStages.FindAll(ws => ws.EmployeeId == employee.ID && ws.EndDate > DateTime.Now);
                    int currentProjects = 0;
                    if (ongoingStages.Count > 0)
                    {
                        currentProjects = ongoingStages
                            .Select(ws => ws.ProjectId)
                            .Distinct()
                            .Count();
                    }

                    AddCellToBody(table, currentProjects.ToString(), tableFont);
                    AddCellToBody(table, ongoingStages.Count.ToString(), tableFont);
                }

                document.Add(table);
            }
            else
            {
                Paragraph noData = new Paragraph("Нет данных для отображения.", tableFont)
                {
                    SpacingAfter = 20
                };
                document.Add(noData);
            }
        }

        private void ComposeClientOrderHistorySection(Document document, List<Client> clients, List<Project> projects, List<Status> statuses, Font tableHeaderFont, Font tableFont)
        {
            // Заголовок раздела
            Paragraph sectionHeader = new Paragraph("История заказов по клиентам", tableHeaderFont)
            {
                SpacingAfter = 10
            };
            document.Add(sectionHeader);

            if (clients.Count > 0)
            {
                foreach (var client in clients)
                {
                    var clientProjects = projects.FindAll(p => p.ClientId == client.ID);
                    if (clientProjects.Count == 0)
                        continue;

                    // Заголовок клиента
                    Paragraph clientHeader = new Paragraph($"Клиент: {client.Name} {client.Surname} ({client.NameCompany})", tableHeaderFont)
                    {
                        SpacingBefore = 10,
                        SpacingAfter = 5
                    };
                    document.Add(clientHeader);

                    // Создаем таблицу с 4 столбцами
                    PdfPTable table = new PdfPTable(4)
                    {
                        WidthPercentage = 100,
                        SpacingBefore = 5,
                        SpacingAfter = 15
                    };
                    table.SetWidths(new float[] { 3f, 2f, 2f, 2f });

                    // Добавляем заголовки таблицы
                    AddCellToHeader(table, "Название проекта", tableHeaderFont);
                    AddCellToHeader(table, "Статус", tableHeaderFont);
                    AddCellToHeader(table, "Сроки начала", tableHeaderFont);
                    AddCellToHeader(table, "Сроки окончания", tableHeaderFont);

                    // Добавляем данные
                    foreach (var project in clientProjects)
                    {
                        AddCellToBody(table, project.ProjectName, tableFont);

                        var status = statuses.Find(s => s.ID == project.StatusId)?.StatusName ?? "Неизвестен";
                        AddCellToBody(table, status, tableFont);

                        AddCellToBody(table, project.StartDate.ToString("dd.MM.yyyy"), tableFont);
                        AddCellToBody(table, project.EndDate.ToString("dd.MM.yyyy"), tableFont);
                    }

                    document.Add(table);
                }
            }
            else
            {
                Paragraph noData = new Paragraph("Нет данных для отображения.", tableFont)
                {
                    SpacingAfter = 20
                };
                document.Add(noData);
            }
        }

        private void AddCellToHeader(PdfPTable table, string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            table.AddCell(cell);
        }

        private void AddCellToBody(PdfPTable table, string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            table.AddCell(cell);
        }
    }
}

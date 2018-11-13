using System.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using System.Windows.Documents;
using System.Windows.Media;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace VIPAR
{
    /// <summary>
    /// Interaction logic for TeamFormationControl.xaml
    /// </summary>
    public partial class KickOffReport : System.Windows.Controls.UserControl
    {
        DateTime date;
        float POAverage = 0.0f;
        float SMAverage = 0.0f;
        string selectedDirectoryPath;
        public KickOffReport()
        {
            InitializeComponent();
        }
        private bool HasErrors()
        {
            bool hasError = false;
            if (string.IsNullOrEmpty(ProjectInfoControl.AuditorsNameTextBox.Text))
            {
                ProjectInfoControl.AuditorsNameTextBox.Background = Brushes.Salmon;
                hasError = true;
            }
            if (string.IsNullOrEmpty(ProjectInfoControl.ProjectNameTextBox.Text))
            {
                ProjectInfoControl.ProjectNameTextBox.Background = Brushes.Salmon;
                hasError = true;
            }
            if (string.IsNullOrEmpty(ProjectInfoControl.ProgramNameTextBox.Text))
            {
                ProjectInfoControl.ProgramNameTextBox.Background = Brushes.Salmon;
                hasError = true;
            }
            if (ProjectInfoControl.StartDatePicker.SelectedDate == null)
            {
                ProjectInfoControl.StartDatePicker.Background = Brushes.Salmon;
                hasError = true;
            }
            if (ProjectInfoControl.EndDatePicker.SelectedDate == null)
            {
                ProjectInfoControl.EndDatePicker.Background = Brushes.Salmon;
                hasError = true;
            }
            return hasError;
        }

        private Application OpenNewExcelFile()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Please select the directory you'd like to export to.";
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    selectedDirectoryPath = fbd.SelectedPath;
            }
            Application app = new Application();
            app.Visible = false;
            return app;

        }
        private void ExportClicked(object sender, RoutedEventArgs e)
        {
            if (HasErrors())
                return;

            var app = OpenNewExcelFile();

            Workbook wb = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet ws = wb.Worksheets[1];
            ws.Name = "Team Formation";
            Worksheet ws2 = wb.Sheets.Add() as Worksheet;
            ws2.Name = "Project Info";
            Worksheet ws3 = wb.Sheets.Add() as Worksheet;
            ws3.Name = "Audit Summary";
            
            PrintPOSheet(ws);
            PrintProjectInfoSheet(ws2);
            PrintAuditSummary(ws3);

            date = DateTime.Now;
            string month = date.Month.ToString();
            if (month.Length == 1)
                month = "0" + month;
            string day = date.Day.ToString();
            if (day.Length == 1)
                day = "0" + day;
            string year = date.Year.ToString();

            wb.SaveAs(selectedDirectoryPath + string.Format("\\{0}_{1}{2}{3}.xlsx",
                ProjectInfoControl.ProjectNameTextBox.Text.Replace(" ", ""), month,
                day, year));

            wb.Close();

            System.Windows.Window.GetWindow(this).Close();
        }

        void PrintAuditSummary(Worksheet ws)
        {
            ws.Columns["A:A"].ColumnWidth = 40;
            ws.Columns["A:A"].Style.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            ws.Columns["B:B"].ColumnWidth = 10;
            ws.Columns["B:B"].Style.HorizontalAlignment = XlHAlign.xlHAlignLeft;

            ws.Range["A1"].Value = "Team Foundation Score Average";
            ws.Range["B1"].Value = string.Format("{0:N1}", POAverage);
            ws.Range["A2"].Value = "Product Ownership Score Average";
            ws.Range["B2"].Value = string.Format("{0:N1}", 3.2f);
            ws.Range["A3"].Value = "Processes and Practices Score Average";
            ws.Range["B3"].Value = string.Format("{0:N1}", 2.8f);
            ws.Range["A4"].Value = "Engineering Practices Score Average";
            ws.Range["B4"].Value = string.Format("{0:N1}", 4.1f);
            ws.Range["A5"].Value = "VIP Compliance Score Average";
            ws.Range["B5"].Value = string.Format("{0:N1}", 1.8f);
        }

        // returns the rating value. Returns zero if no rating was set.
        float PrintRating(Worksheet ws, int rowNum, TextBlock tb, System.Windows.Controls.ComboBox cb, System.Windows.Controls.RichTextBox rtb)
        {
            ws.Range["A" + rowNum.ToString()].Value = tb.Text;

            if(string.IsNullOrEmpty(cb.Text))
                ws.Range["B" + rowNum.ToString()].Value = "No rating was given.";
            else
                ws.Range["B" + rowNum.ToString()].Value = cb.Text;

            ws.Range["C" + rowNum.ToString()].Value = new TextRange(rtb.Document.ContentStart,
                rtb.Document.ContentEnd).Text;

            return string.IsNullOrEmpty(cb.Text) ? 0 : float.Parse(cb.Text.Remove(1));
        }

        void PrintPOSheet(Worksheet ws)
        {
            ws.Columns["A:A"].ColumnWidth = 35;
            ws.Columns["A:A"].Style.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            ws.Columns["B:B"].ColumnWidth = 70;
            ws.Columns["B:B"].Style.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            ws.Columns["C:C"].ColumnWidth = 100;
            ws.Columns["C:C"].Style.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            ws.Columns["C:C"].Style.VerticalAlignment = XlVAlign.xlVAlignCenter;
            ws.Rows["1:20"].RowHeight = 40;

            POAverage += PrintRating(ws, 1, TeamFormationControl.Question1, TeamFormationControl.Rating1, TeamFormationControl.RichTextBox1);
            POAverage += PrintRating(ws, 2, TeamFormationControl.Question2, TeamFormationControl.Rating2, TeamFormationControl.RichTextBox2);
            POAverage += PrintRating(ws, 3, TeamFormationControl.Question3, TeamFormationControl.Rating3, TeamFormationControl.RichTextBox3);
            POAverage += PrintRating(ws, 4, TeamFormationControl.Question4, TeamFormationControl.Rating4, TeamFormationControl.RichTextBox4);
            POAverage += PrintRating(ws, 5, TeamFormationControl.Question5, TeamFormationControl.Rating5, TeamFormationControl.RichTextBox5);
            POAverage += PrintRating(ws, 6, TeamFormationControl.Question6, TeamFormationControl.Rating6, TeamFormationControl.RichTextBox6);
            POAverage += PrintRating(ws, 7, TeamFormationControl.Question7, TeamFormationControl.Rating7, TeamFormationControl.RichTextBox7);
            POAverage += PrintRating(ws, 8, TeamFormationControl.Question8, TeamFormationControl.Rating8, TeamFormationControl.RichTextBox8);
            POAverage += PrintRating(ws, 9, TeamFormationControl.Question9, TeamFormationControl.Rating9, TeamFormationControl.RichTextBox9);
            POAverage += PrintRating(ws, 10, TeamFormationControl.Question10, TeamFormationControl.Rating10, TeamFormationControl.RichTextBox10);
            POAverage += PrintRating(ws, 11, TeamFormationControl.Question11, TeamFormationControl.Rating11, TeamFormationControl.RichTextBox11);

            POAverage = POAverage / 11;
        }

        void PrintProjectInfoSheet(Worksheet ws)
        {
            ws.Columns["A:A"].ColumnWidth = 40;
            ws.Columns["A:A"].Style.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            ws.Columns["B:B"].ColumnWidth = 40;
            ws.Columns["B:B"].Style.HorizontalAlignment = XlHAlign.xlHAlignLeft;

            ws.Range["A1"].Value = ProjectInfoControl.ProjectInfoQ1.Text;
            ws.Range["B1"].Value = ProjectInfoControl.AuditorsNameTextBox.Text;

            ws.Range["A2"].Value = ProjectInfoControl.ProjectInfoQ2.Text;
            ws.Range["B2"].Value = ProjectInfoControl.ProjectNameTextBox.Text;

            ws.Range["A3"].Value = ProjectInfoControl.ProjectInfoQ3.Text;
            ws.Range["B3"].Value = ProjectInfoControl.ProgramNameTextBox.Text;

            ws.Range["A4"].Value = ProjectInfoControl.ProjectInfoQ4.Text;
            ws.Range["B4"].Value = (string)ProjectInfoControl.CurrentPhaseComboBox.SelectionBoxItem;

            ws.Range["A5"].Value = ProjectInfoControl.ProjectInfoQ5.Text;
            ws.Range["B5"].Value = ((DateTime)ProjectInfoControl.StartDatePicker.SelectedDate).ToShortDateString();

            ws.Range["A6"].Value = ProjectInfoControl.ProjectInfoQ6.Text;
            ws.Range["B6"].Value = ((DateTime)ProjectInfoControl.EndDatePicker.SelectedDate).ToShortDateString();

            ws.Range["A7"].Value = ProjectInfoControl.ProjectInfoQ7.Text;
            ws.Range["B7"].Value = ProjectInfoControl.ProjectManagerTextBox.Text;

            ws.Range["A8"].Value = ProjectInfoControl.ProjectInfoQ8.Text;
            ws.Range["B8"].Value = ProjectInfoControl.ProgramManagerTextBox.Text;

            ws.Range["A9"].Value = ProjectInfoControl.ProjectInfoQ9.Text;
            ws.Range["B9"].Value = ProjectInfoControl.PortfolioDirectorTextBox.Text;

            ws.Range["A10"].Value = ProjectInfoControl.ProjectInfoQ10.Text;
            ws.Range["B10"].Value = ProjectInfoControl.EPMOProgramTextBox.Text;

            ws.Range["A11"].Value = ProjectInfoControl.ProjectInfoQ11.Text;
            ws.Range["B11"].Value = ProjectInfoControl.ProductOwnerTextBox.Text;

            ws.Range["A12"].Value = ProjectInfoControl.ProjectInfoQ12.Text;
            ws.Range["B12"].Value = ProjectInfoControl.ConfigManagerTextBox.Text;

            ws.Range["A13"].Value = ProjectInfoControl.ProjectInfoQ13.Text;
            ws.Range["B13"].Value = ProjectInfoControl.ESORepTextBox.Text;

            ws.Range["A14"].Value = ProjectInfoControl.ProjectInfoQ14.Text;
            ws.Range["B14"].Value = ProjectInfoControl.ReleaseAgentTextBox.Text;

            ws.Range["A15"].Value = ProjectInfoControl.ProjectInfoQ15.Text;
            ws.Range["B15"].Value = ProjectInfoControl.ScrumMasterTextBox.Text;

            ws.Range["A16"].Value = ProjectInfoControl.ProjectInfoQ16.Text;
            ws.Range["B16"].Value = ProjectInfoControl.FiveZeroEightRepTextBox.Text;

            ws.Range["A17"].Value = ProjectInfoControl.ProjectInfoQ17.Text;
            ws.Range["B17"].Value = ProjectInfoControl.ReceivingOrgTextBox.Text;

            ws.Range["A18"].Value = ProjectInfoControl.ProjectInfoQ18.Text;
            ws.Range["B18"].Value = ProjectInfoControl.ProjectTeamSizeTextBox.Text;

            ws.Range["A19"].Value = ProjectInfoControl.ProjectInfoQ19.Text;
            ws.Range["B19"].Value = ProjectInfoControl.ContractorsTextBox.Text;

            ws.Range["A20"].Value = ProjectInfoControl.ProjectInfoQ20.Text;
            ws.Range["B20"].Value = (string)ProjectInfoControl.TMSTrainingComboBox.SelectionBoxItem;

            ws.Range["A21"].Value = ProjectInfoControl.ProjectInfoQ21.Text;
            ws.Range["B21"].Value = (string)ProjectInfoControl.CD1ComboBox.SelectionBoxItem;

            ws.Range["A22"].Value = ProjectInfoControl.ProjectInfoQ22.Text;
            ws.Range["B22"].Value = (string)ProjectInfoControl.CD2ComboBox.SelectionBoxItem;
        }
    }
}

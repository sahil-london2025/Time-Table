using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Drawing.Printing;
using System.Threading.Tasks;
using System.Reflection;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private List<ExcelFileData> loadedFiles = new List<ExcelFileData>();
        private int currentFileIndex = 0;
        private ReportDocument reportDocument;
        private ProgressBar progressBar1;

        // Class to store Excel file data
        private class ExcelFileData
        {
            public string FilePath { get; set; }
            public DataTable DataTable { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx",
                Title = "Select Excel Files",
                Multiselect = true // Allow multiple file selection
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Clear previously loaded files
                loadedFiles.Clear();

                // Show all selected files in the textbox
                txtFilePath.Text = string.Join("; ", openFileDialog.FileNames);

                // Configure the progress bar
                progressBar1.Minimum = 0;
                progressBar1.Maximum = openFileDialog.FileNames.Length;
                progressBar1.Value = 0;
                progressBar1.Visible = true;

                // Load each file
                foreach (string filePath in openFileDialog.FileNames)
                {
                    await LoadExcelFileAsync(filePath);
                    progressBar1.Value++;
                    Application.DoEvents();
                }

                progressBar1.Visible = false;

                // Set current file to first one
                currentFileIndex = 0;

                // Load first file's data into dataTable for existing logic
                if (loadedFiles.Count > 0)
                {
                    dataTable = loadedFiles[0].DataTable;
                    UpdateFileIndicator();

                    // Auto-generate report for the first file
                    btnGenerateReport_Click(null, EventArgs.Empty);
                }

                MessageBox.Show($"Loaded {loadedFiles.Count} Excel files successfully.");
            }
        }

        private async Task LoadExcelFileAsync(string filePath)
        {
            try
            {
                string fileExtension = Path.GetExtension(filePath);
                string connectionString;

                if (fileExtension.Equals(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1;\"";
                }
                else if (fileExtension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1;\"";
                }
                else
                {
                    MessageBox.Show("Invalid file format. Please select an Excel file.");
                    return;
                }

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    await Task.Run(() => connection.Open());

                    DataTable sheetTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (sheetTable == null || sheetTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No sheets found in the Excel file.");
                        return;
                    }

                    string sheetName = sheetTable.Rows[0]["TABLE_NAME"].ToString();
                    string query = $"SELECT * FROM [{sheetName}]";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);

                    DataTable fileDataTable = new DataTable();
                    await Task.Run(() => adapter.Fill(fileDataTable));

                    if (fileDataTable.Rows.Count == 0)
                    {
                        MessageBox.Show($"No data found in {Path.GetFileName(filePath)}.");
                        return;
                    }

                    if (fileDataTable.Columns.Contains("Date"))
                    {
                        foreach (DataRow row in fileDataTable.Rows)
                        {
                            if (row["Date"] != DBNull.Value)
                            {
                                if (row["Date"] is string dateStr && DateTime.TryParse(dateStr, out DateTime parsedDate))
                                {
                                    row["Date"] = parsedDate.Date;
                                }
                                else if (row["Date"] is DateTime dt)
                                {
                                    row["Date"] = dt.Date;
                                }
                                else
                                {
                                    row["Date"] = DBNull.Value;
                                }
                            }
                        }

                        DataView dataView = new DataView(fileDataTable);
                        dataView.Sort = "Date ASC, [Start Time] ASC, Department ASC";
                        fileDataTable = dataView.ToTable();
                    }

                    // Store the file data
                    loadedFiles.Add(new ExcelFileData
                    {
                        FilePath = filePath,
                        DataTable = fileDataTable
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }

        private DataTable dataTable;

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            if (loadedFiles.Count == 0)
            {
                MessageBox.Show("Please load an Excel file first.");
                return;
            }

            try
            {
                if (reportDocument != null)
                {
                    reportDocument.Close();
                    reportDocument.Dispose();
                }

                // Use the current file's data
                dataTable = loadedFiles[currentFileIndex].DataTable.Copy();

                // Process data (your existing code...)
                RenameColumn("Department", "University");
                RenameColumn("Staff", "Lecturer");
                RenameColumn("Modules", "Module");
                RenameColumn("Room", "Class_Room");
                RenameColumn("Summary", "Group");

                ProcessClassRoomNumbers();
                ProcessGroupField();
                ProcessModuleField();

                // Add Time column (your existing code...)
                if (!dataTable.Columns.Contains("Time"))
                    dataTable.Columns.Add("Time", typeof(string));

                foreach (DataRow row in dataTable.Rows)
                {
                    if (dataTable.Columns.Contains("Start Time") && dataTable.Columns.Contains("End Time"))
                    {
                        row["Time"] = $"{row["Start Time"]} - {row["End Time"]}";
                    }
                }

                ValidateColumns();

                reportDocument = new ReportDocument();

                // Modified report path loading:
                string reportName = "CrystalReport1.rpt";
                string reportPath = Path.Combine(Application.StartupPath, reportName);

                if (!File.Exists(reportPath))
                {
                    reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportName);

                    if (!File.Exists(reportPath))
                    {
                        //Last Try: embedded resource (requires the file to be added as embedded resource)
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        string resourceName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(reportName));
                        if (!string.IsNullOrEmpty(resourceName))
                        {
                            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                            {
                                // Save the stream to a temporary file
                                string tempReportPath = Path.GetTempFileName() + ".rpt";
                                using (FileStream fileStream = File.Create(tempReportPath))
                                {
                                    stream.CopyTo(fileStream);
                                }

                                // Load the report from the temporary file
                                reportDocument.Load(tempReportPath);

                                // Optionally, delete the temporary file after loading
                                // File.Delete(tempReportPath);  // Consider keeping it for debugging

                                goto ReportLoaded;
                            }

                        }
                        else
                        {

                            // Prompt user to locate the report file if not found in standard locations
                            using (OpenFileDialog dialog = new OpenFileDialog())
                            {
                                dialog.Filter = "Crystal Report Files|*.rpt";
                                dialog.Title = "Locate Crystal Report File";
                                MessageBox.Show("The report file could not be found. Please browse to locate it.",
                                    "Report File Missing", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                if (dialog.ShowDialog() == DialogResult.OK)
                                {
                                    reportPath = dialog.FileName;
                                }
                                else
                                {
                                    throw new FileNotFoundException("Could not locate the report file.");
                                }
                            }
                        }
                    }
                }
                reportDocument.Load(reportPath);

            ReportLoaded:

                reportDocument.SetDataSource(dataTable);

                crystalReportViewer1.ReportSource = reportDocument;
                crystalReportViewer1.Refresh();

                string currentFileName = Path.GetFileName(loadedFiles[currentFileIndex].FilePath);
                UpdateFileIndicator();

                // Only show message if triggered by user click, not during initialization
                if (sender != null)
                {
                    MessageBox.Show($"Report Generated Successfully for {currentFileName}!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void RenameColumn(string oldName, string newName)
        {
            if (dataTable.Columns.Contains(oldName))
                dataTable.Columns[oldName].ColumnName = newName;
        }

        private void ProcessClassRoomNumbers()
        {
            if (!dataTable.Columns.Contains("Class_Room")) return;

            foreach (DataRow row in dataTable.Rows)
            {
                if (row["Class_Room"] != DBNull.Value)
                {
                    string classRoom = row["Class_Room"].ToString().Trim();
                    Match match = Regex.Match(classRoom, @"\d+[A-Za-z]*");
                    row["Class_Room"] = match.Success ? match.Value : classRoom;
                }
            }
        }


        private void ProcessGroupField()
        {
            if (!dataTable.Columns.Contains("Group")) return;

            foreach (DataRow row in dataTable.Rows)
            {
                if (row["Group"] != DBNull.Value)
                {
                    string groupValue = row["Group"].ToString().Trim();
                    row["Group"] = groupValue.Contains("_") ? groupValue.Split('_')[0] : groupValue;
                }
            }
        }

        private void ProcessModuleField()
        {
            if (dataTable.Columns.Contains("Module") && dataTable.Columns.Contains("Summary"))
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string module = row["Module"]?.ToString();
                    string summary = row["Summary"]?.ToString();

                    // Only update Module if it's empty or contains the same as Summary
                    if (string.IsNullOrWhiteSpace(module) || module.Trim() == summary?.Trim())
                    {
                        if (!string.IsNullOrWhiteSpace(summary))
                        {
                            // Remove prefix before first underscore (e.g., "A1_")
                            int underscoreIndex = summary.IndexOf('_');
                            if (underscoreIndex >= 0 && underscoreIndex + 1 < summary.Length)
                            {
                                row["Module"] = summary.Substring(underscoreIndex + 1).Trim();
                            }
                            else
                            {
                                row["Module"] = summary.Trim(); // fallback
                            }
                        }
                        else
                        {
                            row["Module"] = "Room Booking"; // fallback if both are empty
                        }
                    }
                }
            }
        }

        private void ValidateColumns()
        {
            if (!dataTable.Columns.Contains("Start Time") || !dataTable.Columns.Contains("End Time"))
            {
                MessageBox.Show("Start Time or End Time columns are missing.", "Column Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception("Missing required columns.");
            }
        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            if (loadedFiles.Count == 0)
            {
                MessageBox.Show("Please load Excel files first.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Choose output directory for PDFs
                FolderBrowserDialog folderDialog = new FolderBrowserDialog
                {
                    Description = "Select folder to save PDF files"
                };

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string outputFolder = folderDialog.SelectedPath;

                    // Configure progress bar
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = loadedFiles.Count;
                    progressBar1.Value = 0;
                    progressBar1.Visible = true;

                    // Save current index
                    int savedIndex = currentFileIndex;

                    // Export each file one by one
                    for (int i = 0; i < loadedFiles.Count; i++)
                    {
                        try
                        {
                            // Update UI
                            lblFileInfo.Text = $"Exporting {i + 1} of {loadedFiles.Count}: {Path.GetFileName(loadedFiles[i].FilePath)}";
                            Application.DoEvents();

                            // Switch to this file and generate the report
                            currentFileIndex = i;
                            btnGenerateReport_Click(null, EventArgs.Empty);

                            // Get the filename
                            string excelFileName = Path.GetFileNameWithoutExtension(loadedFiles[i].FilePath);
                            string pdfPath = Path.Combine(outputFolder, $"{excelFileName}.pdf");

                            if (reportDocument != null)
                            {
                                // Set up printer settings with landscape orientation
                                System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings
                                {
                                    PrinterName = "Microsoft Print to PDF",
                                    PrintToFile = true,
                                    PrintFileName = pdfPath
                                };

                                System.Drawing.Printing.PageSettings pageSettings = new System.Drawing.Printing.PageSettings(printerSettings)
                                {
                                    Landscape = true // This is key for landscape orientation
                                };

                                // Set print options to landscape
                                reportDocument.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                                reportDocument.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                                reportDocument.PrintOptions.PrinterName = printerSettings.PrinterName;

                                // Print to PDF with landscape orientation
                                reportDocument.PrintToPrinter(printerSettings, pageSettings, false);
                            }

                            // Update progress
                            progressBar1.Value = i + 1;
                            Application.DoEvents();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error exporting {Path.GetFileName(loadedFiles[i].FilePath)}: {ex.Message}",
                                "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    // Restore original selection
                    currentFileIndex = savedIndex;
                    btnGenerateReport_Click(null, EventArgs.Empty);

                    // Hide progress bar
                    progressBar1.Visible = false;

                    MessageBox.Show($"Export process complete. Files saved to:\n{outputFolder}",
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                progressBar1.Visible = false;
                MessageBox.Show($"Error during export process: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (reportDocument == null)
                {
                    MessageBox.Show("No report is loaded.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xls;*.xlsx",
                    Title = "Save Report as Excel",
                    FileName = Path.GetFileNameWithoutExtension(loadedFiles[currentFileIndex].FilePath) + ".xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string excelPath = saveFileDialog.FileName;

                    // Use Export method with proper options
                    ExportOptions exportOptions = reportDocument.ExportOptions;
                    exportOptions.ExportFormatType = ExportFormatType.Excel;
                    exportOptions.ExportDestinationType = ExportDestinationType.DiskFile;

                    DiskFileDestinationOptions diskOptions = new DiskFileDestinationOptions();
                    diskOptions.DiskFileName = excelPath;
                    exportOptions.DestinationOptions = diskOptions;

                    reportDocument.Export();

                    MessageBox.Show($"Report successfully exported to Excel:\n{excelPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to Excel: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrevFile_Click(object sender, EventArgs e)
        {
            if (loadedFiles.Count > 0)
            {
                currentFileIndex = (currentFileIndex - 1 + loadedFiles.Count) % loadedFiles.Count;
                btnGenerateReport_Click(sender, e);
            }
        }

        private void btnNextFile_Click(object sender, EventArgs e)
        {
            if (loadedFiles.Count > 0)
            {
                currentFileIndex = (currentFileIndex + 1) % loadedFiles.Count;
                btnGenerateReport_Click(sender, e);
            }
        }

        private void UpdateFileIndicator()
        {
            if (loadedFiles.Count > 0)
            {
                lblFileInfo.Text = $"File {currentFileIndex + 1} of {loadedFiles.Count}: {Path.GetFileName(loadedFiles[currentFileIndex].FilePath)}";
            }
            else
            {
                lblFileInfo.Text = "No files loaded";
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            crystalReportViewer1.Zoom(1);
            crystalReportViewer1.ShowLogo = false;

            // Create progress bar
            progressBar1 = new ProgressBar
            {
                Location = new System.Drawing.Point(3, 34),
                Name = "progressBar1",
                Size = new System.Drawing.Size(440, 5),
                Visible = false
            };
            panelControls.Controls.Add(progressBar1);

            UpdateFileIndicator();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int margin = 10;
            crystalReportViewer1.Width = this.ClientSize.Width - (margin * 2);
            crystalReportViewer1.Height = this.ClientSize.Height - (margin * 2) - panelControls.Height;
            crystalReportViewer1.Left = margin;
            crystalReportViewer1.Top = panelControls.Bottom + margin;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (reportDocument != null)
            {
                reportDocument.Close();
                reportDocument.Dispose();
            }
        }

        private void lblFileInfo_Click(object sender, EventArgs e)
        {
            //lblFileInfo.AutoSize = true;
            lblFileInfo.Width = 500;
        }
    }
}
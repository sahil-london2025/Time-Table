namespace WindowsFormsApp3
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnExportPDF;
        private System.Windows.Forms.Button btnExit;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private System.Windows.Forms.Button btnPrevFile;
        private System.Windows.Forms.Button btnNextFile;
        private System.Windows.Forms.Label lblFileInfo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnExportPDF = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.panelControls = new System.Windows.Forms.Panel();
            this.btnPrevFile = new System.Windows.Forms.Button();
            this.btnNextFile = new System.Windows.Forms.Button();
            this.lblFileInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(3, 12);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(440, 20);
            this.txtFilePath.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(468, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(562, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Generate Report";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnGenerateReport_Click);
            // 
            // btnExportPDF
            // 
            this.btnExportPDF.Location = new System.Drawing.Point(680, 10);
            this.btnExportPDF.Name = "btnExportPDF";
            this.btnExportPDF.Size = new System.Drawing.Size(100, 23);
            this.btnExportPDF.TabIndex = 3;
            this.btnExportPDF.Text = "Export to PDF";
            this.btnExportPDF.UseVisualStyleBackColor = true;
            this.btnExportPDF.Click += new System.EventHandler(this.btnExportPDF_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(801, 9);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // crystalReportViewer1
            // 
            this.crystalReportViewer1.ActiveViewIndex = -1;
            this.crystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.crystalReportViewer1.Location = new System.Drawing.Point(12, 50);
            this.crystalReportViewer1.Name = "crystalReportViewer1";
            this.crystalReportViewer1.Size = new System.Drawing.Size(1000, 800);
            this.crystalReportViewer1.TabIndex = 5;
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.btnPrevFile);
            this.panelControls.Controls.Add(this.btnNextFile);
            this.panelControls.Controls.Add(this.lblFileInfo);
            this.panelControls.Controls.Add(this.button1);
            this.panelControls.Controls.Add(this.button2);
            this.panelControls.Controls.Add(this.txtFilePath);
            this.panelControls.Controls.Add(this.btnExit);
            this.panelControls.Controls.Add(this.btnExportPDF);
            this.panelControls.Location = new System.Drawing.Point(12, 3);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(1465, 41);
            this.panelControls.TabIndex = 6;
            // 
            // btnPrevFile
            // 
            this.btnPrevFile.Location = new System.Drawing.Point(891, 9);
            this.btnPrevFile.Name = "btnPrevFile";
            this.btnPrevFile.Size = new System.Drawing.Size(38, 23);
            this.btnPrevFile.TabIndex = 9;
            this.btnPrevFile.Text = "<";
            this.btnPrevFile.UseVisualStyleBackColor = true;
            this.btnPrevFile.Click += new System.EventHandler(this.btnPrevFile_Click);
            // 
            // btnNextFile
            // 
            this.btnNextFile.Location = new System.Drawing.Point(927, 9);
            this.btnNextFile.Name = "btnNextFile";
            this.btnNextFile.Size = new System.Drawing.Size(38, 23);
            this.btnNextFile.TabIndex = 10;
            this.btnNextFile.Text = ">";
            this.btnNextFile.UseVisualStyleBackColor = true;
            this.btnNextFile.Click += new System.EventHandler(this.btnNextFile_Click);
            // 
            // lblFileInfo
            // 
            this.lblFileInfo.AutoSize = true;
            this.lblFileInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileInfo.Location = new System.Drawing.Point(994, 14);
            this.lblFileInfo.Name = "lblFileInfo";
            this.lblFileInfo.Size = new System.Drawing.Size(102, 17);
            this.lblFileInfo.TabIndex = 11;
            this.lblFileInfo.Text = "No files loaded";
            this.lblFileInfo.Click += new System.EventHandler(this.lblFileInfo_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(1176, 687);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(241, 19);
            this.label2.TabIndex = 7;
            this.label2.Text = "Developed by - Rishabh Sikenis";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1489, 716);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.crystalReportViewer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Time Table Generation";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Label label2;
    }
}
namespace FLRecordingCleaner
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.projectsPathBox = new System.Windows.Forms.TextBox();
            this.browseBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.unusedRecSizeLabel = new System.Windows.Forms.Label();
            this.unusedRecLabel = new System.Windows.Forms.Label();
            this.totalRecSizeLabel = new System.Windows.Forms.Label();
            this.totalRecLabel = new System.Windows.Forms.Label();
            this.totalProjectsLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gatherInfoBtn = new System.Windows.Forms.Button();
            this.folderPick = new System.Windows.Forms.OpenFileDialog();
            this.folderPickDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.doCleanupBtn = new System.Windows.Forms.Button();
            this.ignoreErrorsCheck = new System.Windows.Forms.CheckBox();
            this.delWorker = new System.ComponentModel.BackgroundWorker();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(268, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select folder where your FL Studio projects are located ";
            // 
            // projectsPathBox
            // 
            this.projectsPathBox.Location = new System.Drawing.Point(12, 38);
            this.projectsPathBox.Name = "projectsPathBox";
            this.projectsPathBox.Size = new System.Drawing.Size(435, 20);
            this.projectsPathBox.TabIndex = 1;
            // 
            // browseBtn
            // 
            this.browseBtn.Location = new System.Drawing.Point(453, 36);
            this.browseBtn.Name = "browseBtn";
            this.browseBtn.Size = new System.Drawing.Size(75, 23);
            this.browseBtn.TabIndex = 2;
            this.browseBtn.Text = "Browse";
            this.browseBtn.UseVisualStyleBackColor = true;
            this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.unusedRecSizeLabel);
            this.groupBox1.Controls.Add(this.unusedRecLabel);
            this.groupBox1.Controls.Add(this.totalRecSizeLabel);
            this.groupBox1.Controls.Add(this.totalRecLabel);
            this.groupBox1.Controls.Add(this.totalProjectsLabel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(516, 153);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Folder information";
            // 
            // unusedRecSizeLabel
            // 
            this.unusedRecSizeLabel.AutoSize = true;
            this.unusedRecSizeLabel.Location = new System.Drawing.Point(167, 121);
            this.unusedRecSizeLabel.Name = "unusedRecSizeLabel";
            this.unusedRecSizeLabel.Size = new System.Drawing.Size(10, 13);
            this.unusedRecSizeLabel.TabIndex = 9;
            this.unusedRecSizeLabel.Text = "-";
            // 
            // unusedRecLabel
            // 
            this.unusedRecLabel.AutoSize = true;
            this.unusedRecLabel.Location = new System.Drawing.Point(167, 99);
            this.unusedRecLabel.Name = "unusedRecLabel";
            this.unusedRecLabel.Size = new System.Drawing.Size(10, 13);
            this.unusedRecLabel.TabIndex = 8;
            this.unusedRecLabel.Text = "-";
            // 
            // totalRecSizeLabel
            // 
            this.totalRecSizeLabel.AutoSize = true;
            this.totalRecSizeLabel.Location = new System.Drawing.Point(167, 77);
            this.totalRecSizeLabel.Name = "totalRecSizeLabel";
            this.totalRecSizeLabel.Size = new System.Drawing.Size(10, 13);
            this.totalRecSizeLabel.TabIndex = 7;
            this.totalRecSizeLabel.Text = "-";
            // 
            // totalRecLabel
            // 
            this.totalRecLabel.AutoSize = true;
            this.totalRecLabel.Location = new System.Drawing.Point(167, 53);
            this.totalRecLabel.Name = "totalRecLabel";
            this.totalRecLabel.Size = new System.Drawing.Size(10, 13);
            this.totalRecLabel.TabIndex = 6;
            this.totalRecLabel.Text = "-";
            // 
            // totalProjectsLabel
            // 
            this.totalProjectsLabel.AutoSize = true;
            this.totalProjectsLabel.Location = new System.Drawing.Point(167, 29);
            this.totalProjectsLabel.Name = "totalProjectsLabel";
            this.totalProjectsLabel.Size = new System.Drawing.Size(10, 13);
            this.totalProjectsLabel.TabIndex = 5;
            this.totalProjectsLabel.Text = "-";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 121);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Unused recordings size:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Unused recordings:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Total recordings size:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Total recordings:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Total projects:";
            // 
            // gatherInfoBtn
            // 
            this.gatherInfoBtn.Location = new System.Drawing.Point(12, 224);
            this.gatherInfoBtn.Name = "gatherInfoBtn";
            this.gatherInfoBtn.Size = new System.Drawing.Size(135, 23);
            this.gatherInfoBtn.TabIndex = 5;
            this.gatherInfoBtn.Text = "Gather Information";
            this.gatherInfoBtn.UseVisualStyleBackColor = true;
            this.gatherInfoBtn.Click += new System.EventHandler(this.gatherInfoBtn_Click);
            // 
            // folderPick
            // 
            this.folderPick.CheckFileExists = false;
            this.folderPick.CheckPathExists = false;
            // 
            // bgWorker
            // 
            this.bgWorker.WorkerReportsProgress = true;
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorker_ProgressChanged);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_OnComplete);
            // 
            // doCleanupBtn
            // 
            this.doCleanupBtn.Location = new System.Drawing.Point(420, 224);
            this.doCleanupBtn.Name = "doCleanupBtn";
            this.doCleanupBtn.Size = new System.Drawing.Size(108, 23);
            this.doCleanupBtn.TabIndex = 6;
            this.doCleanupBtn.Text = "Perform cleanup";
            this.doCleanupBtn.UseVisualStyleBackColor = true;
            this.doCleanupBtn.Click += new System.EventHandler(this.doCleanupBtn_Click);
            // 
            // ignoreErrorsCheck
            // 
            this.ignoreErrorsCheck.AutoSize = true;
            this.ignoreErrorsCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ignoreErrorsCheck.Location = new System.Drawing.Point(329, 228);
            this.ignoreErrorsCheck.Name = "ignoreErrorsCheck";
            this.ignoreErrorsCheck.Size = new System.Drawing.Size(85, 17);
            this.ignoreErrorsCheck.TabIndex = 7;
            this.ignoreErrorsCheck.Text = "Ignore errors";
            this.ignoreErrorsCheck.UseVisualStyleBackColor = true;
            // 
            // delWorker
            // 
            this.delWorker.WorkerReportsProgress = true;
            this.delWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.delWorker_DoWork);
            this.delWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.delWorker_ProgressChanged);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 259);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(516, 23);
            this.progressBar.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 294);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.ignoreErrorsCheck);
            this.Controls.Add(this.doCleanupBtn);
            this.Controls.Add(this.gatherInfoBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.browseBtn);
            this.Controls.Add(this.projectsPathBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FL Studio Unused Recording Cleaner Utility 1.0";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox projectsPathBox;
        private System.Windows.Forms.Button browseBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button gatherInfoBtn;
        private System.Windows.Forms.OpenFileDialog folderPick;
        private System.Windows.Forms.FolderBrowserDialog folderPickDlg;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.Label unusedRecSizeLabel;
        private System.Windows.Forms.Label unusedRecLabel;
        private System.Windows.Forms.Label totalRecSizeLabel;
        private System.Windows.Forms.Label totalRecLabel;
        private System.Windows.Forms.Label totalProjectsLabel;
        private System.Windows.Forms.Button doCleanupBtn;
        private System.Windows.Forms.CheckBox ignoreErrorsCheck;
        private System.ComponentModel.BackgroundWorker delWorker;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}


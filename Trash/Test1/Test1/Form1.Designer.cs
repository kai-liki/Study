namespace Test1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.picBoxCur = new System.Windows.Forms.PictureBox();
            this.picBoxDiff = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.picBoxXChart = new System.Windows.Forms.PictureBox();
            this.picBoxYChart = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelTrack = new System.Windows.Forms.Label();
            this.trackBarDiff = new System.Windows.Forms.TrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numUpDownDis = new System.Windows.Forms.NumericUpDown();
            this.numUpDownDiff = new System.Windows.Forms.NumericUpDown();
            this.btnReset = new System.Windows.Forms.Button();
            this.labelBR = new System.Windows.Forms.Label();
            this.labelUR = new System.Windows.Forms.Label();
            this.labelBL = new System.Windows.Forms.Label();
            this.labelUL = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClnImgList = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxImgs = new System.Windows.Forms.ListBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.openFileDlgImgs = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxCur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxDiff)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxXChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxYChart)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDiff)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDiff)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.picBoxCur, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.picBoxDiff, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(692, 729);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // picBoxCur
            // 
            this.picBoxCur.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBoxCur.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBoxCur.Location = new System.Drawing.Point(3, 3);
            this.picBoxCur.Name = "picBoxCur";
            this.picBoxCur.Size = new System.Drawing.Size(340, 358);
            this.picBoxCur.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxCur.TabIndex = 0;
            this.picBoxCur.TabStop = false;
            this.picBoxCur.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picBoxCur_MouseDown);
            this.picBoxCur.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picBoxCur_MouseUp);
            // 
            // picBoxDiff
            // 
            this.picBoxDiff.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBoxDiff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBoxDiff.Location = new System.Drawing.Point(3, 367);
            this.picBoxDiff.Name = "picBoxDiff";
            this.picBoxDiff.Size = new System.Drawing.Size(340, 359);
            this.picBoxDiff.TabIndex = 1;
            this.picBoxDiff.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoScroll = true;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.picBoxXChart, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.picBoxYChart, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(349, 367);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(340, 359);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // picBoxXChart
            // 
            this.picBoxXChart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBoxXChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBoxXChart.Location = new System.Drawing.Point(3, 3);
            this.picBoxXChart.Name = "picBoxXChart";
            this.picBoxXChart.Size = new System.Drawing.Size(334, 173);
            this.picBoxXChart.TabIndex = 0;
            this.picBoxXChart.TabStop = false;
            // 
            // picBoxYChart
            // 
            this.picBoxYChart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBoxYChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBoxYChart.Location = new System.Drawing.Point(3, 182);
            this.picBoxYChart.Name = "picBoxYChart";
            this.picBoxYChart.Size = new System.Drawing.Size(334, 174);
            this.picBoxYChart.TabIndex = 1;
            this.picBoxYChart.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(349, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(340, 358);
            this.panel1.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelTrack);
            this.groupBox3.Controls.Add(this.trackBarDiff);
            this.groupBox3.Location = new System.Drawing.Point(3, 275);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(325, 79);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "其它";
            // 
            // labelTrack
            // 
            this.labelTrack.AutoSize = true;
            this.labelTrack.Location = new System.Drawing.Point(140, 61);
            this.labelTrack.Name = "labelTrack";
            this.labelTrack.Size = new System.Drawing.Size(0, 13);
            this.labelTrack.TabIndex = 1;
            // 
            // trackBarDiff
            // 
            this.trackBarDiff.Location = new System.Drawing.Point(6, 12);
            this.trackBarDiff.Maximum = 0;
            this.trackBarDiff.Name = "trackBarDiff";
            this.trackBarDiff.Size = new System.Drawing.Size(313, 42);
            this.trackBarDiff.TabIndex = 0;
            this.trackBarDiff.ValueChanged += new System.EventHandler(this.trackBarDiff_ValueChanged);
            this.trackBarDiff.Scroll += new System.EventHandler(this.trackBarDiff_Scroll);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numUpDownDis);
            this.groupBox2.Controls.Add(this.numUpDownDiff);
            this.groupBox2.Controls.Add(this.btnReset);
            this.groupBox2.Controls.Add(this.labelBR);
            this.groupBox2.Controls.Add(this.labelUR);
            this.groupBox2.Controls.Add(this.labelBL);
            this.groupBox2.Controls.Add(this.labelUL);
            this.groupBox2.Location = new System.Drawing.Point(3, 178);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(327, 91);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "监视范围";
            // 
            // numUpDownDis
            // 
            this.numUpDownDis.Location = new System.Drawing.Point(257, 61);
            this.numUpDownDis.Name = "numUpDownDis";
            this.numUpDownDis.Size = new System.Drawing.Size(62, 20);
            this.numUpDownDis.TabIndex = 6;
            // 
            // numUpDownDiff
            // 
            this.numUpDownDiff.Location = new System.Drawing.Point(175, 61);
            this.numUpDownDiff.Name = "numUpDownDiff";
            this.numUpDownDiff.Size = new System.Drawing.Size(61, 20);
            this.numUpDownDiff.TabIndex = 5;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(175, 22);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 25);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "归  零";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // labelBR
            // 
            this.labelBR.AutoSize = true;
            this.labelBR.Location = new System.Drawing.Point(97, 68);
            this.labelBR.Name = "labelBR";
            this.labelBR.Size = new System.Drawing.Size(22, 13);
            this.labelBR.TabIndex = 3;
            this.labelBR.Text = "0,0";
            // 
            // labelUR
            // 
            this.labelUR.AutoSize = true;
            this.labelUR.Location = new System.Drawing.Point(97, 28);
            this.labelUR.Name = "labelUR";
            this.labelUR.Size = new System.Drawing.Size(22, 13);
            this.labelUR.TabIndex = 2;
            this.labelUR.Text = "0,0";
            // 
            // labelBL
            // 
            this.labelBL.AutoSize = true;
            this.labelBL.Location = new System.Drawing.Point(22, 68);
            this.labelBL.Name = "labelBL";
            this.labelBL.Size = new System.Drawing.Size(22, 13);
            this.labelBL.TabIndex = 1;
            this.labelBL.Text = "0,0";
            // 
            // labelUL
            // 
            this.labelUL.AutoSize = true;
            this.labelUL.Location = new System.Drawing.Point(22, 28);
            this.labelUL.Name = "labelUL";
            this.labelUL.Size = new System.Drawing.Size(22, 13);
            this.labelUL.TabIndex = 0;
            this.labelUL.Text = "0,0";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.btnClnImgList);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.listBoxImgs);
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Location = new System.Drawing.Point(3, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(328, 161);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "图形文件";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(6, 72);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 25);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "开  始";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnClnImgList
            // 
            this.btnClnImgList.Location = new System.Drawing.Point(6, 103);
            this.btnClnImgList.Name = "btnClnImgList";
            this.btnClnImgList.Size = new System.Drawing.Size(75, 25);
            this.btnClnImgList.TabIndex = 4;
            this.btnClnImgList.Text = "清空列表";
            this.btnClnImgList.UseVisualStyleBackColor = true;
            this.btnClnImgList.Click += new System.EventHandler(this.btnClnImgList_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(85, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "文件列表";
            // 
            // listBoxImgs
            // 
            this.listBoxImgs.FormattingEnabled = true;
            this.listBoxImgs.Location = new System.Drawing.Point(87, 33);
            this.listBoxImgs.Name = "listBoxImgs";
            this.listBoxImgs.ScrollAlwaysVisible = true;
            this.listBoxImgs.Size = new System.Drawing.Size(238, 121);
            this.listBoxImgs.TabIndex = 2;
            this.listBoxImgs.SelectedIndexChanged += new System.EventHandler(this.listBoxImgs_SelectedIndexChanged);
            this.listBoxImgs.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listBoxImgs_KeyPress);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(6, 40);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 25);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "浏  览";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // openFileDlgImgs
            // 
            this.openFileDlgImgs.Multiselect = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 729);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxCur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxDiff)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxXChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxYChart)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDiff)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDiff)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox picBoxCur;
        private System.Windows.Forms.PictureBox picBoxDiff;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox picBoxXChart;
        private System.Windows.Forms.PictureBox picBoxYChart;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxImgs;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.OpenFileDialog openFileDlgImgs;
        private System.Windows.Forms.Button btnClnImgList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelBR;
        private System.Windows.Forms.Label labelUR;
        private System.Windows.Forms.Label labelBL;
        private System.Windows.Forms.Label labelUL;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TrackBar trackBarDiff;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label labelTrack;
        private System.Windows.Forms.NumericUpDown numUpDownDiff;
        private System.Windows.Forms.NumericUpDown numUpDownDis;

    }
}


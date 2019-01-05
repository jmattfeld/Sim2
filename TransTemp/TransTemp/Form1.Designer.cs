namespace TransTemp
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fIleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDataFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCalibrationFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.outFIleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.saveMaxMinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMaxMinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fIleToolStripMenuItem,
            this.displayChartToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(844, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fIleToolStripMenuItem
            // 
            this.fIleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadDataFileToolStripMenuItem,
            this.loadCalibrationFileToolStripMenuItem,
            this.loadMaxMinToolStripMenuItem,
            this.toolStripMenuItem2,
            this.outFIleToolStripMenuItem,
            this.saveMaxMinToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fIleToolStripMenuItem.Name = "fIleToolStripMenuItem";
            this.fIleToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.fIleToolStripMenuItem.Text = "File...";
            // 
            // loadDataFileToolStripMenuItem
            // 
            this.loadDataFileToolStripMenuItem.Name = "loadDataFileToolStripMenuItem";
            this.loadDataFileToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.loadDataFileToolStripMenuItem.Text = "Load Data File...";
            this.loadDataFileToolStripMenuItem.Click += new System.EventHandler(this.loadDataFileToolStripMenuItem_Click);
            // 
            // loadCalibrationFileToolStripMenuItem
            // 
            this.loadCalibrationFileToolStripMenuItem.Name = "loadCalibrationFileToolStripMenuItem";
            this.loadCalibrationFileToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.loadCalibrationFileToolStripMenuItem.Text = "Load Calibration File...";
            this.loadCalibrationFileToolStripMenuItem.Click += new System.EventHandler(this.loadCalibrationFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(188, 6);
            // 
            // outFIleToolStripMenuItem
            // 
            this.outFIleToolStripMenuItem.Name = "outFIleToolStripMenuItem";
            this.outFIleToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.outFIleToolStripMenuItem.Text = "Out File...";
            this.outFIleToolStripMenuItem.Click += new System.EventHandler(this.outFIleToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // displayChartToolStripMenuItem
            // 
            this.displayChartToolStripMenuItem.Name = "displayChartToolStripMenuItem";
            this.displayChartToolStripMenuItem.Size = new System.Drawing.Size(89, 20);
            this.displayChartToolStripMenuItem.Text = "Display Chart";
            this.displayChartToolStripMenuItem.Click += new System.EventHandler(this.displayChartToolStripMenuItem_Click);
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(12, 59);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Single;
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(802, 280);
            this.chart1.TabIndex = 5;
            this.chart1.Text = "chart1";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 28);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 6;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(136, 27);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 7;
            // 
            // saveMaxMinToolStripMenuItem
            // 
            this.saveMaxMinToolStripMenuItem.Name = "saveMaxMinToolStripMenuItem";
            this.saveMaxMinToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.saveMaxMinToolStripMenuItem.Text = "Save Max Min...";
            this.saveMaxMinToolStripMenuItem.Click += new System.EventHandler(this.saveMaxMinToolStripMenuItem_Click);
            // 
            // loadMaxMinToolStripMenuItem
            // 
            this.loadMaxMinToolStripMenuItem.Name = "loadMaxMinToolStripMenuItem";
            this.loadMaxMinToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.loadMaxMinToolStripMenuItem.Text = "Load Max Min...";
            this.loadMaxMinToolStripMenuItem.Click += new System.EventHandler(this.loadMaxMinToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 351);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Transient Temperature";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fIleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDataFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCalibrationFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outFIleToolStripMenuItem;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ToolStripMenuItem displayChartToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ToolStripMenuItem saveMaxMinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMaxMinToolStripMenuItem;
    }
}


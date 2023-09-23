namespace MazeGenerator
{
    partial class MazeForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            chkShowGen = new CheckBox();
            panel1 = new Panel();
            panel2 = new Panel();
            chkInverseColors = new CheckBox();
            rbtnFillCells = new RadioButton();
            rbtnBuildWalls = new RadioButton();
            rbtnRemoveWalls = new RadioButton();
            txtWidth = new TextBox();
            txtHeight = new TextBox();
            btnIncreaseWidth = new Button();
            btnDecreaseWidth = new Button();
            btnIncreaseHeight = new Button();
            btnDecreaseHeight = new Button();
            label5 = new Label();
            tbarSolveSpeed = new TrackBar();
            chkShowSolve = new CheckBox();
            label2 = new Label();
            chkShowBacktracks = new CheckBox();
            label4 = new Label();
            tbarGenerateSpeed = new TrackBar();
            label3 = new Label();
            cboxAlgorithm = new ComboBox();
            label1 = new Label();
            btnGenerate = new Button();
            btnSolve = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbarSolveSpeed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbarGenerateSpeed).BeginInit();
            SuspendLayout();
            // 
            // chkShowGen
            // 
            chkShowGen.AutoSize = true;
            chkShowGen.Checked = true;
            chkShowGen.CheckState = CheckState.Checked;
            chkShowGen.Location = new Point(22, 439);
            chkShowGen.Margin = new Padding(3, 4, 3, 4);
            chkShowGen.Name = "chkShowGen";
            chkShowGen.Size = new Size(131, 24);
            chkShowGen.TabIndex = 8;
            chkShowGen.Text = "Show Generate";
            chkShowGen.UseVisualStyleBackColor = true;
            chkShowGen.CheckedChanged += chkShowGen_CheckedChanged;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Control;
            panel1.Controls.Add(panel2);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1239, 919);
            panel1.TabIndex = 0;
            panel1.Paint += panel1_Paint;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(chkInverseColors);
            panel2.Controls.Add(rbtnFillCells);
            panel2.Controls.Add(rbtnBuildWalls);
            panel2.Controls.Add(rbtnRemoveWalls);
            panel2.Controls.Add(txtWidth);
            panel2.Controls.Add(txtHeight);
            panel2.Controls.Add(btnIncreaseWidth);
            panel2.Controls.Add(btnDecreaseWidth);
            panel2.Controls.Add(btnIncreaseHeight);
            panel2.Controls.Add(btnDecreaseHeight);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(tbarSolveSpeed);
            panel2.Controls.Add(chkShowSolve);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(chkShowBacktracks);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(tbarGenerateSpeed);
            panel2.Controls.Add(chkShowGen);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(cboxAlgorithm);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(btnGenerate);
            panel2.Controls.Add(btnSolve);
            panel2.Location = new Point(974, 52);
            panel2.Name = "panel2";
            panel2.Size = new Size(219, 821);
            panel2.TabIndex = 2;
            panel2.Paint += panel2_Paint;
            // 
            // chkInverseColors
            // 
            chkInverseColors.AutoSize = true;
            chkInverseColors.Location = new Point(22, 393);
            chkInverseColors.Margin = new Padding(3, 4, 3, 4);
            chkInverseColors.Name = "chkInverseColors";
            chkInverseColors.Size = new Size(123, 24);
            chkInverseColors.TabIndex = 28;
            chkInverseColors.Text = "Inverse Colors";
            chkInverseColors.UseVisualStyleBackColor = true;
            chkInverseColors.CheckedChanged += chkInverseColors_CheckedChanged;
            // 
            // rbtnFillCells
            // 
            rbtnFillCells.AutoSize = true;
            rbtnFillCells.Location = new Point(22, 349);
            rbtnFillCells.Margin = new Padding(3, 4, 3, 4);
            rbtnFillCells.Name = "rbtnFillCells";
            rbtnFillCells.Size = new Size(84, 24);
            rbtnFillCells.TabIndex = 27;
            rbtnFillCells.TabStop = true;
            rbtnFillCells.Text = "Fill Cells";
            rbtnFillCells.UseVisualStyleBackColor = true;
            rbtnFillCells.CheckedChanged += rbgrpAlgorithmType_CheckedChanged;
            // 
            // rbtnBuildWalls
            // 
            rbtnBuildWalls.AutoSize = true;
            rbtnBuildWalls.Location = new Point(22, 317);
            rbtnBuildWalls.Name = "rbtnBuildWalls";
            rbtnBuildWalls.Size = new Size(103, 24);
            rbtnBuildWalls.TabIndex = 26;
            rbtnBuildWalls.Text = "Build Walls";
            rbtnBuildWalls.UseVisualStyleBackColor = true;
            rbtnBuildWalls.CheckedChanged += rbgrpAlgorithmType_CheckedChanged;
            // 
            // rbtnRemoveWalls
            // 
            rbtnRemoveWalls.AutoSize = true;
            rbtnRemoveWalls.Checked = true;
            rbtnRemoveWalls.Location = new Point(22, 287);
            rbtnRemoveWalls.Name = "rbtnRemoveWalls";
            rbtnRemoveWalls.Size = new Size(123, 24);
            rbtnRemoveWalls.TabIndex = 25;
            rbtnRemoveWalls.TabStop = true;
            rbtnRemoveWalls.Text = "Remove Walls";
            rbtnRemoveWalls.UseVisualStyleBackColor = true;
            rbtnRemoveWalls.CheckedChanged += rbgrpAlgorithmType_CheckedChanged;
            // 
            // txtWidth
            // 
            txtWidth.BorderStyle = BorderStyle.FixedSingle;
            txtWidth.Location = new Point(75, 144);
            txtWidth.Name = "txtWidth";
            txtWidth.Size = new Size(40, 27);
            txtWidth.TabIndex = 24;
            txtWidth.TextAlign = HorizontalAlignment.Right;
            txtWidth.KeyDown += txtWidth_KeyDown;
            // 
            // txtHeight
            // 
            txtHeight.BorderStyle = BorderStyle.FixedSingle;
            txtHeight.Location = new Point(75, 89);
            txtHeight.Name = "txtHeight";
            txtHeight.Size = new Size(40, 27);
            txtHeight.TabIndex = 3;
            txtHeight.TextAlign = HorizontalAlignment.Right;
            txtHeight.KeyDown += txtHeight_KeyDown;
            // 
            // btnIncreaseWidth
            // 
            btnIncreaseWidth.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point);
            btnIncreaseWidth.Location = new Point(167, 138);
            btnIncreaseWidth.MaximumSize = new Size(40, 40);
            btnIncreaseWidth.MinimumSize = new Size(40, 40);
            btnIncreaseWidth.Name = "btnIncreaseWidth";
            btnIncreaseWidth.Size = new Size(40, 40);
            btnIncreaseWidth.TabIndex = 23;
            btnIncreaseWidth.Text = "+";
            btnIncreaseWidth.UseVisualStyleBackColor = true;
            btnIncreaseWidth.MouseDown += btnIncreaseWidth_MouseDown;
            // 
            // btnDecreaseWidth
            // 
            btnDecreaseWidth.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point);
            btnDecreaseWidth.Location = new Point(121, 138);
            btnDecreaseWidth.MaximumSize = new Size(40, 40);
            btnDecreaseWidth.MinimumSize = new Size(40, 40);
            btnDecreaseWidth.Name = "btnDecreaseWidth";
            btnDecreaseWidth.Size = new Size(40, 40);
            btnDecreaseWidth.TabIndex = 22;
            btnDecreaseWidth.Text = "-";
            btnDecreaseWidth.UseVisualStyleBackColor = true;
            btnDecreaseWidth.MouseDown += btnDecreaseWidth_MouseDown;
            // 
            // btnIncreaseHeight
            // 
            btnIncreaseHeight.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point);
            btnIncreaseHeight.Location = new Point(167, 83);
            btnIncreaseHeight.MaximumSize = new Size(40, 40);
            btnIncreaseHeight.MinimumSize = new Size(40, 40);
            btnIncreaseHeight.Name = "btnIncreaseHeight";
            btnIncreaseHeight.Size = new Size(40, 40);
            btnIncreaseHeight.TabIndex = 21;
            btnIncreaseHeight.Text = "+";
            btnIncreaseHeight.UseVisualStyleBackColor = true;
            btnIncreaseHeight.MouseDown += btnIncreaseHeight_MouseDown;
            // 
            // btnDecreaseHeight
            // 
            btnDecreaseHeight.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point);
            btnDecreaseHeight.Location = new Point(121, 83);
            btnDecreaseHeight.MaximumSize = new Size(40, 40);
            btnDecreaseHeight.MinimumSize = new Size(40, 40);
            btnDecreaseHeight.Name = "btnDecreaseHeight";
            btnDecreaseHeight.Size = new Size(40, 40);
            btnDecreaseHeight.TabIndex = 20;
            btnDecreaseHeight.Text = "-";
            btnDecreaseHeight.UseVisualStyleBackColor = true;
            btnDecreaseHeight.MouseDown += btnDecreaseHeight_MouseDown;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new Point(13, 717);
            label5.MinimumSize = new Size(194, 0);
            label5.Name = "label5";
            label5.Size = new Size(194, 23);
            label5.TabIndex = 19;
            label5.Text = "Solve Speed";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbarSolveSpeed
            // 
            tbarSolveSpeed.Location = new Point(13, 747);
            tbarSolveSpeed.Margin = new Padding(3, 4, 3, 4);
            tbarSolveSpeed.Name = "tbarSolveSpeed";
            tbarSolveSpeed.Size = new Size(194, 56);
            tbarSolveSpeed.TabIndex = 18;
            tbarSolveSpeed.Scroll += tbarSolveSpeed_Scroll;
            // 
            // chkShowSolve
            // 
            chkShowSolve.AutoSize = true;
            chkShowSolve.Checked = true;
            chkShowSolve.CheckState = CheckState.Checked;
            chkShowSolve.Location = new Point(22, 679);
            chkShowSolve.Margin = new Padding(3, 4, 3, 4);
            chkShowSolve.Name = "chkShowSolve";
            chkShowSolve.Size = new Size(107, 24);
            chkShowSolve.TabIndex = 12;
            chkShowSolve.Text = "Show Solve";
            chkShowSolve.UseVisualStyleBackColor = true;
            chkShowSolve.CheckedChanged += chkShowSolve_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(17, 146);
            label2.Name = "label2";
            label2.Size = new Size(52, 20);
            label2.TabIndex = 16;
            label2.Text = "Width:";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // chkShowBacktracks
            // 
            chkShowBacktracks.AutoSize = true;
            chkShowBacktracks.Location = new Point(22, 645);
            chkShowBacktracks.Margin = new Padding(3, 4, 3, 4);
            chkShowBacktracks.Name = "chkShowBacktracks";
            chkShowBacktracks.Size = new Size(140, 24);
            chkShowBacktracks.TabIndex = 11;
            chkShowBacktracks.Text = "Show Backtracks";
            chkShowBacktracks.UseVisualStyleBackColor = true;
            chkShowBacktracks.CheckedChanged += chkShowBacktracks_CheckedChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(13, 480);
            label4.MinimumSize = new Size(194, 0);
            label4.Name = "label4";
            label4.Size = new Size(194, 23);
            label4.TabIndex = 10;
            label4.Text = "Generate Speed";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbarGenerateSpeed
            // 
            tbarGenerateSpeed.Location = new Point(13, 509);
            tbarGenerateSpeed.Margin = new Padding(3, 4, 3, 4);
            tbarGenerateSpeed.Name = "tbarGenerateSpeed";
            tbarGenerateSpeed.Size = new Size(194, 56);
            tbarGenerateSpeed.TabIndex = 9;
            tbarGenerateSpeed.Scroll += tbarGenSpeed_Scroll;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(13, 195);
            label3.MinimumSize = new Size(194, 0);
            label3.Name = "label3";
            label3.Size = new Size(194, 23);
            label3.TabIndex = 7;
            label3.Text = "Algorithm";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cboxAlgorithm
            // 
            cboxAlgorithm.DropDownStyle = ComboBoxStyle.DropDownList;
            cboxAlgorithm.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            cboxAlgorithm.FormattingEnabled = true;
            cboxAlgorithm.Items.AddRange(new object[] { "Recursive Backtracker", "Recursive Backtracker - Jagged", "Hunt and Kill", "Hunt and Kill - Jagged", "Prim's Algorithm", "Kruskal's Algorithm" });
            cboxAlgorithm.Location = new Point(13, 231);
            cboxAlgorithm.Margin = new Padding(3, 4, 3, 4);
            cboxAlgorithm.Name = "cboxAlgorithm";
            cboxAlgorithm.Size = new Size(194, 33);
            cboxAlgorithm.TabIndex = 6;
            cboxAlgorithm.SelectedIndexChanged += cboxAlgorithm_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(12, 91);
            label1.Name = "label1";
            label1.Size = new Size(57, 20);
            label1.TabIndex = 4;
            label1.Text = "Height:";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnGenerate
            // 
            btnGenerate.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btnGenerate.Location = new Point(13, 19);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(194, 47);
            btnGenerate.TabIndex = 0;
            btnGenerate.Text = "Generate";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // btnSolve
            // 
            btnSolve.Location = new Point(13, 576);
            btnSolve.Name = "btnSolve";
            btnSolve.Size = new Size(194, 47);
            btnSolve.TabIndex = 1;
            btnSolve.Text = "Solve";
            btnSolve.UseVisualStyleBackColor = true;
            btnSolve.Click += btnSolve_Click;
            // 
            // MazeForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1224, 921);
            Controls.Add(panel1);
            Name = "MazeForm";
            Text = "Maze Generator";
            Load += MazeForm_Load;
            Resize += MazeForm_Resize;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tbarSolveSpeed).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbarGenerateSpeed).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btnSolve;
        private Button btnGenerate;
        private Panel panel2;
        private Label label1;
        private Label label3;
        private ComboBox cboxAlgorithm;
        private Label label4;
        private TrackBar tbarGenerateSpeed;
        private CheckBox chkShowGen;
        private CheckBox chkShowBacktracks;
        private CheckBox chkShowSolve;
        private Label label2;
        private Label label5;
        private TrackBar tbarSolveSpeed;
        private Button btnDecreaseHeight;
        private Button btnIncreaseHeight;
        private Button btnIncreaseWidth;
        private Button btnDecreaseWidth;
        private TextBox txtHeight;
        private TextBox txtWidth;
        private RadioButton rbtnRemoveWalls;
        private RadioButton rbtnBuildWalls;
        private RadioButton rbtnFillCells;
        private CheckBox chkInverseColors;
    }
}
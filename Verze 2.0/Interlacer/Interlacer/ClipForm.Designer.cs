namespace Interlacer
{
    partial class ClipForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClipForm));
            this.clipPictureBox = new System.Windows.Forms.PictureBox();
            this.listView3 = new System.Windows.Forms.ListView();
            this.selectionRectangleGroupBox = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rubberHeightNumeric = new System.Windows.Forms.NumericUpDown();
            this.rubberWidthNumeric = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bottomRightYnumeric = new System.Windows.Forms.NumericUpDown();
            this.bottomRightXnumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.topLeftYnumeric = new System.Windows.Forms.NumericUpDown();
            this.topLeftXnumeric = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.clipImagesButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.clipPictureBox)).BeginInit();
            this.selectionRectangleGroupBox.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rubberHeightNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rubberWidthNumeric)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bottomRightYnumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bottomRightXnumeric)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topLeftYnumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topLeftXnumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // clipPictureBox
            // 
            this.clipPictureBox.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.clipPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("clipPictureBox.Image")));
            this.clipPictureBox.InitialImage = null;
            this.clipPictureBox.Location = new System.Drawing.Point(12, 12);
            this.clipPictureBox.Name = "clipPictureBox";
            this.clipPictureBox.Size = new System.Drawing.Size(329, 329);
            this.clipPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.clipPictureBox.TabIndex = 1;
            this.clipPictureBox.TabStop = false;
            this.clipPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.clipPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.clipPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // listView3
            // 
            this.listView3.Location = new System.Drawing.Point(12, 367);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(329, 222);
            this.listView3.TabIndex = 4;
            this.listView3.UseCompatibleStateImageBehavior = false;
            // 
            // selectionRectangleGroupBox
            // 
            this.selectionRectangleGroupBox.Controls.Add(this.groupBox3);
            this.selectionRectangleGroupBox.Controls.Add(this.groupBox2);
            this.selectionRectangleGroupBox.Controls.Add(this.groupBox1);
            this.selectionRectangleGroupBox.Location = new System.Drawing.Point(348, 13);
            this.selectionRectangleGroupBox.Name = "selectionRectangleGroupBox";
            this.selectionRectangleGroupBox.Size = new System.Drawing.Size(252, 207);
            this.selectionRectangleGroupBox.TabIndex = 5;
            this.selectionRectangleGroupBox.TabStop = false;
            this.selectionRectangleGroupBox.Text = "Selection rectangle";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rubberHeightNumeric);
            this.groupBox3.Controls.Add(this.rubberWidthNumeric);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(6, 129);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(240, 49);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Dimensions";
            // 
            // rubberHeightNumeric
            // 
            this.rubberHeightNumeric.Location = new System.Drawing.Point(164, 21);
            this.rubberHeightNumeric.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.rubberHeightNumeric.Name = "rubberHeightNumeric";
            this.rubberHeightNumeric.Size = new System.Drawing.Size(62, 20);
            this.rubberHeightNumeric.TabIndex = 1;
            this.rubberHeightNumeric.ValueChanged += new System.EventHandler(this.rubberHeightNumeric_ValueChanged);
            // 
            // rubberWidthNumeric
            // 
            this.rubberWidthNumeric.Location = new System.Drawing.Point(52, 22);
            this.rubberWidthNumeric.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.rubberWidthNumeric.Name = "rubberWidthNumeric";
            this.rubberWidthNumeric.Size = new System.Drawing.Size(62, 20);
            this.rubberWidthNumeric.TabIndex = 1;
            this.rubberWidthNumeric.ValueChanged += new System.EventHandler(this.rubberWidthNumeric_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(117, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Height:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Width:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bottomRightYnumeric);
            this.groupBox2.Controls.Add(this.bottomRightXnumeric);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(6, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(240, 49);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Bottom right corner coordinates";
            // 
            // bottomRightYnumeric
            // 
            this.bottomRightYnumeric.Location = new System.Drawing.Point(122, 21);
            this.bottomRightYnumeric.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.bottomRightYnumeric.Name = "bottomRightYnumeric";
            this.bottomRightYnumeric.Size = new System.Drawing.Size(62, 20);
            this.bottomRightYnumeric.TabIndex = 1;
            this.bottomRightYnumeric.ValueChanged += new System.EventHandler(this.bottomRightYnumeric_ValueChanged);
            // 
            // bottomRightXnumeric
            // 
            this.bottomRightXnumeric.Location = new System.Drawing.Point(31, 21);
            this.bottomRightXnumeric.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.bottomRightXnumeric.Name = "bottomRightXnumeric";
            this.bottomRightXnumeric.Size = new System.Drawing.Size(62, 20);
            this.bottomRightXnumeric.TabIndex = 1;
            this.bottomRightXnumeric.ValueChanged += new System.EventHandler(this.bottomRightXnumeric_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(99, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Y:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "X:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.topLeftYnumeric);
            this.groupBox1.Controls.Add(this.topLeftXnumeric);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 49);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Top left corner coordinates";
            // 
            // topLeftYnumeric
            // 
            this.topLeftYnumeric.Location = new System.Drawing.Point(122, 21);
            this.topLeftYnumeric.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.topLeftYnumeric.Name = "topLeftYnumeric";
            this.topLeftYnumeric.Size = new System.Drawing.Size(62, 20);
            this.topLeftYnumeric.TabIndex = 1;
            this.topLeftYnumeric.ValueChanged += new System.EventHandler(this.topLeftYnumeric_ValueChanged);
            // 
            // topLeftXnumeric
            // 
            this.topLeftXnumeric.Location = new System.Drawing.Point(31, 21);
            this.topLeftXnumeric.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.topLeftXnumeric.Name = "topLeftXnumeric";
            this.topLeftXnumeric.Size = new System.Drawing.Size(62, 20);
            this.topLeftXnumeric.TabIndex = 1;
            this.topLeftXnumeric.ValueChanged += new System.EventHandler(this.topLeftXnumeric_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(99, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Y:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "X:";
            // 
            // clipImagesButton
            // 
            this.clipImagesButton.Location = new System.Drawing.Point(348, 227);
            this.clipImagesButton.Name = "clipImagesButton";
            this.clipImagesButton.Size = new System.Drawing.Size(122, 114);
            this.clipImagesButton.TabIndex = 6;
            this.clipImagesButton.Text = "Clip";
            this.clipImagesButton.UseVisualStyleBackColor = true;
            this.clipImagesButton.Click += new System.EventHandler(this.clipImagesButton_Click);
            // 
            // ClipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 624);
            this.Controls.Add(this.clipImagesButton);
            this.Controls.Add(this.selectionRectangleGroupBox);
            this.Controls.Add(this.listView3);
            this.Controls.Add(this.clipPictureBox);
            this.Name = "ClipForm";
            this.Text = "ClipForm";
            ((System.ComponentModel.ISupportInitialize)(this.clipPictureBox)).EndInit();
            this.selectionRectangleGroupBox.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rubberHeightNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rubberWidthNumeric)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bottomRightYnumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bottomRightXnumeric)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topLeftYnumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.topLeftXnumeric)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox clipPictureBox;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.GroupBox selectionRectangleGroupBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown topLeftYnumeric;
        private System.Windows.Forms.NumericUpDown topLeftXnumeric;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown rubberHeightNumeric;
        private System.Windows.Forms.NumericUpDown rubberWidthNumeric;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown bottomRightYnumeric;
        private System.Windows.Forms.NumericUpDown bottomRightXnumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button clipImagesButton;
    }
}
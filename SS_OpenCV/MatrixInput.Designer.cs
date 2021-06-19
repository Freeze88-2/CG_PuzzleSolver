
namespace CG_OpenCV
{
    partial class MatrixInput
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
            this.matrix1 = new System.Windows.Forms.TextBox();
            this.matrix4 = new System.Windows.Forms.TextBox();
            this.matrix7 = new System.Windows.Forms.TextBox();
            this.matrix2 = new System.Windows.Forms.TextBox();
            this.matrix5 = new System.Windows.Forms.TextBox();
            this.matrix8 = new System.Windows.Forms.TextBox();
            this.matrix3 = new System.Windows.Forms.TextBox();
            this.matrix6 = new System.Windows.Forms.TextBox();
            this.matrix9 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.weightInput = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // matrix1
            // 
            this.matrix1.Location = new System.Drawing.Point(12, 37);
            this.matrix1.Name = "matrix1";
            this.matrix1.Size = new System.Drawing.Size(29, 20);
            this.matrix1.TabIndex = 0;
            // 
            // matrix4
            // 
            this.matrix4.Location = new System.Drawing.Point(12, 63);
            this.matrix4.Name = "matrix4";
            this.matrix4.Size = new System.Drawing.Size(29, 20);
            this.matrix4.TabIndex = 1;
            // 
            // matrix7
            // 
            this.matrix7.Location = new System.Drawing.Point(12, 89);
            this.matrix7.Name = "matrix7";
            this.matrix7.Size = new System.Drawing.Size(29, 20);
            this.matrix7.TabIndex = 2;
            // 
            // matrix2
            // 
            this.matrix2.Location = new System.Drawing.Point(47, 37);
            this.matrix2.Name = "matrix2";
            this.matrix2.Size = new System.Drawing.Size(29, 20);
            this.matrix2.TabIndex = 3;
            // 
            // matrix5
            // 
            this.matrix5.Location = new System.Drawing.Point(47, 63);
            this.matrix5.Name = "matrix5";
            this.matrix5.Size = new System.Drawing.Size(29, 20);
            this.matrix5.TabIndex = 4;
            // 
            // matrix8
            // 
            this.matrix8.Location = new System.Drawing.Point(47, 89);
            this.matrix8.Name = "matrix8";
            this.matrix8.Size = new System.Drawing.Size(29, 20);
            this.matrix8.TabIndex = 5;
            // 
            // matrix3
            // 
            this.matrix3.Location = new System.Drawing.Point(82, 37);
            this.matrix3.Name = "matrix3";
            this.matrix3.Size = new System.Drawing.Size(29, 20);
            this.matrix3.TabIndex = 6;
            // 
            // matrix6
            // 
            this.matrix6.Location = new System.Drawing.Point(82, 63);
            this.matrix6.Name = "matrix6";
            this.matrix6.Size = new System.Drawing.Size(29, 20);
            this.matrix6.TabIndex = 7;
            // 
            // matrix9
            // 
            this.matrix9.Location = new System.Drawing.Point(82, 89);
            this.matrix9.Name = "matrix9";
            this.matrix9.Size = new System.Drawing.Size(29, 20);
            this.matrix9.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Matrix";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
            this.label2.Location = new System.Drawing.Point(169, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Weight";
            // 
            // weightInput
            // 
            this.weightInput.Location = new System.Drawing.Point(172, 63);
            this.weightInput.Name = "weightInput";
            this.weightInput.Size = new System.Drawing.Size(36, 20);
            this.weightInput.TabIndex = 11;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(155, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Confirm";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // MatrixInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 156);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.weightInput);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.matrix9);
            this.Controls.Add(this.matrix6);
            this.Controls.Add(this.matrix3);
            this.Controls.Add(this.matrix8);
            this.Controls.Add(this.matrix5);
            this.Controls.Add(this.matrix2);
            this.Controls.Add(this.matrix7);
            this.Controls.Add(this.matrix4);
            this.Controls.Add(this.matrix1);
            this.Name = "MatrixInput";
            this.Text = "Matrix Input";
            this.Load += new System.EventHandler(this.MatrixInput_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox matrix1;
        private System.Windows.Forms.TextBox matrix4;
        private System.Windows.Forms.TextBox matrix7;
        private System.Windows.Forms.TextBox matrix2;
        private System.Windows.Forms.TextBox matrix5;
        private System.Windows.Forms.TextBox matrix8;
        private System.Windows.Forms.TextBox matrix3;
        private System.Windows.Forms.TextBox matrix6;
        private System.Windows.Forms.TextBox matrix9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox weightInput;
        private System.Windows.Forms.Button button1;
    }
}
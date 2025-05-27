namespace RevitAgent.LLM
{
    partial class DialogueForm
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
            this.InputTextBox = new System.Windows.Forms.TextBox();
            this.DisplayTextBox = new System.Windows.Forms.RichTextBox();
            this.EnterButton = new System.Windows.Forms.Button();
            this.ClearTextButton = new System.Windows.Forms.Button();
            this.ClearDisplayButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // InputTextBox
            // 
            this.InputTextBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.InputTextBox.Location = new System.Drawing.Point(12, 1062);
            this.InputTextBox.Multiline = true;
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.InputTextBox.Size = new System.Drawing.Size(1863, 330);
            this.InputTextBox.TabIndex = 0;
            // 
            // DisplayTextBox
            // 
            this.DisplayTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.DisplayTextBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DisplayTextBox.Location = new System.Drawing.Point(12, 12);
            this.DisplayTextBox.Name = "DisplayTextBox";
            this.DisplayTextBox.ReadOnly = true;
            this.DisplayTextBox.Size = new System.Drawing.Size(1863, 995);
            this.DisplayTextBox.TabIndex = 1;
            this.DisplayTextBox.Text = "";
            // 
            // EnterButton
            // 
            this.EnterButton.BackColor = System.Drawing.SystemColors.Window;
            this.EnterButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.EnterButton.Location = new System.Drawing.Point(1700, 1013);
            this.EnterButton.Name = "EnterButton";
            this.EnterButton.Size = new System.Drawing.Size(175, 43);
            this.EnterButton.TabIndex = 2;
            this.EnterButton.Text = "Enter";
            this.EnterButton.UseVisualStyleBackColor = false;
            this.EnterButton.Click += new System.EventHandler(this.EnterButton_Click);
            // 
            // ClearTextButton
            // 
            this.ClearTextButton.Location = new System.Drawing.Point(1512, 1013);
            this.ClearTextButton.Name = "ClearTextButton";
            this.ClearTextButton.Size = new System.Drawing.Size(182, 43);
            this.ClearTextButton.TabIndex = 3;
            this.ClearTextButton.Text = "Clear";
            this.ClearTextButton.UseVisualStyleBackColor = true;
            this.ClearTextButton.Click += new System.EventHandler(this.ClearTextButton_Click);
            // 
            // ClearDisplayButton
            // 
            this.ClearDisplayButton.Location = new System.Drawing.Point(12, 1013);
            this.ClearDisplayButton.Name = "ClearDisplayButton";
            this.ClearDisplayButton.Size = new System.Drawing.Size(194, 43);
            this.ClearDisplayButton.TabIndex = 4;
            this.ClearDisplayButton.Text = "Clear History";
            this.ClearDisplayButton.UseVisualStyleBackColor = true;
            this.ClearDisplayButton.Click += new System.EventHandler(this.ClearDisplayButton_Click);
            // 
            // DialogueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1887, 1404);
            this.Controls.Add(this.ClearDisplayButton);
            this.Controls.Add(this.ClearTextButton);
            this.Controls.Add(this.EnterButton);
            this.Controls.Add(this.DisplayTextBox);
            this.Controls.Add(this.InputTextBox);
            this.Name = "DialogueForm";
            this.Text = "DialogueForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox InputTextBox;
        private System.Windows.Forms.RichTextBox DisplayTextBox;
        private System.Windows.Forms.Button EnterButton;
        private System.Windows.Forms.Button ClearTextButton;
        private System.Windows.Forms.Button ClearDisplayButton;
    }
}
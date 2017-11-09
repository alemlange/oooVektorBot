namespace LiteDBManagementStudio
{
    partial class Main
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
            this.DBTreeView = new System.Windows.Forms.TreeView();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.DBGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.DBGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // DBTreeView
            // 
            this.DBTreeView.Location = new System.Drawing.Point(12, 35);
            this.DBTreeView.Name = "DBTreeView";
            this.DBTreeView.Size = new System.Drawing.Size(165, 334);
            this.DBTreeView.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(829, 35);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 1;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // DBGridView
            // 
            this.DBGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DBGridView.Location = new System.Drawing.Point(183, 35);
            this.DBGridView.Name = "DBGridView";
            this.DBGridView.Size = new System.Drawing.Size(640, 334);
            this.DBGridView.TabIndex = 2;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 433);
            this.Controls.Add(this.DBGridView);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.DBTreeView);
            this.Name = "Main";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.DBGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView DBTreeView;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.DataGridView DBGridView;
    }
}


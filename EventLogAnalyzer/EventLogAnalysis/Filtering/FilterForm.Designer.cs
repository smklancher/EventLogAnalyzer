namespace EventLogAnalysis.Filtering
{
    partial class FilterForm
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
            this.ColumnDropdown = new System.Windows.Forms.ComboBox();
            this.RelationDropdown = new System.Windows.Forms.ComboBox();
            this.ValueDropdown = new System.Windows.Forms.ComboBox();
            this.ActionDropdown = new System.Windows.Forms.ComboBox();
            this.ThenLabel = new System.Windows.Forms.Label();
            this.ResetButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.FilterList = new System.Windows.Forms.ListView();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ColumnDropdown
            // 
            this.ColumnDropdown.FormattingEnabled = true;
            this.ColumnDropdown.Location = new System.Drawing.Point(12, 12);
            this.ColumnDropdown.Name = "ColumnDropdown";
            this.ColumnDropdown.Size = new System.Drawing.Size(155, 21);
            this.ColumnDropdown.TabIndex = 0;
            // 
            // RelationDropdown
            // 
            this.RelationDropdown.FormattingEnabled = true;
            this.RelationDropdown.Location = new System.Drawing.Point(173, 12);
            this.RelationDropdown.Name = "RelationDropdown";
            this.RelationDropdown.Size = new System.Drawing.Size(87, 21);
            this.RelationDropdown.TabIndex = 1;
            // 
            // ValueDropdown
            // 
            this.ValueDropdown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ValueDropdown.FormattingEnabled = true;
            this.ValueDropdown.Location = new System.Drawing.Point(266, 12);
            this.ValueDropdown.Name = "ValueDropdown";
            this.ValueDropdown.Size = new System.Drawing.Size(362, 21);
            this.ValueDropdown.TabIndex = 2;
            // 
            // ActionDropdown
            // 
            this.ActionDropdown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ActionDropdown.FormattingEnabled = true;
            this.ActionDropdown.Location = new System.Drawing.Point(668, 12);
            this.ActionDropdown.Name = "ActionDropdown";
            this.ActionDropdown.Size = new System.Drawing.Size(120, 21);
            this.ActionDropdown.TabIndex = 3;
            // 
            // ThenLabel
            // 
            this.ThenLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ThenLabel.AutoSize = true;
            this.ThenLabel.Location = new System.Drawing.Point(634, 15);
            this.ThenLabel.Name = "ThenLabel";
            this.ThenLabel.Size = new System.Drawing.Size(28, 13);
            this.ThenLabel.TabIndex = 4;
            this.ThenLabel.Text = "then";
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(12, 39);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(75, 23);
            this.ResetButton.TabIndex = 5;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            // 
            // AddButton
            // 
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddButton.Location = new System.Drawing.Point(632, 39);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(75, 23);
            this.AddButton.TabIndex = 6;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            // 
            // RemoveButton
            // 
            this.RemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveButton.Location = new System.Drawing.Point(713, 39);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(75, 23);
            this.RemoveButton.TabIndex = 7;
            this.RemoveButton.Text = "Remove";
            this.RemoveButton.UseVisualStyleBackColor = true;
            // 
            // FilterList
            // 
            this.FilterList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterList.CheckBoxes = true;
            this.FilterList.HideSelection = false;
            this.FilterList.Location = new System.Drawing.Point(12, 68);
            this.FilterList.Name = "FilterList";
            this.FilterList.Size = new System.Drawing.Size(776, 331);
            this.FilterList.TabIndex = 8;
            this.FilterList.UseCompatibleStateImageBehavior = false;
            this.FilterList.View = System.Windows.Forms.View.Details;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(553, 415);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 9;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(632, 415);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 10;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // ApplyButton
            // 
            this.ApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ApplyButton.Location = new System.Drawing.Point(713, 415);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyButton.TabIndex = 11;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            // 
            // FilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelButton;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.FilterList);
            this.Controls.Add(this.RemoveButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.ThenLabel);
            this.Controls.Add(this.ActionDropdown);
            this.Controls.Add(this.ValueDropdown);
            this.Controls.Add(this.RelationDropdown);
            this.Controls.Add(this.ColumnDropdown);
            this.Name = "FilterForm";
            this.Text = "FilterForm";
            this.Load += new System.EventHandler(this.FilterForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComboBox ColumnDropdown;
        private ComboBox RelationDropdown;
        private ComboBox ValueDropdown;
        private ComboBox ActionDropdown;
        private Label ThenLabel;
        private Button ResetButton;
        private Button AddButton;
        private Button RemoveButton;
        private ListView FilterList;
        private Button OKButton;
        private Button CancelButton;
        private Button ApplyButton;
    }
}
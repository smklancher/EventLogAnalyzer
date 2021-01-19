
using System.Windows.Forms;

namespace EventLogAnalyzer
{
    partial class EventLogAnalyzer
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
            this.components = new System.ComponentModel.Container();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.lstIndexType = new System.Windows.Forms.ListView();
            this.lstIndex = new System.Windows.Forms.ListView();
            this.lstLines = new System.Windows.Forms.ListView();
            this.SplitFilesAndRest = new System.Windows.Forms.SplitContainer();
            this.lstFiles = new System.Windows.Forms.ListView();
            this.SplitIndexTypeValueAndLinesDetail = new System.Windows.Forms.SplitContainer();
            this.SplitIndexTypesAndIndexValues = new System.Windows.Forms.SplitContainer();
            this.SplitLinesAndDetail = new System.Windows.Forms.SplitContainer();
            this.SplitDetailAndProperties = new System.Windows.Forms.SplitContainer();
            this.txtDetail = new System.Windows.Forms.TextBox();
            this.DebugProperties = new System.Windows.Forms.PropertyGrid();
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.LogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DumpToCombinedCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveCurrentLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveCurrentIndiciesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.cmdRefresh = new System.Windows.Forms.Button();
            this.MessageSearchTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.SplitFilesAndRest)).BeginInit();
            this.SplitFilesAndRest.Panel1.SuspendLayout();
            this.SplitFilesAndRest.Panel2.SuspendLayout();
            this.SplitFilesAndRest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitIndexTypeValueAndLinesDetail)).BeginInit();
            this.SplitIndexTypeValueAndLinesDetail.Panel1.SuspendLayout();
            this.SplitIndexTypeValueAndLinesDetail.Panel2.SuspendLayout();
            this.SplitIndexTypeValueAndLinesDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitIndexTypesAndIndexValues)).BeginInit();
            this.SplitIndexTypesAndIndexValues.Panel1.SuspendLayout();
            this.SplitIndexTypesAndIndexValues.Panel2.SuspendLayout();
            this.SplitIndexTypesAndIndexValues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitLinesAndDetail)).BeginInit();
            this.SplitLinesAndDetail.Panel1.SuspendLayout();
            this.SplitLinesAndDetail.Panel2.SuspendLayout();
            this.SplitLinesAndDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitDetailAndProperties)).BeginInit();
            this.SplitDetailAndProperties.Panel1.SuspendLayout();
            this.SplitDetailAndProperties.Panel2.SuspendLayout();
            this.SplitDetailAndProperties.SuspendLayout();
            this.MenuStrip1.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Timer1
            // 
            this.Timer1.Enabled = true;
            this.Timer1.Interval = 60000;
            // 
            // lstIndexType
            // 
            this.lstIndexType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstIndexType.HideSelection = false;
            this.lstIndexType.Location = new System.Drawing.Point(0, 0);
            this.lstIndexType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lstIndexType.Name = "lstIndexType";
            this.lstIndexType.Size = new System.Drawing.Size(248, 121);
            this.lstIndexType.TabIndex = 4;
            this.lstIndexType.UseCompatibleStateImageBehavior = false;
            this.lstIndexType.View = System.Windows.Forms.View.List;
            // 
            // lstIndex
            // 
            this.lstIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstIndex.HideSelection = false;
            this.lstIndex.Location = new System.Drawing.Point(0, 0);
            this.lstIndex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lstIndex.Name = "lstIndex";
            this.lstIndex.Size = new System.Drawing.Size(248, 320);
            this.lstIndex.TabIndex = 5;
            this.lstIndex.UseCompatibleStateImageBehavior = false;
            this.lstIndex.View = System.Windows.Forms.View.Details;
            // 
            // lstLines
            // 
            this.lstLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLines.HideSelection = false;
            this.lstLines.Location = new System.Drawing.Point(0, 0);
            this.lstLines.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lstLines.Name = "lstLines";
            this.lstLines.Size = new System.Drawing.Size(494, 259);
            this.lstLines.TabIndex = 6;
            this.lstLines.UseCompatibleStateImageBehavior = false;
            // 
            // SplitFilesAndRest
            // 
            this.SplitFilesAndRest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitFilesAndRest.Location = new System.Drawing.Point(14, 68);
            this.SplitFilesAndRest.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SplitFilesAndRest.Name = "SplitFilesAndRest";
            this.SplitFilesAndRest.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitFilesAndRest.Panel1
            // 
            this.SplitFilesAndRest.Panel1.Controls.Add(this.lstFiles);
            // 
            // SplitFilesAndRest.Panel2
            // 
            this.SplitFilesAndRest.Panel2.Controls.Add(this.SplitIndexTypeValueAndLinesDetail);
            this.SplitFilesAndRest.Size = new System.Drawing.Size(747, 528);
            this.SplitFilesAndRest.SplitterDistance = 77;
            this.SplitFilesAndRest.SplitterWidth = 5;
            this.SplitFilesAndRest.TabIndex = 7;
            // 
            // lstFiles
            // 
            this.lstFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFiles.HideSelection = false;
            this.lstFiles.Location = new System.Drawing.Point(0, 0);
            this.lstFiles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(747, 77);
            this.lstFiles.TabIndex = 0;
            this.lstFiles.UseCompatibleStateImageBehavior = false;
            this.lstFiles.View = System.Windows.Forms.View.Details;
            // 
            // SplitIndexTypeValueAndLinesDetail
            // 
            this.SplitIndexTypeValueAndLinesDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitIndexTypeValueAndLinesDetail.Location = new System.Drawing.Point(0, 0);
            this.SplitIndexTypeValueAndLinesDetail.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SplitIndexTypeValueAndLinesDetail.Name = "SplitIndexTypeValueAndLinesDetail";
            // 
            // SplitIndexTypeValueAndLinesDetail.Panel1
            // 
            this.SplitIndexTypeValueAndLinesDetail.Panel1.Controls.Add(this.SplitIndexTypesAndIndexValues);
            // 
            // SplitIndexTypeValueAndLinesDetail.Panel2
            // 
            this.SplitIndexTypeValueAndLinesDetail.Panel2.Controls.Add(this.SplitLinesAndDetail);
            this.SplitIndexTypeValueAndLinesDetail.Size = new System.Drawing.Size(747, 446);
            this.SplitIndexTypeValueAndLinesDetail.SplitterDistance = 248;
            this.SplitIndexTypeValueAndLinesDetail.SplitterWidth = 5;
            this.SplitIndexTypeValueAndLinesDetail.TabIndex = 0;
            // 
            // SplitIndexTypesAndIndexValues
            // 
            this.SplitIndexTypesAndIndexValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitIndexTypesAndIndexValues.Location = new System.Drawing.Point(0, 0);
            this.SplitIndexTypesAndIndexValues.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SplitIndexTypesAndIndexValues.Name = "SplitIndexTypesAndIndexValues";
            this.SplitIndexTypesAndIndexValues.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitIndexTypesAndIndexValues.Panel1
            // 
            this.SplitIndexTypesAndIndexValues.Panel1.Controls.Add(this.lstIndexType);
            // 
            // SplitIndexTypesAndIndexValues.Panel2
            // 
            this.SplitIndexTypesAndIndexValues.Panel2.Controls.Add(this.lstIndex);
            this.SplitIndexTypesAndIndexValues.Size = new System.Drawing.Size(248, 446);
            this.SplitIndexTypesAndIndexValues.SplitterDistance = 121;
            this.SplitIndexTypesAndIndexValues.SplitterWidth = 5;
            this.SplitIndexTypesAndIndexValues.TabIndex = 0;
            // 
            // SplitLinesAndDetail
            // 
            this.SplitLinesAndDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitLinesAndDetail.Location = new System.Drawing.Point(0, 0);
            this.SplitLinesAndDetail.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SplitLinesAndDetail.Name = "SplitLinesAndDetail";
            this.SplitLinesAndDetail.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitLinesAndDetail.Panel1
            // 
            this.SplitLinesAndDetail.Panel1.Controls.Add(this.lstLines);
            // 
            // SplitLinesAndDetail.Panel2
            // 
            this.SplitLinesAndDetail.Panel2.Controls.Add(this.SplitDetailAndProperties);
            this.SplitLinesAndDetail.Size = new System.Drawing.Size(494, 446);
            this.SplitLinesAndDetail.SplitterDistance = 259;
            this.SplitLinesAndDetail.SplitterWidth = 5;
            this.SplitLinesAndDetail.TabIndex = 7;
            // 
            // SplitDetailAndProperties
            // 
            this.SplitDetailAndProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitDetailAndProperties.Location = new System.Drawing.Point(0, 0);
            this.SplitDetailAndProperties.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SplitDetailAndProperties.Name = "SplitDetailAndProperties";
            // 
            // SplitDetailAndProperties.Panel1
            // 
            this.SplitDetailAndProperties.Panel1.Controls.Add(this.txtDetail);
            // 
            // SplitDetailAndProperties.Panel2
            // 
            this.SplitDetailAndProperties.Panel2.Controls.Add(this.DebugProperties);
            this.SplitDetailAndProperties.Size = new System.Drawing.Size(494, 182);
            this.SplitDetailAndProperties.SplitterDistance = 164;
            this.SplitDetailAndProperties.SplitterWidth = 5;
            this.SplitDetailAndProperties.TabIndex = 1;
            // 
            // txtDetail
            // 
            this.txtDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDetail.Location = new System.Drawing.Point(0, 0);
            this.txtDetail.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtDetail.Multiline = true;
            this.txtDetail.Name = "txtDetail";
            this.txtDetail.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDetail.Size = new System.Drawing.Size(164, 182);
            this.txtDetail.TabIndex = 0;
            // 
            // DebugProperties
            // 
            this.DebugProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DebugProperties.LineColor = System.Drawing.SystemColors.ControlDark;
            this.DebugProperties.Location = new System.Drawing.Point(0, 0);
            this.DebugProperties.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.DebugProperties.Name = "DebugProperties";
            this.DebugProperties.Size = new System.Drawing.Size(325, 182);
            this.DebugProperties.TabIndex = 0;
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LogsToolStripMenuItem,
            this.ViewToolStripMenuItem});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.MenuStrip1.Size = new System.Drawing.Size(775, 24);
            this.MenuStrip1.TabIndex = 9;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // LogsToolStripMenuItem
            // 
            this.LogsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClearToolStripMenuItem,
            this.DumpToCombinedCSVToolStripMenuItem});
            this.LogsToolStripMenuItem.Name = "LogsToolStripMenuItem";
            this.LogsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.LogsToolStripMenuItem.Text = "Logs";
            // 
            // ClearToolStripMenuItem
            // 
            this.ClearToolStripMenuItem.Enabled = false;
            this.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem";
            this.ClearToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.ClearToolStripMenuItem.Text = "Clear";
            // 
            // DumpToCombinedCSVToolStripMenuItem
            // 
            this.DumpToCombinedCSVToolStripMenuItem.Name = "DumpToCombinedCSVToolStripMenuItem";
            this.DumpToCombinedCSVToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.DumpToCombinedCSVToolStripMenuItem.Text = "Combined to CSV";
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveCurrentLinesToolStripMenuItem,
            this.SaveCurrentIndiciesToolStripMenuItem});
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.ViewToolStripMenuItem.Text = "View";
            // 
            // SaveCurrentLinesToolStripMenuItem
            // 
            this.SaveCurrentLinesToolStripMenuItem.Name = "SaveCurrentLinesToolStripMenuItem";
            this.SaveCurrentLinesToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.SaveCurrentLinesToolStripMenuItem.Text = "Save Current Lines";
            // 
            // SaveCurrentIndiciesToolStripMenuItem
            // 
            this.SaveCurrentIndiciesToolStripMenuItem.Name = "SaveCurrentIndiciesToolStripMenuItem";
            this.SaveCurrentIndiciesToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.SaveCurrentIndiciesToolStripMenuItem.Text = "Save Current Indicies";
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel1,
            this.ToolStripProgressBar1});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 601);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.StatusStrip1.Size = new System.Drawing.Size(775, 24);
            this.StatusStrip1.TabIndex = 10;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // ToolStripStatusLabel1
            // 
            this.ToolStripStatusLabel1.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
            this.ToolStripStatusLabel1.Size = new System.Drawing.Size(639, 19);
            this.ToolStripStatusLabel1.Spring = true;
            this.ToolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ToolStripProgressBar1
            // 
            this.ToolStripProgressBar1.MarqueeAnimationSpeed = 0;
            this.ToolStripProgressBar1.Name = "ToolStripProgressBar1";
            this.ToolStripProgressBar1.Size = new System.Drawing.Size(117, 18);
            this.ToolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // cmdRefresh
            // 
            this.cmdRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRefresh.Location = new System.Drawing.Point(673, 31);
            this.cmdRefresh.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmdRefresh.Name = "cmdRefresh";
            this.cmdRefresh.Size = new System.Drawing.Size(88, 27);
            this.cmdRefresh.TabIndex = 11;
            this.cmdRefresh.Text = "Refresh";
            this.cmdRefresh.UseVisualStyleBackColor = true;
            // 
            // MessageSearchTextBox
            // 
            this.MessageSearchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageSearchTextBox.Location = new System.Drawing.Point(454, 33);
            this.MessageSearchTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MessageSearchTextBox.Name = "MessageSearchTextBox";
            this.MessageSearchTextBox.Size = new System.Drawing.Size(212, 23);
            this.MessageSearchTextBox.TabIndex = 12;
            // 
            // EventLogAnalyzer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 625);
            this.Controls.Add(this.MessageSearchTextBox);
            this.Controls.Add(this.cmdRefresh);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.SplitFilesAndRest);
            this.Controls.Add(this.MenuStrip1);
            this.MainMenuStrip = this.MenuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "EventLogAnalyzer";
            this.Text = "Universal Log Parser";
            this.Load += new System.EventHandler(this.EventLogAnalyzer_Load);
            this.Shown += new System.EventHandler(this.EventLogAnalyzer_ShownAsync);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.EventLogAnalyzer_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.EventLogAnalyzer_DragEnter);
            this.SplitFilesAndRest.Panel1.ResumeLayout(false);
            this.SplitFilesAndRest.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitFilesAndRest)).EndInit();
            this.SplitFilesAndRest.ResumeLayout(false);
            this.SplitIndexTypeValueAndLinesDetail.Panel1.ResumeLayout(false);
            this.SplitIndexTypeValueAndLinesDetail.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitIndexTypeValueAndLinesDetail)).EndInit();
            this.SplitIndexTypeValueAndLinesDetail.ResumeLayout(false);
            this.SplitIndexTypesAndIndexValues.Panel1.ResumeLayout(false);
            this.SplitIndexTypesAndIndexValues.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitIndexTypesAndIndexValues)).EndInit();
            this.SplitIndexTypesAndIndexValues.ResumeLayout(false);
            this.SplitLinesAndDetail.Panel1.ResumeLayout(false);
            this.SplitLinesAndDetail.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitLinesAndDetail)).EndInit();
            this.SplitLinesAndDetail.ResumeLayout(false);
            this.SplitDetailAndProperties.Panel1.ResumeLayout(false);
            this.SplitDetailAndProperties.Panel1.PerformLayout();
            this.SplitDetailAndProperties.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitDetailAndProperties)).EndInit();
            this.SplitDetailAndProperties.ResumeLayout(false);
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Timer Timer1;
        internal System.Windows.Forms.ListView lstIndexType;
        internal System.Windows.Forms.ListView lstIndex;
        internal System.Windows.Forms.ListView lstLines;
        internal System.Windows.Forms.SplitContainer SplitFilesAndRest;
        internal System.Windows.Forms.ListView lstFiles;
        internal System.Windows.Forms.SplitContainer SplitIndexTypeValueAndLinesDetail;
        internal System.Windows.Forms.SplitContainer SplitIndexTypesAndIndexValues;
        internal System.Windows.Forms.SplitContainer SplitLinesAndDetail;
        internal System.Windows.Forms.TextBox txtDetail;
        internal System.Windows.Forms.SplitContainer SplitDetailAndProperties;
        internal System.Windows.Forms.PropertyGrid DebugProperties;
        internal System.Windows.Forms.MenuStrip MenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem LogsToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ClearToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SaveCurrentLinesToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SaveCurrentIndiciesToolStripMenuItem;
        internal System.Windows.Forms.StatusStrip StatusStrip1;
        internal System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel1;
        internal System.Windows.Forms.ToolStripProgressBar ToolStripProgressBar1;
        internal System.Windows.Forms.Button cmdRefresh;
        internal System.Windows.Forms.ToolStripMenuItem DumpToCombinedCSVToolStripMenuItem;
        internal TextBox MessageSearchTextBox;

        #endregion
    }
}
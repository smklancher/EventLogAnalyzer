
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
            this.lstTraitTypes = new System.Windows.Forms.ListView();
            this.lstTraitValues = new System.Windows.Forms.ListView();
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
            this.loadEventLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveCurrentLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveCurrentIndiciesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timestampAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localTimeEventLogDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uTCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specificUTCOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specificLocalOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filtersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.MessageSearchTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SearchExcludeTextBox = new System.Windows.Forms.TextBox();
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
            // lstTraitTypes
            // 
            this.lstTraitTypes.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstTraitTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTraitTypes.HideSelection = false;
            this.lstTraitTypes.Location = new System.Drawing.Point(0, 0);
            this.lstTraitTypes.Name = "lstTraitTypes";
            this.lstTraitTypes.Size = new System.Drawing.Size(202, 105);
            this.lstTraitTypes.TabIndex = 4;
            this.lstTraitTypes.UseCompatibleStateImageBehavior = false;
            this.lstTraitTypes.View = System.Windows.Forms.View.List;
            // 
            // lstTraitValues
            // 
            this.lstTraitValues.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstTraitValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTraitValues.HideSelection = false;
            this.lstTraitValues.Location = new System.Drawing.Point(0, 0);
            this.lstTraitValues.Name = "lstTraitValues";
            this.lstTraitValues.Size = new System.Drawing.Size(202, 279);
            this.lstTraitValues.TabIndex = 5;
            this.lstTraitValues.UseCompatibleStateImageBehavior = false;
            this.lstTraitValues.View = System.Windows.Forms.View.Details;
            // 
            // lstLines
            // 
            this.lstLines.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLines.HideSelection = false;
            this.lstLines.Location = new System.Drawing.Point(0, 0);
            this.lstLines.Name = "lstLines";
            this.lstLines.Size = new System.Drawing.Size(718, 225);
            this.lstLines.TabIndex = 6;
            this.lstLines.UseCompatibleStateImageBehavior = false;
            // 
            // SplitFilesAndRest
            // 
            this.SplitFilesAndRest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitFilesAndRest.Location = new System.Drawing.Point(12, 59);
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
            this.SplitFilesAndRest.Size = new System.Drawing.Size(924, 458);
            this.SplitFilesAndRest.SplitterDistance = 66;
            this.SplitFilesAndRest.TabIndex = 7;
            // 
            // lstFiles
            // 
            this.lstFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFiles.HideSelection = false;
            this.lstFiles.Location = new System.Drawing.Point(0, 0);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(924, 66);
            this.lstFiles.TabIndex = 0;
            this.lstFiles.UseCompatibleStateImageBehavior = false;
            this.lstFiles.View = System.Windows.Forms.View.Details;
            // 
            // SplitIndexTypeValueAndLinesDetail
            // 
            this.SplitIndexTypeValueAndLinesDetail.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.SplitIndexTypeValueAndLinesDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitIndexTypeValueAndLinesDetail.Location = new System.Drawing.Point(0, 0);
            this.SplitIndexTypeValueAndLinesDetail.Name = "SplitIndexTypeValueAndLinesDetail";
            // 
            // SplitIndexTypeValueAndLinesDetail.Panel1
            // 
            this.SplitIndexTypeValueAndLinesDetail.Panel1.Controls.Add(this.SplitIndexTypesAndIndexValues);
            // 
            // SplitIndexTypeValueAndLinesDetail.Panel2
            // 
            this.SplitIndexTypeValueAndLinesDetail.Panel2.Controls.Add(this.SplitLinesAndDetail);
            this.SplitIndexTypeValueAndLinesDetail.Size = new System.Drawing.Size(924, 388);
            this.SplitIndexTypeValueAndLinesDetail.SplitterDistance = 202;
            this.SplitIndexTypeValueAndLinesDetail.TabIndex = 0;
            // 
            // SplitIndexTypesAndIndexValues
            // 
            this.SplitIndexTypesAndIndexValues.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.SplitIndexTypesAndIndexValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitIndexTypesAndIndexValues.Location = new System.Drawing.Point(0, 0);
            this.SplitIndexTypesAndIndexValues.Name = "SplitIndexTypesAndIndexValues";
            this.SplitIndexTypesAndIndexValues.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitIndexTypesAndIndexValues.Panel1
            // 
            this.SplitIndexTypesAndIndexValues.Panel1.Controls.Add(this.lstTraitTypes);
            // 
            // SplitIndexTypesAndIndexValues.Panel2
            // 
            this.SplitIndexTypesAndIndexValues.Panel2.Controls.Add(this.lstTraitValues);
            this.SplitIndexTypesAndIndexValues.Size = new System.Drawing.Size(202, 388);
            this.SplitIndexTypesAndIndexValues.SplitterDistance = 105;
            this.SplitIndexTypesAndIndexValues.TabIndex = 0;
            // 
            // SplitLinesAndDetail
            // 
            this.SplitLinesAndDetail.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.SplitLinesAndDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitLinesAndDetail.Location = new System.Drawing.Point(0, 0);
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
            this.SplitLinesAndDetail.Size = new System.Drawing.Size(718, 388);
            this.SplitLinesAndDetail.SplitterDistance = 225;
            this.SplitLinesAndDetail.TabIndex = 7;
            // 
            // SplitDetailAndProperties
            // 
            this.SplitDetailAndProperties.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.SplitDetailAndProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitDetailAndProperties.Location = new System.Drawing.Point(0, 0);
            this.SplitDetailAndProperties.Name = "SplitDetailAndProperties";
            // 
            // SplitDetailAndProperties.Panel1
            // 
            this.SplitDetailAndProperties.Panel1.Controls.Add(this.txtDetail);
            // 
            // SplitDetailAndProperties.Panel2
            // 
            this.SplitDetailAndProperties.Panel2.Controls.Add(this.DebugProperties);
            this.SplitDetailAndProperties.Size = new System.Drawing.Size(718, 159);
            this.SplitDetailAndProperties.SplitterDistance = 498;
            this.SplitDetailAndProperties.TabIndex = 1;
            // 
            // txtDetail
            // 
            this.txtDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDetail.Location = new System.Drawing.Point(0, 0);
            this.txtDetail.Multiline = true;
            this.txtDetail.Name = "txtDetail";
            this.txtDetail.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDetail.Size = new System.Drawing.Size(498, 159);
            this.txtDetail.TabIndex = 0;
            // 
            // DebugProperties
            // 
            this.DebugProperties.Cursor = System.Windows.Forms.Cursors.Default;
            this.DebugProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DebugProperties.LineColor = System.Drawing.SystemColors.ControlDark;
            this.DebugProperties.Location = new System.Drawing.Point(0, 0);
            this.DebugProperties.Name = "DebugProperties";
            this.DebugProperties.Size = new System.Drawing.Size(216, 159);
            this.DebugProperties.TabIndex = 0;
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LogsToolStripMenuItem,
            this.ViewToolStripMenuItem});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Size = new System.Drawing.Size(948, 24);
            this.MenuStrip1.TabIndex = 9;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // LogsToolStripMenuItem
            // 
            this.LogsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClearToolStripMenuItem,
            this.DumpToCombinedCSVToolStripMenuItem,
            this.loadEventLogToolStripMenuItem});
            this.LogsToolStripMenuItem.Name = "LogsToolStripMenuItem";
            this.LogsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.LogsToolStripMenuItem.Text = "Logs";
            this.LogsToolStripMenuItem.Visible = false;
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
            this.DumpToCombinedCSVToolStripMenuItem.Enabled = false;
            this.DumpToCombinedCSVToolStripMenuItem.Name = "DumpToCombinedCSVToolStripMenuItem";
            this.DumpToCombinedCSVToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.DumpToCombinedCSVToolStripMenuItem.Text = "Combined to CSV";
            // 
            // loadEventLogToolStripMenuItem
            // 
            this.loadEventLogToolStripMenuItem.Name = "loadEventLogToolStripMenuItem";
            this.loadEventLogToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.loadEventLogToolStripMenuItem.Text = "Load Event Log...";
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveCurrentLinesToolStripMenuItem,
            this.SaveCurrentIndiciesToolStripMenuItem,
            this.OptionsMenuItem,
            this.toggleLineToolStripMenuItem,
            this.timestampAsToolStripMenuItem,
            this.filtersToolStripMenuItem});
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.ViewToolStripMenuItem.Text = "View";
            // 
            // SaveCurrentLinesToolStripMenuItem
            // 
            this.SaveCurrentLinesToolStripMenuItem.Enabled = false;
            this.SaveCurrentLinesToolStripMenuItem.Name = "SaveCurrentLinesToolStripMenuItem";
            this.SaveCurrentLinesToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.SaveCurrentLinesToolStripMenuItem.Text = "Save Current Lines";
            this.SaveCurrentLinesToolStripMenuItem.Visible = false;
            // 
            // SaveCurrentIndiciesToolStripMenuItem
            // 
            this.SaveCurrentIndiciesToolStripMenuItem.Enabled = false;
            this.SaveCurrentIndiciesToolStripMenuItem.Name = "SaveCurrentIndiciesToolStripMenuItem";
            this.SaveCurrentIndiciesToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.SaveCurrentIndiciesToolStripMenuItem.Text = "Save Current Indicies";
            this.SaveCurrentIndiciesToolStripMenuItem.Visible = false;
            // 
            // OptionsMenuItem
            // 
            this.OptionsMenuItem.Name = "OptionsMenuItem";
            this.OptionsMenuItem.Size = new System.Drawing.Size(210, 22);
            this.OptionsMenuItem.Text = "Options";
            this.OptionsMenuItem.Click += new System.EventHandler(this.OptionsMenuItem_Click);
            // 
            // toggleLineToolStripMenuItem
            // 
            this.toggleLineToolStripMenuItem.Name = "toggleLineToolStripMenuItem";
            this.toggleLineToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.toggleLineToolStripMenuItem.Text = "Toggle Line Property View";
            this.toggleLineToolStripMenuItem.Click += new System.EventHandler(this.toggleLineToolStripMenuItem_Click);
            // 
            // timestampAsToolStripMenuItem
            // 
            this.timestampAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.localTimeEventLogDefaultToolStripMenuItem,
            this.uTCToolStripMenuItem,
            this.specificUTCOffsetToolStripMenuItem,
            this.specificLocalOffsetToolStripMenuItem});
            this.timestampAsToolStripMenuItem.Name = "timestampAsToolStripMenuItem";
            this.timestampAsToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.timestampAsToolStripMenuItem.Text = "Timestamp as...";
            // 
            // localTimeEventLogDefaultToolStripMenuItem
            // 
            this.localTimeEventLogDefaultToolStripMenuItem.Checked = true;
            this.localTimeEventLogDefaultToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.localTimeEventLogDefaultToolStripMenuItem.Name = "localTimeEventLogDefaultToolStripMenuItem";
            this.localTimeEventLogDefaultToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.localTimeEventLogDefaultToolStripMenuItem.Text = "Local time (Event Log default)";
            this.localTimeEventLogDefaultToolStripMenuItem.Click += new System.EventHandler(this.localTimeEventLogDefaultToolStripMenuItem_Click);
            // 
            // uTCToolStripMenuItem
            // 
            this.uTCToolStripMenuItem.CheckOnClick = true;
            this.uTCToolStripMenuItem.Name = "uTCToolStripMenuItem";
            this.uTCToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.uTCToolStripMenuItem.Text = "UTC";
            this.uTCToolStripMenuItem.Click += new System.EventHandler(this.uTCToolStripMenuItem_Click);
            // 
            // specificUTCOffsetToolStripMenuItem
            // 
            this.specificUTCOffsetToolStripMenuItem.CheckOnClick = true;
            this.specificUTCOffsetToolStripMenuItem.Name = "specificUTCOffsetToolStripMenuItem";
            this.specificUTCOffsetToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.specificUTCOffsetToolStripMenuItem.Text = "Specific UTC offset...";
            this.specificUTCOffsetToolStripMenuItem.Click += new System.EventHandler(this.specificUTCOffsetToolStripMenuItem_Click);
            // 
            // specificLocalOffsetToolStripMenuItem
            // 
            this.specificLocalOffsetToolStripMenuItem.Name = "specificLocalOffsetToolStripMenuItem";
            this.specificLocalOffsetToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.specificLocalOffsetToolStripMenuItem.Text = "Specific local offset...";
            this.specificLocalOffsetToolStripMenuItem.Click += new System.EventHandler(this.specificLocalOffsetToolStripMenuItem_Click);
            // 
            // filtersToolStripMenuItem
            // 
            this.filtersToolStripMenuItem.Name = "filtersToolStripMenuItem";
            this.filtersToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.filtersToolStripMenuItem.Text = "Filters...";
            this.filtersToolStripMenuItem.Click += new System.EventHandler(this.filtersToolStripMenuItem_Click);
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel1,
            this.ToolStripProgressBar1});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 520);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(948, 22);
            this.StatusStrip1.TabIndex = 10;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // ToolStripStatusLabel1
            // 
            this.ToolStripStatusLabel1.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
            this.ToolStripStatusLabel1.Size = new System.Drawing.Size(831, 17);
            this.ToolStripStatusLabel1.Spring = true;
            this.ToolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ToolStripProgressBar1
            // 
            this.ToolStripProgressBar1.MarqueeAnimationSpeed = 0;
            this.ToolStripProgressBar1.Name = "ToolStripProgressBar1";
            this.ToolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.ToolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // MessageSearchTextBox
            // 
            this.MessageSearchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageSearchTextBox.Location = new System.Drawing.Point(107, 29);
            this.MessageSearchTextBox.Name = "MessageSearchTextBox";
            this.MessageSearchTextBox.Size = new System.Drawing.Size(483, 20);
            this.MessageSearchTextBox.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Filter current lines:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(607, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Not:";
            // 
            // SearchExcludeTextBox
            // 
            this.SearchExcludeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchExcludeTextBox.Location = new System.Drawing.Point(640, 29);
            this.SearchExcludeTextBox.Name = "SearchExcludeTextBox";
            this.SearchExcludeTextBox.Size = new System.Drawing.Size(296, 20);
            this.SearchExcludeTextBox.TabIndex = 15;
            // 
            // EventLogAnalyzer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 542);
            this.Controls.Add(this.SearchExcludeTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MessageSearchTextBox);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.SplitFilesAndRest);
            this.Controls.Add(this.MenuStrip1);
            this.MainMenuStrip = this.MenuStrip1;
            this.Name = "EventLogAnalyzer";
            this.Text = "Event Log Analyzer";
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
        internal System.Windows.Forms.ListView lstTraitTypes;
        internal System.Windows.Forms.ListView lstTraitValues;
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
        internal System.Windows.Forms.ToolStripMenuItem DumpToCombinedCSVToolStripMenuItem;
        internal TextBox MessageSearchTextBox;

        #endregion

        private ToolStripMenuItem OptionsMenuItem;
        private Label label1;
        private ToolStripMenuItem toggleLineToolStripMenuItem;
        private ToolStripMenuItem timestampAsToolStripMenuItem;
        private ToolStripMenuItem localTimeEventLogDefaultToolStripMenuItem;
        private ToolStripMenuItem uTCToolStripMenuItem;
        private ToolStripMenuItem specificUTCOffsetToolStripMenuItem;
        private ToolStripMenuItem loadEventLogToolStripMenuItem;
        private ToolStripMenuItem specificLocalOffsetToolStripMenuItem;
        private Label label2;
        private TextBox SearchExcludeTextBox;
        private ToolStripMenuItem filtersToolStripMenuItem;
    }
}
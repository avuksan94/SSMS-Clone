namespace SqlViewer.View
{
    partial class MainForm
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
            this.twServer = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbContent = new System.Windows.Forms.TextBox();
            this.btnExecuteCRUD = new System.Windows.Forms.Button();
            this.dgResults = new System.Windows.Forms.DataGridView();
            this.tbMessages = new System.Windows.Forms.TextBox();
            this.lbMessages = new System.Windows.Forms.Label();
            this.lbInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgResults)).BeginInit();
            this.SuspendLayout();
            // 
            // twServer
            // 
            this.twServer.Location = new System.Drawing.Point(22, 50);
            this.twServer.Margin = new System.Windows.Forms.Padding(1);
            this.twServer.Name = "twServer";
            this.twServer.Size = new System.Drawing.Size(396, 810);
            this.twServer.TabIndex = 0;
            this.twServer.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.TwServer_AfterCollapse);
            this.twServer.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TwServer_BeforeExpand);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.Size = new System.Drawing.Size(1324, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tbContent
            // 
            this.tbContent.Location = new System.Drawing.Point(440, 26);
            this.tbContent.Margin = new System.Windows.Forms.Padding(1);
            this.tbContent.MaxLength = 60000;
            this.tbContent.Multiline = true;
            this.tbContent.Name = "tbContent";
            this.tbContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbContent.Size = new System.Drawing.Size(872, 585);
            this.tbContent.TabIndex = 2;
            // 
            // btnExecuteCRUD
            // 
            this.btnExecuteCRUD.Location = new System.Drawing.Point(440, 625);
            this.btnExecuteCRUD.Name = "btnExecuteCRUD";
            this.btnExecuteCRUD.Size = new System.Drawing.Size(94, 29);
            this.btnExecuteCRUD.TabIndex = 4;
            this.btnExecuteCRUD.Text = "Execute";
            this.btnExecuteCRUD.UseVisualStyleBackColor = true;
            this.btnExecuteCRUD.Click += new System.EventHandler(this.btnExecuteCRUD_Click);
            // 
            // dgResults
            // 
            this.dgResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgResults.Location = new System.Drawing.Point(440, 672);
            this.dgResults.Name = "dgResults";
            this.dgResults.ReadOnly = true;
            this.dgResults.RowHeadersWidth = 51;
            this.dgResults.RowTemplate.Height = 29;
            this.dgResults.Size = new System.Drawing.Size(870, 188);
            this.dgResults.TabIndex = 5;
            // 
            // tbMessages
            // 
            this.tbMessages.Location = new System.Drawing.Point(22, 890);
            this.tbMessages.Margin = new System.Windows.Forms.Padding(1);
            this.tbMessages.Multiline = true;
            this.tbMessages.Name = "tbMessages";
            this.tbMessages.Size = new System.Drawing.Size(1290, 93);
            this.tbMessages.TabIndex = 6;
            // 
            // lbMessages
            // 
            this.lbMessages.AutoSize = true;
            this.lbMessages.Location = new System.Drawing.Point(22, 869);
            this.lbMessages.Name = "lbMessages";
            this.lbMessages.Size = new System.Drawing.Size(76, 20);
            this.lbMessages.TabIndex = 7;
            this.lbMessages.Text = "Messages:";
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = true;
            this.lbInfo.Location = new System.Drawing.Point(22, 29);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(394, 20);
            this.lbInfo.TabIndex = 8;
            this.lbInfo.Text = "Select a database and click on table sub-menu for Queries";
            this.lbInfo.Click += new System.EventHandler(this.lbInfo_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1324, 990);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.lbMessages);
            this.Controls.Add(this.tbMessages);
            this.Controls.Add(this.dgResults);
            this.Controls.Add(this.btnExecuteCRUD);
            this.Controls.Add(this.tbContent);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.twServer);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SSMS";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TreeView twServer;
        private ToolStrip toolStrip1;
        private TextBox tbContent;
        private Button btnExecuteCRUD;
        private DataGridView dgResults;
        private TextBox tbMessages;
        private Label lbMessages;
        private Label lbInfo;
    }
}
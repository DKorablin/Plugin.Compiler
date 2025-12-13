namespace Plugin.Compiler.UI
{
	partial class AssemblyReferenceDlg
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
			if(disposing && (components != null))
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
			System.Windows.Forms.TabPage tabDotnet;
			System.Windows.Forms.TabPage tabGac;
			System.Windows.Forms.ContextMenuStrip cmsGac;
			System.Windows.Forms.TabPage tabPlugins;
			System.Windows.Forms.TabPage tabBrowse;
			System.Windows.Forms.Button bnCancel;
			System.Windows.Forms.Button bnOk;
			this.searchGac = new AlphaOmega.Windows.Forms.SearchGrid();
			this.searchNet = new AlphaOmega.Windows.Forms.SearchGrid();
			this.lvGac = new System.Windows.Forms.ListView();
			this.lvDotnet = new System.Windows.Forms.ListView();
			this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.tvPlugins = new AlphaOmega.Windows.Forms.AssemblyTreeView();
			this.lvBrowse = new System.Windows.Forms.ListView();
			this.colPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cmsBrowse = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiBrowse = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.bgGac = new System.ComponentModel.BackgroundWorker();
			this.bgDotnet = new System.ComponentModel.BackgroundWorker();
			this.tabMain = new System.Windows.Forms.TabControl();
			tabDotnet = new System.Windows.Forms.TabPage();
			tabGac = new System.Windows.Forms.TabPage();
			cmsGac = new System.Windows.Forms.ContextMenuStrip(this.components);
			tabPlugins = new System.Windows.Forms.TabPage();
			tabBrowse = new System.Windows.Forms.TabPage();
			bnCancel = new System.Windows.Forms.Button();
			bnOk = new System.Windows.Forms.Button();
			tabDotnet.SuspendLayout();
			tabGac.SuspendLayout();
			cmsGac.SuspendLayout();
			tabPlugins.SuspendLayout();
			tabBrowse.SuspendLayout();
			this.cmsBrowse.SuspendLayout();
			this.tabMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabDotnet
			// 
			tabDotnet.Controls.Add(this.searchNet);
			tabDotnet.Controls.Add(this.lvDotnet);
			tabDotnet.Location = new System.Drawing.Point(4, 22);
			tabDotnet.Name = "tabDotnet";
			tabDotnet.Padding = new System.Windows.Forms.Padding(3);
			tabDotnet.Size = new System.Drawing.Size(440, 187);
			tabDotnet.TabIndex = 3;
			tabDotnet.Text = ".NET";
			tabDotnet.UseVisualStyleBackColor = true;
			// 
			// searchNet
			// 
			this.searchNet.DataGrid = null;
			this.searchNet.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.searchNet.EnableFindCase = true;
			this.searchNet.EnableFindHighlight = true;
			this.searchNet.EnableFindPrevNext = true;
			this.searchNet.EnableSearchHighlight = false;
			this.searchNet.ListView = null;
			this.searchNet.Location = new System.Drawing.Point(3, 155);
			this.searchNet.Name = "searchNet";
			this.searchNet.Size = new System.Drawing.Size(440, 29);
			this.searchNet.TabIndex = 1;
			this.searchNet.TreeView = null;
			this.searchNet.Visible = false;
			// 
			// tabGac
			// 
			tabGac.Controls.Add(this.searchGac);
			tabGac.Controls.Add(this.lvGac);
			tabGac.Location = new System.Drawing.Point(4, 22);
			tabGac.Name = "tabGac";
			tabGac.Padding = new System.Windows.Forms.Padding(3);
			tabGac.Size = new System.Drawing.Size(440, 187);
			tabGac.TabIndex = 0;
			tabGac.Text = "GAC";
			tabGac.UseVisualStyleBackColor = true;
			// 
			// searchGac
			// 
			this.searchGac.DataGrid = null;
			this.searchGac.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.searchGac.EnableFindCase = true;
			this.searchGac.EnableFindHighlight = true;
			this.searchGac.EnableFindPrevNext = true;
			this.searchGac.EnableSearchHighlight = false;
			this.searchGac.ListView = null;
			this.searchGac.Location = new System.Drawing.Point(3, 155);
			this.searchGac.Name = "searchGac";
			this.searchGac.Size = new System.Drawing.Size(440, 29);
			this.searchGac.TabIndex = 1;
			this.searchGac.TreeView = null;
			this.searchGac.Visible = false;
			// 
			// lvGac
			// 
			this.lvGac.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lvGac.ContextMenuStrip = cmsGac;
			this.lvGac.FullRowSelect = true;
			this.lvGac.HideSelection = false;
			this.lvGac.Location = new System.Drawing.Point(3, 3);
			this.lvGac.Name = "lvGac";
			this.lvGac.Size = new System.Drawing.Size(434, 151);
			this.lvGac.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lvGac.TabIndex = 0;
			this.lvGac.UseCompatibleStateImageBehavior = false;
			this.lvGac.View = System.Windows.Forms.View.Details;
			// 
			// lvDotnet
			// 
			this.lvDotnet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lvDotnet.ContextMenuStrip = cmsGac;
			this.lvDotnet.FullRowSelect = true;
			this.lvDotnet.HideSelection = false;
			this.lvDotnet.Location = new System.Drawing.Point(3, 3);
			this.lvDotnet.Name = "lvDotnet";
			this.lvDotnet.Size = new System.Drawing.Size(434, 151);
			this.lvDotnet.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lvDotnet.TabIndex = 0;
			this.lvDotnet.UseCompatibleStateImageBehavior = false;
			this.lvDotnet.View = System.Windows.Forms.View.Details;
			// 
			// cmsGac
			// 
			cmsGac.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopy});
			cmsGac.Name = "cmsGac";
			cmsGac.Size = new System.Drawing.Size(103, 26);
			cmsGac.Opening += new System.ComponentModel.CancelEventHandler(this.cmsGac_Opening);
			cmsGac.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsGac_ItemClicked);
			// 
			// tsmiCopy
			// 
			this.tsmiCopy.Name = "tsmiCopy";
			this.tsmiCopy.Size = new System.Drawing.Size(102, 22);
			this.tsmiCopy.Text = "&Copy";
			// 
			// tabPlugins
			// 
			tabPlugins.Controls.Add(this.tvPlugins);
			tabPlugins.Location = new System.Drawing.Point(4, 22);
			tabPlugins.Name = "tabPlugins";
			tabPlugins.Padding = new System.Windows.Forms.Padding(3);
			tabPlugins.Size = new System.Drawing.Size(440, 187);
			tabPlugins.TabIndex = 1;
			tabPlugins.Text = "Plugins";
			tabPlugins.UseVisualStyleBackColor = true;
			// 
			// tvPlugins
			// 
			this.tvPlugins.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvPlugins.ImageIndex = 0;
			this.tvPlugins.Location = new System.Drawing.Point(3, 3);
			this.tvPlugins.Name = "tvPlugins";
			this.tvPlugins.SelectedImageIndex = 0;
			this.tvPlugins.ShowNonPublicMembers = false;
			this.tvPlugins.Size = new System.Drawing.Size(434, 181);
			this.tvPlugins.TabIndex = 0;
			// 
			// tabBrowse
			// 
			tabBrowse.Controls.Add(this.lvBrowse);
			tabBrowse.Location = new System.Drawing.Point(4, 22);
			tabBrowse.Name = "tabBrowse";
			tabBrowse.Size = new System.Drawing.Size(440, 187);
			tabBrowse.TabIndex = 2;
			tabBrowse.Text = "Browse";
			tabBrowse.UseVisualStyleBackColor = true;
			// 
			// lvBrowse
			// 
			this.lvBrowse.AllowColumnReorder = true;
			this.lvBrowse.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPath});
			this.lvBrowse.ContextMenuStrip = this.cmsBrowse;
			this.lvBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvBrowse.FullRowSelect = true;
			this.lvBrowse.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvBrowse.Location = new System.Drawing.Point(0, 0);
			this.lvBrowse.Name = "lvBrowse";
			this.lvBrowse.Size = new System.Drawing.Size(440, 187);
			this.lvBrowse.TabIndex = 0;
			this.lvBrowse.UseCompatibleStateImageBehavior = false;
			this.lvBrowse.View = System.Windows.Forms.View.Details;
			// 
			// colPath
			// 
			this.colPath.Text = "Path";
			// 
			// cmsBrowse
			// 
			this.cmsBrowse.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiBrowse,
            this.tsmiRemove});
			this.cmsBrowse.Name = "cmsBrowse";
			this.cmsBrowse.Size = new System.Drawing.Size(153, 70);
			this.cmsBrowse.Opening += new System.ComponentModel.CancelEventHandler(this.cmsBrowse_Opening);
			this.cmsBrowse.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsBrowse_ItemClicked);
			// 
			// tsmiBrowse
			// 
			this.tsmiBrowse.Name = "tsmiBrowse";
			this.tsmiBrowse.Size = new System.Drawing.Size(152, 22);
			this.tsmiBrowse.Text = "&Browse...";
			// 
			// tsmiRemove
			// 
			this.tsmiRemove.Name = "tsmiRemove";
			this.tsmiRemove.Size = new System.Drawing.Size(152, 22);
			this.tsmiRemove.Text = "&Remove";
			// 
			// bnCancel
			// 
			bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			bnCancel.Location = new System.Drawing.Point(385, 231);
			bnCancel.Name = "bnCancel";
			bnCancel.Size = new System.Drawing.Size(75, 23);
			bnCancel.TabIndex = 2;
			bnCancel.Text = "&Cancel";
			bnCancel.UseVisualStyleBackColor = true;
			// 
			// bnOk
			// 
			bnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			bnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			bnOk.Location = new System.Drawing.Point(304, 231);
			bnOk.Name = "bnOk";
			bnOk.Size = new System.Drawing.Size(75, 23);
			bnOk.TabIndex = 1;
			bnOk.Text = "&OK";
			bnOk.UseVisualStyleBackColor = true;
			// 
			// bgGac
			// 
			this.bgGac.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgGac_DoWork);
			this.bgGac.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgGac_RunWorkerCompleted);
			// 
			// bgDotnet
			// 
			this.bgDotnet.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgDotnet_DoWork);
			this.bgDotnet.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgDotnet_RunWorkerCompleted);
			// 
			// tabMain
			// 
			this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabMain.Controls.Add(tabGac);
			this.tabMain.Controls.Add(tabDotnet);
			this.tabMain.Controls.Add(tabPlugins);
			this.tabMain.Controls.Add(tabBrowse);
			this.tabMain.Location = new System.Drawing.Point(12, 12);
			this.tabMain.Name = "tabMain";
			this.tabMain.SelectedIndex = 0;
			this.tabMain.Size = new System.Drawing.Size(448, 213);
			this.tabMain.TabIndex = 0;
			this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
			this.tabMain.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tvAssemblies_KeyDown);
			// 
			// AssemblyReferenceDlg
			// 
			this.AcceptButton = bnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = bnCancel;
			this.ClientSize = new System.Drawing.Size(472, 266);
			this.Controls.Add(bnOk);
			this.Controls.Add(bnCancel);
			this.Controls.Add(this.tabMain);
			this.DoubleBuffered = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(195, 144);
			this.Name = "AssemblyReferenceDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Reference";
			tabDotnet.ResumeLayout(false);
			tabGac.ResumeLayout(false);
			cmsGac.ResumeLayout(false);
			tabPlugins.ResumeLayout(false);
			tabBrowse.ResumeLayout(false);
			this.cmsBrowse.ResumeLayout(false);
			this.tabMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView lvGac;
		private System.Windows.Forms.ListView lvDotnet;
		private System.Windows.Forms.ListView lvBrowse;
		private System.Windows.Forms.TabControl tabMain;
		private AlphaOmega.Windows.Forms.AssemblyTreeView tvPlugins;
		private System.Windows.Forms.ToolStripMenuItem tsmiRemove;
		private System.Windows.Forms.ToolStripMenuItem tsmiBrowse;
		private System.Windows.Forms.ColumnHeader colPath;
		private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
		private AlphaOmega.Windows.Forms.SearchGrid searchGac;
		private AlphaOmega.Windows.Forms.SearchGrid searchNet;
		private System.ComponentModel.BackgroundWorker bgGac;
		private System.ComponentModel.BackgroundWorker bgDotnet;
		private System.Windows.Forms.ContextMenuStrip cmsBrowse;
	}
}
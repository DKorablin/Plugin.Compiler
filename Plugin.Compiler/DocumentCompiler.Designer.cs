namespace Plugin.Compiler
{
	partial class DocumentCompiler
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
			System.Windows.Forms.ContextMenuStrip cmsReference;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentCompiler));
			this.tsmiReferenceAdd = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiReferenceRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiReferenceAddNamespace = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiReferenceRemoveNamespace = new System.Windows.Forms.ToolStripMenuItem();
			this.splitMain = new System.Windows.Forms.SplitContainer();
			this.splitCode = new System.Windows.Forms.SplitContainer();
			this.txtSource = new FastColoredTextBoxNS.FastColoredTextBox();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.tsbnAssemblies = new System.Windows.Forms.ToolStripSplitButton();
			this.tsbnFullSource = new System.Windows.Forms.ToolStripButton();
			this.tsbnDebug = new System.Windows.Forms.ToolStripButton();
			this.tsddlLanguages = new System.Windows.Forms.ToolStripComboBox();
			this.tsbnCompile = new System.Windows.Forms.ToolStripSplitButton();
			this.tsmiCompile = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiBuild = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiProject = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiImport = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiExport = new System.Windows.Forms.ToolStripMenuItem();
			this.tsbnCompilerVersion = new System.Windows.Forms.ToolStripDropDownButton();
			this.lvErrors = new System.Windows.Forms.ListView();
			this.colNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colLine = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cmsErrors = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiErrorsClear = new System.Windows.Forms.ToolStripMenuItem();
			this.ilStatus = new System.Windows.Forms.ImageList(this.components);
			this.ilObjects = new System.Windows.Forms.ImageList(this.components);
			this.txtMethodArgs = new System.Windows.Forms.TextBox();
			this.tvReference = new AlphaOmega.Windows.Forms.AssemblyTreeView();
			cmsReference = new System.Windows.Forms.ContextMenuStrip(this.components);
			cmsReference.SuspendLayout();
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			this.splitCode.Panel1.SuspendLayout();
			this.splitCode.Panel2.SuspendLayout();
			this.splitCode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtSource)).BeginInit();
			this.toolStrip.SuspendLayout();
			this.cmsErrors.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmsReference
			// 
			cmsReference.ImageScalingSize = new System.Drawing.Size(20, 20);
			cmsReference.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiReferenceAdd,
            this.tsmiReferenceRemove,
            this.tsmiReferenceAddNamespace,
            this.tsmiReferenceRemoveNamespace});
			cmsReference.Name = "cmsReference";
			cmsReference.Size = new System.Drawing.Size(215, 100);
			cmsReference.Opening += new System.ComponentModel.CancelEventHandler(this.cmsReference_Opening);
			cmsReference.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsReference_ItemClicked);
			// 
			// tsmiReferenceAdd
			// 
			this.tsmiReferenceAdd.Name = "tsmiReferenceAdd";
			this.tsmiReferenceAdd.Size = new System.Drawing.Size(214, 24);
			this.tsmiReferenceAdd.Text = "Add &Reference...";
			// 
			// tsmiReferenceRemove
			// 
			this.tsmiReferenceRemove.Name = "tsmiReferenceRemove";
			this.tsmiReferenceRemove.Size = new System.Drawing.Size(214, 24);
			this.tsmiReferenceRemove.Text = "&Remove Reference";
			// 
			// tsmiReferenceAddNamespace
			// 
			this.tsmiReferenceAddNamespace.Name = "tsmiReferenceAddNamespace";
			this.tsmiReferenceAddNamespace.Size = new System.Drawing.Size(214, 24);
			this.tsmiReferenceAddNamespace.Text = "Add &Namespace";
			// 
			// tsmiReferenceRemoveNamespace
			// 
			this.tsmiReferenceRemoveNamespace.Name = "tsmiReferenceRemoveNamespace";
			this.tsmiReferenceRemoveNamespace.Size = new System.Drawing.Size(214, 24);
			this.tsmiReferenceRemoveNamespace.Text = "Remove Namespa&ce";
			// 
			// splitMain
			// 
			this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitMain.Location = new System.Drawing.Point(0, 0);
			this.splitMain.Name = "splitMain";
			this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitMain.Panel1
			// 
			this.splitMain.Panel1.Controls.Add(this.splitCode);
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add(this.lvErrors);
			this.splitMain.Size = new System.Drawing.Size(532, 310);
			this.splitMain.SplitterDistance = 177;
			this.splitMain.TabIndex = 0;
			this.splitMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.splitMain_MouseDoubleClick);
			// 
			// splitCode
			// 
			this.splitCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitCode.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitCode.Location = new System.Drawing.Point(0, 0);
			this.splitCode.Name = "splitCode";
			// 
			// splitCode.Panel1
			// 
			this.splitCode.Panel1.Controls.Add(this.tvReference);
			// 
			// splitCode.Panel2
			// 
			this.splitCode.Panel2.Controls.Add(this.txtSource);
			this.splitCode.Panel2.Controls.Add(this.txtMethodArgs);
			this.splitCode.Panel2.Controls.Add(this.toolStrip);
			this.splitCode.Size = new System.Drawing.Size(532, 177);
			this.splitCode.SplitterDistance = 177;
			this.splitCode.TabIndex = 0;
			// 
			// txtSource
			// 
			this.txtSource.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
			this.txtSource.AutoIndentCharsPatterns = "\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;]+);\n^\\s*(case|default)\\s*[^:]*(" +
    "?<range>:)\\s*(?<range>[^;]+);\n";
			this.txtSource.AutoScrollMinSize = new System.Drawing.Size(46, 19);
			this.txtSource.BackBrush = null;
			this.txtSource.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.Strategy2;
			this.txtSource.CharHeight = 19;
			this.txtSource.CharWidth = 9;
			this.txtSource.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtSource.DelayedEventsInterval = 500;
			this.txtSource.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSource.Font = new System.Drawing.Font("Consolas", 9.75F);
			this.txtSource.IsReplaceMode = false;
			this.txtSource.LeftBracket = '(';
			this.txtSource.LeftBracket2 = '{';
			this.txtSource.LeftPadding = 17;
			this.txtSource.Location = new System.Drawing.Point(0, 44);
			this.txtSource.Name = "txtSource";
			this.txtSource.Paddings = new System.Windows.Forms.Padding(0);
			this.txtSource.RightBracket = ')';
			this.txtSource.RightBracket2 = '}';
			this.txtSource.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.txtSource.Size = new System.Drawing.Size(351, 133);
			this.txtSource.TabIndex = 1;
			this.txtSource.Zoom = 100;
			this.txtSource.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.txtSource_TextChanged);
			this.txtSource.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSource_KeyDown);
			// 
			// toolStrip
			// 
			this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbnAssemblies,
            this.tsbnFullSource,
            this.tsbnDebug,
            this.tsddlLanguages,
            this.tsbnCompile,
            this.tsbnCompilerVersion});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(351, 28);
			this.toolStrip.TabIndex = 0;
			this.toolStrip.Text = "toolStrip1";
			// 
			// tsbnAssemblies
			// 
			this.tsbnAssemblies.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnAssemblies.Image = ((System.Drawing.Image)(resources.GetObject("tsbnAssemblies.Image")));
			this.tsbnAssemblies.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnAssemblies.Name = "tsbnAssemblies";
			this.tsbnAssemblies.Size = new System.Drawing.Size(39, 25);
			this.tsbnAssemblies.ToolTipText = "References";
			this.tsbnAssemblies.ButtonClick += new System.EventHandler(this.tsbnAssemblies_ButtonClick);
			this.tsbnAssemblies.DropDownOpening += new System.EventHandler(this.tsbnAssemblies_DropDownOpening);
			// 
			// tsbnFullSource
			// 
			this.tsbnFullSource.CheckOnClick = true;
			this.tsbnFullSource.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnFullSource.Image = ((System.Drawing.Image)(resources.GetObject("tsbnFullSource.Image")));
			this.tsbnFullSource.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnFullSource.Name = "tsbnFullSource";
			this.tsbnFullSource.Size = new System.Drawing.Size(24, 25);
			this.tsbnFullSource.Text = "Full Source";
			this.tsbnFullSource.Click += new System.EventHandler(this.tsbnFullSource_Click);
			// 
			// tsbnDebug
			// 
			this.tsbnDebug.CheckOnClick = true;
			this.tsbnDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnDebug.Image = global::Plugin.Compiler.Properties.Resources.iconDebug;
			this.tsbnDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnDebug.Name = "tsbnDebug";
			this.tsbnDebug.Size = new System.Drawing.Size(24, 25);
			this.tsbnDebug.Text = "Include Debug info";
			this.tsbnDebug.Click += new System.EventHandler(this.tsbnDebug_Click);
			// 
			// tsddlLanguages
			// 
			this.tsddlLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.tsddlLanguages.Name = "tsddlLanguages";
			this.tsddlLanguages.Size = new System.Drawing.Size(121, 28);
			this.tsddlLanguages.SelectedIndexChanged += new System.EventHandler(this.tsddlLanguages_SelectedIndexChanged);
			// 
			// tsbnCompile
			// 
			this.tsbnCompile.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.tsbnCompile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnCompile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCompile,
            this.tsmiBuild,
            this.toolStripSeparator1,
            this.tsmiProject});
			this.tsbnCompile.Image = ((System.Drawing.Image)(resources.GetObject("tsbnCompile.Image")));
			this.tsbnCompile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnCompile.Name = "tsbnCompile";
			this.tsbnCompile.Size = new System.Drawing.Size(39, 25);
			this.tsbnCompile.Text = "Compile";
			this.tsbnCompile.ButtonClick += new System.EventHandler(this.tsbnCompile_Click);
			this.tsbnCompile.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsbnCompile_DropDownItemClicked);
			// 
			// tsmiCompile
			// 
			this.tsmiCompile.Enabled = false;
			this.tsmiCompile.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.tsmiCompile.Name = "tsmiCompile";
			this.tsmiCompile.Size = new System.Drawing.Size(138, 26);
			this.tsmiCompile.Text = "&Compile";
			// 
			// tsmiBuild
			// 
			this.tsmiBuild.Enabled = false;
			this.tsmiBuild.Name = "tsmiBuild";
			this.tsmiBuild.Size = new System.Drawing.Size(138, 26);
			this.tsmiBuild.Text = "&Build...";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(135, 6);
			// 
			// tsmiProject
			// 
			this.tsmiProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSave,
            this.tsmiImport,
            this.tsmiExport});
			this.tsmiProject.Name = "tsmiProject";
			this.tsmiProject.Size = new System.Drawing.Size(138, 26);
			this.tsmiProject.Text = "&Project";
			this.tsmiProject.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsmiProject_DropDownItemClicked);
			// 
			// tsmiSave
			// 
			this.tsmiSave.Enabled = false;
			this.tsmiSave.Name = "tsmiSave";
			this.tsmiSave.Size = new System.Drawing.Size(129, 26);
			this.tsmiSave.Text = "&Save";
			// 
			// tsmiImport
			// 
			this.tsmiImport.Name = "tsmiImport";
			this.tsmiImport.Size = new System.Drawing.Size(129, 26);
			this.tsmiImport.Text = "&Import";
			// 
			// tsmiExport
			// 
			this.tsmiExport.Enabled = false;
			this.tsmiExport.Name = "tsmiExport";
			this.tsmiExport.Size = new System.Drawing.Size(129, 26);
			this.tsmiExport.Text = "&Export";
			// 
			// tsbnCompilerVersion
			// 
			this.tsbnCompilerVersion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnCompilerVersion.Image = ((System.Drawing.Image)(resources.GetObject("tsbnCompilerVersion.Image")));
			this.tsbnCompilerVersion.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnCompilerVersion.Name = "tsbnCompilerVersion";
			this.tsbnCompilerVersion.Size = new System.Drawing.Size(34, 25);
			this.tsbnCompilerVersion.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsbnCompilerVersion_DropDownItemClicked);
			// 
			// lvErrors
			// 
			this.lvErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNumber,
            this.colLine,
            this.colDescription});
			this.lvErrors.ContextMenuStrip = this.cmsErrors;
			this.lvErrors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvErrors.FullRowSelect = true;
			this.lvErrors.Location = new System.Drawing.Point(0, 0);
			this.lvErrors.Name = "lvErrors";
			this.lvErrors.Size = new System.Drawing.Size(532, 129);
			this.lvErrors.SmallImageList = this.ilStatus;
			this.lvErrors.TabIndex = 0;
			this.lvErrors.UseCompatibleStateImageBehavior = false;
			this.lvErrors.View = System.Windows.Forms.View.Details;
			this.lvErrors.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvErrors_MouseDoubleClick);
			// 
			// colNumber
			// 
			this.colNumber.DisplayIndex = 1;
			this.colNumber.Text = "Number";
			// 
			// colLine
			// 
			this.colLine.DisplayIndex = 0;
			this.colLine.Text = "Ln";
			// 
			// colDescription
			// 
			this.colDescription.Text = "Description";
			this.colDescription.Width = 74;
			// 
			// cmsErrors
			// 
			this.cmsErrors.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsErrors.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiErrorsClear});
			this.cmsErrors.Name = "cmsErrors";
			this.cmsErrors.Size = new System.Drawing.Size(113, 28);
			this.cmsErrors.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsErrors_ItemClicked);
			// 
			// tsmiErrorsClear
			// 
			this.tsmiErrorsClear.Name = "tsmiErrorsClear";
			this.tsmiErrorsClear.Size = new System.Drawing.Size(112, 24);
			this.tsmiErrorsClear.Text = "&Clear";
			// 
			// ilStatus
			// 
			this.ilStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilStatus.ImageStream")));
			this.ilStatus.TransparentColor = System.Drawing.Color.Fuchsia;
			this.ilStatus.Images.SetKeyName(0, "Warning.bmp");
			this.ilStatus.Images.SetKeyName(1, "Error.bmp");
			// 
			// ilObjects
			// 
			this.ilObjects.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilObjects.ImageStream")));
			this.ilObjects.TransparentColor = System.Drawing.Color.Fuchsia;
			this.ilObjects.Images.SetKeyName(0, "Assembly.bmp");
			this.ilObjects.Images.SetKeyName(1, "Namespace.bmp");
			// 
			// txtMethodArgs
			// 
			this.txtMethodArgs.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtMethodArgs.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtMethodArgs.Location = new System.Drawing.Point(0, 28);
			this.txtMethodArgs.Name = "txtMethodArgs";
			this.txtMethodArgs.ReadOnly = true;
			this.txtMethodArgs.Size = new System.Drawing.Size(351, 16);
			this.txtMethodArgs.TabIndex = 2;
			// 
			// tvReference
			// 
			this.tvReference.ContextMenuStrip = cmsReference;
			this.tvReference.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvReference.ImageIndex = 0;
			this.tvReference.Location = new System.Drawing.Point(0, 0);
			this.tvReference.Name = "tvReference";
			this.tvReference.SelectedImageIndex = 0;
			this.tvReference.ShowNonPublicMembers = false;
			this.tvReference.Size = new System.Drawing.Size(177, 177);
			this.tvReference.TabIndex = 0;
			this.tvReference.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tvReference_KeyDown);
			// 
			// DocumentCompiler
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitMain);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "DocumentCompiler";
			this.Size = new System.Drawing.Size(532, 310);
			cmsReference.ResumeLayout(false);
			this.splitMain.Panel1.ResumeLayout(false);
			this.splitMain.Panel2.ResumeLayout(false);
			this.splitMain.ResumeLayout(false);
			this.splitCode.Panel1.ResumeLayout(false);
			this.splitCode.Panel2.ResumeLayout(false);
			this.splitCode.Panel2.PerformLayout();
			this.splitCode.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtSource)).EndInit();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.cmsErrors.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.SplitContainer splitMain;
		private System.Windows.Forms.SplitContainer splitCode;
		private System.Windows.Forms.ListView lvErrors;
		private System.Windows.Forms.ColumnHeader colLine;
		private System.Windows.Forms.ColumnHeader colNumber;
		private System.Windows.Forms.ColumnHeader colDescription;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripSplitButton tsbnAssemblies;
		private FastColoredTextBoxNS.FastColoredTextBox txtSource;
		private AlphaOmega.Windows.Forms.AssemblyTreeView tvReference;
		private System.Windows.Forms.ToolStripComboBox tsddlLanguages;
		private System.Windows.Forms.ToolStripSplitButton tsbnCompile;
		private System.Windows.Forms.ToolStripMenuItem tsmiCompile;
		private System.Windows.Forms.ToolStripMenuItem tsmiBuild;
		private System.Windows.Forms.ToolStripButton tsbnFullSource;
		private System.Windows.Forms.ImageList ilStatus;
		private System.Windows.Forms.ImageList ilObjects;
		private System.Windows.Forms.ContextMenuStrip cmsErrors;
		private System.Windows.Forms.ToolStripMenuItem tsmiErrorsClear;
		private System.Windows.Forms.ToolStripMenuItem tsmiReferenceAddNamespace;
		private System.Windows.Forms.ToolStripMenuItem tsmiReferenceRemove;
		private System.Windows.Forms.ToolStripButton tsbnDebug;
		private System.Windows.Forms.ToolStripMenuItem tsmiReferenceAdd;
		private System.Windows.Forms.ToolStripDropDownButton tsbnCompilerVersion;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem tsmiProject;
		private System.Windows.Forms.ToolStripMenuItem tsmiImport;
		private System.Windows.Forms.ToolStripMenuItem tsmiExport;
		private System.Windows.Forms.ToolStripMenuItem tsmiSave;
		private System.Windows.Forms.ToolStripMenuItem tsmiReferenceRemoveNamespace;
		private System.Windows.Forms.TextBox txtMethodArgs;
	}
}
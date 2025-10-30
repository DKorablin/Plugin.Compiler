using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using AlphaOmega.Reflection;
using AlphaOmega.Windows.Forms;
using Plugin.Compiler.Bll;
using Plugin.Compiler.Properties;
using Plugin.Compiler.UI;
using Plugin.Compiler.Xml;
using SAL.Flatbed;
using SAL.Windows;
using System.Linq; // For roslyn diagnostics

namespace Plugin.Compiler
{
	public partial class DocumentCompiler : UserControl, IPluginSettings<DocumentCompilerSettings>
	{
		private IPluginDescription _callerPlugin;
		private DocumentCompilerSettings _settings;
		private PartialCompiler _compiler;

		Object IPluginSettings.Settings => this.Settings;

		public DocumentCompilerSettings Settings => this._settings ?? (this._settings = new DocumentCompilerSettings());
		
		private PartialCompiler Compiler
		{
			get => this._compiler;
			set
			{
				this._compiler = value;
				if(value != null)
				{
					tsbnFullSource.Checked = this._compiler.IsFullSourceCode;
					tsbnDebug.Checked = this._compiler.IsIncludeDebugInfo;
					tsddlLanguages.SelectedIndex = this._compiler.LanguageId;
					txtSource.Text = this._compiler.SourceCode;
					this.SelectedCompilerVersion = this._compiler.CompilerVersion;
					this.ReloadReferences();
				}
			}
		}

		private PluginWindows Plugin => (PluginWindows)this.Window.Plugin;

		private IWindow Window => (IWindow)base.Parent;

		/// <summary>Plugin that triggers the event</summary>
		private IPluginDescription CallerPlugin
		{
			get
			{
				if(this._callerPlugin == null)
				{
					if(this.Settings.CallerPluginId != null)
						this._callerPlugin = this.Plugin.Host.Plugins[this.Settings.CallerPluginId];
					else
					{
						foreach(IPluginDescription plugin in this.Plugin.Host.Plugins)
							if(plugin.Instance == this.Plugin)
							{
								this._callerPlugin = plugin;
								break;
							}
					}
				}
				return this._callerPlugin;
			}
		}

		/// <summary>Local compiler execution</summary>
		private Boolean IsCallerPluginLocal => this.CallerPlugin.Instance == this.Plugin;

		/// <summary>Message that will be called upon completion of compilation</summary>
		public event EventHandler<DataEventArgs> SaveEvent;

		/// <summary>Selected language in the list of compilers</summary>
		private ComboListItem SelectedLanguage => (ComboListItem)tsddlLanguages.SelectedItem;

		/// <summary>The selected compiler in the list of compilers</summary>
		private CompilerInfo SelectedCompiler => this.SelectedLanguage.Tag;

		/// <summary>The selected compiler version in the list of compilers</summary>
		private String SelectedCompilerVersion
		{
			get
			{
				foreach(ToolStripMenuItem item in tsbnCompilerVersion.DropDownItems)
					if(item.Checked)
						return (String)item.Tag;
				return null;
			}
			set
			{
				foreach(ToolStripMenuItem item in tsbnCompilerVersion.DropDownItems)
					item.Checked = ((String)item.Tag) == value;
				this.Compiler.CompilerVersion = value;
			}
		}

		public DocumentCompiler()
		{
			this.InitializeComponent();

#if NETFRAMEWORK
			this.txtSource.Language = FastColoredTextBoxNS.Language.CSharp;
#else
			this.txtSource.Language = FastColoredTextBoxNS.Text.Language.CSharp;
#endif
			splitMain.Panel2Collapsed = true;
		}

		protected override void OnCreateControl()
		{
			this.Window.Caption = ".NET compiler";
			this.Window.SetTabPicture(Resources.Compiler_Icon);
			tvReference.ShowNonPublicMembers = this.Plugin.Settings.ShowNonPublicMembers;

			this._compiler = new PartialCompiler(this.Plugin);

			foreach(String version in DynamicCompiler.GetFrameworkVersions())
				tsbnCompilerVersion.DropDownItems.Add(new ToolStripMenuItem($"Microsoft .NET {version}") { Tag = version, CheckOnClick = true, Checked = version == Constant.DefaultCompilerVersion, });

			String language = this.Plugin.Settings.DefaultCompilerLanguage;
			foreach(CompilerInfo info in this.Compiler.GetSupportedCompilers())
			{
				ComboListItem item = new ComboListItem(this.Compiler.GetSupportedLanguage(info)) { Tag = info, };
				Int32 index = tsddlLanguages.Items.Add(item);
				if(language == null)
					language = item.Text;

				if(language == item.Text)
					tsddlLanguages.SelectedIndex = index;
			}

			this.DataBind();

			base.OnCreateControl();
		}

		private void DataBind()
		{
			String className = this.Settings.ClassName ?? "Undefined";

			this.Window.Caption = this.CallerPlugin.Name + " - " + this.Window.Caption;
			this.Compiler = this.Plugin.SettingsCompiler.AttachPluginMethodRow(this.CallerPlugin, className);
			if(this.Compiler == null)
				try
				{
					ProjectXmlLoader loader = new ProjectXmlLoader(this.Plugin);
					this.Compiler = loader.LoadCompilerString(this.CallerPlugin, className);
				} catch(XmlException exc)
				{//TODO: We need to somehow validate the name of the first method we create...
					this.Plugin.Trace.TraceData(TraceEventType.Error, 1, exc);
				}
			if(this.Compiler == null)
				this.Compiler = new PartialCompiler(this.Plugin, this.CallerPlugin)
				{
					ClassName = className
				};

			this.Compiler.ArgumentsType = this.Settings.ArgumentsType == null
				? null
				: Array.ConvertAll(this.Settings.ArgumentsType, type => type.Name);

			this.Compiler.ReturnType = this.Settings.ReturnType?.Name;

			if(this.Compiler.References.Count == 0)
			{
				//Old version builds are attached
				this.Compiler.References.AddNamespace("System", "System", "System.IO");
				this.Compiler.References.AddNamespace("System.Windows.Forms", "System.Windows.Forms");
				if(this.Settings.ArgumentsType != null)
					foreach(Type argument in this.Settings.ArgumentsType)
						this.Compiler.References.AddNamespace(argument.Assembly, argument.Namespace);
				if(this.Settings.ReturnType != null)
					this.Compiler.References.AddNamespace(this.Settings.ReturnType.Assembly, this.Settings.ReturnType.Namespace);

				/*this.Compiler.References.AddNamespace(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SAL.Flatbed.dll"), "SAL.Flatbed");
				this.Compiler.References.AddNamespace(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SAL.Windows.dll"), "SAL.Windows");*/
				this.ReloadReferences();
			}

			txtMethodArgs.Text = this.Compiler.FullMethodDescription;
		}

		#region Event Handlers
		private void txtSource_TextChanged(Object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
		{
			if(this.Compiler != null)
				this.Compiler.SourceCode = txtSource.Text;
			tsmiCompile.Enabled = tsmiBuild.Enabled = tsmiSave.Enabled = tsmiExport.Enabled = !String.IsNullOrEmpty(txtSource.Text.Trim());
		}

		private void tvReference_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.Back:
			case Keys.Delete:
				if(tvReference.SelectedNode != null)
				{
					e.Handled = true;
					this.cmsReference_ItemClicked(tvReference, new ToolStripItemClickedEventArgs(tsmiReferenceRemove));
				}
				break;
			}
		}

		private void cmsReference_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == tsmiReferenceAdd)
				this.tsbnAssemblies_ButtonClick(this, EventArgs.Empty);
			else if(e.ClickedItem == tsmiReferenceRemove)
			{
				TreeNode node = tvReference.SelectedNode;
				if(MessageBox.Show($"Are you sure you want to remove assembly {node.Text} from compilation?", this.Window.Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					this.Compiler.References.RemoveAssembly((String)node.Tag);
					node.Remove();
				}
			} else if(e.ClickedItem == tsmiReferenceAddNamespace)
			{
				if(this.GetSelectedAssemblyAndNamespace(out String assembly, out String ns))
				{
					this.Compiler.References.AddNamespace(assembly, ns);

					if(tsbnFullSource.Checked)
						txtSource.Text = $"using {ns};{Environment.NewLine}" + txtSource.Text;
				}
			} else if(e.ClickedItem == tsmiReferenceRemoveNamespace)
			{
				if(this.GetSelectedAssemblyAndNamespace(out String assembly, out String ns)
					&& MessageBox.Show($"Are you sure you want to remove namespace {ns} in assembly {assembly} from compilation?", this.Window.Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					this.Compiler.References.RemoveNamespace(assembly, ns);
					if(tsbnFullSource.Checked)
					{
						String codeNamespace = $"using {ns};{Environment.NewLine}";
						Int32 index = txtSource.Text.IndexOf(codeNamespace);
						if(index > -1)
							txtSource.Text = txtSource.Text.Remove(index, codeNamespace.Length);
					}
				}
			}
			else throw new NotImplementedException(e.ClickedItem.ToString());
		}

		private void tsbnCompilerVersion_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			foreach(ToolStripMenuItem item in tsbnCompilerVersion.DropDownItems)
				if(item != e.ClickedItem)
				{
					item.Checked = false;
					break;
				}

			this.Compiler.CompilerVersion = e.ClickedItem.Tag.ToString();
		}

		private void tsddlLanguages_SelectedIndexChanged(Object sender, EventArgs e)
		{
			if(tsbnFullSource.Checked)
			{
				String source = this.Compiler.TryToClearFullSource(txtSource.Text);
				if(source != null)
					txtSource.Text = this.Compiler.GetFullSourceCode(this.SelectedLanguage.Text, source);
				else
					MessageBox.Show(Resources.wrnRemoveClassFormatting, this.Window.Caption);
			}
			this.Compiler.LanguageId = tsddlLanguages.SelectedIndex;
		}

		private void tsbnCompile_Click(Object sender, EventArgs e)
			=> this.tsbnCompile_DropDownItemClicked(sender, new ToolStripItemClickedEventArgs(tsmiCompile));

		private void tsbnCompile_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == tsmiProject)
				return;

			lvErrors.Items.Clear();
			try
			{
				this.Compiler.CompiledAssemblyFilePath = String.Empty;

				if(e.ClickedItem == tsmiCompile)
				{//Compile and build assembly
					Object result = this.Compiler.CompileAndInvoke<Object>(this.CallerPlugin.Instance);

					if(result != null)
						this.Plugin.Trace.TraceEvent(TraceEventType.Information, 10, result.ToString());
				} else if(e.ClickedItem == tsmiBuild)
				{//Compile and save the assembly to disk
					using(SaveFileDialog dlg = new SaveFileDialog() { OverwritePrompt = true, AddExtension = true, FileName = this.Compiler.CompiledAssemblyFilePath, DefaultExt = "dll", Filter = "DLL file (*.dll)|*.dll|Batch file (*.bat)|*.bat", })
						if(dlg.ShowDialog() == DialogResult.OK)
						{
							switch(dlg.FilterIndex)
							{
							case 1://.dll
								this.Compiler.CompiledAssemblyFilePath = dlg.FileName;
								this.Compiler.CompileAssembly();
								break;
							case 2://.bat
								String batFile = this.Compiler.GetBatchCode();
								File.WriteAllText(dlg.FileName, batFile);
								break;
							}
						}
				} else if(e.ClickedItem is ToolStripSeparator)
					return;
				else
					throw new NotImplementedException(e.ClickedItem.ToString());

				splitMain.Panel2Collapsed = true;
				if(this.IsCallerPluginLocal)//Saving local plugin settings
					this.Plugin.SettingsCompiler.ModifyPluginRow(this.CallerPlugin, this.Compiler);

			} catch(CompilerException exc)
			{
				splitMain.Panel2Collapsed = false;
#if NETFRAMEWORK
				ListViewItem[] items = new ListViewItem[exc.Result.Errors.Count];
				for(Int32 loop = 0, count = items.Length;loop < count;loop++)
				{
					CompilerError error = exc.Result.Errors[loop];
					ListViewItem item = new ListViewItem(error.ErrorNumber)
					{
						ImageIndex = error.IsWarning ? 0 : 1,
					};
					item.SubItems.Add(error.Line.ToString("n0"));
					item.SubItems.Add(error.ErrorText);
					items[loop] = item;
				}
#else
				var diags = exc.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error || d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Warning).ToArray();
				ListViewItem[] items = new ListViewItem[diags.Length];
				for(Int32 loop = 0, count = items.Length;loop < count;loop++)
				{
					var d = diags[loop];
					var span = d.Location.GetLineSpan();
					Int32 line = span.IsValid ? span.StartLinePosition.Line + 1 : 0;
					ListViewItem item = new ListViewItem(d.Id)
					{
						ImageIndex = d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Warning ? 0 : 1,
					};
					item.SubItems.Add(line.ToString("n0"));
					item.SubItems.Add(d.GetMessage());
					items[loop] = item;
				}
#endif
				lvErrors.Items.AddRange(items);
				lvErrors.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			}
		}

		private void tsmiProject_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == tsmiImport)
			{//Import project information
				using(OpenFileDialog dlg = new OpenFileDialog() { CheckFileExists = true, DefaultExt = "xml", Filter = "XML file (*.xml)|*.xml|All files (*.*)|*.*", Title = "Open Project File", })
					if(dlg.ShowDialog() == DialogResult.OK)
					{
						ProjectXmlLoader loader = new ProjectXmlLoader(this.Plugin);
						this.Compiler = loader.LoadCompilerFile(this.CallerPlugin, dlg.FileName);
					}
			} else if(e.ClickedItem == tsmiExport)
			{//Export project information
				using(SaveFileDialog dlg = new SaveFileDialog() { OverwritePrompt = true, AddExtension = true, FileName = this.Compiler.ClassName + ".xml", DefaultExt = "xml", Filter = "XML file (*.xml)|*.xml|Aff files (*.*)|*.*", })
					if(dlg.ShowDialog() == DialogResult.OK)
						File.WriteAllText(dlg.FileName, ProjectXmlLoader.SaveCompiler(this.Compiler));
			} else if(e.ClickedItem == tsmiSave)
			{//Save project information
				if(tsbnCompile.Enabled && lvErrors.Items.Count == 0)
					this.OnSaveEvent();
				else
					MessageBox.Show("Please compile you code without any errors or warnings before saving any changes.", this.Window.Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
			} else throw new NotImplementedException(e.ClickedItem.ToString());
		}

		private void tsbnFullSource_Click(Object sender, EventArgs e)
		{
			String sourceCode = tsbnFullSource.Checked
				? this.Compiler.GetFullSourceCode(this.SelectedLanguage.Text, txtSource.Text)
				: this.Compiler.TryToClearFullSource(txtSource.Text);

			if(sourceCode == null)
				MessageBox.Show(Resources.wrnRemoveClassFormatting, this.Window.Caption);

			txtSource.Text = sourceCode;
			this.Compiler.IsFullSourceCode = tsbnFullSource.Checked;
			txtMethodArgs.Visible = !this.Compiler.IsFullSourceCode;
		}

		private void tsbnDebug_Click(Object sender, EventArgs e)
			=> this.Compiler.IsIncludeDebugInfo = tsbnDebug.Checked;

		private void tsbnAssemblies_ButtonClick(Object sender, EventArgs e)
		{
			using(AssemblyReferenceDlg dlg = new AssemblyReferenceDlg(this.Plugin))
				if(dlg.ShowDialog() == DialogResult.OK)
				{
					foreach(String assembly in dlg.SelectedAssemblies)
					{
						this.Compiler.References.AddAssembly(assembly);
						TreeNode node = File.Exists(assembly)
							? tvReference.BindAssembly(assembly)
							: tvReference.BindAssembly(AssemblyCache.QueryAssemblyInfo(assembly));
						node.Tag = assembly;
					}
				}
		}

		private void tsbnAssemblies_DropDownOpening(Object sender, EventArgs e)
		{
			tsbnAssemblies.DropDownItems.Clear();
			foreach(String assembly in this.Compiler.References)
			{
				String text;
				if(File.Exists(assembly))
					text = Path.GetFileName(assembly);
				else if(assembly.Contains(","))
					text = assembly.Substring(0, assembly.IndexOf(','));//GAC DisplayName
				else
					text = assembly;

				ToolStripMenuItem item = (ToolStripMenuItem)tsbnAssemblies.DropDownItems.Add(text);
				item.Tag = assembly;
				item.Image = ilObjects.Images[(Int32)AssemblyTreeView.TreeImageIndex.Assembly];
				foreach(String ns in this.Compiler.References[assembly])
					item.DropDownItems.Add(ns).Image = ilObjects.Images[(Int32)AssemblyTreeView.TreeImageIndex.Namespace];
			}
		}

		private void cmsErrors_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == tsmiErrorsClear)
			{
				lvErrors.Items.Clear();
				splitCode.Panel2Collapsed = true;
			} else
				throw new NotImplementedException();
		}

		private void txtSource_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.A | Keys.Control:
				e.Handled = true;
				txtSource.SelectAll();
				break;
			}
		}

		private void cmsReference_Opening(Object sender, CancelEventArgs e)
		{
			tsmiReferenceAddNamespace.Visible = tvReference.SelectedNode != null
				&& tvReference.SelectedNode.ImageIndex == (Int32)AssemblyTreeView.TreeImageIndex.Namespace;

			tsmiReferenceRemove.Visible = tvReference.SelectedNode != null
				&& tvReference.SelectedNode.ImageIndex == (Int32)AssemblyTreeView.TreeImageIndex.Assembly;

			if(tvReference.SelectedNode != null
				&& tvReference.SelectedNode.ImageIndex == (Int32)AssemblyTreeView.TreeImageIndex.Namespace
				&& this.Compiler != null)
			{
				if(this.GetSelectedAssemblyAndNamespace(out String assembly, out String ns))
					tsmiReferenceRemoveNamespace.Visible = this.Compiler.References.IsNamespaceAdded(assembly, ns);
			} else
				tsmiReferenceRemoveNamespace.Visible = false;
		}

		private void lvErrors_MouseDoubleClick(Object sender, MouseEventArgs e)
		{
			if(lvErrors.SelectedItems.Count > 0 && tsbnFullSource.Checked)
			{
				Int32 lineNumber = Int32.Parse(lvErrors.SelectedItems[0].SubItems[colLine.Index].Text) - 1;
				txtSource.Selection = txtSource.GetLine(lineNumber);
				txtSource.DoSelectionVisible();
				txtSource.Invalidate();

			}
		}

		private void splitMain_MouseDoubleClick(Object sender, MouseEventArgs e)
		{
			if(splitMain.SplitterRectangle.Contains(e.Location))
			{
				splitMain.Panel2Collapsed = true;
				txtSource.Focus();
			}
		}
		#endregion Event Handlers
		#region Methods
		protected override Boolean ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch(keyData)
			{
			case Keys.Return:
				if(lvErrors.Focused && lvErrors.SelectedItems.Count > 0 && tsbnFullSource.Checked)
				{
					this.lvErrors_MouseDoubleClick(txtSource, null);
					return true;
				}
				return false;
			case Keys.F5:
				this.tsbnCompile_Click(this, EventArgs.Empty);
				return true;
			case Keys.Control | Keys.S:
				this.tsmiProject_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(tsmiSave));
				return true;
			default:
				return base.ProcessCmdKey(ref msg, keyData);
			}
		}

		private Boolean GetSelectedAssemblyAndNamespace(out String assembly, out String ns)
		{
			assembly = ns = null;
			TreeNode root = tvReference.SelectedNode;
			while(root != null)
				if(root.Parent != null && root.Parent.ImageIndex == (Int32)AssemblyTreeView.TreeImageIndex.Assembly)
				{
					assembly = (String)root.Parent.Tag;
					ns = root.Text;
					break;
				} else
					root = root.Parent;

			return assembly != null && ns != null;
		}

		/// <summary>Update all assemblies available in the project</summary>
		private void ReloadReferences()
		{
			tvReference.Nodes.Clear();
			for(Int32 loop = this.Compiler.References.Count - 1; loop >= 0; loop--)
			{
				String assembly = this.Compiler.References[loop];
				try
				{
					TreeNode node = File.Exists(assembly)
						? tvReference.BindAssembly(assembly)
						: tvReference.BindAssembly(AssemblyCache.QueryAssemblyInfo(assembly));//TODO: There will be an error if the assembly is not loaded from the file system.
					node.Tag = assembly;
				} catch(Exception exc)
				{
					if(MessageBox.Show($"{exc.Message}{Environment.NewLine}Do you want to remove incorrect reference?", assembly, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
						this.Compiler.References.RemoveAssembly(assembly);
				}
			}
		}

		private void OnSaveEvent()
		{
			var evt = this.SaveEvent;
			if(evt == null)
				this.Plugin.SettingsCompiler.ModifyPluginRow(this.CallerPlugin, this.Compiler);
			else
				evt(this, new CompilerDataEventArgs(this.Compiler));
		}
		#endregion Methods
	}
}
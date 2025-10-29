using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using AlphaOmega.Reflection;
using AlphaOmega.Windows.Forms;
using SAL.Flatbed;

namespace Plugin.Compiler.UI
{
	internal partial class AssemblyReferenceDlg : Form
	{
		private readonly PluginWindows _plugin;
		public AssemblyReferenceDlg(PluginWindows plugin)
		{
			this._plugin = plugin;
			this.InitializeComponent();
			gridSearch.ListView = lvGac;
			tvPlugins.ShowNonPublicMembers = plugin.Settings.ShowNonPublicMembers;
			this.tabMain_SelectedIndexChanged(this, EventArgs.Empty);
		}

		protected override void OnShown(EventArgs e)
		{
			gridSearch.Visible = false;
			base.OnShown(e);
		}

		/// <summary>The list of selected assemblies</summary>
		public IEnumerable<String> SelectedAssemblies
		{
			get
			{
				switch(tabMain.SelectedIndex)
				{
				case 0://GAC
					if(lvGac.SelectedItems.Count > 0)
					{
						Int32 pathIndex = AssemblyReferenceDlg.GetColumnIndex(lvGac, "Path");
						for(Int32 loop = 0, count = lvGac.SelectedItems.Count; loop < count; loop++)
						{
							//result[loop] = lvGac.SelectedItems[loop].SubItems[pathIndex].Text;//Path to the assembly on disk
							yield return (String)lvGac.SelectedItems[loop].Tag;//Assembly Name
						}
					}
					break;
				case 1://Plugins
					if(tvPlugins.SelectedNode != null)
					{
						TreeNode parentNode = tvPlugins.SelectedNode;
						while(parentNode.Parent != null)
							parentNode = parentNode.Parent;
						yield return ((TreeNodeAsm)parentNode).Assembly.Location;//TODO: It won't work if the plugin isn't loaded from the file system.
					}
					break;
				case 2://Browse
					for(Int32 loop = 0, count = lvBrowse.Items.Count; loop < count; loop++)
						yield return (String)lvBrowse.Items[loop].Tag;
					break;
				default: throw new NotImplementedException(String.Format("Index: {0} Name: {1}", tabMain.SelectedIndex, tabMain.SelectedTab.Text));
				}
			}
		}

		private void tabMain_SelectedIndexChanged(Object sender, EventArgs e)
		{
			switch(tabMain.SelectedIndex)
			{
			case 0://GAC
				if(lvGac.Items.Count == 0 && !bgGac.IsBusy)
				{
					lvGac.Items.Clear();
					lvGac.Columns.Clear();
					bgGac.RunWorkerAsync();
				}
				break;
			case 1://Plugins
				if(tvPlugins.Nodes.Count == 0)
					foreach(IPluginDescription plugin in this._plugin.Host.Plugins)
						tvPlugins.BindAssembly(plugin.Instance.GetType().Assembly);
				break;
			case 2://Browse
				break;
			default:
				throw new NotImplementedException(String.Format("Index: {0} Name: {1}", tabMain.SelectedIndex, tabMain.SelectedTab.Text));
			}
		}

		private void cmsBrowse_Opening(Object sender, CancelEventArgs e)
			=> tsmiRemove.Visible = lvBrowse.SelectedItems.Count > 0;

		private void cmsBrowse_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			cmsBrowse.Close(ToolStripDropDownCloseReason.ItemClicked);
			if(e.ClickedItem == tsmiBrowse)
			{
				using(OpenFileDialog dlg = new OpenFileDialog() { CheckFileExists = true, Multiselect = true, Filter = "Assemblies (*.dll)|*.dll|All files (*.*)|*.*", Title = "Add reference", })
					if(dlg.ShowDialog() == DialogResult.OK)
					{
						ListViewItem[] items = Array.ConvertAll(dlg.FileNames, delegate (String filePath)
						{
							String path = Path.GetDirectoryName(filePath);
							ListViewGroup pathGroup = null;
							foreach(ListViewGroup group in lvBrowse.Groups)
								if(group.Header.Equals(path))
									pathGroup = group;
							if(pathGroup == null)
								pathGroup = lvBrowse.Groups.Add(String.Empty, path);

							return new ListViewItem(Path.GetFileName(filePath)) { Tag = filePath, Group = pathGroup, };
						});
						lvBrowse.Items.AddRange(items);
						lvBrowse.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
					}
			} else if(e.ClickedItem == tsmiRemove)
			{
				if(lvBrowse.SelectedItems.Count > 0)
					if(MessageBox.Show("Are you sure you want to remove selected assembly from list?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
						while(lvBrowse.SelectedItems.Count > 0)
							lvBrowse.SelectedItems[0].Remove();
			} else throw new NotImplementedException($"Sender: {sender} ClickedItem: {e.ClickedItem}");
		}

		private void cmsGac_Opening(Object sender, CancelEventArgs e)
			=> e.Cancel = lvGac.SelectedItems.Count == 0;

		private void cmsGac_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == tsmiCopy)
				Clipboard.SetText((String)lvGac.SelectedItems[0].Tag);
			else throw new NotImplementedException($"Sender: {sender} ClickedItem: {e.ClickedItem}");
		}

		private void tvAssemblies_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.Delete:
			case Keys.Back:
				this.cmsBrowse_ItemClicked(sender, new ToolStripItemClickedEventArgs(tsmiRemove));
				e.Handled = true;
				break;
			}
		}

		void bgGac_DoWork(Object sender, DoWorkEventArgs e)
		{
			AssemblyCacheEnum asmCache = new AssemblyCacheEnum();
			List<ListViewItem> itemsToAdd = new List<ListViewItem>();
			foreach(String displayName in asmCache)
			{
				String[] properties = displayName.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				if(properties.Length > 0)
				{
					Int32 index;
					ListViewItem item = new ListViewItem() { Tag = displayName, };

					foreach(String asmProperty in properties)
					{
						String[] nameValue = asmProperty.Split(new Char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
						switch(nameValue.Length)
						{
						case 1://The assembly name
							index = AssemblyReferenceDlg.GetColumnIndex(lvGac, "Name");
							item.Text = nameValue[0];//Adding a column for the assembly name
							break;
						case 2://Build parameters
							index = AssemblyReferenceDlg.GetColumnIndex(lvGac, nameValue[0].Trim());

							while(lvGac.Columns.Count > item.SubItems.Count)//I'm adding columns to a row because GetColumnIndex can extend the ListView.
								item.SubItems.Add(String.Empty);
							item.SubItems[index].Text = nameValue[1];
							break;
						default://ХЗ
							throw new NotImplementedException($"Can't format assembly '{displayName}' parameter '{asmProperty}'");
						}
					}

					String path = AssemblyCache.QueryAssemblyInfo(displayName);
					index = AssemblyReferenceDlg.GetColumnIndex(lvGac, "Path");

					while(lvGac.Columns.Count > item.SubItems.Count)//I'm adding columns to a row because GetColumnIndex can extend the ListView.
						item.SubItems.Add(String.Empty);

					item.SubItems[index].Text = path;

					itemsToAdd.Add(item);
				}
			}
			e.Result = itemsToAdd.ToArray();
		}

		void bgGac_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
		{
			ListViewItem[] itemsToAdd = (ListViewItem[])e.Result;
			lvGac.Items.AddRange(itemsToAdd);
			lvGac.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		/// <summary>Get the index of an added or previously created column</summary>
		/// <param name="columnName">Name of an added or previously created column</param>
		/// <returns>Column index</returns>
		private static Int32 GetColumnIndex(ListView lv, String columnName)
		{
			foreach(ColumnHeader columnItem in lv.Columns)
				if(columnItem.Text.Equals(columnName))
					return columnItem.Index;

			Func<Int32> method = () => lv.Columns.Add(new ColumnHeader() { Text = columnName, });

			return lv.InvokeRequired
				? (Int32)lv.Invoke(method)
				: method.Invoke();
		}
	}
}
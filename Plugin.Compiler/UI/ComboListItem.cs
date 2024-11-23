using System;
using System.Windows.Forms;
using System.CodeDom.Compiler;

namespace Plugin.Compiler.UI
{
	internal class ComboListItem : ListViewItem
	{
		public new CompilerInfo Tag
		{
			get => base.Tag as CompilerInfo;
			set => base.Tag = value;
		}

		public ComboListItem(String text)
			: base(text) { }

		public override String ToString()
			=> base.Text;
	}
}
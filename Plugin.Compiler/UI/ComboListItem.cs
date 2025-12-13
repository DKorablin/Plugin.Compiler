using System;
using System.Windows.Forms;
using Plugin.Compiler.Bll;

namespace Plugin.Compiler.UI
{
	internal class ComboListItem : ListViewItem
	{
		public new CompilerInfo2 Tag
		{
			get => base.Tag as CompilerInfo2;
			set => base.Tag = value;
		}

		public ComboListItem(CompilerInfo2 compilerInfo)
			: base(compilerInfo.GetSupportedLanguage())
			=> this.Tag = compilerInfo;

		public override String ToString()
			=> base.Text;
	}
}
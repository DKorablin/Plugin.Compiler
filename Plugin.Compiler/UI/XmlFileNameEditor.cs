using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Plugin.Compiler.UI
{
	internal class XmlFileNameEditor : UITypeEditor
	{
		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			using(SaveFileDialog dlg = new SaveFileDialog() { FileName = value as String ?? String.Empty, Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.* ", OverwritePrompt = true, })
				return dlg.ShowDialog() == DialogResult.OK
					? dlg.FileName
					: value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.Modal;
	}
}
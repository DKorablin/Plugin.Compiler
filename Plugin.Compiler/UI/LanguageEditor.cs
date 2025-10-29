using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using AlphaOmega.Design;
using Plugin.Compiler.Bll;

namespace Plugin.Compiler.UI
{
	/// <summary>Compilation language selection editor</summary>
	internal class LanguageEditor : ListBoxEditorBase
	{
		/// <summary>Get an array of elements with available compilation languages</summary>
		/// <returns>Available compilation languages</returns>
		protected override IEnumerable<ListBoxItem> GetValues()
		{
			DynamicCompiler compiler = new DynamicCompiler();
			foreach(CompilerInfo info in compiler.GetSupportedCompilers())
			{
				String language = compiler.GetSupportedLanguage(info);
				yield return new ListBoxItem(language, language);
			}
		}
	}
}
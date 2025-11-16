using System;
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
			foreach(CompilerInfo2 info in CompilerInfo2.GetSupportedCompilers())
			{
				String language = info.GetSupportedLanguage();
				yield return new ListBoxItem(language, language);
			}
		}
	}
}
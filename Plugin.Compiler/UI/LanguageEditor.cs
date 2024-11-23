using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using AlphaOmega.Design;
using Plugin.Compiler.Bll;

namespace Plugin.Compiler.UI
{
	/// <summary>Редактор выбора языка компиляции</summary>
	internal class LanguageEditor : ListBoxEditorBase
	{
		/// <summary>Получить массив элементов с доступными языками компиляции</summary>
		/// <returns>Доступные языки компиляции</returns>
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
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Compiler.Bll
{
	internal sealed class CompilerInfo2
	{
		public CompilerInfo CompilerInfo { get; }

		/// <summary>Gets the list of all supported languages by current compiler info</summary>
		/// <returns>The list of supported languages</returns>
		public String[] GetLanguages()
		{
#if NETFRAMEWORK
		return this.CompilerInfo.GetLanguages();
#else
			return new[] { "CS", };
#endif
		}

		public static IEnumerable<CompilerInfo2> GetSupportedCompilers()
		{
#if NETFRAMEWORK
			return CodeDomProvider.GetAllCompilerInfo().Select(c => new CompilerInfo2(c));
#else
			yield return new CompilerInfo2();//Roslyn only supports C# here.
#endif
		}

		/// <summary>Get a list of supported languages</summary>
		/// <returns>Array of languages ​​supported by the compiler</returns>
		public static IEnumerable<String> GetSupportedLanguages()
		{
			var languages = CompilerInfo2.GetSupportedCompilers();
			foreach(var language in languages)
			{
				String[] names = language.GetLanguages();
				if(names.Length > 0)
					yield return names[names.Length - 1];
			}
		}

		/// <summary>Get a supported compiler</summary>
		public static CompilerInfo2 GetSupportedCompiler(Int32 index)
		{
			CompilerInfo2[] compilers = CompilerInfo2.GetSupportedCompilers().ToArray();
			return compilers.Length > index ? compilers[index] : null;
		}

		/// <summary>Get the supported language from the compiler information</summary>
		/// <param name="compiler">Compiler information</param>
		/// <returns>Language name from the compiler information</returns>
		public String GetSupportedLanguage()
		{
			String[] names = this.GetLanguages();
			Int32 length = names.Length;
			return length > 0 ? names[length - 1] : String.Empty;
		}

		private CompilerInfo2(CompilerInfo compilerInfo)
			=> this.CompilerInfo = compilerInfo ?? throw new ArgumentNullException(nameof(compilerInfo));

		private CompilerInfo2()
		{ }
	}
}
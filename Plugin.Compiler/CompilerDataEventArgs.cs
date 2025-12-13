using System;
using System.Collections.Generic;
using Plugin.Compiler.Bll;
using Plugin.Compiler.Xml;
using SAL.Flatbed;

namespace Plugin.Compiler
{
	internal class CompilerDataEventArgs : DataEventArgs
	{
		private const String XmlKey = "XML";
		private readonly PartialCompiler _compiler;

		public override Int32 Version => 0;

		public override Int32 Count => 1;

		public override IEnumerable<String> Keys => new String[] { XmlKey, };

		internal CompilerDataEventArgs(PartialCompiler compiler)
		{
			_ = compiler ?? throw new ArgumentNullException(nameof(compiler));

			this._compiler = compiler;
		}

		public override T GetData<T>(String key)
		{
			if(String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			switch(key)
			{
			case XmlKey:
				return (T)(Object)ProjectXmlLoader.SaveCompiler(this._compiler);
			default:
				throw new KeyNotFoundException(String.Format("Key '{0}' not used", key));
			}
		}
	}
}
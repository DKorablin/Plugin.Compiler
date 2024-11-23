using System;
using System.CodeDom.Compiler;

namespace Plugin.Compiler.Bll
{
	internal class CompilerException : ApplicationException
	{
		public String SourceCode { get; private set; }
		public CompilerResults Result { get; private set; }
		internal CompilerException(String sourceCode, CompilerResults result)
		{
			this.SourceCode = sourceCode;
			this.Result = result;
		}
	}
}

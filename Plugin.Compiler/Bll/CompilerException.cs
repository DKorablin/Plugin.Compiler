using System;
#if NETFRAMEWORK
using System.CodeDom.Compiler;
#endif
#if !NETFRAMEWORK
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
#endif

namespace Plugin.Compiler.Bll
{
	internal sealed class CompilerException : ApplicationException
	{
		public String SourceCode { get; private set; }
#if NETFRAMEWORK
		public CompilerResults Result { get; private set; }
		internal CompilerException(String sourceCode, CompilerResults result)
		{
			this.SourceCode = sourceCode;
			this.Result = result;
		}
#else
		public IEnumerable<Diagnostic> Diagnostics { get; private set; }
		internal CompilerException(String sourceCode, IEnumerable<Diagnostic> diagnostics)
		{
			this.SourceCode = sourceCode;
			this.Diagnostics = diagnostics;
		}
#endif
	}
}

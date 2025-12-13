using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plugin.Compiler.Bll
{
	internal class CompiledAssemblyLinks
	{
		private Crc32 Crc { get; } = new Crc32();
		private Dictionary<UInt32, String> AssemblyLinks { get; } = new Dictionary<UInt32, String>();

		public void Add(String sourceCode, Assembly asm)
		{
			UInt32 key = this.Crc.Get(sourceCode);
			String fullName = asm.FullName;

			this.AssemblyLinks.Add(key, fullName);
		}

		public Assembly TryGet(String sourceCode)
		{
			UInt32 key = this.Crc.Get(sourceCode);
			return this.AssemblyLinks.TryGetValue(key, out String fullName)
				? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => p.FullName == fullName)
				: null;
		}

		public void Clear()
			=> this.AssemblyLinks.Clear();
	}
}
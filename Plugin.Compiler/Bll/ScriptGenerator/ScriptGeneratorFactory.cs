using System;

namespace Plugin.Compiler.Bll.ScriptGenerator
{
	internal class ScriptGeneratorFactory
	{
		private readonly ScriptType _type;

		public ScriptGeneratorFactory(ScriptType type)
			=> this._type = type;

		public String GenerateScript(AssemblyCollection references, String sourceCode)
		{
			switch(this._type)
			{
			case ScriptType.Assembly:
				throw new InvalidOperationException($"This type of script can't be generated using {nameof(ScriptGeneratorFactory)}");
			case ScriptType.BatFramework:
				return new FrameworkBatScript(references, sourceCode).GenerateScript();
			case ScriptType.BatNet:
				return new NetBatScript(references, sourceCode).GenerateScript();
			case ScriptType.PowerShellFramework:
				return new FrameworkPowerShellScript(references, sourceCode).GenerateScript();
			case ScriptType.PowerShellNet:
				return new NetPowerShellScript(references, sourceCode).GenerateScript();
			default:
				throw new NotImplementedException($"{this._type} script generator is not implemented by {nameof(ScriptGeneratorFactory)}");
			}
		}
	}
}
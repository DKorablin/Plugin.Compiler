namespace Plugin.Compiler.Bll.ScriptGenerator
{
	/// <summary>The type of the script to generate</summary>
	internal enum ScriptType
	{
		/// <summary>Compile assembly using currently supported CLR version</summary>
		Assembly = 0,
		/// <summary>Generate .NET Framework (&lt;=4.8) batch script</summary>
		BatFramework = 1,
		/// <summary>Generate .NET (4.8+) batch script</summary>
		BatNet = 2,
		/// <summary>Generate .NET Framework (&lt;=4.8) PowerShell script</summary>
		PowerShellFramework = 3,
		/// <summary>Generate .NET (4.8+) PowerShell script</summary>
		PowerShellNet = 4,
	}
}
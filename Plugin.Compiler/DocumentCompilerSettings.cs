using System;

namespace Plugin.Compiler
{
	public class DocumentCompilerSettings
	{
		private Type _returnType;

		/// <summary>Plugin that triggers the event</summary>
		public String CallerPluginId { get; set; }

		/// <summary>Class name for source code</summary>
		public String ClassName { get; set; }

		/// <summary>Array of method arguments</summary>
		/// <remarks>By default, the array of arguments is used as params <see cref="System.Object"/>[]</remarks>
		public Type[] ArgumentsType { get; set; }

		/// <summary>Method return type</summary>
		/// <remarks>
		/// By default, the return type is <see cref="System.Void"/>.
		/// Using the <see cref="System.Void"/> type will return empty.
		/// </remarks>
		public Type ReturnType
		{
			get => this._returnType;
			set => this._returnType = value == typeof(void) ? null : value;
		}
	}
}
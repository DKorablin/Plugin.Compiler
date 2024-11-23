using System;

namespace Plugin.Compiler
{
	public class DocumentCompilerSettings
	{
		private Type _returnType;

		/// <summary>Плагин, вызывающий событие</summary>
		public String CallerPluginId { get; set; }

		/// <summary>Наименование класса для иходного кода</summary>
		public String ClassName { get; set; }

		/// <summary>Массив аргументов метода</summary>
		/// <remarks>По умолчанию, массив аргументов используется как params <see cref="System.Object"/>[]</remarks>
		public Type[] ArgumentsType { get; set; }

		/// <summary>Тип результата возврата метода</summary>
		/// <remarks>
		/// По умолчанию тип возврата равен <see cref="System.Void"/>.
		/// При использовании типа <see cref="System.Void"/> возврат будет пустым
		/// </remarks>
		public Type ReturnType
		{
			get => this._returnType;
			set => this._returnType = value == typeof(void) ? null : value;
		}
	}
}
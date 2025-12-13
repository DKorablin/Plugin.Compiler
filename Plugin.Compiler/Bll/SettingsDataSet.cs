using System;
using System.IO;
namespace Plugin.Compiler.Bll
{
	internal partial class SettingsDataSet
	{
		/// <summary>Таблица описания метода динамической компиляции</summary>
		partial class PluginTableRow
		{
			/// <summary>Результат выполнения динамического метода</summary>
			public String ReturnTypeI
			{
				get	=> this.IsReturnTypeNull()
					? null
					: this.ReturnType;
				set
				{
					if(value == null)
						this.SetReturnTypeNull();
					else
						this.ReturnType = value;
				}
			}

			/// <summary>Путь скомпилированной сборки на файловой системе</summary>
			public String AssemblyPathI
			{
				get => this.AssemblyPath;
				set
				{
					if(File.Exists(value))
						this.AssemblyPath = value;
					else
						this.SetAssemblyPathNull();
				}
			}
		}
	}
}
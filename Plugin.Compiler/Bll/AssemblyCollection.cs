using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace Plugin.Compiler.Bll
{
	/// <summary>Класс массива сборок, которые будут добавлены в компилируемый код</summary>
	internal class AssemblyCollection : IEnumerable<String>
	{
		/// <summary>Список ссылок</summary>
		private Dictionary<String, String[]> References { get; set; }

		/// <summary>Получение пространств имён в определнной сборки</summary>
		/// <param name="assembly">Сборка в которой получить список всех пространств имён</param>
		/// <returns>Массив пространств имён</returns>
		public String[] this[String assembly]
		{
			get => this.References[assembly];
			private set => this.References[assembly] = value;
		}

		/// <summary>Получить наименование сборки по индексу</summary>
		/// <param name="index">Индекс сборки</param>
		/// <returns>Наименование сборки привязанной к проекту</returns>
		public String this[Int32 index] => this.References.Keys.ToArray()[index];

		/// <summary>Получить количство сборок в массиве</summary>
		public Int32 Count => this.References.Count;

		/// <summary>Создание массива сборок</summary>
		internal AssemblyCollection()
			=> this.References = new Dictionary<String, String[]>();

		/// <summary>Установить массив сборок, которые будут добавлены в исходный код</summary>
		/// <param name="assemblies">Список сборок для добавления в компиляцию</param>
		public void AddAssemblies(IEnumerable<String> assemblies)
		{
			_ = assemblies ?? throw new ArgumentNullException(nameof(assemblies));

			foreach(String asm in assemblies)
				this.AddAssembly(asm);
		}

		/// <summary>Добавить сборку в компиляцию</summary>
		/// <param name="assembly">Сборка, которая будет добавлена в компиляцию</param>
		public void AddAssembly(String assembly)
		{
			if(String.IsNullOrEmpty(assembly))
				throw new ArgumentNullException(nameof(assembly));

			if(!this.IsAssemblyAdded(assembly))
				this.References.Add(assembly, new String[] { });
		}

		/// <summary>Проверка нахождения сборки в компиляции</summary>
		/// <param name="assembly">Cборка, которую необходимо добавить в список сборок</param>
		/// <returns>Сборка уже добавлена в список</returns>
		public Boolean IsAssemblyAdded(String assembly)
		{
			if(String.IsNullOrEmpty(assembly))
				throw new ArgumentNullException(nameof(assembly));

			foreach(String key in this.References.Keys)
				if(key.Equals(assembly))
					return true;
			return false;
		}

		/// <summary>Проверка нахождения пространства имён в компиляции</summary>
		/// <param name="assembly">Сбока в которой осуществляется поиск</param>
		/// <param name="referencedNamespace">Наименование пространства имён</param>
		/// <returns>Пространство имён уже добавлено в список</returns>
		public Boolean IsNamespaceAdded(String assembly, String referencedNamespace)
		{
			if(String.IsNullOrEmpty(assembly))
				throw new ArgumentNullException(nameof(assembly));
			if(String.IsNullOrEmpty(referencedNamespace))
				throw new ArgumentNullException(nameof(referencedNamespace));

			foreach(String ns in this.References[assembly])
				if(ns.Equals(referencedNamespace))
					return true;
			return false;
		}

		/// <summary>Добавить сборку и пространство имён в компиляцию</summary>
		/// <param name="assembly">Сборка с добавляемым списком пространств имён</param>
		/// <param name="referencedNamespaces">Пространства имён, которые будут добавлены в компиляцию</param>
		public void AddNamespace(Assembly assembly, params String[] referencedNamespaces)
		{
			String assemblyName = assembly.GlobalAssemblyCache ? assembly.FullName : assembly.Location;
			this.AddNamespace(assemblyName, referencedNamespaces);
		}

		/// <summary>Добавить пространство имён в компиляцию</summary>
		/// <param name="assembly">Наименование или путь к сборке</param>
		/// <param name="referencedNamespaces">Пространства имён, которые будут добавлены в компиляцию</param>
		public void AddNamespace(String assembly, params String[] referencedNamespaces)
		{
			if(String.IsNullOrEmpty(assembly))
				throw new ArgumentNullException(nameof(assembly));
			if(referencedNamespaces == null)
				throw new ArgumentNullException(nameof(referencedNamespaces));

			this.AddAssembly(assembly);
			foreach(String ns in referencedNamespaces)
				if(!this.IsNamespaceAdded(assembly, ns))
				{
					List<String> namespaces = new List<String>(this.References[assembly]);
					namespaces.Add(ns);
					this.References[assembly] = namespaces.ToArray();
				}
		}

		/// <summary>Удалить все сборки</summary>
		public void Clear()
			=> this.References.Clear();

		/// <summary>Удалить сборку</summary>
		/// <param name="assembly">Наименование сборки для удаления</param>
		public void RemoveAssembly(String assembly)
		{
			if(String.IsNullOrEmpty(assembly))
				throw new ArgumentNullException(nameof(assembly));

			this.References.Remove(assembly);
		}

		/// <summary>Удалить пространствои имён из сборки</summary>
		/// <param name="assembly">Наименование сборки в котором находится удаляемое пространство имён</param>
		/// <param name="referencedNamespace">Наименование пространства имён для удаления</param>
		public void RemoveNamespace(String assembly, String referencedNamespace)
		{
			if(String.IsNullOrEmpty(assembly))
				throw new ArgumentNullException(nameof(assembly));
			if(String.IsNullOrEmpty(referencedNamespace))
				throw new ArgumentNullException(nameof(referencedNamespace));

			if(this.IsNamespaceAdded(assembly, referencedNamespace))
			{
				List<String> namespaces = new List<String>(this[assembly]);
				namespaces.Remove(referencedNamespace);
				if(namespaces.Count == 0)
					this.RemoveAssembly(assembly);
				else
					this[assembly] = namespaces.ToArray();
			}
		}

		public IEnumerator<String> GetEnumerator()
			=> this.References.Keys.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> this.GetEnumerator();
	}
}
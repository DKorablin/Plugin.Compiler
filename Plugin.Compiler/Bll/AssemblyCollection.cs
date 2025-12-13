using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace Plugin.Compiler.Bll
{
	/// <summary>Class array of assemblies that will be added to the compiled code</summary>
	internal class AssemblyCollection : IEnumerable<String>
	{
		/// <summary>List of reference</summary>
		private Dictionary<String, String[]> References { get; set; }

		/// <summary>Getting namespaces in a specific assembly</summary>
		/// <param name="assembly">The assembly in which to get a list of all namespaces</param>
		/// <returns>An array of namespaces</returns>
		public String[] this[String assembly]
		{
			get => this.References[assembly];
			private set => this.References[assembly] = value;
		}

		/// <summary>Get assembly name by index</summary>
		/// <param name="index">Assembly index</param>
		/// <returns>Name of the assembly associated with the project</returns>
		public String this[Int32 index] => this.References.Keys.ToArray()[index];

		/// <summary>Get the number of assemblies in the array</summary>
		public Int32 Count => this.References.Count;

		/// <summary>Creating an array of assemblies</summary>
		internal AssemblyCollection()
			=> this.References = new Dictionary<String, String[]>();

		/// <summary>Set an array of assemblies to be added to the source code</summary>
		/// <param name="assemblies">List of assemblies to add to the compilation</param>
		public void AddAssemblies(IEnumerable<String> assemblies)
		{
			_ = assemblies ?? throw new ArgumentNullException(nameof(assemblies));

			foreach(String asm in assemblies)
				this.AddAssembly(asm);
		}

		/// <summary>Add assembly to compilation</summary>
		/// <param name="assembly">The assembly to be added to the compilation</param>
		public void AddAssembly(String assembly)
		{
			if(String.IsNullOrEmpty(assembly))
				throw new ArgumentNullException(nameof(assembly));

			if(!this.IsAssemblyAdded(assembly))
				this.References.Add(assembly, new String[] { });
		}

		/// <summary>Checking if an assembly is in the compilation</summary>
		/// <param name="assembly">The assembly to add to the list of assemblies</param>
		/// <returns>The assembly has already been added to the list</returns>
		public Boolean IsAssemblyAdded(String assembly)
		{
			if(String.IsNullOrEmpty(assembly))
				throw new ArgumentNullException(nameof(assembly));

			foreach(String key in this.References.Keys)
				if(key.Equals(assembly))
					return true;
			return false;
		}

		/// <summary>Checking if a namespace is present in the compilation</summary>
		/// <param name="assembly">Assembly to search</param>
		/// <param name="referencedNamespace">Namespace name</param>
		/// <returns>Namespace already added to the list</returns>
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

		/// <summary>Add assembly and namespace to compilation</summary>
		/// <param name="assembly">Assembly with the list of namespaces to add</param>
		/// <param name="referencedNamespaces">Namespaces to be added to the compilation</param>
		public void AddNamespace(Assembly assembly, params String[] referencedNamespaces)
		{
			String assemblyName = assembly.GlobalAssemblyCache ? assembly.FullName : assembly.Location;
			this.AddNamespace(assemblyName, referencedNamespaces);
		}

		/// <summary>Add namespace to compilation</summary>
		/// <param name="assembly">Name or path to the assembly</param>
		/// <param name="referencedNamespaces">Namespaces to be added to the compilation</param>
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

		/// <summary>Delete all assemblies</summary>
		public void Clear()
			=> this.References.Clear();

		/// <summary>Delete assembly</summary>
		/// <param name="assembly">Name of the assembly to delete</param>
		public void RemoveAssembly(String assembly)
		{
			if(String.IsNullOrEmpty(assembly))
				throw new ArgumentNullException(nameof(assembly));

			this.References.Remove(assembly);
		}

		/// <summary>Remove namespaces from an assembly</summary>
		/// <param name="assembly">Name of the assembly containing the namespace to be removed</param>
		/// <param name="referencedNamespace">Name of the namespace to remove</param>
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
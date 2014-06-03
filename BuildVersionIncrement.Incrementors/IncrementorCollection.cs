using Qreed.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace BuildVersionIncrement.Incrementors
{
	internal class IncrementorCollection
	{
		private Dictionary<string, BaseIncrementor> _incrementors = new Dictionary<string, BaseIncrementor>();
		public BaseIncrementor this[string name]
		{
			get
			{
				BaseIncrementor result;
				if (this._incrementors.ContainsKey(name))
				{
					result = this._incrementors[name];
				}
				else
				{
					result = null;
				}
				return result;
			}
		}
		public BaseIncrementor this[OLD_IncrementStyle oldStyle]
		{
			get
			{
				return this[oldStyle.ToString()];
			}
		}
		public int Count
		{
			get
			{
				return this._incrementors.Keys.Count;
			}
		}
		public IncrementorCollection()
		{
			this._incrementors.Add(BuiltInBaseIncrementor.NoneIncrementor.Instance.Name, BuiltInBaseIncrementor.NoneIncrementor.Instance);
		}
		public void AddFrom(Assembly asm)
		{
			Logger.Write("Locating incrementors in assembly \"" + asm.FullName + "\" ...", LogLevel.Debug);
			List<Type> typesThatDeriveFromType = ReflectionHelper.GetTypesThatDeriveFromType(asm, typeof(BaseIncrementor), false, false);
			Logger.Write("Located " + typesThatDeriveFromType.Count + " incrementors.", LogLevel.Debug);
			foreach (Type current in typesThatDeriveFromType)
			{
				if (current != typeof(BuiltInBaseIncrementor.NoneIncrementor))
				{
					Logger.Write("Creating instance of incrementor type \"" + current.FullName + "\".", LogLevel.Info);
					BaseIncrementor baseIncrementor = (BaseIncrementor)Activator.CreateInstance(current);
					this._incrementors.Add(baseIncrementor.Name, baseIncrementor);
				}
			}
		}
		public string[] GetIncrementorNames()
		{
			string[] array = new string[this.Count];
			this._incrementors.Keys.CopyTo(array, 0);
			return array;
		}
		public BaseIncrementor[] GetIncrementors()
		{
			BaseIncrementor[] array = new BaseIncrementor[this.Count];
			this._incrementors.Values.CopyTo(array, 0);
			return array;
		}
	}
}

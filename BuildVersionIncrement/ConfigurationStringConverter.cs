using EnvDTE;
using System;
using System.Collections;
using System.ComponentModel;
namespace BuildVersionIncrement
{
	internal class ConfigurationStringConverter : TypeConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			SolutionItem solutionItem = ((SolutionItemIncrementSettings)context.Instance).SolutionItem;
			object[] array = this.CreateList(solutionItem);
			TypeConverter.StandardValuesCollection result;
			if (array != null)
			{
				result = new TypeConverter.StandardValuesCollection(array);
			}
			else
			{
				result = null;
			}
			return result;
		}
		private object[] CreateList(SolutionItem solutionItem)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add("Any");
			if (solutionItem.ItemType == SolutionItemType.Solution)
			{
				SolutionConfigurations solutionConfigurations = solutionItem.Solution.SolutionBuild.SolutionConfigurations;
				string a = "";
				foreach (SolutionConfiguration solutionConfiguration in solutionConfigurations)
				{
					if (a != solutionConfiguration.Name)
					{
						arrayList.Add(solutionConfiguration.Name);
						a = solutionConfiguration.Name;
					}
				}
			}
			else
			{
				object[] array = (object[])solutionItem.Project.ConfigurationManager.ConfigurationRowNames;
				object[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					object value = array2[i];
					arrayList.Add(value);
				}
			}
			return arrayList.ToArray();
		}
	}
}

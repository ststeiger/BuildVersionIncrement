using EnvDTE;
using System;
namespace BuildVersionIncrement
{
	public static class GlobalVariables
	{
		public static string GetGlobalVariable(Globals globals, string varName, string defaultValue)
		{
			string result;
			if (globals == null)
			{
				result = defaultValue;
			}
			else
			{
				object[] array = (object[])globals.VariableNames;
				if (globals.get_VariableExists(varName))
				{
					object[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						object obj = array2[i];
						if (obj.ToString() == varName)
						{
							result = (string)globals[varName];
							return result;
						}
					}
				}
				result = defaultValue;
			}
			return result;
		}
		public static void SetGlobalVariable(Globals globals, string variableName, string value, string defaultValue)
		{
			if (globals != null)
			{
				string globalVariable = GlobalVariables.GetGlobalVariable(globals, variableName, null);
				if (globalVariable != null)
				{
					if (string.Compare(globalVariable, value, true) != 0)
					{
						GlobalVariables.SetGlobalVariable(globals, variableName, value);
					}
				}
				else
				{
					if (string.Compare(value, defaultValue, true) != 0)
					{
						GlobalVariables.SetGlobalVariable(globals, variableName, value);
					}
				}
			}
		}
		public static void SetGlobalVariable(Globals globals, string varName, string value)
		{
			globals[varName] = value;
			globals.set_VariablePersists(varName, true);
		}
	}
}

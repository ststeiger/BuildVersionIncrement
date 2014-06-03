using EnvDTE;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
namespace BuildVersionIncrement
{
	public static class Common
	{
		public static string MakeAbsolutePath(string basePath, string relativePath)
		{
			char[] array = new char[]
			{
				Path.DirectorySeparatorChar
			};
			if (string.IsNullOrEmpty(basePath))
			{
				throw new ArgumentNullException("basePath");
			}
			if (string.IsNullOrEmpty(relativePath))
			{
				throw new ArgumentNullException("relativePath");
			}
			if (relativePath.Contains(".."))
			{
				List<string> list = new List<string>(relativePath.Split(array, StringSplitOptions.RemoveEmptyEntries));
				List<string> list2 = new List<string>(basePath.Split(array, StringSplitOptions.RemoveEmptyEntries));
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] == "..")
					{
						if (i == 0)
						{
							if (list2.Count > 1)
							{
								list2.RemoveAt(list2.Count - 1);
							}
							list.RemoveAt(0);
							i = -1;
						}
						else
						{
							list.RemoveRange(i - 1, 2);
							i -= 2;
						}
					}
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string current in list)
				{
					stringBuilder.Append(current);
					stringBuilder.Append(Path.DirectorySeparatorChar);
				}
				relativePath = stringBuilder.ToString().TrimEnd(array);
				stringBuilder.Length = 0;
				foreach (string current in list2)
				{
					stringBuilder.Append(current);
					stringBuilder.Append(Path.DirectorySeparatorChar);
				}
				basePath = stringBuilder.ToString().TrimEnd(array);
			}
			string pathRoot = Path.GetPathRoot(relativePath);
			string result;
			if (!string.IsNullOrEmpty(pathRoot))
			{
				if (pathRoot[0] == Path.DirectorySeparatorChar)
				{
					result = Path.GetPathRoot(basePath).TrimEnd(new char[]
					{
						Path.DirectorySeparatorChar
					}) + relativePath;
				}
				else
				{
					result = relativePath;
				}
			}
			else
			{
				result = Path.Combine(basePath.TrimEnd(array) + Path.DirectorySeparatorChar, relativePath);
			}
			return result;
		}
		public static string MakeRelativePath(string basePath, string targetPath)
		{
			if (string.IsNullOrEmpty(basePath))
			{
				throw new ArgumentNullException("basePath");
			}
			if (string.IsNullOrEmpty(targetPath))
			{
				throw new ArgumentNullException("targetPath");
			}
			bool flag = Path.IsPathRooted(basePath) && Path.IsPathRooted(targetPath);
			string result;
			if (flag)
			{
				bool flag2 = string.Compare(Path.GetPathRoot(basePath), Path.GetPathRoot(targetPath), true) != 0;
				if (flag2)
				{
					result = targetPath;
					return result;
				}
			}
			StringCollection stringCollection = new StringCollection();
			string[] array = basePath.Split(new char[]
			{
				Path.DirectorySeparatorChar
			});
			string[] array2 = targetPath.Split(new char[]
			{
				Path.DirectorySeparatorChar
			});
			int num = Math.Min(array.Length, array2.Length);
			int num2 = -1;
			for (int i = 0; i < num; i++)
			{
				if (string.Compare(array[i], array2[i], true) != 0)
				{
					break;
				}
				num2 = i;
			}
			if (num2 == -1)
			{
				result = targetPath;
			}
			else
			{
				for (int i = num2 + 1; i < array.Length; i++)
				{
					if (array[i].Length > 0)
					{
						stringCollection.Add("..");
					}
				}
				for (int i = num2 + 1; i < array2.Length; i++)
				{
					stringCollection.Add(array2[i]);
				}
				string[] array3 = new string[stringCollection.Count];
				stringCollection.CopyTo(array3, 0);
				string text = string.Join(Path.DirectorySeparatorChar.ToString(), array3);
				result = text;
			}
			return result;
		}
		public static AssemblyFileVersionAttribute GetAssemblyFileAttribute(Assembly assembly)
		{
			AssemblyFileVersionAttribute result;
			if (null == assembly)
			{
				result = null;
			}
			else
			{
				object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
				if (null == customAttributes)
				{
					result = null;
				}
				else
				{
					result = (AssemblyFileVersionAttribute)customAttributes[0];
				}
			}
			return result;
		}
		public static string DumpProperties(Properties props)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Property property in props)
			{
				try
				{
					stringBuilder.Append(string.Format("Name: \"{0}\" Value: \"{1}\"\r\n", property.Name, property.Value));
				}
				catch
				{
					stringBuilder.Append(string.Format("Name: \"{0}\" Value: \"(UNKNOWN)\"\r\n", property.Name));
				}
			}
			return stringBuilder.ToString();
		}
	}
}

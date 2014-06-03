using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
namespace Qreed.Reflection
{
	public static class EnumHelper
	{
		public static string GetDescription(Enum value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			string text = value.ToString();
			FieldInfo field = value.GetType().GetField(text);
			DescriptionAttribute[] array = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (array != null && array.Length > 0)
			{
				text = array[0].Description;
			}
			return text;
		}
		public static IList ToList(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ArrayList arrayList = new ArrayList();
			Array values = Enum.GetValues(type);
			foreach (Enum @enum in values)
			{
				arrayList.Add(new KeyValuePair<Enum, string>(@enum, EnumHelper.GetDescription(@enum)));
			}
			return arrayList;
		}
		public static void DataBind(Type enumType, ListControl control, object targetObject, string propertyName)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (targetObject == null)
			{
				throw new ArgumentNullException("targetObject");
			}
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException(propertyName);
			}
			IList dataSource = EnumHelper.ToList(enumType);
			control.DataSource = dataSource;
			control.DisplayMember = "Value";
			control.ValueMember = "Key";
			PropertyInfo propInfo = targetObject.GetType().GetProperty(propertyName);
			if (propInfo == null)
			{
				throw new ApplicationException("The targetObject doesn't contains a property named " + propertyName);
			}
			control.SelectedValue = propInfo.GetValue(targetObject, null);
			control.SelectedValueChanged += delegate(object sender, EventArgs e)
			{
				propInfo.SetValue(targetObject, control.SelectedValue, null);
			};
		}
	}
}

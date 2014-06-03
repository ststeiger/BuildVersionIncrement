using System;
using System.ComponentModel;
using System.Globalization;
namespace BuildVersionIncrement.Incrementors
{
	internal class BaseIncrementorConverter : TypeConverter
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
			BaseIncrementor[] incrementors = BuildVersionIncrementor.Instance.Incrementors.GetIncrementors();
			return new TypeConverter.StandardValuesCollection(incrementors);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			object result;
			if (value.GetType() == typeof(string))
			{
				result = BuildVersionIncrementor.Instance.Incrementors[(string)value];
			}
			else
			{
				result = base.ConvertFrom(context, culture, value);
			}
			return result;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			object result;
			if (destinationType == typeof(string))
			{
				if (value == null)
				{
					result = "None";
				}
				else
				{
					result = ((BaseIncrementor)value).Name;
				}
			}
			else
			{
				result = base.ConvertTo(context, culture, value, destinationType);
			}
			return result;
		}
	}
}

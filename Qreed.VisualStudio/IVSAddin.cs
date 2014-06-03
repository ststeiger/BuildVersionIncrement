using EnvDTE;
using EnvDTE80;
using Extensibility;
using System;
namespace Qreed.VisualStudio
{
	public interface IVSAddin
	{
		DTE2 ApplicationObject
		{
			get;
		}
		AddIn AddInInstance
		{
			get;
		}
		string CommandBarResourceName
		{
			get;
		}
		ext_ConnectMode ConnectMode
		{
			get;
		}
	}
}

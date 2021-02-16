// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Dynamic;
using T2.CLS.LoggerExtensions.Core.Config;

namespace T2.Cls.LogTransport.Common.Config
{
	public class OutputConfig
	{
		#region Properties

		public string Output { get; set; }

		public string System { get; set; }

		public BufferSettingsConfig BufferSettings { get; set; }
		
		#endregion

		#region OverrideMethods

		public override string ToString()
		{
			return $"{{ {Environment.NewLine} " +
					$"	Output: {Output};  {Environment.NewLine} " +
					$"	System: {System};  {Environment.NewLine} " +
					$"	BufferSettingsConfig {BufferSettings}" +
					$"	}}" ;
		}

		#endregion
	}
}
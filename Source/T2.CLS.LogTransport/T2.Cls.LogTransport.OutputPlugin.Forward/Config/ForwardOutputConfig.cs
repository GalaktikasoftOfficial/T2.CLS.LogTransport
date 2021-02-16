// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System.Collections.Generic;
using T2.Cls.LogTransport.Common.Config;

namespace T2.Cls.LogTransport.OutputPlugin.Forward.Config
{
	public sealed class ForwardOutputConfig : OutputConfig
	{
		#region Properties

		public List<string> Servers { get; set; } = new List<string>();

		#endregion
	}
}
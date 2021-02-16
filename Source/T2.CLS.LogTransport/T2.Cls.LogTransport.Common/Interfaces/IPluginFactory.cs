// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System.Collections.Generic;

namespace T2.Cls.LogTransport.Common.Interfaces
{
	public interface IPluginFactory
	{
		#region Methods

		IOutputPlugin GetOutput(string systemName);

		IEnumerable<IOutputPlugin> OutputPlugins { get; }

		#endregion
	}
}
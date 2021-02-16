// Copyright (C) 2019 Topsoft (https://topsoft.by)

namespace T2.Cls.LogTransport.Common.Interfaces
{
	public interface ILogMetrics
	{
		#region Methods

		void LogEntriesHandled(int count);

		void LogEntriesWritten(int count);

		#endregion
	}
}
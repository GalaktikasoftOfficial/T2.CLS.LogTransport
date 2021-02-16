// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System.IO;
using System.Threading.Tasks;

namespace T2.Cls.LogTransport.Common.Interfaces
{
	public interface IOutputPlugin
	{
		#region Methods

		ValueTask<long> WriteLogAsync(StreamReader streamReader);
		long WriteLog(string content);

		long PendingCount { get; }

		double SendRatePerSecond { get; }

		#endregion
	}
}
using System;
using System.Collections.Generic;
using System.Text;

namespace T2.Cls.LogTransport.Common.Structs
{
	public struct MetricsData
	{
		public MetricsData(long pending, double sendRatePerSecond)
		{
			Pending = pending;
			SendRatePerSecond = sendRatePerSecond;
		}

		public long Pending { get; }

		public double SendRatePerSecond { get; }

		public override string ToString()
		{
			return $"Metric Data {{Pending : {Pending}; SendRatePerSecond : {SendRatePerSecond}}}";
		}
	}
}

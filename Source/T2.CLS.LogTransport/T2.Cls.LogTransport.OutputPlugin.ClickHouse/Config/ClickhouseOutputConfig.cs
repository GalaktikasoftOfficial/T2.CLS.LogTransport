// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;
using T2.Cls.LogTransport.Common.Config;

namespace T2.Cls.LogTransport.OutputPlugin.ClickHouse.Config
{
	public sealed class ClickhouseOutputConfig : OutputConfig
	{
		#region Properties

		public List<string> Columns { get; set; } = new List<string>();

		public string Database { get; set; } = "default";

		public string Host { get; set; } = "localhost";

		public string Password { get; set; }

		public int Port { get; set; } = 8123;

		public string Table { get; set; }

		public string User { get; set; } = "default";

		#endregion

		#region override Methods

		public override string ToString()
		{
			var baseString = base.ToString();
			return $"{{" +
					$"	Base: {baseString}; {Environment.NewLine}" +
					$"	Host: {Host};  {Environment.NewLine}" +
					$"	Database: {Database};  {Environment.NewLine}" +
					$"	Table: {Table};  {Environment.NewLine}" +
					$"}}";
		}

		#endregion
	}
}
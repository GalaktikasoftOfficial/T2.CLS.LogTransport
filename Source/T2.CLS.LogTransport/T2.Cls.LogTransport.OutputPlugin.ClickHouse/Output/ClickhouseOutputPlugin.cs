// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using T2.CLS.LoggerExtensions.Core.Config;
using T2.Cls.LogTransport.Common.Interfaces;
using T2.Cls.LogTransport.Common.Config;
using T2.Cls.LogTransport.OutputPlugin.ClickHouse.Config;
using T2.Cls.LogTransport.Common.Output;
using T2.Cls.LogTransport.OutputPlugin.ClickHouse.Extensions;

namespace T2.Cls.LogTransport.OutputPlugin.ClickHouse.Output
{
	public sealed class ClickhouseOutputPlugin : BufferedOutputPlugin
	{
		#region Fields

		//private readonly RowFormatter _formatter;
		private readonly HttpClient _httpClient = new HttpClient();
		//private readonly string _insertQuery;
		private readonly string _insertJsonQuery;
		private readonly ILogMetrics _metrics;
		private readonly OutputConfig _outputConfig;
		private readonly string _queryUrl;
		private readonly List<string> _columns;
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger _logger;

		#endregion

		#region Ctors

		public ClickhouseOutputPlugin(ClickhouseOutputConfig outputConfig, ILogMetrics metrics,
			ILoggerFactory loggerFactory) : base (
				outputConfig.System, metrics, outputConfig?.BufferSettings, loggerFactory)
		{
			_loggerFactory = loggerFactory;
			_logger = _loggerFactory.CreateLogger<ClickhouseOutputPlugin>();

            _logger.SLT00018_Debug_ClickhouseOutputPlugin_outputConfig_outputConfig(outputConfig);

			_outputConfig = outputConfig;
			_metrics = metrics;
			_columns = outputConfig.Columns;

			var columns = new Dictionary<string, int>();

			foreach (var column in outputConfig.Columns)
				columns[column] = columns.Count;

			//_insertQuery = $"INSERT INTO {outputConfig.Database}.{outputConfig.Table} ({string.Join(", ", outputConfig.Columns)}) FORMAT Values ";
			_insertJsonQuery = $"INSERT INTO {outputConfig.Database}.{outputConfig.Table} FORMAT JSONEachRow ";
			//_formatter = new RowFormatter(columns);

			var urlBuilder = new UriBuilder("http", outputConfig.Host, outputConfig.Port);
			var properties = new Dictionary<string, string>();

			if (outputConfig.User != null)
				properties.Add("user", outputConfig.User);

			if (outputConfig.Password != null)
				properties.Add("password", outputConfig.Password);

			properties.Add("input_format_null_as_default", "1");

			_queryUrl = QueryHelpers.AddQueryString(urlBuilder.ToString(), properties);
		}

		#endregion

		#region Methods

		private string BuildQueryJsonContent(IReadOnlyList<string> jsonLines)
		{

			var jsonStringBuilder = new StringBuilder();

			jsonStringBuilder.AppendLine(_insertJsonQuery);

			foreach (var jsonLine in jsonLines)
			{
				jsonStringBuilder.AppendLine(CheckColumns(jsonLine));
			}

			return jsonStringBuilder.ToString();
		}

		private string CheckColumns(string jsonLine)
		{
			var jobject = JObject.Parse(jsonLine);
		
			var excludeProps = jobject.Properties().Where(t => !_columns.Contains(t.Name)).Select(t => t.Name).ToList();
			excludeProps.ForEach(t =>
			{
				jobject.Remove(t);
				//_logger.CLT00001_Warning_Property_propertyName_ignored(t);
			});

			return jobject.ToString();
		}


		/*private string BuildQueryContent(IReadOnlyList<string> jsonLines)
		{
			var jsonStringBuilder = new StringBuilder();

			jsonStringBuilder.Append('[');

			var first = true;

			foreach (var line in jsonLines)
			{
				if (first == false)
					jsonStringBuilder.Append(',');

				jsonStringBuilder.AppendLine(line);

				first = false;
			}

			jsonStringBuilder.Append(']');

			var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonStringBuilder.ToString()));
			var formatter = _formatter;
			var stringBuilder = new StringBuilder();
			using var stringWriter = new StringWriter(stringBuilder);

			stringWriter.WriteLine(_insertQuery);
			var rowCount = formatter.FormatRow(ref reader, stringWriter);

			if (rowCount != jsonLines.Count)
				Debug.WriteLine($"LineCount: {jsonLines.Count}, JsonRowCount: {rowCount}");

			stringWriter.Flush();

			return stringBuilder.ToString();
		}*/

		protected override void PushBuffer(IReadOnlyList<string> buffer)
		{
			var queryUrl = _queryUrl;
			
			//var content = BuildQueryContent(buffer);
			var content = BuildQueryJsonContent(buffer);
			
			using var stringContent = new StringContent(content);
			
			var result = _httpClient.PutAsync(queryUrl, stringContent).Result;

			if (result.IsSuccessStatusCode == false)
			{
                //_logger.CLT00002_Error_Error_sending_log_in_ClickHouse_content(result.Content.ReadAsStringAsync().Result);
							   
				throw new Exception(result.Content.ReadAsStringAsync().Result);
			}

			_metrics?.LogEntriesHandled(buffer.Count);
		}

		#endregion

		#region Nested Types

		/*private abstract class ClickHouseValue
		{
			#region Methods

			public abstract void FormatValue(StringWriter writer);

			public abstract void ReadValue(ref Utf8JsonReader reader);

			#endregion
		}

		private sealed class NumberValue : ClickHouseValue
		{
			#region Properties

			public double Value { get; set; }

			#endregion

			#region Methods

			public override void FormatValue(StringWriter writer)
			{
				writer.Write(Value);
			}

			public override void ReadValue(ref Utf8JsonReader reader)
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.Number:
						Value = reader.GetDouble();

						break;
					case JsonTokenType.True:
						Value = 1;

						break;
					case JsonTokenType.False:
						Value = 0;

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			#endregion
		}

		private sealed class NullValue : ClickHouseValue
		{
			#region Static Fields and Constants

			public static readonly ClickHouseValue Instance = new NullValue();

			#endregion

			#region Ctors

			private NullValue()
			{
			}

			#endregion

			#region Methods

			public override void FormatValue(StringWriter writer)
			{
				writer.Write("null");
			}

			public override void ReadValue(ref Utf8JsonReader reader)
			{
				if (reader.TokenType == JsonTokenType.Null)
				{
				}
				else
					throw new ArgumentOutOfRangeException();
			}

			#endregion
		}*/

		/*private sealed class StringValue : ClickHouseValue
		{
			#region Properties

			public string Value { get; set; }

			#endregion

			#region Methods

			public override void FormatValue(StringWriter writer)
			{
				writer.Write('\'');
				writer.Write(Value);
				writer.Write('\'');
			}

			public override void ReadValue(ref Utf8JsonReader reader)
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.String:
						Value = reader.GetString();

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			#endregion
		}*/

		/*private sealed class RowFormatter
		{
			#region Fields

			private readonly Dictionary<string, int> _columns;
			private readonly List<ClickHouseValue> _row = new List<ClickHouseValue>();

			#endregion

			#region Ctors

			public RowFormatter(Dictionary<string, int> columns)
			{
				_columns = columns;

				foreach (var _ in _columns)
					_row.Add(NullValue.Instance);
			}

			#endregion

			#region Methods

			private static ClickHouseValue CreateValue(JsonTokenType token)
			{
				switch (token)
				{
					case JsonTokenType.String:
						return new StringValue();
					case JsonTokenType.Number:
						return new NumberValue();
					case JsonTokenType.True:
						return new NumberValue();
					case JsonTokenType.False:
						return new NumberValue();
					case JsonTokenType.Null:
						return NullValue.Instance;
					default:
						throw new ArgumentOutOfRangeException(nameof(token), token, null);
				}
			}

			public int FormatRow(ref Utf8JsonReader reader, StringWriter writer)
			{
				if (reader.Read() == false || reader.TokenType != JsonTokenType.StartArray)
					throw new InvalidOperationException();

				var first = true;
				var rowCount = 0;

				while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
				{
					rowCount++;

					while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
					{
						var name = reader.GetString();
						reader.Read();

						if (_columns.TryGetValue(name, out var index))
						{
							if (_row[index]?.GetType() != GetValueType(reader.TokenType))
								_row[index] = CreateValue(reader.TokenType);

							_row[index].ReadValue(ref reader);
						}
					}

					if (first == false)
						writer.Write(",\n");

					var delimiter = '(';

					foreach (var value in _row)
					{
						writer.Write(delimiter);
						value.FormatValue(writer);

						delimiter = ',';
					}

					writer.Write(')');

					first = false;

					if (reader.TokenType != JsonTokenType.EndObject)
						throw new InvalidOperationException();
				}

				if (reader.TokenType != JsonTokenType.EndArray)
					throw new InvalidOperationException();

				return rowCount;
			}

			private static Type GetValueType(JsonTokenType token)
			{
				switch (token)
				{
					case JsonTokenType.String:
						return typeof(StringValue);
					case JsonTokenType.Number:
						return typeof(NumberValue);
					case JsonTokenType.True:
						return typeof(NumberValue);
					case JsonTokenType.False:
						return typeof(NumberValue);
					case JsonTokenType.Null:
						return typeof(NullValue);
					default:
						throw new ArgumentOutOfRangeException(nameof(token), token, null);
				}
			}

			#endregion
		}
		*/

		#endregion
	}
}
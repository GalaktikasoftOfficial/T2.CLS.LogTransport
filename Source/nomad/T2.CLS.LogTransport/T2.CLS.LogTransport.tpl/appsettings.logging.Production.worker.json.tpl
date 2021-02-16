{{ define "DurableClickhouse"}}
				"DurableClickhouse": {
				  "name": "DurableClickhouse",
				  "type": "DurableClickhouse",
				  "requestUri": {{key "T2.CLS.LogTransport/Worker/Logging/Nlog/DurableClickhouse/requestUri"}},
				  "bufferPath": "{{env "meta.LogBufferPath" | replaceAll "\\" "\\\\" }}{{env "NOMAD_TASK_NAME" }}\\{{env "NOMAD_ALLOC_INDEX"}}",
				  "workerCount": 2,
				  "attributes": [
					{
					  "name": "SystemTag",
					  "layout": "Common Logging Service XUM API"
					},
					{
					  "name": "SourceContext",
					  "layout": "${logger}"
					}
				  ]
				},
{{end}}
{{ define "DurableClickhouseRule"}}
				{
					"logger": "*",
					"minLevel": "Trace",
					"writeTo": "DurableClickhouse"
				},
{{end}}
{{ define "Nlog"}}
		, "NLog": {
			"throwConfigExceptions": true,
			"internalLogLevel": "info",
			"internalLogFile": "${basedir}/logs/internal-nlog.txt",
			"autoReload": true,
			"extensions": [
				{ "assembly": "NLog.Extensions.Logging" },
				{ "assembly": "T2.CLS.LoggerExtensions.Core" },
				{ "assembly": "T2.CLS.LoggerExtensions.NLog" }
			],
			"targets": {				
				{{ if keyOrDefault "T2.CLS.LogTransport/Worker/Logging/Nlog/SendLogInClickHouse" "false" | parseBool }}
					{{ executeTemplate "DurableClickhouse" }}
				{{end}}
				{{keyOrDefault "T2.CLS.LogTransport/Worker/Logging/Nlog/OtherTargets" ""}}
			},
			"rules": [
				{{ if keyOrDefault "T2.CLS.LogTransport/Worker/Logging/Nlog/SendLogInClickHouse" "false" | parseBool }}
					{{ executeTemplate "DurableClickhouseRule" }}
					{{keyOrDefault "T2.CLS.LogTransport/Worker/Logging/Nlog/OtherTargetRules" ""}}
				{{end}}
			]
		}
{{end}}
{
	"Logging": {
		"IncludeScopes": false,
		"LogLevel": {
			{{ if keyExists "T2.CLS.LogTransport/Worker/Logging/LogLevel/Default"}}
				"Default": {{key "T2.CLS.LogTransport/Worker/Logging/LogLevel/Default"}},
			{{end}}
			{{ if keyExists "T2.CLS.LogTransport/Worker/Logging/LogLevel/Default"}}
				"System": {{key "T2.CLS.LogTransport/Worker/Logging/LogLevel/System"}},
			{{end}}
			{{ if keyExists "T2.CLS.LogTransport/Worker/Logging/LogLevel/Default"}}
				"Microsoft": {{key "T2.CLS.LogTransport/Worker/Logging/LogLevel/Microsoft"}}
			{{end}}
		}
		{{ if keyOrDefault "T2.CLS.LogTransport/Worker/Logging/UseNLog" "false" | parseBool }}
			{{ executeTemplate "Nlog" }}
		{{end}}
	}
}

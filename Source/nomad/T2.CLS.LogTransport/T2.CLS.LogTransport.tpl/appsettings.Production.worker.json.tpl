{
	{{ if keyExists "T2.CLS.LogTransport/Worker/AllowedHosts" }} "AllowedHosts" : {{key "T2.CLS.LogTransport/Worker/AllowedHosts" }} ,{{end}}
	"Transport": {
		"ValidateInputData": {{keyOrDefault "T2.CLS.LogTransport/Worker/Transport/ValidateInputData" "false" }}
	},
	"BufferSettings": {
		"BufferPath": "{{env "meta.LogBufferPath" | replaceAll "\\" "\\\\" }}{{env "NOMAD_TASK_NAME" }}\\{{env "NOMAD_ALLOC_INDEX"}}",
		"WorkerCount": 1,
		"ReadLimit": 1024,
		"MemoryBufferLimit": 512,
		"FileBufferLimit": 0,
		"FlushTimeout": 3000,
		"ResendTimeout": 10000
	}	,
	
	"OutputPlugins": [
		{{ range  $i, $k := ls "T2.CLS.LogTransport/OutputPlugins" }}
			{{ if ne $i 0 }},{{ end }}{{.Value}}
		{{ end }}
	]
}
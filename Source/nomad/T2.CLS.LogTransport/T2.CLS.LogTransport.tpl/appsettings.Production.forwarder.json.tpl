{
	{{ if keyExists "T2.CLS.LogTransport/Forwarder/AllowedHosts" }} "AllowedHosts" : {{key "T2.CLS.LogTransport/Forwarder/AllowedHosts" }} ,{{end}}
	"Transport": {
		"ValidateInputData": {{keyOrDefault "T2.CLS.LogTransport/Forwarder/Transport/ValidateInputData" "false" }}
	},
	"BufferSettings": {
		"BufferPath": "{{env "meta.LogBufferPath" | replaceAll "\\" "\\\\" }}{{env "NOMAD_TASK_NAME" }}\\{{env "NOMAD_ALLOC_INDEX"}}",
		"WorkerCount": 1,
		"ReadLimit": 1024,
		"MemoryBufferLimit": 512,
		"FileBufferLimit": 0,
		"FlushTimeout": 3000,
		"ResendTimeout": 10000
	},
	{{ define "worker_urls" }} 	
		{{range $i, $k := service "T2-CLS-LOGTRANSPORT-WORKER"}}
			{{ if ne $i 0 }},{{ end }}
		   "http://{{.Address}}:{{.Port}}"	
		{{end}}
	{{end}}

	"OutputPlugins": [
		{{ range  $i, $k := ls "T2.CLS.LogTransport/OutputPlugins" }}
			{{ if ne $i 0 }},{{ end }}
			{
				"Output": "Forward",
				"System": "{{.Key}}",
				"Servers": 
				[
					{{executeTemplate "worker_urls"}}
				] 
			}		
		{{ end }}
	]
}
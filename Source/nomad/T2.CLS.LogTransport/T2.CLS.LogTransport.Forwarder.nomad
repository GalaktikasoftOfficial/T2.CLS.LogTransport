	job "T2-CLS-LOGTRANSPORT-FORWARDER" {
    affinity {
      attribute = "${attr.kernel.name}"
      value     = "windows"
      weight    = 50
    }
	meta {
		"applicationVersion" = "%%applicationVersion%%"
	}
	
    region      = "global"
    datacenters = ["dc1"]
    type        = "service"
    group "T2-CLS-LOGTRANSPORT-FORWARDER-GROUP" {
        count = 1
        task "T2-CLS-LOGTRANSPORT-FORWARDER-DAMON" {

			artifact {
                ## Replace with wherever your service artifact is hosted
                source = "${meta.pathToSourceServer}damon.exe.zip"
				destination = "local/.."
            }
			
            artifact {
                ## Replace with wherever your service artifact is hosted
                source = "${meta.pathToSourceServer}T2.CLS.LogTransport/${NOMAD_META_applicationVersion}/T2.CLS.LogTransport.zip"
				destination = "local/.."
            }
			
			artifact {
				source = "${meta.pathToSourceServer}T2.CLS.LogTransport/${NOMAD_META_applicationVersion}/T2.CLS.LogTransport.tpl.zip"
				destination = "local/../tpl"
			}
			
			artifact {
				source = "${meta.pathToSourceServer}by01-app21.certificates.zip"
				destination = "local/../Certificates/"
			}
			
			template {
				source = "tpl/appsettings.Production.forwarder.json.tpl"
				destination = "appsettings.Production.json"
			}
						
			template {
				source = "tpl/appsettings.logging.Production.forwarder.json.tpl"
				destination = "appsettings.logging.Production.json"
			}
						
			template 
			{
				data = <<EOH
					ASPNETCORE_Kestrel__Certificates__Default__Password="{{key "Certificate/Default__Password"}}"
					ASPNETCORE_Kestrel__Certificates__Default__Path="{{key "Certificate/Default__Path"}}"
				EOH
				
				destination = "secrets/file.env"
				env = true			
			}			
			
            driver = "raw_exec"
            config {
                # Damon is the task entrypoint
                command = "damon.exe"
                # Your command and services should be in the 'args' section
				args    = ["${NOMAD_TASK_DIR}\\..\\T2.CLS.LogTransport.exe"]
            }
           
			resources {
                cpu    = 102
                memory = 102
                network {
                    mbits = 10

                    ## For metrics exposed by Damon
                    port "T2_CLS_LOGTRANSPORT_FORWARDER_PORT" {
                      static = "4000"
                    }
                    port "damon" {}
                }
            }
            env {
                ## Example of overriding NOMAD_CPU_LIMIT to give it more CPU than allocated
                "DAMON_CPU_LIMIT" = "20000"
                "DAMON_MEMORY_LIMIT" = "20000"	
				"ASPNETCORE_ENVIRONMENT" = "Production"
				"ASPNETCORE_URLS" = "https://0.0.0.0:${NOMAD_PORT_T2_CLS_LOGTRANSPORT_FORWARDER_PORT}"
				
            }
			
            ## For damon's metrics endpoint
            service {
                port = "damon"
                name = "${NOMAD_TASK_NAME}"
				tags = ["damon"]
            }
			service {
                port = "T2_CLS_LOGTRANSPORT_FORWARDER_PORT"
                name = "${NOMAD_JOB_NAME}"
				tags = ["T2", "CLS", "T2_CLS_LOGTRANSPORT_FORWARDER", "${NOMAD_META_applicationVersion}"]				
            }
        }
    }
}

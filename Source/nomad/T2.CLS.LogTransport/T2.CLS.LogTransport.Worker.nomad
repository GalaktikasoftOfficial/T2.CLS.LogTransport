	job "T2-CLS-LOGTRANSPORT-WORKER" {
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
    group "T2-CLS-LOGTRANSPORT-WORKER-GROUP" {
        count = 2
        task "T2-CLS-LOGTRANSPORT-WORKER" {

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
				source = "tpl/appsettings.Production.Worker.json.tpl"
				destination = "appsettings.Production.json"
			}
						
			template {
				source = "tpl/appsettings.logging.Production.worker.json.tpl"
				destination = "appsettings.logging.Production.json"
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

                    port "T2_CLS_LOGTRANSPORT_WORKER_PORT" {}
                    port "damon" {}
                }
            }
            env {
                "DAMON_CPU_LIMIT" = "20000"
                "DAMON_MEMORY_LIMIT" = "20000"	
				"ASPNETCORE_ENVIRONMENT" = "Production"
				"ASPNETCORE_URLS" = "http://0.0.0.0:${NOMAD_PORT_T2_CLS_LOGTRANSPORT_WORKER_PORT}"
            }
			
            ## For damon's metrics endpoint
            service {
                port = "damon"
                name = "${NOMAD_JOB_NAME}-damon"
				tags = ["damon"]
            }
            service {
                port = "T2_CLS_LOGTRANSPORT_WORKER_PORT"
                name = "${NOMAD_JOB_NAME}"
				tags = [ "T2", "CLS", "T2_CLS_LOGTRANSPORT_WORKER", "${NOMAD_META_applicationVersion}"]
            }
        }
    }
}

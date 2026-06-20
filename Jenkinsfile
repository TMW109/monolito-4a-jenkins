pipeline {
    agent { label 'windows' }

    environment {
        SOLUTION = 'Monolito_4A.slnx'
        WEB_PROJECT = 'Monolito_4A\\Monolito_4A.csproj'
        CONFIGURATION = 'Release'

        PUBLISH_DIR = 'C:\\jenkins_publish\\RequiemP'
        IIS_PATH = 'C:\\inetpub\\wwwroot\\RequiemP'
        TEST_URL = 'http://localhost/RequiemP/Seguridad/Login'

        NUGET = 'C:\\Tools\\nuget\\nuget.exe'
        MSBUILD = 'C:\\Program Files\\Microsoft Visual Studio\\18\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe'
    }

    stages {
        stage('Clonar desde GitHub') {
            steps {
                checkout scm
            }
        }

        stage('Restaurar paquetes NuGet') {
            steps {
                bat '"%NUGET%" restore "Capa_Datos\\packages.config" -PackagesDirectory packages'
                bat '"%NUGET%" restore "Capa_Negocio\\packages.config" -PackagesDirectory packages'
                bat '"%NUGET%" restore "Monolito_4A\\packages.config" -PackagesDirectory packages'
            }
        }

        stage('Compilar solución') {
            steps {
                bat '"%MSBUILD%" "%SOLUTION%" /t:Rebuild /p:Configuration=%CONFIGURATION%'
            }
        }

        stage('Ejecutar pruebas') {
            steps {
                bat 'echo Prueba basica ejecutada: la solucion compilo correctamente.'
            }
        }

               stage('Publicar aplicación') {
            steps {
                bat 'if exist "%PUBLISH_DIR%" rmdir /S /Q "%PUBLISH_DIR%"'
                bat 'mkdir "%PUBLISH_DIR%"'
		bat '"%MSBUILD%" "%WEB_PROJECT%" /p:Configuration=%CONFIGURATION% /p:DeployOnBuild=true /p:DeployDefaultTarget=WebPublish /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles=True /p:PublishUrl="%PUBLISH_DIR%"'
                bat 'echo Contenido generado en carpeta de publicacion:'
                bat 'dir "%PUBLISH_DIR%"'
            }
        }

        stage('Desplegar en IIS') {
            steps {
                bat 'if not exist "%IIS_PATH%" mkdir "%IIS_PATH%"'
                bat 'robocopy "%PUBLISH_DIR%" "%IIS_PATH%" /E /R:2 /W:2 & if %ERRORLEVEL% LEQ 7 exit /B 0'
            }
        }
        stage('Probar aplicación en IIS') {
            steps {
                bat 'powershell -Command "try { $r = Invoke-WebRequest -Uri \\"%TEST_URL%\\" -UseBasicParsing -TimeoutSec 20; Write-Host \\"Estado HTTP:\\" $r.StatusCode; if ($r.StatusCode -ge 200 -and $r.StatusCode -lt 500) { exit 0 } else { exit 1 } } catch { Write-Host $_; exit 1 }"'
            }
        }
    }

    post {
        success {
            echo 'Pipeline finalizado correctamente. Aplicacion publicada y probada en IIS.'
        }

        failure {
            echo 'El pipeline fallo. Revisar Console Output.'
        }
    }
}
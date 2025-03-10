pipeline {
    agent any
	environment { 
        CC = "gcb@kodfu.com"
        SENT_TO="ibrahim.kaya@kodfu.com"
    }
    
     stages {
		stage('Restore packages'){
		   steps{
			  sh "dotnet restore API/API.csproj"
			  sh "dotnet restore UI/UI.csproj"
			  sh "dotnet restore MobilAPI/MobilAPI.csproj"
			 }
		}
		stage('Clean'){
		steps{
			sh "dotnet clean API/API.csproj"
			sh "dotnet clean UI/UI.csproj"
			sh "dotnet clean MobilAPI/MobilAPI.csproj"
		 }
	   }
	   stage('Build Mobile'){
		   steps{
			  sh """
					rm -rf /var/jenkins_home/workspace/joyblast-mobile_master/MobilAPI/bin/Release/net6.0/*
					dotnet build MobilAPI/MobilAPI.csproj --configuration Release
					dotnet publish -c Release MobilAPI/MobilAPI.csproj --no-build
				"""
			}
		 }

		 stage('Build Backend'){
		   steps{
			  sh """
					rm -rf /var/jenkins_home/workspace/joyblast-mobile_master/API/bin/Release/net6.0/*
					dotnet build API/API.csproj --configuration Release
					dotnet publish -c Release API/API.csproj --no-build
				"""
			}
		 }
		 stage('Build UI'){
		   steps{
			  sh """
					rm -rf /var/jenkins_home/workspace/joyblast-mobile_master/UI/bin/Release/net6.0/*
					dotnet build UI/UI.csproj --configuration Release
					dotnet publish -c Release UI/UI.csproj --no-build

				"""
			}
		 }

		 
		stage ('Deploy') { 
			steps{ 
					sh "ssh root@65.108.102.251 rm -rf /var/www/joyblast-mobile/"
					sh "rsync -azP /var/jenkins_home/workspace/joyblast-mobile_master/MobilAPI/bin/Release/net6.0/publish/ root@65.108.102.251:/var/www/joyblast-mobile/"
					sh "ssh root@65.108.102.251 service kestrel-joyblast-mobile restart"

					sh "ssh root@65.108.102.251 rm -rf /var/www/joyblast/joyblast-api/"
					sh "rsync -azP /var/jenkins_home/workspace/joyblast-mobile_master/API/bin/Release/net6.0/publish/ root@65.108.102.251:/var/www/joyblast/joyblast-api/"
					sh "ssh root@65.108.102.251 service kestrel-joyblast restart"

					sh "rsync -azP /var/jenkins_home/workspace/joyblast-mobile_master/UI/bin/Release/net6.0/publish/wwwroot/ root@65.108.102.251:/var/www/joyblast/joyblast-client/"

			}
		}
	}
	post { 
			success {
				office365ConnectorSend message:"${currentBuild.currentResult}: Job ${env.JOB_NAME} build ${env.BUILD_NUMBER}\n More info at: ${env.BUILD_URL}", status:"${currentBuild.currentResult}",webhookUrl:"https://kodfu.webhook.office.com/webhookb2/0112bfe0-31c1-4262-9f5a-82c20884d261@496969fd-056e-4f80-bb8a-6f11b8df3f96/IncomingWebhook/9a6fd2986deb49d3a0822b26a0e59f38/1eaa63f3-303a-4bad-984a-ba27d09c321e",color:"05b222"
			}
			unstable {

					office365ConnectorSend message:"${currentBuild.currentResult}: Job ${env.JOB_NAME} build ${env.BUILD_NUMBER}\n More info at: ${env.BUILD_URL}", status:"${currentBuild.currentResult}",webhookUrl:"https://kodfu.webhook.office.com/webhookb2/0112bfe0-31c1-4262-9f5a-82c20884d261@496969fd-056e-4f80-bb8a-6f11b8df3f96/IncomingWebhook/9a6fd2986deb49d3a0822b26a0e59f38/1eaa63f3-303a-4bad-984a-ba27d09c321e",color:"d00000"

			}
			failure {
					
					office365ConnectorSend message:"${currentBuild.currentResult}: Job ${env.JOB_NAME} build ${env.BUILD_NUMBER}\n More info at: ${env.BUILD_URL}", status:"${currentBuild.currentResult}",webhookUrl:"https://kodfu.webhook.office.com/webhookb2/0112bfe0-31c1-4262-9f5a-82c20884d261@496969fd-056e-4f80-bb8a-6f11b8df3f96/IncomingWebhook/9a6fd2986deb49d3a0822b26a0e59f38/1eaa63f3-303a-4bad-984a-ba27d09c321e",color:"d00000"
			}
		}
}
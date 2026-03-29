pipeline {
    agent any

    environment {
        AWS_REGION = 'us-east-1'
        ECR_REPOSITORY = 'contact-csharp-app'
        AWS_ACCOUNT_ID = credentials('aws-account-id')
        IMAGE_TAG = "${env.BUILD_NUMBER}"
    }

    stages {
        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/zen-tech-training/contact-csharp-app.git'
            }
        }

        stage('Restore & Build') {
            steps {
                sh 'dotnet restore'
                sh 'dotnet build -c Release --no-restore'
            }
        }

        stage('Docker Build') {
            steps {
                script {
                    env.REPOSITORY_URI = "${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com/${ECR_REPOSITORY}"
                }
                sh 'docker build -t $REPOSITORY_URI:$IMAGE_TAG -t $REPOSITORY_URI:latest .'
            }
        }

        stage('Push to ECR') {
            steps {
                withAWS(region: "${AWS_REGION}", credentials: 'aws-jenkins-creds') {
                    sh 'aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin $REPOSITORY_URI'
                    sh 'docker push $REPOSITORY_URI:$IMAGE_TAG'
                    sh 'docker push $REPOSITORY_URI:latest'
                }
            }
        }
    }

    post {
        always {
            cleanWs()
        }
    }
}

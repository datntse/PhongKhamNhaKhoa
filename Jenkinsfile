#!groovy

pipeline {
    agent {
        label 'Host'
    }
    parameters {
        booleanParam(
                name: 'Deploy',
                defaultValue: false,
                description: 'Deploy HTMS to docker'
        )
    }
    stages {
        stage('Checkout SCM') {
            steps {
                deleteDir();
                echo 'Check out Git'
                script {
                    env.COMMIT_ID = checkout(scm).GIT_COMMIT
                }
            }
        }
        stage('Build QuanLyPhongKhamNhaKhoa') {
            steps {
                sh """
                    docker build -t clinic-be:${env.COMMIT_ID} .
                """
            }
        }
        stage('Deploy') {
            when{
                expression {
                    return params.Deploy
                }
            }
            steps {
                echo 'Deploying HTMS';
                script {
                    sh  """
                            docker stop clinic-be || true
                            docker rm clinic-be || true
                            docker run --publish 7210:80 --detach --restart=always --name clinic-be clinic-be:${env.COMMIT_ID}
                        """
                }
            }
        }
    }
    post {
        success {
            build job: 'remove-docker-image', parameters: [string(name: 'IMAGE_NAME', value: 'clinic-be'), string(name: 'COMMIT_ID', value: '${env.COMMIT_ID}')]
            script {
                currentBuild.description = sh(
                        script: "docker image ls | grep clinic-be | awk \'{print \$3}\'",
                        returnStdout: true
                ).trim()
            }
        }
    }
}
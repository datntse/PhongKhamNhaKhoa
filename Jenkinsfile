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
        booleanParam(
                name: 'DEL_OLD_IMG',
                defaultValue: true,
                description: 'Only keep newest build'
        )
    }
    stages {
        stage('Build QuanLyPhongKhamNhaKhoa') {
            steps {
                sh """
                    docker build -t clinic-be:${env.BUILD_NUMBER} .
                """
                script {
                    currentBuild.description = "image_id -> " + $sh(
                            script: "docker image ls | grep clinic-be:${env.BUILD_NUMBER} | awk \'{print \$3}\'",
                            returnStdout: true
                    ).trim()
                }
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
        stage('Delete old build') {
            when {
                expression {
                    return params.DEL_OLD_IMG
                }
            }
            steps {
                build job: 'remove-docker-image', parameters: [string(name: 'IMAGE_NAME', value: 'clinic-be'), string(name: 'COMMIT_ID', value: '${env.BUILD_NUMBER}')]
            }
        }
    }
}
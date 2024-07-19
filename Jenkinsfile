#!groovy

pipeline {
    agent {
        label 'Host'
    }
    parameters {
        string(
                name: 'BUILD_NUM',
                defaultValue: '',
                description: 'Deploy target'
        )
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
            when{
                expression {
                    return !params.Deploy
                }
            }
            steps {
                sh """
                    docker build -t clinic-be:${env.BUILD_NUMBER} .
                """
                script {
                    currentBuild.description = "image_id -> clinic-be:" + sh(
                            script: "docker image ls | grep clinic-be | grep ${env.BUILD_NUMBER} | awk \'{print \$2}\'",
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
                script {
                    sh  """
                            if [[ -z "${params.BUILD_NUM}" ]]; then
                                echo "BUILD_NUM is required";
                                exit -1;
                            fi
                            docker stop clinic-be || true
                            docker rm clinic-be || true
                            docker run --publish 7210:80 --detach --restart=always --name clinic-be clinic-be:${params.BUILD_NUM}
                        """
                }
            }
        }
        stage('Delete old build') {
            when {
                expression {
                    return params.DEL_OLD_IMG && !params.Deploy
                }
            }
            steps {
                build job: 'remove-docker-image', parameters: [string(name: 'IMAGE_NAME', value: 'clinic-be'), string(name: 'BUILD_NUM', value: "${env.BUILD_NUMBER}")]
            }
        }
    }
}
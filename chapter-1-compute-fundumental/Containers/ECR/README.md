# [Elastic Container Registry](https://docs.aws.amazon.com/AmazonECR/latest/userguide/Registries.html)
* control and manage your docker images
* notes
    * can access by your account or IAM users
    * docker push/pull then your docker should be setup for auth
    * controlled by IAM and repository policies
* setup docker login (to do pull/push)
    * there are so many ways but easiest: 
    * `aws ecr get-login-password --region ap-southeast-2 | docker login --username AWS --password-stdin aws_account_id.dkr.ecr.ap-southeast-2.amazonaws.com`
* too see repo/images
    * `aws ecr describe-repositories`
    * `aws ecr list-images --repository-name`
* create repo:
    * `aws ecr create-repository --repository-name sample-repo --image-scanning-configuration scanOnPush=true --region ap-southeast-2`
* push:
    * throught tagging
    * **each image one repository**!!!!
    * after creating repository **with the same name as your image**
    * tag it with aws account
        * ` docker tag setting-publisher:latest  <account-id>.dkr.ecr.ap-southeast-2.amazonaws.com/<name-of-image(or repo):latest`
    * push it
        *  `docker push  <account-id>.dkr.ecr.ap-southeast-2.amazonaws.com/<image-name>:latest`
* pull
    * `docker pull aws_account_id.dkr.ecr.ap-southeast-2.amazonaws.com/<image-name>:latest`
* lifecycle policies
    * thorough rules, can apply some actions on a specific repo with tags (prefix, ...)
        * example cleanning up old images
    * todo: counld not find.
* Monitorying:
    * to see how ECR is using resources (ex: storage)
    * can be done in cloudwatch
    * todo: more digging
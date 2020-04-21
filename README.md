# aws-architect-cert
Everything you need to know about aws to pass architect cert exam
* all must ~ 35 services
## Topics inspired by:
	https://cloudacademy.com/blog/choosing-the-right-aws-certification/
	https://cloudacademy.com/learning-paths/cloud-academy-solutions-architect-associate-certification-v152-184/
## Topics
* Compute (https://aws.amazon.com/products/compute/)
	+ must: 
		- Virtual Machines(EC2) 
		- Containers (ECR, EKS) 
		- ServerLess (Lamda)
		- AWS Batch
	+ optional: Forgate, Amazon Lightsail, Amazon EC2 Spot Instances,  AWS Compute Optimizer, Beanstalk, AWS Serverless Application Repository
* Storage
	+ Database
			- must: Aurora, Relational Database Service, DynamoDB, ElastiCache
			- optional: Neptune, (QLDB), TimeStream, 
	+ File service  (https://aws.amazon.com/products/storage/)
		- must: EBS, EFS, FSx, S3 (Glacier, Glacier deep), storage gateway,
		- optional: aws backup
	
* Network (https://aws.amazon.com/products/networking/)
	+ must: architecure (VPC, Elastic Load Balancing), connectivity (Route 53, PrivateLink, Direct Connect, VPN), delivery (cloudFront, API Gateway)
	+ optional:  AWS Global Accelerator, AWS Transit Gateway, App Mesh, Cloud Map
* Resilience: ??
	+ Integration
		- must:  kensis, sqs, sns, swf, MQ, AWS Step Functions
		- optional:
* Security: 
	+ must: IAM, Security Hub, Certificate Manager, Key Management Service, Secrets Manager
	+ optional: GuardDuty, Inspector, CloudHSM, Directory Service, Firewall Manager, Shield
* Cost optimization: ??
* Deployment:
	+ devops
		- must: Code deploy
		- optional: CodeCommit, CodeBuild, CodeDeploy, CodePipeline, CodeStar, X-Ray
	+ Management:
		- cloudformation, AWS Service Catalog, AWS Config
		- optional: AWS Systems Manager
	+ Analytics
		- no need
* Others (optional)
	+ Blockchain
	+ [business applications services](https://docs.aws.amazon.com/whitepapers/latest/aws-overview/business-applications.html) 
	+ [customer engagement](https://docs.aws.amazon.com/whitepapers/latest/aws-overview/customer-engagement.html)

## Boilerplate for every topic
[titles] (what to do) (Two directory for each service: 1- document (has a linq to 2) 2- code examples  
* Intro
* Why using it
* comparison to other solutions (ex: if sqs what are the others: rabitmq, db handcraft, ... )
* limitation in real world scenarios
* Usecases (design patterns) using it
* Linq to the usecase code in the same or different repository of yours
* Linq to the fundumental software engineers usecase (Martin Fowler) and explain a bit
* Tools in github (ex: justSaying for sns, serverless for lambda)

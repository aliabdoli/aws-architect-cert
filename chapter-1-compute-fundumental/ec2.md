# EC2 - Amazon Elastic Compute Cloud
https://docs.aws.amazon.com/AWSEC2/latest/WindowsGuide/concepts.html

# Big picture overview:

## Features
*	virtual computing environment = instance
*	Preconfigurable templates for instances = Amazon Machine Images (AMI)
*	Different range of Cpu/network/Storage/Memory = instance types
*	Secure login = Key pairs (public key: amazon, private key: you)
*	storage of temp data (gone when instance is stopped/terminated) = instance store volume
*	persistant = Elastic Block Store (EBS volumes)
*	Firewall to controll Ip ranges/protocol/ports = Security groups
*	Static Ipv4 address for dynamic compute nodes = Elastic Ip Address
*	Metadata = Tags on instances/...
*	virtual networks to isolate your instance from rest of your aws = Virtual Private Clouds (VPC)

## KeyPair for instance
*	?? what are different ways to secure ec2?
*	public/private key to login to instances
*	different for different regions
*	ec2 -> networks and security -> key pairs
*	use 
	+	kp name when launch
	+	pem file everytime login
*	pem: 
	+	private key file
	+	generates once (need to be saved somewhere)

## VPC (Virtual private cloud)
*	?? what are vpc exactly?
*	It is NOT in Ec2 menu 
	+	completely different service

## Security Group
*	a firewall to controll inbound/outbound protocols/ips/...
*	Specific to 
	+	Region
	+	VPC (why??)
*	anywhere: 0.0.0.0/0
*	My Ip: 
	+	uses your ips
	+	if ISP or company with range, you need 
*	Dont open udp unless short time or testing (Why??)

##	windows instance [with EBS-backed]
*	charges event if idle (untill terminated)
*	Security (what more???)
	+ keypair/security group/vpc
*	Amazon Machine Image (AMI):
	+	what are t2.large/medium/... types??
*	Instance Type
	+	Hardware config (different from AMI)
* There are heck of config in tabs when creating ec2, what are those and usecases???

## connecting to win instance
*	by default Administrator (depends on the language)
*	if using domain, it comes from AWS Directory Service (what is that???)
	<domain>/admin
*	depends on win license, different number of simultanous udp
*

# Best practices (windows)
*	update windows drivers(trusted advisor, sns, aws ssm)(what???)
*	launch new instances with new ami s(new drivers, os patches, ...)
	+	monthly new amis
* 	update launch agents (ec2Config, ec2launch): 
	+ what are those 2 scripts??
*	Security and Network
	+	???
* 	storage:
	+ separated data EBS???
	+ temp store for temp data
		-	if use db in temp store??
*	resource management
	+	instance metadata and custom tags -> track and idnetify resources
	+ check the limitation of EC2????
*	backup & recovery
	+	EBS by ebs snapshots???
	+	instance by creating ami
	+	deploy critical points to different availibility zones??
		- replicate your data
	+	application should handle dynamic ip when instance restarts (howw???)
	+	Monitoring (???)
	+	Failovers
		- elastic network interface(???)
		- Autoscaling
	+	regularly test failing / recovery of instances and EBS (how??)


# 	Amazon machine Image
*	not covered

#	Instances
*	before launching to production
	+	instance types
	+	purchase options
	+ 	managing all ec2 s in hybrid environemtn (aws system manager???)

## Instance types
* the hardware type (cpu, memory, storage, network)

*	ec2 on hosts: 
	+	if all requests as much, resources will be share equally (of the same type)
	+	if not underuse, using instances get more 
	+	based on the type, instances get more part of shared resource
		- IO type, more io resources that the other instances in the same host
*	intance types: General, Memory, Compute, Storage, Accelarated computing
*	there are old and new generation of types (check documentation)
*	how to find the correct type:
	+ 	run benchmark app and check the performance
	+	running benchmark is cheap, bcos you get charged for instances hourly
	+	how to create a benchmark app for perofmance???
*	Nitro based instances??
*	Networks
	+	put instances in "placement group"??? for high performance computing??
	+	Enable enhanced networking?? "Enhance networking on windows"
	+ MTU ???
*	storage:	
	+ how to find right storage??
* limits ??
	+	https://aws.amazon.com/ec2/faqs/#How_many_instances_can_I_run_in_Amazon_EC2
	+	https://docs.aws.amazon.com/AWSEC2/latest/WindowsGuide/ec2-resource-limits.html

### General purpose instances (burstable performance)
*	burstable performance
	+	on workload, more just CPU
	+	charged by cpu credit
* 	burstable requirements
	+	just on on-demand instances, reserved and spot, and not for scheduled and dedicated (????)
	+	should pass minimum memory requirements
*	Best practices
	+	recomanded ami
	+	turn on instance recovery (why important here??)
*	It needs more research????
### accelarated computing instances
*	for scientific purposes 
*	needs more research????
### changing the instance type	
*	limitations???

## instance purchase options??
## instance lifecycle
*	for billing for different states check out documentations
	+	billed for some transition states
	+	billed even in idle
* Stop/start (just for EBS-backed)
	+	you can stop the instance, fix and rerun it
	+	rerun will be on new host
	+	Ip does not change
	+	charge full hour for start
*	instance Hibernate (just for EBS-backed):??
*	reboot
	+	better use aws reboot than os
		- same private ip, dns, host, ...
* retirement
	+	aws to retire and instance because of irripairable failure in host hardware
	+ 	it becomes stopped or teminated
*	check documents difference (Reboot, Stop, Hibernate, and Terminate)

### Launch
*	7 ways: (differences???)
	+ 	wizard, 
	+	launch template, 
	+	exisiting instance, 
	+	aws cli, 
	+	windows ps, 
	+	ec2 fleet, 
	+	market place
###	Recover
*	if it is "system status check" failure
	+	which is about the hardware problems
	+	different from instance status check
*	causes
	+	network connectivity loss, system power loss, ...
*	showing system (os) problems that needs aws involement
*	two things will happen
	+ aws automatically recover it
		- same ip,... 
	+	you subscribe to cloud watch and get notified by sns aws raises
		- you might want to recover before aws does
*	after recover you lose store data
*	aws might not always do recovery for few reasons (check documentation)
##	Configure Instance ???
##	Identifying instances ??
*	app might need to know they are runnig on ec2
*	what if windows??? and not linux
#	Amazon Elastic Inference
#	Monitoring
*	It s for: Availability, reliablity and performance
*	FIRST: Questions before monitoring design:	
	+	goals
	+	resources
	+	how often
	+	tools
	+	who performs monitoring
	+	who should be notified when wrong
*	Second: base line and compare at various time and conditions
*	aws ec2 metrics (already there)
	+	cpuutilization, networkin, networkout, diskreadops, diskwriteops, diskreadbytes,
		diskwritebytes
##	Automated and manual monitoring
###	Automated monitoring tools
*	they watch and REPORT Back to you
*	list of automated
	+	System status check
	+	Instance status check
		-	needs your involement
		-	ex: rebooting, ...
		-	ex:	failed system status, misconfigured network, ...
	+	cloudwatch alarm
		-	watch standard metrics (cpu, ...)
		-	it can do basic actions (recover, stop, terminate, reboot)
		-	it sends event to 
			+	it can be Auto scaling policy
			+	it can be sns event
		-	state should sustain to raise event
	+	cloudwatch event
		-	if aws resources change their status, they raise this kinda event
		-	you can atomate the action by listening to it
		-	ec2 change state causes this event 
	+	cloudwatch logs
		-	see logs from ec2 instances, aws cloudTrail???, ...
	+	Amazon EC2 Monitoring Scripts???
	+	AWS Management Pack for Microsoft System Center Operations Manager??
### Manual monitoring tools
*	they are on the consol
*	ec2 dashboard
	+	service health, scheduled events, instas state, sta checks
	+	alarm status, instance metric details, volume metric details
*	cloudwatch dashboard
	+	alarms and status, graph of them, health status
	+	also, all aws resource metrics, monitor data, create alarms, see at glance alarms
##	Best pactices
*	monitor small problems before they become big
*	have monitor plan for all resources in aws and ask questions (in Monitoring)
*	monitoring should be fully automated
*	log file of ec2 should be checked
## Monitoring the status of instance
*	status check: result of automated checks by aws
*	status of an event: see the stuff scheduled for your instances as events???
###	status check
*	automated (no way to disable)
*	fixed metrics
*	set cloud watch alarm to monitor them
*	aws does it every minute
*	two types
	+	system check (aws involement)
	+	instance status check
		-	it sends address resolution protocol to network interface???
		-	you should address yourself (rebooting or changing the confi)
*	two ways to check
	+	console
	+	command line
		-	aws ec2 describe-instance-status
*	two statuses: Healthy, impaired
*	Creating and Editing Status Check Alarms(command/alarm)???
## monitoring instance using cloudwatch
*	cloudwatch gets data almose realtime and make them readable
*	data stays there for 15 monthly
*	ec2 sends data in every 5 minutes
	+	if you want 1 minute, active detail monitoring on instance
*	amazon console has its own graphs on those cloudwatch matrics
### list of all metrics???
https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/viewing_metrics_with_cloudwatch.html
## check Get statistics, grap metrics from official documentation
##	Create a CloudWatch Alarm for an Instance??
##	Create Alarms That Stop, Terminate, Reboot, or Recover an Instance??
##	Automating ec2 with cloudwatch
*	when an event matches the rule, some action can be done
	+	invoking aws lambda
	+	ec2 run command
	+	relaying event to Kinesis Data Stream
	+	Activating an step function in state machine
	+	notify sns or sqs
*	write some code????
##	Logging Amazon EC2 and Amazon EBS API Calls with AWS CloudTrail
*	cloudtrail: a service to record the actions by user, roles, ... (????)
*	Ec2 and EBS are connected to cloudtrail
*	you can enable cloudtrail to delivery continuesly in s3
*	you can create different trails for frequent actions on ec2????

# Network and security **[Must Read]**
*	The whole list of Features for network and security for ec2
	+	Key/pairs
	+	Security groups
	+	Access to EC2
	+	Ip addressing
	+	BYO Ip 
	+	Elastic Ip Address
	+	Elastic Network interface
	+	Enhanced Networking
	+	Elastic Fabric Adapter
	+	placement groups
	+	max transmission unit (MTU) for ec2
	+	virtual private cloud
	+	ec2-classic
## Key/pairs
*	if you want to login to the windows installed on ec2:
	+	Instead of password, using private/public key encryption 
*	you can keep creating the different key/pairs
*	public key is in instance (~/.ssh/authorized_keys)
*	private key never saved in aws, you need to save it	
	+ if lose it, no way to recore private key
*	it s not a good idea to use default user
	+	if multiple users to connect to instance, create multiple keys
*	you can use third party (public key) in ec2 (??)

##	security groups
*	virtual firewall to controll traffic to instances
*	one ec2 can have multiple sg
*	on launching ec2, sg that created for that vpc 
	+	Vpc owns sg s. first ec2 -> vpc then pick sg
*	sg is assigned to network interface (check section here)
*	change instance sg, change primary network interface (eth0)?
*	still you can have your own firewall if sg is not enough
### Sg rules:
*	by default, sg allow all onbounding traffic
*	always permissive (cant create rule to deny access)
*	sg are stateful ???
*	the change of sg can have different impacts
*	multiple rules on the same sg, aggrigates all
		+	having 100 rules for one instance can be problamatic ???
* rules have:
	+	protocol: which allowed
	+	port range
	+	icmp type and code ???
	+	source and destination (inboud/outbound)
		-	all the cirdr, blocking, ... ???
		- can be other sg???? what are all the valid values for these two??
### Connection tracking
*	????
###	Controlling access
*	with IAM, control access of other users, services, applications  to use ec2
*	some stupid topics: Network Access to Your Instance, Amazon EC2 Permission Attributes ??
*	IAM policies??
*	IAM Roles??
*	Network access??
###	Elastic Ip Address
*	it is static Ipv4
*	dynamic cloud computing
*	if an instance fails, you can remap it to other instance (mask the failure)
*	Notes:
	+	need t?r account, then associate to instance/network interface
	+	?????
##	Network interface: 
*	an introduction of how ips work https://www.youtube.com/watch?v=PYTG7bvpvRI
	+	netstat -nq
*	it s a logical network card
*	Features
	+	ONE primary private Ip address from Vpc ip range (one ip that vpc understands)
		-	thats why it s always attached to a vpc
	+	One or more secandary ip from vpc ip range
		-	secondary is when you wanna connect to multiple Lans
		-	So, with primary
##	Enhanced networking ??
##	Elastic Facbric Adapter ??
##	placement group??
##	MTU??
##	Vpc??
##	EC2-Classic??
#	Storage
##	EBS ??
##	Instance Store??
##	FileStore ??
##	instance volume limits ??
##	Block Device Mapping ??
##	Resource and Tags
## Aws Management pack
##	Ec2Rescue for windows server
##	Troubleshooting





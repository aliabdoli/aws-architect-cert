# Elastic Kubernetes Service
## pre-requisite 
* Kubernetes pre-requisite knowledge
    * etcd: 
        * it is a highly available key-value store
        * all sensitive data about clusters in it
        * if using it, making backup plan for it
    * Api server
        * the touch point to the clusters
        * Restful
        * It s responsible to store all objects
            * not inside it but communicating with backend
            * itself is stateless
    * Control plane
        * manages worker nodes and pods in the cluster
    * Pod:
        * smallest deployable object
    * (worker) nodes
        * worker **machines** that run containers
    * storageclass
        * todo: what does that mean in kubernetes?!!
    * deployment, replication controller, or replica set
        * todo: what are those
    * autoscaling cluster (todo)
    * Metrics Server (todo)
    * pod autoscaling (todo)
    * Vertical Pod Autoscaler(todo)
    * (network/classic) LoadBalancer service (todo)

* no need to maintain your kubernetes. (it is managed kubernetes)
* Integrated Services
    * ECR
    * Elastic Load Balancer
    * IAM
    * VPC (for isolation)
* Architeture
    * Every cluster one kubernetes instance
    * two api server nodes, and three etcd nodes
    * it has zone and availability zone
    * vpc is used to make sure
        * other control plane from other clusters or other accounts cannot communicate
    * IAM
        * cluster communicate with other services through IAM
        * ex: if it needs to create ELB, it needs access
* Getting Started
    * step 0:
        * create IAM, security groups and vpc
        * Install **eksctl and aws authenticator**
        * Role
            * one role with EKS policy
        * vpc
            * with two private and one public subnets
            * use cloud formation in aws s3 which is ready for above
                * https://amazon-eks.s3.us-west-2.amazonaws.com/cloudformation/2020-03-23/amazon-eks-vpc-private-subnets.yaml
                * todo: open above with cloudFormation designer and dig into it
            * what to capture
                * security groups, vpcIds and subnet ids
        * cluster 
            * use SG, vpc and subnet from Cloudformation in previous step
            * todo: why do we need all above network settings. who uses it?
            * **if using console, the user you log in and create it has access to cluster, ex: kubectl commands**
            * creating cluster through aws cmd (**recomended**)
                * https://docs.aws.amazon.com/eks/latest/userguide/getting-started-eksctl.html
    * step 1
        * creating kubeconfig through console and assign vpc, ... from step 0
        * aws eks --region region-code update-kubeconfig --name cluster_name
        * change your remote (ex: vs code)
        * kubectl get svc
    * step 2 - configure node
        * kubectl daemon needs permission to call to aws apis 
            * so nodes need roles
        * create role:  AmazonEKSWorkerNodePolicy,  AmazonEC2ContainerRegistryReadOnly,  AmazonEKS_CNI_Policy
            * **they are all under EC2 policies**
        * create node group
            * assign role 
            * subnets: 
                * todo: dig into it
            * remote access
                * highly recomanded to be **enabled**
                * you can connect to nodes for debug, ...
                * you need to use ssh
                * use linux ones in your key/pair values (menu) in aws 
## Clusters
* intro
    * two types of components
        * control plane and worker nodes
        * control plane
            * all nodes for kubernetes software
                * like etcd, kube api, ...
                * they are single tentented
            * volumes are encrypted (todo: how to read them?)
            * can connect to worker nodes
                * through elastic network interface in vpc for subnets
            * connect to load balancer to comunicate with
        * worker nodes
            * communicate to control plane via api server endpoint and certificate that clusters create
* Creating cluster
    * **the IAM or Role that creates the cluster goes to RBAC auth kube table and only that can communicate with clusters through kubectl**
    * Cluster endpoint access
    * by default
        * cluster can be accessed by public
        * that s why kubectl can communicate with it from your local
        * it is secured by both IAM and kube role based access control
        * worker nodes and cluster can communicate and even go out of vpc (not to public but inside aws network)
    * enabling private access
        * communication between cluster and worker nodes cannot leave vpc if enable
        * todo: how that helps and what usecase?
* logging
    * logs go to cloudwatch
    * log type can be specified
    * log types:
        * api, audit, authenticator, scheduler
## Aws Fargate
* eks can communicate with Fargate
* todo: what is Fargate and why communicating?!!
## Storage
* to define what aws storage (EBS) for storageclass of cluster 
* todo: what is storageclass in kubernetes (usecases?)
## Auto Scaling
* Three ways of autoscaling (refer to kube)
    * cluster autoscaling
        * when pods in one node have resource starvation and in other node are underutilized
    * horizental pod autoscaling
        * add number of pods in a node
    * vertical pod autoscaling
        * more/less cpu/memory based on your application need
## load balancing
* aws Classic load balancer = kube servicetype of loadbalancer
*  todo: ??
* aws application load balancer
    * todo: ??
## Networking (todo: ??)
* you need vpc to limit access to any aws resource 
* for EKS, your vpc need to meet some requirements 
* Vpc requirment for EKS
    * public and private subnets
        * two public and two private subnets
        * each one public and one private is for one availability zone
        * public IP address
            * any resource in public subnet can have access to it
            * if in private (like working nodes), they need to use **NAT** for each availability zone to communicate with outer world
            * SGs to reject any inbound traffic from outside
                * and allow outbound traffic
## app meshing to eks (todo:)
    * https://docs.aws.amazon.com/eks/latest/userguide/appmesh-getting-started.html
## how it works in the company (todo)




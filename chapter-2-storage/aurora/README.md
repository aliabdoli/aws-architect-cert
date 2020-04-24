# [Aurora](https://docs.aws.amazon.com/AmazonRDS/latest/AuroraUserGuide/CHAP_AuroraOverview.html)
* managed mysql and postgres
* it does replication and clustering and distributed storage for you
* it uses all features of aws rds
* using instance endpoint
    * you normally connect to the cluster and it knows where to route
    * you connect to a specific instance if
        * you debug that specific instance
        * for some loads you want that specific instance to be used
* higly availability
    * it has primary (for read/write) and replica for read
    * it also manages failoveron primary
        * cluster makes a new primary and turn one of the read replica to primary
        * or you can manually use a failover instance (directly connecting to the instance) in case of failover
            * basically do not wait for cluster
## What is Aurora
* Aurora db clusters
    * two types of clusters topology 
        * (common) one primary, multiple read replica
        * multiple primary (read replica does not make sense)
    * one primary topology
    ![OnePrimaryCluster](OnePrimaryCluster.jpg)
    * it uses distributed storage and that s why one primary can write to too many sections
    * **how many replicas**
        * replica storage (2 each availability zone * 3 = 6)
        * replica reader instance = upto 15
    * **todo: it does not shard!!! even if doing it, on the same instance which is primary!!**
* connection management
    * clusters manages routing 
        * some group of instances for ddl,read,write
        * routing is done by ports and internally
            * so you do not need to change client for it
        * clusters use endpoints for different types
    * Types of endpoint
        * cluster endpoint
            * Cluster endpoint
                * to primary endpoint
                * to do write/DDL
                * in case of failover, still serves your request
                    * **with least interruption**
                    * **it slows down the process but still serve**
                * `mydbcluster.cluster-123456789012`
            * read endpoint
                * **it is a different endpoint and need to be managed in your client code**
                * it has load balancing for reads
                * `mydbcluster.cluster-ro-123456789012`
            * **custom endpoint**
                * a group of instances that used for specific use case
                * they are normally write instances
                * you create it, aws generates a url and you give it to the users
                * ex: it is a one-time job that needs to populate a table for reporting later
                * **they normally have specific config of instance**
                    * like high memory or cpu
                * when you hit the custom url
                    * aws load balance between the group of db instances you defined
                * `myendpoint.cluster-custom`
            * Instance endpoint
                * directly to one instance
                * normally for debuging
                * really rare
        * Using the Cluster Endpoint
            * primary for data modification and ddl
                * creating indexes, ...
        * using read endpoint
            * load balancing the reads
            * **todo: all execution plans do not exist in one place, for the same query if it goes to different reads, that db instance need to recreate it!!**
        * using costum endpoints
            * **Do not use CNAMEs in route53**
            * todo: why?
            * **they do not have the name of your cluster in it**
                * so if your cluster name changes, still valid
                * `endpointName.cluster-custom-customerDnsIdentifier.dnsSuffix`
            * todo: read more
* **db instance classes**
    * configure your computation and memory capacity of db instances
    * **two main types**
        * Memory optimized
        * burstable performance
    * todo: might dig more, about the hardware, ...
* **Storage reliablity**
    * **distributed ssd**
    * **only one storage for all availability zones**
    * **data gets copied to other availability zones automatically (when primary node updates, it *synchronously* replicate it**
    * **it is just one, so you can spinup new dbinstances quickly without coping the data**
        * it is really good for failovers and also load balancing!!
    * **it grows automatically**
    * **reliablity**
        * autorepair, cache warming, crash recovery
        * **todo: what are those?!!**
* availability
    * by replica
        * when primary node modifies data, it syncronously write to different parts of its distributed storage
    * you can use Multi-AZ
        * **todo: what is it?**
* replication options
    * todo:??

## getting started
* **todo: codeing**
## configure 
* **todo: coding**
* **todo: how it auto scale manually?**
* **Serverless clusters**
    * **todo: ??**
## **todo: there are some other topics to dig in**
## Best practices
* basic operational guidelines
    * monitor your cluster on cloudwatch
        * cpu, memory, ... might go up and you need to scale your deployment
    * if DNS is cached on the client,
        * needs to be invalidated
        * the cluster url might change over time
    * test failover
        * to get idea how long it takes for aws to bring a new instance on
## **proof of concept**
* it is to see if aurora is a good fit for your app
* overview
    * to make sure you know how to scale based on your app
    * to use best practices
* Steps
    1. idenfity objectives
        *  through put or latency of queries requirement
        * how much downtime is allowed? (planned/unplanned)
        * metrics
        * dateset size or load level? (todo: what is it)
    2. understand your workload characteristics
        * it is good for OLTP and a bit of reporting (you do not need dateware house)
        * characteristics
            * High concurrency: hundered/thousands simultanous clients
            * large volume of low latency **queries**
            * **short and realtime transactions**
            * **highly selective query patterns by indexes**
                * otherwise elastic search (if queries are not indexable)
                * or dynamo (if queries do not vary that much and limitted)
        * it is good for **high velacity data**
        * **what is high velacity**
            * being inserted/update very frequently
            * **accessing multiple columns but small number of rows**
        * aurora can process (if using large db instance class)
            * 600,000 reads per sec
            * 200,000 modification per sec
        * you can also do reporting on the same oltp
            * cos it has 15 read replica
        * **todo**
            * sound it sucks when update on many rows
            * if you reporting fetch so many rows
            * all problems with relational db is still there
    * **todo: there are some other steps**

     


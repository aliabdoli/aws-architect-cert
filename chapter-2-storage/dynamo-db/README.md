https://itnext.io/creating-a-blueprint-for-microservices-and-event-sourcing-on-aws-291d4d5a5817

# DynamoDb
* no sql service
    * todo: what kind of no sql?
## **IMPORTANT: TODO https://www.youtube.com/watch?v=jzeKPKpucS0**
* big list of customers, update all with ages > x
* big list of customers, a group of them becomes popular for write (not read)
* implement versioning
* what are the other common problems
## **IMPORTANT: [Under the hood architecure](https://www.youtube.com/watch?v=yvBR71D0nAQ)**
* it is an api (http)
* at every request, it auth/author by IAM
* **storage node is in different server than router**
* it has a router to do auth
    ![router](Router.jpg) 
* how data is stored
    * **(wrong: it is true just for one partiion) three separate servers in different availability zone**
    * every avail zone has a replica
* how modification happens
    * every write to any availability zone, does elaction
        * if one other node acknowledge the write, it returns successful
        * does not wait for the other (third one to acknowledge)
        * third will acknowledge write quickly
    ![replica](Replica.jpg)
* how to make sure that write is strongly consistent
    * there is **only one lead storage node**
    * **lead node is always up-to-date**
    * **lead node is not dynamically chosen**
    * when lead modifies, it does the change and propogate to others
* heartbeat
    * if leader is lost (no heartbeat in few sec)
        * one of the replicas, take the lead (if leader agrees)
* how data are stored 
    * **real world and righ: thousands of storage nodes and routers in every avail zone**
    * **every storage node has the sharded data with 3 (data) replica in different zones**
        * **so, one piece of data, has 3 replication in 3 different zones**
    * **each sharded (partiioned) data has it is own storage node (which is different server)**
    * **sync happens for those three peers for that specific shard or partition**
* sharding logic (**partition Metadata System**)
    * sharding tells us which storage node the request should be routed to
    * it is determined by partition key
        * storage server = HashFunction(partition key)
    * multiple items with different partion key can be in the same shard
    ![ShardLogic](ShardLogic.jpg)
* eventually consitent read
    * dyamo allows you to **read** not only from lead node but from others in other availability zones (for that shard)
    * those nodes (cos they are not read), might not have latest update for that pointing time
    * **cos write always happen in lead node, we do not have eventually consistency write**
    * reads might not be up to date 1 of third (cos election in lead making sure at least of the read replica is up-to-date)
        * it also depends on the latency of sync between lead and others
    * when gets consitent? 
        * depends on like network, how much your node is behind
* Storage Node
    * it uses B-tree to find data
    * it captures all events that caused the changes in data
        * **replication log**
        * event sourcing
    ![StorageNode](StorageNode.jpg)
* auto admin
    * dynamo admin
        * rebooting servers, resharding, ...
    * one role is reparing storage node:
        * monitoring and making sure that partitions have the right data
        * if any failure, it repairs it in different ways
    * it is responsible to if storage has failed
        * create a new storage node and from replication log of other replica nodes, recreate it
        * then update Partition Metadata System
* **Global secondary index**
    * the have their own partitioning (sharding) separate from base table
        * but you can query back the whole data
    * how it gets sync
        * through log propagator
        ![GlobalIndex](GlobalIndex.jpg)
    * so, **updating global indexes are eventually consitent (to base table change and to replicas)**
* **provisioning tables**
    * the unit to measure: WCU (write), RCU (read)
        * **request per sec/per amount of data**
    * it is defined **per table**
    * it devides **equeally** the whole table CUs to different shards
    * to manage, "Token Base Alghorithm"
    * if exceed the provision: **getting throtled**
    * **if *one shard* exceeds the capacity initially allocated: hot shard**
        * some of the requests at the beginning get throttled
        * dynamo adapt to new load and eventually (if still you have capacity left for your table) uses capacity and no throttle
        * basically it changes the provision for each shard based on their use over time
* **auto scaling (on your *table not your shard*)**
    * when you have spikes in your workload
        * you might keep getting throttled
    * you need to ramp up/down CUs 
    * how it works
        * it uses cloud watch alarms and auto-scaling in aws services
        ![AutoScaling](AutoScaling.jpg)
        * two alarms for each table
            * when you are going above current provision
            * when you are going under current provision
* **important note on scaling**
    * data 
        * data gets scattered by hash function
        * the input of that hash is your partition key
        * if your partition key is not distributed enough (cannot get hashed uniformly)
            * so much data in one storage node
            * **it is likly to get hot shard**
            * unbalanced usage
        * todo: however, hashfunction might change when resharding!!!! 
    * unbalanced in time (bursting)
        * your table different load diffent time
            * use auto scaling 
            * **it does not solve hot shard problem**
* restoring and backup (**it is a solution for rebuilding models in eventsourcing**)
    * it happens through **Replication Log**
    * it stores them in **S3 buckets**
    * it also **takes snapshot (time to time) to make sure not to replay the whole thing all over again cos time consuming**
    * each shard does restore independently!!!!
    * restore to a pointing time
![Restoring](Restoring.jpg)
    * if resharding happend
        * it replay for event new shard!!
    * restoring optimization
        * you need to know when take snapshots, when to remove unused in s3, ...
        * example delete logs when have snapshot
            * then you lose travelling to times before snapshots!!!
            * if enable PITR, it keeps all the old replication logs
* dynamo db streams
    * **no duplicate in them**
    * **they are in order**
        * they control it through the key generation of change record
            * it just **guarantees exec order** in one shard (through those keys)
    * **dynamo uses Kinesis for streaming**
        * Kenesis has stream checkpoint, shard, records concept (similar)
    * shards seprately write to the stream (Kinesis)
## What is dynamodb

### How it works
* core components
    * Tables, items, attributes
        * Tables are collection of related data
        * items: 
            * piece of info can be retrieved *uniquely* 
            * *no limitation on number of items stored in dynamodb*
            * **primary key can be composite key**
            * schemaless
        * attributes: kinda like columns
        * **primary key**
            * uniquely identify an item
            * **dynamodb uses ssd for storage, and that has partitions**
                * **so when we say partition, it s not an instance, it s a partition key (dynamoDb) that will map to partition key (ssd concept) on the disk by a hash function**
                * to find the actual phisycal address 
                    * phisycal address = HashFunction(partitionKey)
            * they need to be scalar (binary, string, number)
            * two types
                * PartitionKey
                    * using the partition key as primaryKey
                    * in every partition, one item
                    * **dynamo uses partition key as the input of hash function to find the actual physical partition (ssd disk) of data**
                    * **that hash function distributed dynamo partitions evenly on disk partitions**
                * partition key (hash) + sort key (range)
                    * **items with the same partition key (phisycally) stored together and sorted by sortkey**
                    * items can have the same partitionkey but if so, different sort key
        * **Secondary index (alternate key)**
            * more flexibility for querying
            * two types
                * global 
                    * partition key in your index is different from primary key partition key
                    * max 5
                * local
                    * partion key in your index is the same but sort key is different
                    * max 20
            * every index needs a base table
            * index maitanence
                * automatically by changes in the base db
            * **projection to index**
                * you need to specify which fields in base table will be duplicated to the index
                * **index does not have pointers or any thing to the base table**
                * **it is equivalent to index with include columns in sql**
        * **DynamoDb streaming**
            * streaming is like logs of changes in your db that has operational value 
            * it s used to **capture data modification **
            * it is represented by stream record
                * it capture changes on **items** being modified
            * when active on a table, it has three types
                * added
                * updated (before and after of any attributes changed)
                * deleted
            * stream record
                * has table name, timespan, ...
                * durability 24 hrs
            * **stream record can be directed to lambda, ... to take action based on CRUD ops in dynamo**
    * Naming rules and datatypes
        * three types: binary, string, number
        * todo: each of them has limitations, might worth having a look
    * **Read consistency**
        * **writes are strongly consistent**
            * todo: why? do we have one master?
        * failover
            * replica of data in different availability zones
            * they get sync automaically
            * **it fails over to other availability zone**
                * todo: not sure?!!
        * write is durable when gets acknowledged
            * it takes maybe 1 sec to sync in other replicas (availability zones)
        * two types of read (read-after-write or just read)
            * eventually consistent
                * data in write response might be stale 
            * strongly consistent
                * data in write response reflects the latest data after **all prior writes**
                    * todo: what does it wait for in the background?!!!
                    * **todo: my assumtion compare to MangoDb, needs to be verified**
                        * there is no one primary instance (there are so many)
                        * replica (in different avail zones) is not synced by masters
                * notes
                    * more likely to get 500 if network is down between avail zones
                    * more latency
                    * cannot be used in global index
                    * consumes more capacity in your read/write provisioning
    * Read/write capacity mode:
        * **unlike others you can scale up/down a table (not whole db)**
        * **indexes consume capacity too**
        * two types: on-demand, provisioned
        * on-demand
            * when work load of whole table (not shard) changes overtime
            * When
                * new table (but you do not expect crazy burst)
                * unpredicted trafic
                * pay as you go
            * every peak double the capacity and if it goes down, it goes down

            
### from sql to amazondb
* main differences
    * Connection: in sql, client need to maintain the connection while dynamodb is http (no maintaining and managing)
    * Authentication: in sql, can dbms do it or offload it to os or nt, 
    * author: in sql itself, in dynamo in IAM
    * execution: sql keeps in exec plan for requests, dynamodb (in terms of running) is stateless
    * payment: normally dynamodb is pay as you go (concept of always running vs pay as you go services in general)

## programming with dynamodb
* two level of apis (lowlevel, high level)
* todo: ?

## working with dynamodb
* **todo: so important, needs coding**

## on demand back up and restore (toro:?)

## dynamodb transactions
* to performa transaction on **multiple items in the same table or multiple tables**
* group multiple put, update, delete, *conditionCheck* 
    * submit as TransactWriteItem
    * it updates all or nothing
* **you can have transaction on read too**
* todo: it puts lock!!!!
* how it works
    * **TransactionWriteItemApi**
        * limitation: 25 modification action and total size of change, 4 MB
        * **different from BatchWriteItem**
            * in batch, some of the items might fail and other get modified
        * **you canot target the same item with different actions in the same transaction**
            * cannot have conditionalCheck and update on the same item
        * **allowed actions**
            * put
                * update if exist or insert, conditional check is allowed
            * update
                * on a specific item
                    * can remove/update/insert attributes on that item
            * delete
                * to delete single item
            * conditionalCheck
                * if item exist
                * if item s attributes fulfill a condition
        * when transaction commited
            * it gets propagate to stream, *global secondary index*, backups
            * if you restore before propagation, you might lose after propagated
        * **Idempotent**
            * you might bee executing your requests multiple time
                * cos of timeout or any connectivity issues
            * to avoid it, 
                * pass on a token to your transaction
                * all subsequent requests with the same token get successful without running on the server
                    * you can check by consumed capacity which in this case zero
            * notes
                * client token is valid for 10 min
                * if change request parameters but the same token
                    * you get error
        * Error handling for writing
            * transaction returns error if
                * conditions in actions do not met
                * confilict in one item
                * size too big
    * **tranaction read item**
        * upto 25 get items together
        * when error
            * **if transaction read conflicts with other ongoing transaction write**
    * Isolation Levels for DynamoDB Transactions
        * isolation and read commited
            * todo: read more
    * Transaction Conflict Handling in DynamoDB
        * todo: read more
    * best practices on transaction
        * todo: read more
* using Iam with transaction (todo)

## In Memory Acceleration with DynamoDb Accelerator (**DAX**)
* dynamodb can response in **millisecond**
* with cache you can make it **microsecond**
* it is eventually consitent for read
* why
    * from millisec to microsec
    * minimal change on client
    * in bursty or read-heavy work load, it is cost effective
        * does not use RUC
* **Usecases**
    * fast response for read
        * online gaming, trading, ...
    * reading small number of items more frequently
        * in trading, one product becomes so popular
        * **do need to wait for adaption on UCs for some partitions and get throttled till then**
    * app is read intensive but const effective
    * when there are repeated reads on some items
* **should not be used**
    * if strongly consitent read
    * if does not need micro sec
    * if write intensive
    * if cache is handled somewhere else
        * like client, server, ...
* how it works
    * creates a cache (DAX) cluster on top of dynamo
    * cluster has vpc
    * client routes everything to cache
        * if cache cannot handle it, pass it on to dynamo
    * How DAX Processes Requests
        * in the cluster one node is primary
            * todo: why cache needs primary
        * **it has intelligent routing and load balancing for nodes**
        * read operations
            * if a node can handle it, it returns what it has to client
            * if cannot
                * call dynamo
                * update itself
                * return response to the client
                * sync other replicas
            * if request is strongly consitent read
                * it directly pass it on to dynamo
        * write operations
            * first written to dynamodb
            * then DAX cluster
            * successful if both are successful
        * Request Rate Limiting
            * if you go off the limit of DAX node, it throttles
            * you need a cloudwatch to provision it if throttle count is too high
    * Item Cache
        * based on response of every getItem or GetItemBatch
        * TTL
            * default 5 min
        * LRU (least recently used)
    * Query cache
        * result from query or scan
        * stored by their parameter values
        * the rest is the same as item cache
* DAX cluster component
    * Nodes
        * smallest block of cache
        * it has its own instance
        * if you wanna scale it up
            * add more nodes to your cluster
    * clusters
        * manages all nodes
        * **it is one of the nodes which is primary node**
        * responsibility of every node
            * fulfill requests
            * ops to db
        * responsibility of cluster node (in addition to above)
            * evicting data according to policy
            * write to db
            * routing the requests (load balancer) (scalability)
            * if cluster fails, candidate other read replica (failover)
* todo: some other topics about DAX, worth reading


## no sql workbench
* **a tool to observe/work with no sql data**
* it does datamodeling, visualization, 
* todo: might worth digging more

## todo: some other topics worth checking

## **Improtant** Best practices
* NoSql design
    * Differences Between Relational Data Design and NoSQL 
        * in RDBMS, query is so flexible but expensive and hard to scale
        * in RDBMS, you just care about flexibility and query optimization do the performance
            * in nosql, you create schema based on your query and trying to make it performant
    * Two Key Concepts for NoSQL Design
        * in relational, you first create schema and then improve on query effectiveness
        * in no sql, 
            * first you need to know business needs (query/modification) and then design your table for the cheapest query
                * design + improvement at the same time
            * as least table as possible even if you repeat the data
    * Approaching NoSQL Design
        * **three concept when creating tables (queries) in nosql**
            * data size
                * on one query, how much data will be retrieved
            * data shape    
                * instead of reshaping data based on query, whatever you see in tables is the query result
            * data valocity
                * distribute data across partitions uniformly (based on shard hash)
                * avoid hot shard
        * **after designing your queries (above)**
            * keep related data together
                * as few tables as possible
            * use sort order
            * distribute queries
                * avoid hot shard
            * use global index
                * to improve queries instead of creating new tables
    * **partition key design**
        * for both **base table and secondary index** it needs to be uniformly distributed
        * using burst capacity
            * saving some of your uc if one partition might get hot later
        * **adaptive capacity (on table)**
            * if hot partition, more uc to that partition
            * todo: some other topic must read
        * Distributing Workloads
            * to avoid hot shard
            * does not mean that percentage of your partition can be hot
            * does not mean that you need to access every item in that partition
            * it means the more distinct values, the more likely that data spreads evenly
            * example of good/bad partition keys
                * user id
                    * if there are so many users
                    * and if one user is not so popular
            * basically if you have less items in one partition, less likely to get hot
        * Write Sharding
            * to help with uniform primary keys
                * add random number to partition key
                * add a number that can be calculated by other attributes
            * **different ways**
                * random suffix
                    * at the end of your PK
                        * ex: <date>-<random>
                    * downside
                        * if you wanna query all in your actual pk (no suffix) you need multiple queries
                        * event for one item read, hard to work out the pk id!!!
                * calulate suffix
                    * example add order id hash with modulo
                    * downside
                        * still for all read, need N reads
                        * but for specific item read, can easily calculate the pk
        * **Uploading data**
            * imagine you wanna upload huge data for composite key
            * you should not upload on the same partition (hot shard)
            * ex:
                * pk=user id, sort key=message id
                * bad way:
                    * upload all messages for one user in one go
                    * hot shard for that user id
                * good way
                    * first message of all users, then second message of all users
                    * workload distributed on different partitions
    * Sort Key design
        * best ones
            * you can use between, <> efficiently
            * you can implement hierarchy (one to many)
                * country-region-state..
        * **sort key as version control**
            * design
                * meta data that has last version
                * v0 always the active
            * process
                * get last version from meta and update it
                * update the version of v0 = lastVersion + 1
                * insert client data as v0
            * so many wholes ant problems
                * todo: important: check the online info, write code
    * secondary index
        * general
            * global
                * **global cannot provide strongly consistent read**
                * they consumes your UCs
                * **make sure of both**
                    * **what you wanna select (projection) (like covering index)**
                    * **what is your where clauses**
            * local
                * it is by b-tree
                * **you can avoid fetching from the actual data by convering index on local indexd**
                    * fetch (if you can) just data on the local secondary index
                * be careful with the limit when you get collection 
        * **Sparse Indexes**
            * **Sort keys for base key have a record just for ones that have values!!! they are sparse**
            * **pk in the global index is sparse too**
            * example of sort key for base table
                * wanna find the open orders of a customer
                    * pk: customer id, sort: if_open
                    * **is_open exists if order is open**
                        * once the order is open, thay field needs to be removed
                        * **do not use boolean (just exist/not exist)**
            * example of partition key on global index
                * players that play different games and if in one game the score above something, they get gold badge
                * pk for global index is *gold badge*
                    * if an item has it, query on global index returns it 
        * **Materialized aggregated queries**
            * materialize data in base table sort key
                * sort key has data and some other meta data that can be used by secondary index
            * ex:
                * base table: pk = songid, sort key: download ids, Details, Month-2018-01
                * each sort key "value" stores different data
                    * download Id: info about download
                    * detail: info about song itself
                    * Month-2018-01: Date = GSI pk and Monthly download = GSI sort key
            * **store materialized by data streaming from dynamo db (for example to a lambda) to do calculation for you**
        * **GSI overloading** 
            * **when sort key of base table is partition key of GSI**
            * sort key in base is columns (you wanna query a lot)
            * sort key in GSI is a generic column called "Data" 
                * (must be unique) so you can use it for your GSI
            * **So, with one GSI you can query anything!!!**
            * [example](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-gsi-overloading.html) 
        * GSI sharding
            * for selective queries
            * to avoid scanning the whole table
            * todo: what does it mean??
        * Creating Replica by GSI
            * GSI is basically a replica of table (of course a projection of it)
            * So:
                * set different provision of read for different reader
                    * one app can be throttled so using base table to read
                    * the other should not, using GSI to read which is the same data
                * eliminating the read from main table entirely
                    * reads on GSI and write on main table
            * how
                * GSI with the **same partition key as base table**
                    * GSI sort keys can be different based on your read
            * remember reads are eventually consistent
            * helpin dynamo with read replicas
                * already has read/write replica in different availability zone
                * it is additional help for high reads
    * large items
        * it is actually large attributes
            * example long texts that you **do not wanna query on them**
        * two basic approaches
            * compress data in the rows
            * put the data in s3 and put the url in table
        * s3 approach
            * two notes
                * putting data in s3 and updating dynamo is not transactional, you need to manag it in your app
                * make sure identifier in s3 does not grow so long (dynamo cuts it)
    * **time series data**
        * normally for whole app you need one table
        * **however, for timeseries, one table per application per period**
        * problem
            * more recent data in app, is used more often(as a whole)
            * specifically happens when storing events for app
        * solution
            * create table for current workload for specific app
                * with a proper (high) required provisining
            * name of the table should have period in it (ex: April)
            * in the client, based on period, work with specific table
            * when period is about to finish
                * create new table with period name in its name
                * make read/write capacity of old one to low
                    * cos no one need it no more
    * **Many-to-Many relationship**
        * it is called **graph data**
        * two approaches
            * adjacency list design pattern
                * for every relation (M Many to Many N)
                    * base table: pk M and partition N
                    * GSI: pk N and partion N
                        * opposite to base table
                * can query all Ns related to Ms and vice versa
                * example
                * invoice and bills
            * materialized graph
                * if queries are like this 
                    * ranking across peers
                    * common relationship between entities
                    * neighbor entity state
                * above queries cannot be fulfilled with adjacency
                * those queries are more about the relation data itself than the objects around them
                * how:
                    * data should be like this in M:N rel
                        * MId, RelType_NId, RelInfo, MInfo
                    * base table: pk: Md, sk: RelType_NId
                    * using index overloading
                        * GSI1: pk: RelInfo
                        * GSI: pk: RelType_NId
                * change in base data, propagate to indexes and edges automatically
                * todo: if data is not real time aggregated sensitive, there is a solution to do it offline, what does it mean?
        * **hybrid relational db**
            * meaning your sql and dynamo should exist shoulder to shoulder
            * for many reasons cannot move completely and in one go to no sql
            * you have heavy reporing 
            * however, your rdb cannot provide low latency on reads/writes
            * **use dynamo as materialized view repo**
            * solution
                * **use dynamo as incremented cache**
                * keep syncing sql with dynamo when write
                    * sp in sql to update dynamo
                * keep sync dynamo with sql
                    * dynamo streaming to update sql
                * **on every read in sql**
                    * push changes to dynamo too
        * Relational modeling
            * when you have relational entities and you wanna model it in dynamo
            * **why rdms is slow**
                * need to optimize query cos data is distributed
                * need to put locks to assure ACID
                * needs to do joins cos data is distributed
            * **Why nosql is quick**
                * all the hierarchy in just one item
                * composite key puts related items physically close to each other
            * First step
                * different mindset
                    * rdms: firs schema, try to normalize, think of queries (access patterns) later
                    * nosql: query (access paterns) first, then design schema
                * to start design
                    * review user stories to find out the access patterns
                    * check logs of existing systems to work out patterns
                * design
                    * after finding the access patterns
                    * start to **denormalize** complicated queries
                    * use GSI and LSI, use Adjacency design patter, index overloading, ..
                    * you should have as least table as possible
                * [example](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-modeling-nosql-B.html) 
                    * todo: implement it


        
    * query and scanning
        * performancen for scan
            * scanning goes over all items in base table/Index table (to find attribute which is not pk or sk fulfill something)
                * you should limit it by
                    * conditions
                    * limit of records to return
            * **scanning should be avoided as much as possible**
                * as table grows, it becomes slower
                * it eats up all Uc
        * avoid sudden spike in read
            * scan can use up your Uc and throttle other important reads
            * scan can cos the **sudden increase in RUC**
                * cos it does not know how many uc will be used!!!
            * scan can **use up one partition RUC**
                * cos it reads adjacent rows which might be in the same partition
                * this causes hot 
        * how to use scan if needed
            * reduce page size on scan
                * scan uses 1MB on each roundtrip
            * isolate scans
                * use shadow table (replica) just for scans
        * parallel scans
            * instead of sequencially go through the table, do it in different parts of it
            * it does not heat up one partition
            * todo: read mroe
        * total segment
            * todo: what is it?
    * global tables
        * todo: what is it?
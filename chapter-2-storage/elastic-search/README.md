# Aws ElasticSearch (ES) 


## What/Why/How is ElasticSearch in General (out of aws)
* when you wanna search on strings
* why not sql and use queries like
    * `where MyField like 'search%' `
    * those are heavy operations on sql 
    * normally ends in scan
    * it gets worse when you have so many parameters in where clause
* elastic search is a search engine
    * format, store, retrieve data
    * difference
        * **you need to manually set your index to improve it**
* no tables or schema
    * all documents
* two jobs
    * query
        * any types of search (structured/none-structured, geo, metrics)
    * analyze
        * zoom in/out, aggregate
* it uses federated index
* it is resful api
    * over http
* some concepts:
    * index : 
        * group of related documents
        * identifier
        * any ops (CRUD) needs to be done by **index**
    * type
        * **related documents inside an index**
    * documents
        * actual data to store
    * shard
        * part of a specific index that are fully functional
        * they are fully functional as normal indexes
        * **they are distrubuted accross different nodes**
    * replica
        * duplicate of shards
* how to search
    * some wild cards on the search query
        * multiple: demo1,demo2,demo3
        * strings on indexes: d*1
* different types of apis
    * document: 
        * ops on document level 
        * (bulk or one docs)
        * CRUD
    * search: across all indexes and types 
    * aggregation: ditto 
    * index: ops on index level 
    * cluster: cluster level
* **architecture and scaling**
    * you can shard each index
    * two types of shards
        * primary : does manipulation
        * replicas : copy of primary
    * todo: not sure if only primary can manipulate data
    * **every primary has at least one replica**
    * nodes
        * virtual env of ES
        * can be in one or multiple machines
    * ES decides how to spread data between your shards!!
    * **to scale out: add more shard to your specific index**
        * routing request to your shard is automated
    * **to scale up: add more replica to your primary**




## intro
* what is elastic search
    * a tool for search and analytics
    * usecases
        * log analytics
        * realtime application monitoring
        * clickstream analysis
* domain
    * first thing to create
    * sync with all nodes
    * cluster with instance type, settings, instance counts
* features
    * todo: ??

## todo: some topics skipped (todo: come back and have a look)
## streaming data to ES
* data from some resources can be sent/streamed to ES
* some services have built in
    * Kinesis firehouse and cloudwatch logs
* some needs to setup
    * **dynamo db**, s3, lambda, kenisis log data


## Kibana and ES
* aws by default provides one Kibana for ES
* you can access it by some link generation
* Kibana is a js app
* it configures a proxy in public vpc so clients can have access
## todo: 
* how authentication works if using it in the app
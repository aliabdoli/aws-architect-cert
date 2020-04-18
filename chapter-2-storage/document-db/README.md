# DocumentDb
## todo:
* different types of no sql db(s): graph, document, key/value, column store
## what is it
### intro
* it is **Managed MongoDb**
* why
    * auto increasing storage
        * do not need to do it manually
        * upto 64 TB
    * read throughput
        * read instances up to 15
        * **all share the same storage**
            * does not need sync between the read replcas **in different instances**
            * new read instance gets created fast
            * **data is duplicated 6 times on the same storage on the same machine**
            * read replica instances (readers) can be on different machines
    * scaling resources
        * automatically for instances for cpu/memory/...
    * whole db on vpc (you can isolate it)
    * auto monitoring
    * auto failover
    * auto backup
    * can encrypt db keys
* **clusters**
    * **data is copied in different availability zones**
    * (see cluster pic) [https://docs.aws.amazon.com/documentdb/latest/developerguide/what-is.html]
    * just one master (for all availability zones)
    * every availability zone, two copy of data (in total 6 copies of data)
    * **only master can write*
    * **it writes to all availability zones**
    * can have upto 16 read replica
* instances
    * every one virtual db env

### How it works?
* First create the cluster (behid the seen)
    * cluster volume, instances get geerated
    * cluster volume is distributed storage
        * data gets duplicated across all of duplicate but same storage
* instances
    * **one primary instance**
        * does the data modification
    * up to 16 read instance
        * does read
        * they do not need to be from same instance class
        * they can be provisioned
* how read/write work
    * **data has duplicate in the storage**
    * when write in primary instance
        * it run the state (not the data) in all duplicates
    * when read
        * there are so many duplicates in volume and replicas
        * they can read from any of them
        * **read is eventually consistent**
* endpionts
    * can connect to cluster endpoint for all read/writes **with replica set mode** (recommended)
    * can connect to a specific instance
* cluster endpoint
    * it is the endpoint of primery
    * failover: has at least one **read failover** when crashes
* read enpoint
    * load balancing the reads
* **replica set mode**
    * todo: dig in
* storage
    * **one volume and 6 copies of data**
* Amazon DocumentDB Replication
    * auto repair
        * if one copy of data is crashed, automatically gets copied by the other
    * survivable cache warming
        * page cache is in different process of db (todo: what does that mean?)
        * if db crashs, still cache can be valid till db is back up
* **Read preference options**
    * Write durability
        * data in DocumentDb is in **distributed, self-healing storage**
            * they have nodes too (it is not related to db instances, they are nodes of storage)
        * when write
            * making sure the majority of **storage replica nodes** acknowledge the right is done
        * all writes is done in primary instances
            * when it acknowledges write is done, **no roll back and it is durable**
        * DocumentDb is highly durable
            * cos durablity is handed over to storage (distributed storage)
    * Read isolation
        * **only return data that is durable**
        * it means it **blocks read when write is happenning**
    * Read Consistency
        * read cannot be rolled back (todo: what does it mean?)
        * if you read from master, always strongly consistent (read-after-write consistency)
        * if you read from replicas, eventually consistent
    * read preference option (todo: ?)
        * three types
            * primary
            * primary preferred
            * secondary 
            * secondary preferred
    * High availability
        * if primary crashes
            * one of read replica gets replaced
                * brief interruption
            * if no read replica
                * it gets recreated
                * bit of interruption
    * read scalability
        * read replca
* TTL deletes (Time to live deletes)
    * you can specify how long your record should exist
        * if exceeds, db removes it
    * it happens in the background
    * **no guarante that happens in that timeframe (but close)**
    * it depends on you document size, resource utilization, ...
    * todo: in document talks about other approachs



### what is document Db
* used when no need to model (normalization, rows, columns, locks) and json is more intuitive that data models
* **it makes the development faster**
    * no need to change schema in db every time you change data model
* used when schema is so different in different scenarios
    * ex: to fulfill every request, you might need different schema
* however, it heavily impacts query
* Use cases
    * user profile
        * every user provides different info
        * some address, phone, ... and some do not
        * you can implement it with inheritance in sql db tho
    * Real time big-data
        * rout BI data for query 
    * Content management
* understanding documents
    * it is **semi-structured**
    * no normalizing across tables every which has unique structure
    * using neted key-value to find document structure
    * **different documents can be stored in the same db**
        * they come in differnt strcuture
        * **documents are self describing of schema: like json values**
        * you need to process **similar data but different format**
    * sql vs none relational terms
        * sql:      table, row, column, primary key, nested tables or objects
        * no sql:   collections, document, field, object id, embeded documents
        * other stays the same: view, index, array
    * simple documents
        * field-value
        * no nesting
        * it s self-describing
            * can be in json or any other format
    * embeded documents
        * documents inside another document
        * field - other document
        * it is good for grouping the documents
        * it is still self-describing
    * example
        * **inheritance in individual document**
        * ex: different types of publication (book, paper)
            * just put type field in each type
            * no seprating the documents (two tables for book, paper) (Table per type) or unused fields in a covering table (Table per inheritance)  
    * **understanding Normalization in a Document Database**
        * imagine you update the parent and you want it to go through the chile also (foreign key)
        * **do not worry, the storage is cheap, so make a new record and retrieve the latest**
    * working with documents
        * **collection = table**
            * except to schema enforcing
        * for every related objects, put them in the same collections
        * adding documents
            * **in DocumentDb, first insertion of collection results in creation of new db (if not existed)**
            * adding one ducument
                * returns
                    * acknowledgement = true
                    * **isertedId**
            * adding multiple
        * query documents
            * find (your criteria)
            * todo: not sure if you can do on none-indexed and if so, what would happen!!
        * Updating Documents
            * **you need identifier of the document**
            * in that document you can update everything (arrays, nested, ...)
            * **response is the whole updated object**
            * if field does not exist, it creates a new field
            * Inserting New Fields into an Embedded Document
                * just the same, use "."
            * removing a field
                * use $unset in update instead of $set
            * Removing a Field from Multiple Documents 
                * use multi: true
                * it removes it from all document in that collection
        * deleting the document
            * you need id

        
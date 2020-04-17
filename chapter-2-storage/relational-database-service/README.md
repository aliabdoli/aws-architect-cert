# Relational Database Service
## intro
* why
    * you can scale in memory and cpu
    * backups, failure detection and recovery all managed
    * some sp s are not allowed to call in here
    * support primary/secondary sync automatically (high availability)
    * in addition to noraml security to instance, you can use IAM
* DB instance
    * machine that your db is running
    * one DBMS (your choice of any relational: MySql, mssql, ...)
    * it runs on your vpc
        * can control the network
* security
    * can assign ip ranges to access to your instance
* monitoring
    * on cloud watch
* **db instance class**
    * hardware spec 
    * based on cpu/disk, ...
    * todo: not sure if learn
    * todo: no auto-scaling?!!!
* db instance storage
    * different type of disks
    * todo: not sure if learn
* availability zones
    * different dbinstances in different areas
    * independent and synced
    * it supports failover if one availability becomes unavailable
        * todo: how failover works in sql
* **High availability**
    * this is for failover scenarios
        * when one db instance is down, what to do
    * different dbms
        * sql server: **db mirorring (DBM)**
        * others: **MultiAz**
    * **this is not a solution for read replica, check read replica aws solutions**
    * it creates different replica that
        * are sync with master
        * if anything happend to master, they can be replaced
        * support both read/write
## Getting Started
* Creaing MSSql db instance
## configuring db instances
* **RDS proxy**
    * it is for scalability and reliablity
        * when thre is a surge in request
            * normally your db instance need more memory/cpu to handle them (queue them, ...)
    * two major things it does 
        * keep connections and manage them
            * if surge, throttle and squence request to db
            * if requests exceed the number  you specify, it rejects
            * reduce overhead to process credentials, ... (using the same ) 
                * proxy does it on behalf of db
        * automatically move to standby server when failure happens
    * terminalogy
        * it reuse connection **after each transaction per session** 
            * this reuse called **multiplexing**
        * just one endpoint
            * through that you can connect to read-write or read of actual db
        * target group: all db instances in one cluster (if aurora)
        * engine family: all db instances using same protocol to communicate
    * Connection Pooling
        * openning and closing connections
        * has cpu/memory overhead
        * ssl/tls
        * solutions
            * trust clients not exhaust it
                * clients will have cpu/memory overhead
                * they might breach
            * db instance
                * in surge, huge overhead
            * delegate it to rds proxy
    * Security
        * todo: ?
    * Failover
        * why
            * when something is wrong with db instance
            * intended like db upgrade
        * MultiAz
            * one reader instance (standby db)

        * in failover, you have mild quick outage

        * with proxy
            * route requests to reader instance (standby db)
            * todo: some additional stuf
    * transaction
        * not reusing connections per session per transaction
    * setup
        * todo: read more
* Option Groups
    * some dbms, for security, have different group of users
## Managing Db instance
* managing read replica
    * some dbms support read replica
        * master db for read/write and read replicas to master
    * changes in the master **async to read replicas**
    * why
        * take i/o and cpu overhead of writing to a separate machine
        * still can serve read trafic when write (master) is down
        * implementing disastrous recovery
    * if replica in different machine
        * ddl operation can be done
            * just for mysql
            * ex: rebuilding indexes
        * sharding
            * still you can have replicas in each shard
        * Implementing failure recovery 
    * with ORM like entity framework
        * https://stackoverflow.com/questions/35773560/how-to-achieve-read-write-separation-with-entity-framework
* todo: not sure

## back up
* todo: not sure

## MSSql on amazon
## sharding in mssql
* need to be managed by yourself in app layer (or some service)
* Aurora does it for you
* todo: how it works
* todo: how application (client work with it?)



    

## [Lambda](https://docs.aws.amazon.com/lambda/latest/dg/welcome.html)
### intro
* no provisioning or managing services
    * os maintanance
    * logging management
    * code monitoring
* pay as you go
* trigger types
    * event from dynamo, s3, ...
    * http request by api gateway
    * **direct api calls using aws sdk**
* pros and cons
    * up:
        * just manage your code
    * down:
        * cannot configure os, memory, ...
* concurrency
    * number of requests that function can server at the pointing time
    * specific number or **auto provisioned**
* **limits** (todo: go off the limit of each one and see what happens)
    * memory: up to 3 MB
    * timeout (15 mins)
    * env var: 4KB
    * policy: 20KB
    * Layers: 5
    * burst concurrency (auto-scalling): 500-3000
    * Invocation frequency per Region (todo: ??)
    * Invocation frequency per function version or alias (requests per second) (todo: ??)
    * payload (both request and response) : 6 MB sync and 256KB async
    * Deployment package
    * /tmp directory storage(todo: ??)
    * **execution process: 1024 sec**
* there are **two tools to deploy your lambda**
    * serverless
    * dotnet lambda 
* Serveless
    * app to deploy and config lambda
    * it creates cloudformation for each deployment (might be multiple lambdas)    
    * creates zip code in s3 to store your file
        * every deployment it checks the hash of new and previous deployment
    * Still you need cloudformation
        * for your IAM 
    * Each deployment new version of service
    * full deploy, **deploys the service** (every function in serverless.yaml) and all s3, ...
        * `sls deploy`
    * you can deploy **without using cloudformation for dev**
        * `sls deploy function -f hello`
        * cos cloud formation is slow process
    * deploy already packaged in s3
        * `serverless deploy --package path-to-package`
    * create first project
        * csharp: 
            * `dotnet new lambda.EmptyFunction --name BlogFunction --profile default --region ap-southeast-2`
        * `sls create --template aws-csharp --path myService`
        * it works for .netcore2.1 version
        * todo: netcore 3.0 not supported?!!!
        * todo: why cannot for solution, it only works in project (not solution!!)
            * `dotnet lambda packge`
    * it has some naming conventions to create the function name: 
        * `<service>-<env>-<functionn>`
* dotnet lambda (cli command)
    * this [repository](https://github.com/aws/aws-lambda-dotnet) has some boiler plate project for different type of lambda functions
    * use vs templates for lambda functions including all settings required for dotnet lambda
    * todo: it can be plugged into vs for deploy, ... . how?
    * todo: dotnet lambda vs serverless

## Permission
* two types of permissions
    * Access to lambda resources (func/layers): 
        * attach users to policies that has that 
    * Access for lambda to use other resources (Execution Role): 
        * attach it to lambda itself
        * minimum permission is cloudwatch logs (to be able to log)
* permissions can be specified (todo: what are all options?)
    * naming patterns
    * conditions
* to review in console: in function -> permissions => permission summary
* Execution role:
    * when create function, needs a role
    * by default it has minumumt of (createlog, pushevents to log) on cloudwatch
    * trusted role policy (todo: ??)
* resource based policy (todo:??)
* user policy 
    * to give access in IAM to have access to function 
    * todo: ??
* resource and conditions (??)
* permission boundries (??)

## managing functions
* what to config
    * config console, env variables, concurrency, verioning, aliases, layers, network, db, tags
* Configuration console (todo: ??)
* Env variables
    * on unpublished version by key, value pair
    * variables are locked with that version
    * needs to be enrypted on client and on aws (by the secret in aws) (todo: how)
    * aws enrypt the env variable on itself at "rest" (todo: what is rest?)
    * env variables used by aws on func
        * reserved (cannot be used)
            * _HANDLER, AWS_REGION, AWS_LAMBDA_FUNCTION_NAME, ...
        * reserved by can be overwritten
            * PATH, Lang, ...
            * used by cicd
    * enryption
        * on Server
            * by default all encrypted at rest
            * you can change it with CMK key
                * CMK key is in your account
            * to stop access from someone seeing the variable values but still manage it
                * remove access of that user from CMK key of your account
        * on client
            * at console when adding evn variable, enable helper tick
            * on client at the code 
        * todo: **injecting sensitive data with serverless**
* concurrency
    * number of request can be processed at pointing time
    * when the request comes in
        * if no instance is ready or all busy
            * create new instance
        * if an instance ready (and not busy), do not create a new one, uses that one
    * there is limit in different regions
    * to make sure always can serve
        * configure with reserved concurrency (todo: how)
    * when new instance gets created
        * aws gets instance ready (meaning moving your code to that instance, run initialization outside your handler todo: ??)
        * **if that pre-run is too long**, it causes latency fluctuations in serving requests in newly created instances (**Latency fluctuations**)
            * ex:
                * your code or your dependency files are too big 
                    * **Service that you have in serverless has so many functions and all use the same code base. Then your code becomes huge**
                * creating SDK client during initialization
            * solve it buy allocating **provisioned concurrecy** before increase in your request
                * **connecting it to auto scaling service**
                    * scheduled or utilization (like latency)
    * configuring concurrency
        * Reserved concurrency
            * specify the exact number of functions you need at the time
            * it is limited to (**your aws account** - 100)
            * if requests go above that, they get throttled
            * other functions without reserved, use the same pool upto your account limit
            * other functions cannot stop your func from scaling (you ve got reserved number)
            * these functions are always up (no initial latency)
        * provisioned concurrency
            * it spins up instances (no flacutation latency if ...)
            * it does not spin it up straight away
                * so if you provision so many, still latency fluctuations
            * there is a limit in the max first hour and rest afterwards
            * you need to pay extra for initialization (apart from others)
            * if goes above provisioned, still use the remaining shared pool
                * **That is the difference with reserved as reserved gets throttled in this situation**
    * Initialization Code (can cause latency fluctuations)
        * Lamda copy your code to the instance
            * everytime new distance comes in
        * then run initialization code 
            * in net core **all you have in your handler constrcutor runs as initializer**
                * if time consuming in contructor, latency fluctuations
                * ex:
                    ```Csharp
                    public class Service 
                    {
                        public Service(){
                            // Configure Handler1
                            // Configure Handler2
                            ...
                            // Configure Handler 3
                        }
                    } 
                    ``` 
                    * configuring everything for all handlers evenif you just wanna use one of them at the time!!!
                    * it can happen in DI (if singlton, ...) 
        * Auto scaling (todo: ???)
            * if utilization changes by percent
* Versioning
    * if you have beta version of it
    * todo: not sure if dig into it?
* layers
    * if other code is required but not changed at all
    * so, it does not need to be part of your deployment
    * make the deployment zip file small (not anything else!!!)
    * it copies those file in /opt of server running
    * using layers .net core
        * challenging
        * todo: not sure if it gives that much value
* Alliases
    * point to verions
    * can change the version that same alliese points to
    * todo: not sure of usecase
* Networking
    * you can add VPC (optional)
    * it can have access to all resources in that VPC
        * like db, ...
    * lambda creates elastic network interface (eni) for every SG and subnets (todo: ??)
        * it creates on first invokation!!!
        * if not using it for so long, aws recliam it!!!
            * and first request after a while gets rejected!!
    * cos it creates eni when connecting the vpc
        * you need to add roles that can create/delete/describe eni!!!
                    
        
                  
                   

## Invoking the function
* can be called sync or async
    * sync: wait for it to get the response
    * async: 
        * put it in a queue
        * it returns immediately with no reponse
        * it can do retries
        * can send invocation records to destinations
        * ex: Kenesis, Sqs, dynamo (event source mapping)
            * can send in bulk
    * they have different scaling and errors
* Sychronous calls
    * if sync, then lambda return response of operation to you
    * it does not need to be connect (as a trigger) to anything to be able to be called
    * if error, ... it returns it to you when called
    * also has http status code!!!
    * you can see the log (not in response)
    * `aws lambda invoke --function-name lambdaServerless-dev-first  --payload "{ \"id\": \"1\" }"  --cli-binary-format raw-in-base64-out response.json` 
* **Asynchronous calls**
    * * `aws lambda invoke --function-name lambdaServerless-dev-first  --payload "{ \"id\": \"1\" }"  --cli-binary-format raw-in-base64-out --invocation-type Event response.json`
    * straight returns successful (if can load the function)
    * put it in the internal queue to later pick them up
        * we had problem the way that it picked up from the queue
        * todo: what kinda queue, does it preserve the orders, how about multiple invockations
        * it retries if errors
            * **it retries two more times**
            * **one min to second and 2 min to third attempts**
        * if function does not have concurrency for **all events**(error is full or off the limit of reserved concurrency)
            * it throttles any additional requests with error!!!
            * **it send it back to the queue and retry interval goes from 1 sec to 5 mins up to 6 hours**
            * **you can see the queue in the cloud watch trace (the ones stuck)**
        * interval queue
            * eventually consistant
                * **one successful execution from queue can be executed multiple times**
            * **events might get deleted from queue without being processed!!!**
                * when the queue is too backed up that they expire before lambda picks them up
            * queue can get backed up
                * if so many retries for failed requests
                    * you can configure not to do it
                * concurrency is so low compare to requests rate
            * to avoid above
                * code needs to handle duplicate requests
                * enough concurrency should be available
                * **set the time that event can sit in the queue before getting deleted**
                    * make it short and handle it in deleted messages
            * to handle deleted messages!!
                * define **destination**  for your lambda
                    * you can send **invocation context (reqest/response)** to route successful/failed messages to other services 
                    * it can be conditional (if fail or successful different services)
                    * services like: sqs, sns, lambda, eventbridge
                * define **dead letter queue**
                    * different from destination
                    * it is built in feature
                    * just for deleted messages (not when they are waiting for retry!!)
* event sourcing mapping
    * for triggers that do not directly call lambda functions
    * sqs, dynamo, kensis
    * it reads from the source and call your function
    * it does it in the batch
        * you can specify the size of the batch
    * two batches
        * source batch: queues from source
        * event batch: events that call the lambda
    * both batches in terms of size and order is configurable
* Function State
    * there is no healthcheck for a function
    * you can check it by status of the function
    * todo: deployment ??
* **Error Handling**
    * two types of errors
        * invocation errors: before reaching to your function
        * function error: causes by runtime or function code
    * checkout the different common errors of each type in documentation
    * function errors (unlike invocation errors) do not cause 4xx or 5xx
        * it adds header x-amz-function-error
    * handling errors (**in all of them your function code needs to tollerate duplicate/half dropped off processing message**)
        * calling directly
            * you should have retry, send to queue, or ignore strategy
        * async call    
            * all the queue strategy discussed above
        * event sourcing
            * retries the **entire batch of failed items**
            * keep retirying till expired
            * to see them go to cloudwatch trace
            * for sqs sources you can manage retry time
        * aws services
            * sync: each service has its own stratagy
                * ex: api gateway might return it to the user
            * async: check async section
## lambda runtime 
* Execution Context
    * it is temp runtime environment that init any external dependency that your code wants
        * http endpoints, db connections
    * takes sometime to set up execution context
        * all bootstapping
    * todo: not sure
## lambda applications
* a group of lambda functions (like services)
* it is on console
* todo: dig more

## working with other services
* todo: dig into


    

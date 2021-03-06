﻿using System;
using Amazon.Lambda.Core;


namespace LambdaServerless
{
    public class SecondProcessRequestFunction
    {
        //it is wrong to use this kinda data in lambda but to show how it works
        public int ClassLevelCounter { get; set; } = 0;

        public string FunctionHandler(Input input, ILambdaContext context)
        {
            ClassLevelCounter++;
            SharedContext.Counter++;
            return $"input: {input?.Id}, ClassLevelCount: {ClassLevelCounter}, SharedContexCounter: {SharedContext.Counter}";
        }
        public class Input
        {
            public string Id { get; set; }
        }
    }
}





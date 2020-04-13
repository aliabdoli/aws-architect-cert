using System;
using System.Collections.Generic;
using System.Text;

namespace LambdaServerless
{
    //shared across all functions (bad practice)
    public class SharedContext
    {
        public static int Counter = 0;
    }
}

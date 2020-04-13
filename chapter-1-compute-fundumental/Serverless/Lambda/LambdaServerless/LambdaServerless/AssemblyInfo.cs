using System;
using System.Collections.Generic;
using System.Text;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaServerless
{
}

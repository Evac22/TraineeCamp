using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([BlobTrigger("traineecamp22/{name}", Connection = "DefaultEndpointsProtocol=https;AccountName=traineecamp22;AccountKey=v6/uQe14jmK5ZGHm+oHRhi9p8se3D38HeCSKjGULLZcVTk79c08TwCqUHXdMPahw/zeDSKH3DTM9+AStNYVSow==;EndpointSuffix=core.windows.net")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}

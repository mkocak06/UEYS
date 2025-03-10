using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.ConfigModels
{
    public class S3Settings
    {
        public string BucketName { get; set; }
        public string ServiceURL { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public bool SSL { get; set; }
        public string SignatureVersion { get; set; }
        public bool ForcePathStyle { get; set; }
    }
}

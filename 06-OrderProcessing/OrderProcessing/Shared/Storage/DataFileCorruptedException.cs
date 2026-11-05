using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessing.Shared.Storage
{
    public  class DataFileCorruptedException :Exception
    {
        public DataFileCorruptedException(string fileName) : base($"The JSON file '{fileName}' is corrupted.")
        {
        }
        public DataFileCorruptedException(string fileName, Exception innerException) 
            : base($"The JSON file '{fileName}' is corrupted.", innerException)
        {
        }

    }
}

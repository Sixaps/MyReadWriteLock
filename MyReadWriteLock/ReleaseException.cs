using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReaderWriterLock
{
    class ReleaseException : Exception
    {
        public override string ToString() {
            return "try to release the write lock before get it";
        }
    }
}

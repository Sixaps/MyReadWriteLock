using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReaderWriterLock
{
    class DowngradeException : Exception
    {
        public override string ToString() {
            return "Try to downgrade the writelock before get the write lock!!";
        }
    }
}

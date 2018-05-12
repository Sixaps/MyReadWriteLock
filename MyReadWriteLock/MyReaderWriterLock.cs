using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyReaderWriterLock
{
    class MyReaderWriterLock
    {
        // if lock's value is -1, the thread has a write lock
        const int _writerLock = -1;
        //the quantity of locks can not be larger than 65535
        const int _maxLockNum = 65535;
        // record the lock's state. if _lock > 0, there are some readers having the lock.
        int _lock;

        //record the writer's state. if isWriteWaiting > 0, there are some writers waiting for locks.
        int isWriteWaiting = 0;
        
        Boolean isWriteFirst = false;

        //record the current thread which have the write lock
        Thread currentThread;

        public MyReaderWriterLock(Boolean isWriteFirst = true) {
            this.isWriteFirst = isWriteFirst;
        }
        //get a read lock
        public void GetReadLock()
        {
            var w = new SpinWait();
            var tmpLock = _lock;
            while (tmpLock == _writerLock ||
                tmpLock >= _maxLockNum || 0 < Interlocked.CompareExchange(ref isWriteWaiting, 0, 0)  ||
                tmpLock != Interlocked.CompareExchange(ref _lock, tmpLock + 1, tmpLock))
            {
                w.SpinOnce();
                tmpLock = _lock;
            }
        }

        // get the write lock
        public void GetWriteLock()
        {
            var w = new SpinWait();
            if(isWriteFirst)
                Interlocked.Increment(ref isWriteWaiting);
            while (0 != Interlocked.CompareExchange(ref _lock, _writerLock, 0))
            {
                w.SpinOnce();
            }
            currentThread = Thread.CurrentThread;
        }

        public void DowngradeToRead()
        {
            if(currentThread != Thread.CurrentThread)
                throw new DowngradeException();
            Interlocked.CompareExchange(ref _lock, 1, _writerLock);
        }

        public void ReleaseReadLock()
        {
            Interlocked.Decrement(ref _lock);
        }

        public void ReleaseWriteLock()
        {
            if ( currentThread != Thread.CurrentThread || _writerLock != _lock) {
                throw new ReleaseException();
            }
            isWriteWaiting--;
            currentThread = null;
            Interlocked.CompareExchange(ref _lock, 0, _writerLock);
        }

        public override string ToString()
        {
            return "lock counter: " + _lock;
        }

    }
}

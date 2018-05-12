using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyReaderWriterLock
{
    class Program
    {
        const int incrementTimes = 5;
        const int iterations = 20;
        static int num, collisions;
        static MyReaderWriterLockUsingSleep _myLockUsingSleep = new MyReaderWriterLockUsingSleep();
        static MyReaderWriterLock _myLock = new MyReaderWriterLock(false);
        static MyReaderWriterLock _myLockWriteFirst = new MyReaderWriterLock();


        static void WriteUnlocked()
        {
            for (int j = 0; j < iterations; j++)
            {
                Thread.Sleep(1);
                for (int i = 0; i < incrementTimes; i++)
                    num++;
            }
        }

        static void ReadUnlocked()
        {
            for (int j = 0; j < iterations; j++)
            {
                Thread.Sleep(1);
                if (num % incrementTimes != 0)
                    collisions++;
            }
        }

       

        static void MyWrite()
        {
            for (int j = 0; j < iterations; j++)
            {
                _myLock.GetWriteLock();
                try
                {
                    Thread.Sleep(1);
                    for (int i = 0; i < incrementTimes; i++)
                        num++;
                }
                finally
                {
                    try
                    {
                        _myLock.ReleaseWriteLock();
                    }
                    catch (ReleaseException e) {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        static void MyRead()
        {
            for (int j = 0; j < iterations; j++)
            {
                _myLock.GetReadLock();
                try
                {
                    Thread.Sleep(1);
                    if (num % incrementTimes != 0)
                        collisions++;
                }
                finally
                {
                    _myLock.ReleaseReadLock();
                }
            }
        }

        static void MyWrite_writeFirst()
        {
            for (int j = 0; j < iterations; j++)
            {
                _myLockWriteFirst.GetWriteLock();
                try
                {
                    Thread.Sleep(1);
                    for (int i = 0; i < incrementTimes; i++)
                        num++;
                }
                finally
                {
                    try
                    {
                        _myLockWriteFirst.ReleaseWriteLock();
                    }
                    catch (ReleaseException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        static void MyRead_writeFirst()
        {
            for (int j = 0; j < iterations; j++)
            {
                _myLockWriteFirst.GetReadLock();
                try
                {
                    Thread.Sleep(1);
                    if (num % incrementTimes != 0)
                        collisions++;
                }
                finally
                {
                    _myLockWriteFirst.ReleaseReadLock();
                }
            }
        }

        static void MyWrite_usingSleep()
        {
            for (int j = 0; j < iterations; j++)
            {
                _myLockUsingSleep.GetWriteLock();
                try
                {
                    Thread.Sleep(1);
                    for (int i = 0; i < incrementTimes; i++)
                        num++;
                }
                finally
                {
                    try
                    {
                        _myLockUsingSleep.ReleaseWriteLock();
                    }
                    catch (ReleaseException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        static void MyRead_usingSleep()
        {
            for (int j = 0; j < iterations; j++)
            {
                _myLockUsingSleep.GetReadLock();
                try
                {
                    Thread.Sleep(1);
                    if (num % incrementTimes != 0)
                        collisions++;
                }
                finally
                {
                    _myLockUsingSleep.ReleaseReadLock();
                }
            }
        }



        static void Main(string[] args)
        {
            //TimeTest();
            //ResultTest_WriteFirst();
            ResultTest();
            Console.WriteLine("运行结束");
            Thread.Sleep(1000000);
        }

        private static void ResultTest() {
            Action[] tasks = new Action[] { WriteOneTime, ReadOneTime };
            tasks = Enumerable.Repeat(tasks, 5).SelectMany(x => x).ToArray();
            Parallel.Invoke(tasks);
        }

        private static void ResultTest_WriteFirst()
        {
            Action[] tasks = new Action[] { WriteOneTime_WriteFirst, ReadOneTime_WriteFirst };
            tasks = Enumerable.Repeat(tasks, 5).SelectMany(x => x).ToArray();
            Parallel.Invoke(tasks);
        }

        static void WriteOneTime_WriteFirst() {
            _myLockWriteFirst.GetWriteLock();
            Console.WriteLine("写线程----------" + Thread.CurrentThread.ManagedThreadId + ": 开始写");
            num += 1;
            Thread.Sleep(20);
            Console.WriteLine("写线程----------" + Thread.CurrentThread.ManagedThreadId + ": 写结束");
            _myLockWriteFirst.ReleaseWriteLock();
        }
        static void ReadOneTime_WriteFirst()
        {
            _myLockWriteFirst.GetReadLock();
            Console.WriteLine("读线程" + Thread.CurrentThread.ManagedThreadId + ": 开始读");
            Thread.Sleep(20);
            Console.WriteLine("读线程" + Thread.CurrentThread.ManagedThreadId + ": 读得 num = " + num);
            _myLockWriteFirst.ReleaseReadLock();
        }

        static void WriteOneTime()
        {
            _myLock.GetWriteLock();
            Console.WriteLine("写线程----------" + Thread.CurrentThread.ManagedThreadId + ": 开始写");
            num += 1;
            Thread.Sleep(20);
            Console.WriteLine("写线程----------" + Thread.CurrentThread.ManagedThreadId + ": 写结束");
            _myLock.ReleaseWriteLock();
        }
        static void ReadOneTime()
        {
            _myLock.GetReadLock();
            Console.WriteLine("读线程" + Thread.CurrentThread.ManagedThreadId + ": 开始读");
            Thread.Sleep(20);
            Console.WriteLine("读线程" + Thread.CurrentThread.ManagedThreadId + ": 读得 num = " + num);
            _myLock.ReleaseReadLock();
        }
        private static void TimeTest() {
            Console.WriteLine("性能测试: ");
            withoutThread();
            Test(new Action[] { ReadUnlocked, WriteUnlocked }, "不加锁");
            Test(new Action[] { MyRead_usingSleep, MyWrite_usingSleep }, "加锁——使用睡眠");
            Test(new Action[] { MyRead, MyWrite }, "加锁——非公平");
            Test(new Action[] { MyRead_writeFirst, MyWrite_writeFirst }, "加锁——写优先");
            Thread.Sleep(1000000);
        }

        private static void withoutThread() {
            int write_times = 10;
            num = collisions = 0;
            var dt = DateTime.Now;
            for (int times = 0; times < write_times; times++)
            {
                WriteUnlocked();
            }
            for (int times = 0; times < 500; times++)
            {
                ReadUnlocked();
            }
            var dt2 = DateTime.Now;
            Console.WriteLine();
            Console.WriteLine("线性执行:");
            Console.WriteLine("耗时: " + (dt2 - dt).TotalMilliseconds);
            Console.WriteLine("读冲突次数: " + collisions);
            Console.WriteLine("结果: " + num);
            Console.WriteLine("预期结果: " + iterations * incrementTimes * write_times);
        }

        private static Action[] Test(Action[] tasks, string test)
        {
            int write_times = 10;
            num = collisions = 0;
            Action[] task1s = Enumerable.Repeat(tasks[0], 500).ToArray();
            Action[] task2s = Enumerable.Repeat(tasks[1], write_times).ToArray();
            tasks = task1s.Concat(task2s).ToArray();
            var dt = DateTime.Now;
            Parallel.Invoke(tasks);
            var dt2 = DateTime.Now;
            Console.WriteLine();
            Console.WriteLine(test + ":");
            Console.WriteLine("耗时: " + (dt2 - dt).TotalMilliseconds);
            Console.WriteLine("读冲突次数: " + collisions);
            Console.WriteLine("结果: " + num);
            Console.WriteLine("预期结果: " + iterations * incrementTimes * write_times);
            return tasks;
        }

    }
}

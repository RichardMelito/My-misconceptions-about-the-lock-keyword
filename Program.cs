using System;
using System.Threading;

namespace My_misconceptions_about_the_lock_keyword
{
class InnerClass
{
    public int FieldThatAllTheThreadsWant = 0;
}

class OuterClass
{
    public InnerClass Inner = new InnerClass();

    public void ChangeFieldAndWriteToTheConsole()
    {
        for (int i = 0; i < 20; ++i)
        {
            ++Inner.FieldThatAllTheThreadsWant;
            Console.WriteLine("{0}; field = {1}", 
                Thread.CurrentThread.Name, 
                Inner.FieldThatAllTheThreadsWant);

            Thread.Sleep(1);
        }
    }
}

    class Program
    {
        static OuterClass Outer = new OuterClass();

        static void LockOnOuter()
        {
            lock (Outer)
            {
                Outer.ChangeFieldAndWriteToTheConsole();
            }
        }

        static void LockOnInner()
        {
            lock (Outer.Inner)
            {
                Outer.ChangeFieldAndWriteToTheConsole();
            }
        }

        static void DontLock()
        {
            Outer.ChangeFieldAndWriteToTheConsole();
        }

        static void WrongWay()
        {
            var t1 = new Thread(LockOnOuter)
            {
                Name = "T1"
            };

            var t2 = new Thread(LockOnInner)
            {
                Name = "T2"
            };

            var t3 = new Thread(DontLock)
            {
                Name = "T3"
            };

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();

            Console.WriteLine("Final value: {0}", Outer.Inner.FieldThatAllTheThreadsWant);
        }

        static void RightWayOuter()
        {
            var t1 = new Thread(LockOnOuter)
            {
                Name = "T1"
            };

            var t2 = new Thread(LockOnOuter)
            {
                Name = "T2"
            };

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
            Console.WriteLine("Final value: {0}", Outer.Inner.FieldThatAllTheThreadsWant);
        }

        static void RightWayInner()
        {
            var t1 = new Thread(LockOnInner)
            {
                Name = "T1"
            };

            var t2 = new Thread(LockOnInner)
            {
                Name = "T2"
            };

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Console.WriteLine("Final value: {0}", Outer.Inner.FieldThatAllTheThreadsWant);
        }

        static void WrongWayChangingVariable()
        {
            var t1 = new Thread(LockOnOuter)
            {
                Name = "T1"
            };

            var t2 = new Thread(LockOnOuter)
            {
                Name = "T2"
            };

            t1.Start();

            // sleep to ensure that t1 has actually started before reassigning
            Thread.Sleep(10);
            Outer = new OuterClass();

            t2.Start();

            t1.Join();
            t2.Join();

            Console.WriteLine("Final value: {0}", Outer.Inner.FieldThatAllTheThreadsWant);
        }

        static void Main(string[] args)
        {
            //WrongWay();
            //RightWayOuter();
            //RightWayInner();
            WrongWayChangingVariable();

            Console.ReadKey();
        }
    }
}

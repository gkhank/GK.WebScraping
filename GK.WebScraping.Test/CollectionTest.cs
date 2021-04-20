using GK.WebScraping.Model.Code.Collections.GK.WebScraping.Model.Collection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace GK.WebScraping.Test
{
    [TestClass]
    public class CollectionTest
    {
        [TestMethod]
        public void PriorityQueueOrderTest()
        {
            PriorityQueue<Int32> queue = new PriorityQueue<int>(new TestComparer());
            for (int i = 1000; i <= 3000; i++)
            {
                queue.Enqueue(i);
            }


            for (int i = 1000; i <= 3000; i++)
            {
                var x = queue.Dequeue();
                Debug.WriteLine("Processing item : " + x);
                Assert.AreEqual(i, x);
            }

        }

        [TestMethod]
        public void PriorityQueueRandomInsertTest()
        {
            PriorityQueue<Int32> queue = new PriorityQueue<int>(new TestComparer());

            SortedList<Int32, Int32> sorted = new SortedList<int, int>();
            Random rnd = new Random();
            for (int i = 1000; i <= 3000; i++)
            {
                var x = rnd.Next(1000, i);
                sorted.Add(i, x);
                queue.Enqueue(x);
            }

            Assert.AreEqual(queue.Count, sorted.Count);


            for (int i = 1000; i <= 3000; i++)
            {
                var expected = sorted[i];
                var actual = queue.Dequeue();

                //Pause so you have enough time to debug...
                Boolean result = expected == actual;
                if (!result)
                    Thread.Sleep(10000);

                Assert.AreEqual(expected, actual);
            }

        }
    }


    class TestComparer : IComparer<Int32>
    {
        public Int32 Compare(Int32 x, Int32 y)
        {
            if (x == y)
                return 0;
            else if (x > y)
                return -1;
            else
                return 1;
        }
    }
}

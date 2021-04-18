using GK.WebScraping.Model.Code.Collections.GK.WebScraping.Model.Collection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GK.WebScraping.Test
{
    [TestClass]
    public class CollectionTest
    {
        [TestMethod]
        public void PriorityQueueTest()
        {
            PriorityQueue<Int32> queue = new PriorityQueue<int>(new TestComparer());

            queue.Enqueue(5);
            queue.Enqueue(3);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(4);


            while (queue.Count > 0)
                Debug.WriteLine(queue.Dequeue());

        }
    }


    class TestComparer : IComparer<Int32>
    {
        public Int32 Compare(Int32 x, Int32 y)
        {
            if (x == y)
                return 0;
            else if (x > y)
                return 1;
            else
                return -1;
        }
    }
}

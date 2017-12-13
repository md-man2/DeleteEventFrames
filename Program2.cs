using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.EventFrame;

namespace UnCheckOut_AF
{
    class Program
    {
        static void Main()
        {
            PISystems myPISystems = new PISystems();
            PISystem myPISystem = myPISystems.DefaultPISystem;

            Console.WriteLine("Please Enter the name of the AF Database");
            string resp = Console.ReadLine();
            AFDatabase myAFDB = myPISystem.Databases[resp];

            //Console.WriteLine("Please Enter a search start time");
            //string starttime = Console.ReadLine();

            //Console.WriteLine("Please Enter a search end time");
            //string endtime = Console.ReadLine();

            bool MaxedEF;
            int loopcount = 0;
            Stopwatch stopWatch = new Stopwatch();
            
            do
            {
                ++loopcount;
                Console.WriteLine("current loops count: {0}. Last iteration in ms: {1}", loopcount, stopWatch.ElapsedMilliseconds);
                //Console.WriteLine("current loops count: {0}.", loopcount);
                stopWatch.Reset();
                stopWatch.Start();
                OSIsoft.AF.AFNamedCollectionList<AFEventFrame> EventFrameList = AFEventFrame.FindEventFrames(myAFDB, null,
                    null, AFSearchField.Description, true, AFSortField.StartTime, AFSortOrder.Ascending, 1000, 1000);

                if (EventFrameList.Count == 1000)
                {
                    MaxedEF = true;
                }
                else
                {
                    MaxedEF = false;
                }

                Parallel.ForEach(EventFrameList, EventFrame =>
                {
                    // Try Deleting the Event Frame
                    try
                    {
                        EventFrame.Delete();
                        EventFrame.CheckIn();
                        Console.WriteLine("Sucessfully Deleted Event Frame: {0}", EventFrame.Name);
                    }
                    catch
                    {
                        Console.WriteLine("Error, unable to Delete Event Frame: {0}", EventFrame.Name);
                    }
                });
                stopWatch.Stop();
            }
            while (MaxedEF == true);

            Console.WriteLine("Press Enter to Exit");
            Console.ReadKey();
        }
    }
}

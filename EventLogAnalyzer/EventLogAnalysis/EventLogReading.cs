//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Diagnostics.Eventing.Reader;
//using System.Linq;
//using System.Threading.Tasks;
// Maybe need to come back to this approach
// https://stackoverflow.com/questions/31509513/how-to-create-an-eventbookmark-when-querying-the-event-log
//namespace EventLogAnalysis
//{
//    public class EventLogReading
//    {
//        private static volatile bool myHasStoppedReading = false;

//        // adapted from https://stackoverflow.com/questions/53430475/reading-windows-logs-efficiently-and-fast
//        public static ConcurrentQueue<ELRecord> ParseEventsParallel(EventLogQuery query)
//        {
//            var sw = Stopwatch.StartNew();

//            const int BatchSize = 100;

//            ConcurrentQueue<EventRecord> events = new ConcurrentQueue<EventRecord>();
//            var readerTask = Task.Factory.StartNew(() =>
//            {
//                using (EventLogReader reader = new EventLogReader(query))
//                {
//                    EventRecord ev;
//                    int count = 0;
//                    while ((ev = reader.ReadEvent()) != null)
//                    {
//                        if (count % BatchSize == 0)
//                        {
//                            events.Enqueue(ev);
//                        }
//                        count++;
//                    }
//                }
//                myHasStoppedReading = true;
//            });

//            ConcurrentQueue<ELRecord> eventsWithStrings = new();

//            Action conversion = () =>
//            {
//                EventRecord ev = null;
//                using (var reader = new EventLogReader(query))
//                {
//                    while (!myHasStoppedReading || events.TryDequeue(out ev))
//                    {
//                        if (ev != null)
//                        {
//                            reader.Seek(ev.Bookmark);
//                            for (int i = 0; i < BatchSize; i++)
//                            {
//                                ev = reader.ReadEvent();
//                                if (ev == null)
//                                {
//                                    break;
//                                }
//                                eventsWithStrings.Enqueue(new ELRecord(ev));
//                            }
//                        }
//                    }
//                }
//            };

//            Parallel.Invoke(Enumerable.Repeat(conversion, 8).ToArray());

//            sw.Stop();
//            Debug.WriteLine($"Got {eventsWithStrings.Count} events with strings in {sw.Elapsed.TotalMilliseconds:N3}ms");

//            return eventsWithStrings;
//        }

//        public static List<ELRecord> ParseEvents(EventLogQuery query)
//        {
//            var sw = Stopwatch.StartNew();
//            List<ELRecord> parsedEvents = new();

//            using (EventLogReader reader = new EventLogReader(query))
//            {
//                EventRecord ev;
//                while ((ev = reader.ReadEvent()) != null)
//                {
//                    parsedEvents.Add(new ELRecord(ev));
//                }
//            }

//            sw.Stop();
//            Console.WriteLine($"Got {parsedEvents.Count} events with strings in {sw.Elapsed.TotalMilliseconds:N3}ms");

//            return parsedEvents;
//        }
//    }
//}
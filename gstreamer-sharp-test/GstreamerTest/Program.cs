﻿using System;
using Gst;


namespace GstreamerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Pipeline pipeline;
            Element source, sink;
            Bus bus;
            Message msg;
            StateChangeReturn ret;

            Application.Init(ref args);
            source = ElementFactory.Make("videotestsrc", "source");
            sink = ElementFactory.Make("autovideosink", "sink");

            pipeline = new Pipeline("test-pipeline");

            if (pipeline == null || source == null || sink == null)
            {
                Console.WriteLine("Not all elements could be created11");
                return;
            }
            pipeline.Add(source, sink);
            if (!source.Link(sink))
            {
                Console.WriteLine("Elements could not be linked");
                return;
            }
            source["pattern"] = 18;
            ret = pipeline.SetState(State.Playing);
            if (ret == StateChangeReturn.Failure)
            {
                Console.WriteLine("Unable to set the pipeline to the playing state.");
                return;
            }

            bus = pipeline.Bus;
            msg = bus.TimedPopFiltered(Constants.CLOCK_TIME_NONE, MessageType.Error | MessageType.Eos);

            if (msg != null)
            {
                switch (msg.Type)
                {
                    case MessageType.Error:
                        msg.ParseError(out GLib.GException err, out string debug);
                        Console.WriteLine($"Error received from element {msg.Src.Name}: {err.Message}");
                        Console.WriteLine("Debugging information {0}", debug ?? "(none)");
                        break;
                    case MessageType.Eos:
                        Console.WriteLine("End of stream reached");
                        break;
                    default:
                        Console.WriteLine("Unexpected message received");
                        break;
                }
            }
            pipeline.SetState(State.Null);
        }
    }
}

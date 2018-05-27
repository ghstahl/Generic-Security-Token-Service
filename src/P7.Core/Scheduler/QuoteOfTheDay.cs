using System;
using System.Collections.Generic;
using System.Text;

namespace P7.Core.Scheduler
{
    public class QuoteOfTheDay
    {
        public static QuoteOfTheDay Current { get; set; }

        static QuoteOfTheDay()
        {
            Current = new QuoteOfTheDay { Quote = "No quote", Author = "Maarten" };
        }

        public string Quote { get; set; }
        public string Author { get; set; }
    }

}

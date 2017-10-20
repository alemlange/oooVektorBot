using System.Collections.Generic;

namespace Bot.CommandParser
{
    public class SessionParser
    {
        protected List<string> Restaurants { get; set; }

        protected int TablesCount { get; set; }

        public SessionParser(List<string> restaurantNames, int tablesCount)
        {
            Restaurants = restaurantNames;
            TablesCount = tablesCount;
        }
    }
}
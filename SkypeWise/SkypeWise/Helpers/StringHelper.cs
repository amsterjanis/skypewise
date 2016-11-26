using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;

namespace SkypeWise.Helpers
{
    public static class StringHelper
    {
        public static Activity toReply(this string message)
        {
            return new Activity().CreateReply(message);
        }
    }
}
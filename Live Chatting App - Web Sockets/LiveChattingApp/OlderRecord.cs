using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveChattingApp
{
    /// <summary>
    /// Class used to make a list of object of storing all past msgs
    /// </summary>
    public class OlderRecord
    {
        /// <summary>
        /// Constructor setting up values
        /// </summary>
        /// <param name="username">user;s provided username</param>
        /// <param name="time">time of msg send action</param>
        /// <param name="msg">message</param>
        public OlderRecord(string username, DateTimeOffset time, string msg)
        {
            UserName = username;
            Time = time;
            AssociatedMessage = msg;
        }
        public string UserName { get; set; }
        public DateTimeOffset Time { get; set; }
        public string AssociatedMessage { get; set; }
    }
}

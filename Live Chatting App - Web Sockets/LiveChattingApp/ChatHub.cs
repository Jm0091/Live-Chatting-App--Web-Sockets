using LiveChattingApp.Data;
using LiveChattingApp.Model;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// I Jems Chaudhary, certify that this material is my original work.
/// </summary>
namespace LiveChattingApp
{
    public class ChatHub:Hub
    {
        /// <summary>
        /// Storing context of database
        /// </summary>
        private readonly ChatDBContext _dbContext;
        
        /// <summary>
        /// Storing value of username used in disconnecting tasks
        /// </summary>
        private string username;
        
        /// <summary>
        /// Storing error msg into class variable (mostly used in else statements)
        /// </summary>
        private string ERROR_MSG="";
        
        /// <summary>
        /// List of users having key-connection id and value-username (Used to check user is already in or not)
        /// </summary>
        private static ConcurrentDictionary<string, string> usersList = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Constructor initiallizing ChatDbContext
        /// </summary>
        /// <param name="dbContext">context for the chathub</param>
        public ChatHub(ChatDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Sending latest msg to all clients
        ///     - username conditions: checking the username is unique, not numbers in input, length smaller than 10
        ///     - password conditions: checking length smaller than 140
        /// </summary>
        /// <param name="userName">username</param>
        /// <param name="textMsg">message send by user</param>
        /// <returns>Task sending msg to all client</returns>
        public async Task SendAllMsgsAsync(string userName, string textMsg)
        {
            // If user is not connected in room - Checking conditions, Sending appropriate msgs
            if (!usersList.ContainsKey(Context.ConnectionId))       
            {
                if (CheckUsername(userName))
                {
                    await OlderMsgsAsync();                                         //printing all older msgs
                    await FirstPart_UserAsync(userName);
                    await SecondPart_MessageAsync(userName, textMsg);               
                }
                else
                {
                    await Clients.All.SendAsync("ReceiveMessage", "ERROR", DateTimeOffset.Now, ERROR_MSG);
                }
            }
            else        //if user is already in connection, just processing msg part
            {
                await SecondPart_MessageAsync(userName, textMsg);
            }
        }

        /// <summary>
        /// Method fetching all info frm database and  storing into List of OlderRecord object
        ///         - sorting based on time.
        /// </summary>
        /// <returns>Task sending older msgs to all client</returns>
        public async Task OlderMsgsAsync()
        {
            var timeList = from c in _dbContext.ConnectivityTime
                            orderby c.ConnectingTime
                           select new { c.ConnectingTime, c.DisconnectingTime, c.UserName};

            var msgList = from m in _dbContext.Message
                          orderby m.MsgTime
                           select new { m.UserName, m.MsgTime, m.Msg};
            
            //for storing all records in List of object type OlderRecord using foreach loops
            List<OlderRecord> olderRecords = new List<OlderRecord>();
            
            foreach (var item in timeList)
                olderRecords.Add(new OlderRecord(item.UserName, item.ConnectingTime, "Connected!"));
            
            foreach (var item in timeList)
                olderRecords.Add(new OlderRecord(item.UserName, item.DisconnectingTime, "Disconnected!"));
            
            foreach (var item in msgList)
                olderRecords.Add(new OlderRecord(item.UserName, item.MsgTime, item.Msg));
            
            olderRecords.Sort( (o1, o2) => o1.Time.CompareTo(o2.Time));

            await Clients.All.SendAsync("PrintOlderMessage", olderRecords);
        }

        /// <summary>
        /// First part - adding user to userList and in db, and sending connection msg back.
        /// </summary>
        /// <param name="userName">username</param>
        /// <returns>Task sending connecting msg to all client</returns>
        public async Task FirstPart_UserAsync(string userName)
        {
            //Adding User to users table
            if (!usersList.ContainsKey(Context.ConnectionId))  //use this for user thing - checking
            {
                usersList.TryAdd(Context.ConnectionId, userName);
                var user = new UserProfile
                {
                    UserId = new Guid(),
                    UserName = userName,
                    UserCreationTime = DateTimeOffset.Now
                };
                _dbContext.UserProfile.Add(user);
                await _dbContext.SaveChangesAsync();
                await Clients.Caller.SendAsync("ReceiveMessage", "Chat Hub", user.UserCreationTime, $" --> User: {user.UserName} Connected!");
            }
        }
        

        /// <summary>
        /// SecondPart of sending Messages to all users after checking msg condition
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="textMsg">msg sent by user</param>
        /// <returns>Task sending msg to all client</returns>
        public async Task SecondPart_MessageAsync(string userName, string textMsg)
        {
            if (CheckMessage(textMsg))
            {
                //storing value to Table - 'Message'
                var message = new Message
                {
                    MsgId = new Guid(),
                    UserName = userName,
                    Msg = textMsg,
                    MsgTime = DateTimeOffset.Now
                };
                _dbContext.Message.Add(message);
                await _dbContext.SaveChangesAsync();
                
                username = userName;            // updating username class variable - used in disonnecting time logging

                await Clients.All.SendAsync("ReceiveMessage", message.UserName, message.MsgTime, message.Msg);
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", "ERROR", DateTimeOffset.Now, ERROR_MSG);
            }
        }
        
        /// <summary>
        /// Checking username's conditions 
        /// </summary>
        /// <param name="userName">username</param>
        /// <returns>true if username is valid</returns>
        public bool CheckUsername(string userName)
        {
            string checkMe = userName;
            if (userName == null || userName.Length > 10)
            {
                ERROR_MSG = "Invalid Username - Can only be 1-10 letters!";
                return false;
            }
            else if (checkMe.Any(char.IsDigit))
            {
                ERROR_MSG = "Invalid Username - Numbers can not be used!";
                return false;
            }
            else if (UsernameInDb(userName))            //checking username is valid or not
            {
                ERROR_MSG = "Invalid Username - User with provided username is already exists!";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checking username is already in db or not
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>true of user is in db</returns>
        public bool UsernameInDb(string username)
        {
            var user = from u in _dbContext.UserProfile
                       where u.UserName == username
                       select u.UserName;
            
            string dbResult = user.ToList().FirstOrDefault();
            if (dbResult != null && dbResult.Equals(username))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// CHecking message condition
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckMessage(string msg)
        {
            if (msg == null || msg.Length > 140)
            {
                ERROR_MSG = "Invalid Message - Can only be 1-140 letters long!";
                return false;
            }
            return true;
        }

        /// <summary>
        /// overriden method displays Welcome msg
        /// </summary>
        /// <returns>Task sending Welcome msg to user connecting</returns>
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "Chat Hub", DateTimeOffset.Now, "Welcome!");
            await base.OnConnectedAsync();
        }
        /// <summary>
        /// Storing value of user's connecting and disconnecting time
        /// </summary>
        /// <param name="ex">exception</param>
        /// <returns>Task sending Welcome msg to user connecting</returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            DateTimeOffset disconnecting_TIME = DateTimeOffset.Now;
            if (usersList.ContainsKey(Context.ConnectionId))        //double checking user is in the list or not
            {
                string value;
                bool valueFound = usersList.TryGetValue(Context.ConnectionId, out value);
                if (valueFound)
                {
                    var db_connectionTime = from c in _dbContext.UserProfile
                                        where c.UserName.Equals(value)
                                        select c.UserCreationTime;

                    DateTimeOffset connectionTime = db_connectionTime.ToList().FirstOrDefault();
                    username = value;
                    //Adding Times to table - 'ConnectivityTime'
                    var timer = new ConnectivityTime
                    {
                        TimeId = new Guid(),
                        ConnectingTime = connectionTime,
                        UserName = username,
                        DisconnectingTime = disconnecting_TIME
                    };
                    _dbContext.ConnectivityTime.Add(timer);
                    await _dbContext.SaveChangesAsync();
                }
                else {
                    username = "User";
                }
                usersList.TryRemove(Context.ConnectionId, out username);
            }
            else {
                username = "User";
            }
            await Clients.All.SendAsync("ReceiveMessage", "Chat Hub", DateTimeOffset.UtcNow, $"{username} Disconnected!");
            await base.OnDisconnectedAsync(ex);
        }
    }
}

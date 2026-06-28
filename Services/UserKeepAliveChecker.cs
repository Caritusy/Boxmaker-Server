using protocol.game;

namespace BoxMaker_Server
{
    public class UserKeepAliveChecker
    {
        private static Dictionary<string,smsg_login> User = new Dictionary<string,smsg_login>();

        public static void AddNewConnection(string ip,int port,smsg_login userLoginData)
        {
            if (User.ContainsKey(ip + port.ToString())) 
            {
                User.Remove(ip + port.ToString());
            }
            User.Add(ip + port.ToString(), userLoginData);
        }

        public bool TryGetUser(string ip, int port,out smsg_login user)
        {
            return User.TryGetValue(ip + port.ToString(), out user);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoBuf;
using protocol.game;
using System.Collections;
using System.Net;
using System.Text;

namespace BoxMaker_Server
{
    public partial class BoxmakerProxy : Controller
    {
        public static BoxmakerProxy Instance { get; private set; }

        public BoxmakerProxy()
        {
            if (Instance == null) Instance = this;
        }

        private static Dictionary<int,string> KeepAliveDict = new Dictionary<int,string>();

        public static int GetOnlinePlayerCount()
        {
            return KeepAliveDict.Count;
        }

        public static string ServerVerD = "1.121";

        public bool UserTokenVerify(int userid,string sig)
        {
            string token = "";
            if (!KeepAliveDict.TryGetValue(userid, out token))
            {
                Console.WriteLine($"[red][服务端] 由于重启/客户端token已过期太久,无法找到UID:{userid}的token，下发过期提示。");
                return false;
            }
            if (token != sig)
            {
                Console.WriteLine($"[red][服务端] {userid}的Token{sig}已过期，下发过期提示。");
                return false;
            }
            return true;

        }

    }
}

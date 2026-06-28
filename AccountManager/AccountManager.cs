using Newtonsoft.Json;
using protocol.game;
using protocol.map;
using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace BoxMaker_Server
{
    public partial class AccountManager
    {
        private const string AccountPathSeparator = "&&&&&&&&&&&&&&&&&&&&";
        private const int ServerMapIdBase = 10010000;

        private static readonly string path = AppDomain.CurrentDomain.BaseDirectory;

        private static readonly string AccDataPath = Path.Combine(path, "AccountData");

        private static readonly string MapDataPath = Path.Combine(path, "MapData");


        public static readonly object MapFileLock = new object();

        public static List<ServerMap> serverMaps = new List<ServerMap>();

        private static readonly ConcurrentDictionary<string, int> PlayingMap = new ConcurrentDictionary<string, int>();


        private static string UserInfoPath(string userPath)
        {
            return Path.Combine(userPath, "userinfo");
        }

        private static string UserMapPath(string userPath)
        {
            return Path.Combine(userPath, "map");
        }

        private static string UserMapDataPath(string userPath)
        {
            return Path.Combine(userPath, "mapdata");
        }

        private static string UserClearListPath(string userPath)
        {
            return Path.Combine(userPath, "clearlist");
        }

        private static string UserMissionPath(string userPath)
        {
            return Path.Combine(userPath, "missiondata");
        }

        private static string UserRecentPlayPath(string userPath)
        {
            return Path.Combine(userPath, "recentplay");
        }

        private static string UserFavoritePath(string userPath)
        {
            return Path.Combine(userPath, "favorite");
        }

        private static string UserPlayerStatePath(string userPath)
        {
            return Path.Combine(userPath, "playerstate");
        }

        private static string ServerMapPath(int mapId)
        {
            return Path.Combine(MapDataPath, mapId.ToString());
        }

        private static string AccountDirectoryPath(int userid, string openid)
        {
            return Path.Combine(AccDataPath, $"{userid}{AccountPathSeparator}{openid}");
        }

        private static void FillMissionPlayData(smsg_mission_play retDat, ServerMap map)
        {
            retDat.map_data = map.mapData.data;
            retDat.map_name = map.map.name;
            retDat.user_name = map.map.owner_name;
            retDat.user_head = map.map.head;
            retDat.user_country = map.map.country;
            retDat.y = map.FailureY;
            retDat.x = map.FailureX;
        }

        public static bool IsValidString(string input)
        {
            // 检查字符串是否只包含 ASCII 字符
            if (!IsASCII(input))
            {
                return false;
            }

            // 检查字符串是否包含 Windows 不可用的文件名字符
            // Windows 不可用的文件名字符包括 \ / : * ? " < > |
            return !Regex.IsMatch(input, "[\\\\/:*?\"<>|]");
        }

        static bool IsASCII(string input)
        {
            foreach (char c in input)
            {
                if (c > 127)
                {
                    return false;
                }
            }
            return true;
        }

        public static void SetPlayingMap(string sig, int mapid)
        {
            PlayingMap[sig] = mapid;
        }

        public static int GetPlayingMap(string sig)
        {
            return PlayingMap.TryGetValue(sig, out int id) ? id : 0;
        }

        public static int GetAccounts()
        {
            EnsureAccountPathCache();
            lock (AccountCacheLock)
            {
                return AccountPathByUserId.Count;
            }
        }

        public static void Init()
        {
            if (!Directory.Exists(AccDataPath))
            {
                Directory.CreateDirectory(AccDataPath);
            }
            if (!Directory.Exists(MapDataPath))
            {
                Directory.CreateDirectory(MapDataPath);
            }
        }
    }
}

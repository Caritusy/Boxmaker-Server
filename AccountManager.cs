using Newtonsoft.Json;
using protocol.game;
using protocol.map;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BoxMaker_Server
{
    public class AccountManager
    {
        private static string path = AppDomain.CurrentDomain.BaseDirectory;

        private static string AccDataPath = $"{path}/AccountData/";

        private static string MapDataPath = $"{path}/MapData/";

        public static byte[] DefaultMapArray = new byte[]
        {
            111, 23, 0, 0, 0, 0, 4, 0, 126, 0,
            0, 222, 123, 71, 108, 185, 44, 14, 83, 209,
            9, 65, 130, 24, 107, 245, 125, 17, 216, 94,
            81, 153, 244, 129, 136, 212, 125, 41, 177, 2,
            138, 50, 184, 246, 63, 215, 14, 255, 59, 67,
            236, 18, 224, 152, 31, 147, 143, 139, 54, 73,
            181, 138, 94, 246, 217, 130, 212, 234, 170, 113,
            187, 253, 200, 239, 56, 12, 58, 209, 252, 73,
            27, 252, 200, 241, 225, 50, 149, 55, 246, 192,
            232, 196, 209, 124, 191, 43, 156, 1, 77, 216,
            238, 229, 249, 55, 32, 8, 31, 80, 198, 58,
            116, 59, 84, 181, 80, 47, 221, 95, 202, 203,
            147, 84, 102, 253, 228, 39, 91, 136, 250, 24,
            189, 41, 248, 220, 89, 150, 39, 255, 234, 38,
            128, 0, 3, 91, 133, 22, 92, 53, 66, 18,
            98, 73, 41, 108, 213, 222, 7, 32, 196, 48,
            254, 35, 76, 241, 111, 158, 83, 160, 188, 25,
            214, 216, 200, 178, 54, 88, 150, 137, 20, 59,
            108, 21, 143, 168, 86, 168, 157, 50, 195, 170,
            228, 221, 6, 107, 26, 181, 7, 135, 96, 63,
            155, 138, 0, 58, 209, 163, 183, 88, 225, 201,
            134, 9, 46, 170, 21, 57, 69, 137, 165, 126,
            59, 202, 166, 25, 235, 232, 35, 68, 208, 95,
            255, 255, 7, 200, 0, 0
        };

        public static readonly object MapFileLock = new object();

        public static List<ServerMap> serverMaps = new List<ServerMap>();

        private static Dictionary<string, int> PlayingMap = new Dictionary<string, int>();

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
            if (PlayingMap.TryGetValue(sig, out _))
            {
                PlayingMap.Remove(sig);
            }
            PlayingMap.Add(sig, mapid);
        }

        public static int GetPlayingMap(string sig)
        {
            int id = 0;
            if (!PlayingMap.TryGetValue(sig, out id))
            {
                return 0;
            }
            return id;
        }

        public static int GetAccounts()
        {
            return System.IO.Directory.GetDirectories(AccDataPath).Length;
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
               
        public static bool MapCheck(string userp)
        {
            string MapPath = userp + "/" + "map";
            string MapDataPath = userp + "/" + "mapdata";
            if (!System.IO.File.Exists(MapPath))
            {
                using (System.IO.File.Create(MapPath)) { }
            }
            if (!System.IO.File.Exists(MapDataPath))
            {
                using (System.IO.File.Create(MapDataPath)) { }
            }
            if (File.ReadAllTextAsync(MapPath).Result.Length == 0)
            {
                var d = new List<edit_data>();
                for (int i = 0; i < 12; i++)
                {
                    d.Add(new edit_data { id = 0, name = "", date = "", upload = 0, url = null });
                }
                File.WriteAllTextAsync(MapPath,JsonConvert.SerializeObject(d));
            }
            if (File.ReadAllTextAsync(MapDataPath).Result.Length == 0)
            {
                var d = new List<MapDataHelper>();
                for (int i = 0; i < 12; i++)
                {
                    d.Add(new MapDataHelper { id = i});
                }
                File.WriteAllTextAsync(MapDataPath, JsonConvert.SerializeObject(d));
            }
            if (!System.IO.File.Exists(userp + "/clearlist"))
            {
                File.WriteAllTextAsync(userp + "/clearlist", JsonConvert.SerializeObject(new List<int>()));
            }
            return true;
        }
               
        public static bool TryGetEditList(int uid, out List<edit_data> retList)
        {
            string userp = "";
            if (!TryGetUserPath(uid, out userp))
            {
                retList = null;
                return false;
            }
            MapCheck(userp);
            string d = File.ReadAllTextAsync(userp + "/" + "map").Result;
            retList = JsonConvert.DeserializeObject<List<edit_data>>(d);
            return true;
        }
               
        public static bool TryGetEditInfoList(int uid, out List<MapDataHelper> retList)
        {
            string userp = "";
            if (!TryGetUserPath(uid, out userp))
            {
                retList = null;
                return false;
            }
            MapCheck(userp);
            string d = File.ReadAllTextAsync(userp + "/" + "mapdata").Result;
            retList = JsonConvert.DeserializeObject<List<MapDataHelper>>(d);
            return true;
        }
               
        public static bool TryCreateMap(int uid,int id, byte[] url,string date,out edit_data map)
        {
            edit_data mapD = new edit_data();
            string userp = "";
            if (!TryGetUserPath(uid,out userp))
            {
                map = mapD;
                return false; 
            }
            string MapPath = userp + "/" + "map";

            mapD.id = id;
            mapD.name = "empty";
            mapD.date = date;
            mapD.url = url;
            mapD.upload = 0;
            List<edit_data> list;
            if (!TryGetEditList(uid, out list))
            {
                map = mapD;
                return false;
            }
            if (list.Any(o => o.id == id))
            {
                map = mapD;
                return false;
            }
            list[id - 1] = mapD;
            File.WriteAllTextAsync(MapPath,JsonConvert.SerializeObject(list));
            map = mapD;
            return true;
        }
               
        public static bool TryGetUserPath(int uid, out string path)
        {
            string[] d = Directory.GetDirectories(AccDataPath);
            foreach (string p in d)
            {
                string pathE = Path.GetFileName(p);
                string[] a = pathE.Split("&&&&&&&&&&&&&&&&&&&&");
                if (a[0] == uid.ToString())
                {
                    path = p;
                    return true;
                }
            }
            path = "";
            return false;
        }
               
        public static bool TryGetUserPath(string userName, out string path)
        {
            string[] d = Directory.GetDirectories(AccDataPath);
            foreach (string p in d)
            {
                string pathE = Path.GetFileName(p);
                string[] a = pathE.Split("&&&&&&&&&&&&&&&&&&&&");
                if (a[1] == userName)
                {
                    path = p;
                    return true;
                }
            }
            path = "";
            return false;
        }
               
        public static bool TryGetAccount(int uid, out smsg_login loginData)
        {
            Init();
            string p = "";
            if (!TryGetUserPath(uid, out p))
            {
                loginData = null;
                return false;
            }
            loginData = JsonConvert.DeserializeObject<smsg_login>(File.ReadAllTextAsync(p + "/userinfo").Result);
            return true;

        }
               
        public static bool TryGetAccount(string openid, out smsg_login loginData)
        {
            Init();
            string p = "";
            if (!TryGetUserPath(openid, out p))
            {
                loginData = null;
                return false;
            }
            loginData = JsonConvert.DeserializeObject<smsg_login>(File.ReadAllTextAsync(p + "/userinfo").Result);
            return true;

        }
               
        public static bool TrySaveAccount(string openid, smsg_login loginData)
        {
            Init();
            string p = "";
            if (!TryGetUserPath(openid, out p))
            {
                return false;
            }
            //loginData = JsonConvert.DeserializeObject<smsg_login>(File.ReadAllText(p + "/userinfo"));
            File.WriteAllTextAsync(p + "/userinfo",JsonConvert.SerializeObject(loginData));
            return true;
        }
               
        public static bool TrySaveAccount(int userid, smsg_login loginData)
        {
            Init();
            try
            {
                string p = "";
                if (!TryGetUserPath(userid, out p))
                {
                    return false;
                }
                //loginData = JsonConvert.DeserializeObject<smsg_login>(File.ReadAllText(p + "/userinfo"));
                File.WriteAllTextAsync(p + "/userinfo", JsonConvert.SerializeObject(loginData));
                return true;
            }
            catch { return false; }
        }
               
        public static smsg_login NewGuest()
        {
            Init();
            int userid = Directory.GetDirectories(AccDataPath).Length;
            var accD = new smsg_login()
            {
                userid = userid,
                name = "游客" + userid.ToString(),
                openid = "guest" + userid.ToString(),
                openkey = "empty",
                level = 1,
                exp = 0,
                head = 0,
                nationality = "--",
                visitor = 1,
            };
            Directory.CreateDirectory(AccDataPath + "/" + accD.userid + "&&&&&&&&&&&&&&&&&&&&" + accD.openid);
            File.WriteAllTextAsync(AccDataPath + "/" + accD.userid + "&&&&&&&&&&&&&&&&&&&&" + accD.openid + "/userinfo", JsonConvert.SerializeObject(accD));
            return accD;
        }
               
        public static smsg_login RegisterAccount(int userid, cmsg_register regData)
        {
            Init();
            string userp = "";
            if (!TryGetUserPath(userid, out userp))
            {
                return null;
            }
            var loginData = JsonConvert.DeserializeObject<smsg_login>(File.ReadAllTextAsync(userp + "/userinfo").Result);
            string newDire = AccDataPath + userid.ToString() + "&&&&&&&&&&&&&&&&&&&&" + regData.openid;
            if (!Directory.Exists(newDire))
            {
                Directory.CreateDirectory(newDire);
            }
            foreach (string d in Directory.GetFiles(userp))
            {
                string FileName = Path.GetFileName(d);
                System.IO.File.Copy(d, newDire + "/" + FileName);
            }
            if (Directory.GetFiles(userp).Length != 0)
            {
                foreach (var d in Directory.GetFiles(userp))
                {
                    System.IO.File.Delete(d);
                }
            }
            Directory.Delete(userp);
            loginData.name = regData.nickname;
            loginData.openid = regData.openid;
            loginData.openkey = regData.openkey;
            loginData.nationality = regData.nationality;
            loginData.head = regData.head;
            loginData.visitor = 0;
            File.WriteAllTextAsync(newDire + "/userinfo", JsonConvert.SerializeObject(loginData));
            return loginData;
        }
               
        public static bool CompleteEditGuide(int userid, cmsg_complete_guide data)
        {
            Init();
            string userp = "";
            if (!TryGetUserPath(userid, out userp))
            {
                return false;
            }
            MapCheck(userp);
            smsg_login userD;
            if (!TryGetAccount(userid, out userD))
            {
                return false;
            }
            userD.guide = 666;
            if (!TrySaveAccount(userid, userD))
            {
                return false;
            }
            if (!TryCreateMap(userid, 1, data.url, DateTime.Now.ToString("yyyy/MM/dd HH:mm"),out _))
            {
                return false;
            }
            if (!TrySaveMap(userid, new cmsg_save_map() { id = 1, url = data.url, mapdata = data.data }))
            {
                return false;
            }
            return true;
        }
               
        public static bool TrySaveMap(int userid,cmsg_save_map saveData)
        {
            Init();
            string p = "";
            if (!TryGetUserPath(userid, out p))
            {
                return false;
            }
            MapCheck(p);
            List<edit_data> EditD;
            if (!TryGetEditList(userid, out EditD))
            {
                return false;
            }
            List<MapDataHelper> InfoD;
            if (!TryGetEditInfoList(userid, out InfoD))
            {
                return false;
            }
            EditD[saveData.id-1].url = saveData.url;
            InfoD[saveData.id-1].data = saveData.mapdata;
            File.WriteAllTextAsync(p + "/" + "map",JsonConvert.SerializeObject(EditD));
            File.WriteAllTextAsync(p + "/" + "mapdata", JsonConvert.SerializeObject(InfoD));
            return true;
        }
               
        public static bool TryChangeMapName(int userid, cmsg_change_map_name pckData)
        {
            Init();
            string userp = "";
            if (!TryGetUserPath(userid, out userp))
            {
                return false;
            }
            MapCheck(userp);
            List<edit_data> EditD;
            if (!TryGetEditList(userid, out EditD))
            {
                return false;
            }
            EditD[pckData.id - 1].name = pckData.name;
            File.WriteAllTextAsync(userp + "/" + "map", JsonConvert.SerializeObject(EditD));
            return true;
        }
               
        public static bool TryDeleteMap(int userid, cmsg_delete_map pckData)
        {
            Init();
            string userp = "";
            if (!TryGetUserPath(userid, out userp))
            {
                return false;
            }
            MapCheck(userp);
            List<edit_data> EditD;
            if (!TryGetEditList(userid, out EditD))
            {
                return false;
            }
            List<MapDataHelper> InfoD;
            if (!TryGetEditInfoList(userid, out InfoD))
            {
                return false;
            }
            EditD[pckData.id - 1] = new edit_data() { url = new byte[0] };
            InfoD[pckData.id - 1].data = null;
            File.WriteAllTextAsync(userp + "/" + "map", JsonConvert.SerializeObject(EditD));
            File.WriteAllTextAsync(userp + "/" + "mapdata", JsonConvert.SerializeObject(InfoD));
            return true;
        }
               
        public static bool TryUploadMap(int userid, cmsg_upload_map pckData)
        {
            Init();
            string userp = "";
            if (!TryGetUserPath(userid, out userp))
            {
                return false;
            }
            smsg_login userD;
            if (!TryGetAccount(userid, out userD))
            {
                return false;
            }
            List<edit_data> EditD;
            if (!TryGetEditList(userid, out EditD))
            {
                return false;
            }
            List<MapDataHelper> InfoD;
            if (!TryGetEditInfoList(userid, out InfoD))
            {
                return false;
            }
            edit_data currentUpload = EditD[pckData.id - 1];
            ServerMap map = new ServerMap();
            map.map = new map_info()
            {
                id = 10010000 + serverMaps.Count + 1,
                head = userD.head,
                owner_name = userD.name,
                owner_id = userD.userid,
                country = userD.nationality,
                url = currentUpload.url,
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                name = currentUpload.name,
            };
            map.mapData = InfoD[pckData.id - 1];
            if (pckData.video != null)
            {
                map.recs.Add(new ServerMap.RECInfo() {recData = pckData.video,video_id = 0 });
                map.ranks.Add(new map_point_rank() 
                {
                    player_country = userD.nationality,
                    player_level = userD.level,
                    player_name = userD.name,
                    user_id = userD.userid,
                    player_point = pckData.time,
                    video_id = 0,
                    visitor = userD.visitor,
                });
            }
            serverMaps.Add(map);
            File.WriteAllTextAsync(MapDataPath + "/" + map.map.id.ToString(),JsonConvert.SerializeObject(map));
            currentUpload.upload = 1;
            File.WriteAllTextAsync(userp + "/" + "map", JsonConvert.SerializeObject(EditD));
            return true;
        }
               
        public static List<ServerMap> GetServerMapList()
        {
            Init();
            List<ServerMap> retList = new List<ServerMap> ();
            string[] maps = Directory.GetFiles(MapDataPath);
            foreach (string s in maps) 
            {
                ServerMap map = JsonConvert.DeserializeObject<ServerMap>(File.ReadAllTextAsync(s).Result);
                retList.Add(map);
            }
            return retList;
        }
               
        public static ServerMap GetMapInfo(int mapid)
        {
            if (System.IO.File.Exists(MapDataPath + "/" + mapid.ToString()))
            {
                return JsonConvert.DeserializeObject<ServerMap>(File.ReadAllTextAsync(MapDataPath + "/" + mapid.ToString()).Result);
            }
            return null;
        }
               
        public static int GetMapExp(int mapid)
        {
            ServerMap map = AccountManager.GetMapInfo(mapid);
            return Utils.get_map_exp(map.map.pas,map.map.amount);
        }
               
        public static bool TryComment(int mapid, cmsg_comment pckDat)
        {
            try
            {
                Init();
                var map = GetMapInfo(mapid);
                smsg_login userD;
                if (!TryGetAccount(pckDat.common.userid, out userD))
                {
                    return false;
                }
                comment cmt = new comment()
                {
                    country = userD.nationality,
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    head = userD.head,
                    name = userD.name,
                    text = pckDat.text,
                    userid = userD.userid,
                    visitor = userD.visitor,
                };
                map.comments.Add(cmt);
                File.WriteAllTextAsync(MapDataPath + "/" + mapid.ToString(), JsonConvert.SerializeObject(map));
                return true;
            }
            catch (Exception ex) 
            {
                return true;
            }
        }
               
        public static bool TryCompleteMap(cmsg_complete_map pckDat,out smsg_complete_map retDat)
        {
            int mapid = AccountManager.GetPlayingMap(pckDat.common.sig);
            ServerMap map = AccountManager.GetMapInfo(mapid);
            map.map.amount += 1;

            if (map.map.id == 10010001)
            {
                smsg_login acc;
                if (!TryGetAccount(pckDat.common.userid, out acc))
                {
                    
                }
                if (acc.guide == 200)
                {
                    acc.guide = 666;
                }
                if (acc.testify == 0)
                {
                    acc.testify = 4;
                }
                TrySaveAccount(acc.userid, acc);
            }

            if (pckDat.suc == 0)
            {
                smsg_login acc;
                if (!TryGetAccount(pckDat.common.userid, out acc))
                {
                    retDat = null;
                    return false;
                }
                string userP = "";
                if (!TryGetUserPath(pckDat.common.userid, out userP))
                {
                    retDat = null;
                    return false;
                }
                MapCheck(userP);
                bool removeLast = false;
                if (map.ranks.Where(x => x.user_id == pckDat.common.userid).Count() != 0)
                {
                    var ranklist = map.ranks.Where(x => x.user_id == pckDat.common.userid).ToList();
                    foreach (var rank in ranklist)
                    {
                        if (rank.player_point > pckDat.time) map.ranks.Remove(rank);
                        if (rank.player_point <= pckDat.time) removeLast = true;
                    }
                }

                map.ranks.Add(new map_point_rank()
                {
                    player_country = acc.nationality,
                    player_level = acc.level,
                    player_name = acc.name,
                    player_point = pckDat.time,
                    user_id = acc.userid,
                    video_id = int.Parse($"{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}{new Random().Next(0,1000)}") + DateTime.Now.Millisecond,
                    visitor = acc.visitor,
                });

                if (removeLast) map.ranks.Remove(map.ranks.Last());
                if (!removeLast) map.recs.Add(new ServerMap.RECInfo() { video_id = map.ranks.Last().video_id, recData = pckDat.video });

                map.ranks.Sort((x, y) => y.player_point.CompareTo(x.player_point));

                map.map.pas += 1;

                retDat = new smsg_complete_map()
                {
                    exp = GetMapExp(mapid),
                    mapid = mapid,
                    rank = map.ranks.Count - map.ranks.FindIndex(x => x.user_id == pckDat.common.userid),
                };

                if (mapid == 10010001) retDat.testify = 4;

                acc.exp += GetMapExp(mapid);
                TrySaveAccount(pckDat.common.userid,acc);

                CheckExp(pckDat.common.userid);

                File.WriteAllTextAsync(MapDataPath + "/" + mapid.ToString(), JsonConvert.SerializeObject(map));
                var ClearList = JsonConvert.DeserializeObject<List<int>>(File.ReadAllTextAsync(userP + "/clearlist").Result);
                if (!ClearList.Contains(mapid)) ClearList.Add(mapid);
                File.WriteAllTextAsync(userP + "/clearlist", JsonConvert.SerializeObject(ClearList));

            }
            else
            {
                map.FailureX.Add(pckDat.x);
                map.FailureY.Add(pckDat.y);
                retDat = new smsg_complete_map()
                {
                    mapid = mapid,
                };
                File.WriteAllTextAsync(MapDataPath + "/" + mapid.ToString(), JsonConvert.SerializeObject(map));
            }

            return true;
        }
               
        public static bool TryReplayMap(cmsg_replay_map pckDat)
        {
            int mapid = AccountManager.GetPlayingMap(pckDat.common.sig);
            ServerMap map = AccountManager.GetMapInfo(mapid);
            if (map == null)
            {
                return false;
            }
            map.map.amount += 1;
            File.WriteAllTextAsync(MapDataPath + "/" + mapid.ToString(), JsonConvert.SerializeObject(map));
            return true;
        }
               
        public static int TryDownloadMap(cmsg_download_map pckDat)
        {
            ServerMap map = GetMapInfo(pckDat.id);
            int userid = pckDat.common.userid;

            if (map == null)
            {
                return -2;
            }

            string p = "";
            if (!TryGetUserPath(userid, out p))
            {
                return -2;
            }

            List<edit_data> EditD;
            if (!TryGetEditList(userid, out EditD))
            {
                return -2;
            }

            List<MapDataHelper> InfoD;
            if (!TryGetEditInfoList(userid, out InfoD))
            {
                return -2;
            }

            var dat = EditD.Where(x => x.id == 0);

            if (dat.Count() == 0)
            {
                return -29;
            }
            
            int index = EditD.FindIndex(x => x.id == 0);
            EditD[index].id = index + 1;
            EditD[index].date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            EditD[index].name = map.map.name;
            EditD[index].url = map.map.url;
            InfoD[index].id = index + 1;
            InfoD[index].data = map.mapData.data;

            File.WriteAllTextAsync(p + "/" + "map", JsonConvert.SerializeObject(EditD));
            File.WriteAllTextAsync(p + "/" + "mapdata", JsonConvert.SerializeObject(InfoD));
            return 0;
        }
               
        public static bool TrySearchMap(cmsg_search_map pckDat,out smsg_view_map retDat)
        {
            int mapid = 0;
            if (int.TryParse(pckDat.name, out mapid) && mapid > 10010000)
            {
                if (System.IO.File.Exists(MapDataPath + "/" + mapid.ToString()))
                {
                    var map = GetMapInfo(mapid);
                    var d = new smsg_view_map();
                    d.page = 0;
                    d.infos.Add(ServerMap.ToMap_Show(map,pckDat.common.userid));
                    retDat = d;
                    return true;
                }
            }
            else
            {
                var maps = serverMaps.Where(x => x.map.name.Contains(pckDat.name)).ToList();
                var resMaps = new List<map_show>();
                for (int i = 0;i < 11;i++)
                {
                    if (i + 1 > maps.Count)
                    {
                        break;
                    }
                    resMaps.Add(ServerMap.ToMap_Show(maps[i], pckDat.common.userid));
                }
                var d = new smsg_view_map();
                d.page = (int)MathF.Ceiling(maps.Count / 10);
                d.infos = resMaps;
                retDat = d;
                return true;
            }
            retDat = null;
            return false;
        }
         
        public async static void CheckExp(int userid)
        {
            smsg_login acc;
            if (!TryGetAccount(userid, out acc))
            {
                return;
            }
            for (int i = acc.level; i <= 59; i++)
            {
                int expToUpgrade = Utils.levelToExpNeed[i + 1];
                if (acc.exp >= expToUpgrade)
                {
                    acc.level += 1;
                    acc.exp -= expToUpgrade;
                    continue;
                }
                else
                {
                    break;
                }
            }
            TrySaveAccount(acc.userid,acc);
        }
              
        static bool isWantedDif(int Now, int Target,int difStatic)
        {
            if (difStatic == 0) return Now == Target;
            return Now == Target && Target == difStatic;
        }


             
        public static bool TryGetMapList(cmsg_view_map pckDat,out smsg_view_map retDat)
        {
            retDat = new smsg_view_map();
            List<map_show> maps = new List<map_show>();
            List<ServerMap> mapinfo = AccountManager.serverMaps.ToList();
            if (pckDat.type == 1) //最热
            {
                if (mapinfo.Count > 1)
                {
                    mapinfo.Sort((x, y) => y.map.amount.CompareTo(x.map.amount));
                }
            }
            if (pckDat.type == 2) //最新
            {
                if (mapinfo.Count > 1)
                {
                    mapinfo.Sort((x, y) => y.map.id.CompareTo(x.map.id));
                }
            }
            if (pckDat.type == 4) //L1 
            {
                mapinfo = serverMaps.Where(x => (Utils.get_map_nd(x.map.pas,x.map.amount) == 1 && isWantedDif(Utils.get_map_nd(x.map.pas, x.map.amount),1,x.map.difficulty)) || x.map.difficulty == 1).ToList();
                if (mapinfo.Count > 2)
                {
                    mapinfo.Sort((x, y) => x.map.id.CompareTo(y.map.id));
                    mapinfo.Reverse();
                }
            }
            if (pckDat.type == 5) //L2
            {
                mapinfo = serverMaps.Where(x => (Utils.get_map_nd(x.map.pas, x.map.amount) == 2 && isWantedDif(Utils.get_map_nd(x.map.pas, x.map.amount), 2, x.map.difficulty)) || x.map.difficulty == 2).ToList();
                if (mapinfo.Count > 2)
                {
                    mapinfo.Sort((x, y) => x.map.id.CompareTo(y.map.id));
                    mapinfo.Reverse();
                }
            }
            if (pckDat.type == 6) //L3
            {
                mapinfo = serverMaps.Where(x => (Utils.get_map_nd(x.map.pas, x.map.amount) == 3 && isWantedDif(Utils.get_map_nd(x.map.pas, x.map.amount), 3, x.map.difficulty)) || x.map.difficulty == 3).ToList();
                if (mapinfo.Count > 2)
                {
                    mapinfo.Sort((x, y) => x.map.id.CompareTo(y.map.id));
                    mapinfo.Reverse();
                }
            }
            if (pckDat.type == 7) //L4
            {
                mapinfo = serverMaps.Where(x => (Utils.get_map_nd(x.map.pas, x.map.amount) == 4 && isWantedDif(Utils.get_map_nd(x.map.pas, x.map.amount), 4, x.map.difficulty)) || x.map.difficulty == 4).ToList();
                if (mapinfo.Count > 2)
                {
                    mapinfo.Sort((x, y) => x.map.id.CompareTo(y.map.id));
                    mapinfo.Reverse();
                }
            }
            if (pckDat.type == 100) //我的收藏
            {
                mapinfo = new List<ServerMap>();
            }
            if (pckDat.type == 101) //我的游玩 
            {
                mapinfo = serverMaps.Where(x => x.map.owner_id == pckDat.common.userid).ToList();
                if (mapinfo.Count > 2)
                {
                    mapinfo.Sort((x, y) => x.map.id.CompareTo(y.map.id));
                    mapinfo.Reverse();
                }
            }
            if (pckDat.type == 102) //最近游玩
            {
                mapinfo = new List<ServerMap>();
            }
            for (int i = pckDat.index * 10; i < pckDat.index * 10 + 10; i++)
            {
                if (i >= mapinfo.Count)
                {
                    break;
                }
                maps.Add(ServerMap.ToMap_Show(mapinfo[i],pckDat.common.userid));
            }
            retDat.page = (int)MathF.Ceiling(mapinfo.Count / 10);
            retDat.infos = maps;
            return true;
        }

        public static bool TryGetMissionData(cmsg_mission_view pckDat, out smsg_mission_view retDat)
        {
			int userid = pckDat.common.userid;
            retDat = new smsg_mission_view();

			string p = "";
			if (!TryGetUserPath(userid, out p))
			{
				return false;
			}

            if (!System.IO.File.Exists($"{p}/missiondata"))
            {
                File.WriteAllTextAsync($"{p}/missiondata",JsonConvert.SerializeObject(new MissionData()));
            }

            var mission = JsonConvert.DeserializeObject<MissionData>(File.ReadAllTextAsync($"{p}/missiondata").Result);

            if (mission == null)
            {
                return false;
            }
            retDat = mission.missionData;
            return true;

		}

        public static bool TryStartMission(cmsg_mission_start pckDat, out smsg_mission_play retDat)
        {
			int userid = pckDat.common.userid;
            retDat = new();

			string p = "";
			if (!TryGetUserPath(userid, out p))
			{
				return false;
			}

			var mission = JsonConvert.DeserializeObject<MissionData>(File.ReadAllTextAsync($"{p}/missiondata").Result);
			if (mission == null)
			{
				return false;
			}

            Random rand = new Random();
            List<ServerMap> FinalMaps = new List<ServerMap>();
            switch (pckDat.hard)
            {
                case 1:
                    var temp = serverMaps.Where(x => (Utils.get_map_nd(x.map.pas, x.map.amount) == 1 && isWantedDif(Utils.get_map_nd(x.map.pas, x.map.amount), 1, x.map.difficulty)) || x.map.difficulty == 1).ToList();
                    for (int i = 0; i < 8; i++)
                    {
                        FinalMaps.Add(temp[rand.Next(0, temp.Count - 1)]);
                    }
                    break;
                case 2:
					var temp0 = serverMaps.Where(x => (Utils.get_map_nd(x.map.pas, x.map.amount) == 2 && isWantedDif(Utils.get_map_nd(x.map.pas, x.map.amount), 2, x.map.difficulty)) || x.map.difficulty == 2).ToList();
					for (int i = 0; i < 8; i++)
					{
						FinalMaps.Add(temp0[rand.Next(0, temp0.Count - 1)]);
					}
					break;
                case 3:
					var temp1 = serverMaps.Where(x => (Utils.get_map_nd(x.map.pas, x.map.amount) == 3 && isWantedDif(Utils.get_map_nd(x.map.pas, x.map.amount), 3, x.map.difficulty)) || x.map.difficulty == 3).ToList();
					for (int i = 0; i < 16; i++)
					{
						FinalMaps.Add(temp1[rand.Next(0, temp1.Count - 1)]);
					}
					break;
                case 4:
					var temp2 = serverMaps.Where(x => (Utils.get_map_nd(x.map.pas, x.map.amount) == 4 && isWantedDif(Utils.get_map_nd(x.map.pas, x.map.amount), 4, x.map.difficulty)) || x.map.difficulty == 4).ToList();
					for (int i = 0; i < 16; i++)
					{
						FinalMaps.Add(temp2[rand.Next(0, temp2.Count - 1)]);
					}
					break;
                default:
                    return false;
            }

            mission.randomMaps = FinalMaps;
            mission.missionData.hard = pckDat.hard;
            mission.missionData.start = 1;
            mission.missionData.life = 100;
            mission.missionData.index = 0;

			File.WriteAllTextAsync($"{p}/missiondata", JsonConvert.SerializeObject(mission));

            var map = mission.randomMaps[0];

			retDat.map_data = map.mapData.data;
            retDat.map_name = map.map.name;
            retDat.user_name = map.map.owner_name;
            retDat.user_head = map.map.head;
            retDat.user_country = map.map.country;
            retDat.y = map.FailureY;
            retDat.x = map.FailureX;
            return true;
		}

		public static bool TryContinueMission(cmsg_mission_continue pckDat, out smsg_mission_play retDat)
		{
			int userid = pckDat.common.userid;
			retDat = new();

			string p = "";
			if (!TryGetUserPath(userid, out p))
			{
				return false;
			}

			var mission = JsonConvert.DeserializeObject<MissionData>(File.ReadAllTextAsync($"{p}/missiondata").Result);
            if (mission == null)
            {
                return false;
            }

			var map = mission.randomMaps[mission.missionData.index];

			retDat.map_data = map.mapData.data;
			retDat.map_name = map.map.name;
			retDat.user_name = map.map.owner_name;
			retDat.user_head = map.map.head;
			retDat.user_country = map.map.country;
			retDat.y = map.FailureY;
			retDat.x = map.FailureX;
			return true;
		}

		public static bool TryReplayMission(cmsg_mission_replay pckDat)
        {
			int userid = pckDat.common.userid;

			string p = "";
			if (!TryGetUserPath(userid, out p))
			{
				return false;
			}

			var mission = JsonConvert.DeserializeObject<MissionData>(File.ReadAllTextAsync($"{p}/missiondata").Result);
			if (mission == null)
			{
				return false;
			}

            mission.missionData.life--;

			File.WriteAllTextAsync($"{p}/missiondata", JsonConvert.SerializeObject(mission));
			return true;

		}

        public static bool TryClearMapMission(cmsg_mission_success pckDat,out object retDat,out int m_res)
        {
			int userid = pckDat.common.userid;
            retDat = new();
            m_res = 0;

			string p = "";
			if (!TryGetUserPath(userid, out p))
			{
				return false;
			}

			var mission = JsonConvert.DeserializeObject<MissionData>(File.ReadAllTextAsync($"{p}/missiondata").Result);
			if (mission == null)
			{
				return false;
			}

            if (mission.missionData.index + 1 >= mission.randomMaps.Count)
            {
                retDat = new smsg_mission_finish();
                var ret = (smsg_mission_finish)retDat;
                ret.exp = (int)Math.Pow(new Random().Next(20 / ( 5 - mission.missionData.hard), 100 / ( 5 - mission.missionData.hard)),new Random().Next(1, mission.missionData.hard)) + mission.missionData.life;
                ret.authors = mission.GetAuthorList();
                ret.suc = 1;
                retDat = ret;
				smsg_login acc;
				if (!TryGetAccount(pckDat.common.userid, out acc))
				{
					return false;
				}

				acc.exp += ret.exp;
				TrySaveAccount(pckDat.common.userid, acc);

				CheckExp(pckDat.common.userid);


				m_res = -1;
                mission.missionData.start = 0;
                mission.missionData.hard = 0;
                mission.missionData.life = 0;
                mission.missionData.index = 0;
                if (mission.missionData.br_max + 1 <= 4) mission.missionData.br_max++;
				mission.randomMaps.Clear();
			}
            else
            {
                retDat = new smsg_mission_play();
				mission.missionData.index++;
				var ret = (smsg_mission_play)retDat;
                var map = mission.randomMaps[mission.missionData.index];
				ret.map_data = map.mapData.data;
				ret.map_name = map.map.name;
				ret.user_name = map.map.owner_name;
				ret.user_head = map.map.head;
				ret.user_country = map.map.country;
				ret.y = map.FailureY;
				ret.x = map.FailureX;
			}

			File.WriteAllTextAsync($"{p}/missiondata", JsonConvert.SerializeObject(mission));

            return true;
		}

		public static bool TryFailMission(cmsg_mission_fail pckDat, out object retDat, out int m_res)
		{
			int userid = pckDat.common.userid;
			retDat = new();
			m_res = 0;

			string p = "";
			if (!TryGetUserPath(userid, out p))
			{
				return false;
			}

			var mission = JsonConvert.DeserializeObject<MissionData>(File.ReadAllTextAsync($"{p}/missiondata").Result);
			if (mission == null)
			{
				return false;
			}

			if (mission.missionData.life - 1 == 0)
			{
				retDat = new smsg_mission_finish();
				var ret = (smsg_mission_finish)retDat;
				ret.exp = 0;
				ret.authors = mission.GetAuthorList();
                ret.suc = 0;
                retDat = ret;
				m_res = -1;

				mission.missionData.start = 0;
				mission.missionData.hard = 0;
				mission.missionData.life = 0;
				mission.missionData.index = 0;
				mission.randomMaps.Clear();
			}
			else
			{
                retDat = null;

			}

			File.WriteAllTextAsync($"{p}/missiondata", JsonConvert.SerializeObject(mission));

			return true;
		}

		public static bool TryDropMission(cmsg_mission_continue pckDat)
        {
			int userid = pckDat.common.userid;

			string p = "";
			if (!TryGetUserPath(userid, out p))
			{
				return false;
			}

			var mission = JsonConvert.DeserializeObject<MissionData>(File.ReadAllTextAsync($"{p}/missiondata").Result);
			if (mission == null)
			{
				return false;
			}

            mission.missionData.index = 0;
            mission.missionData.start = 0;
            mission.missionData.hard = 0;
            mission.missionData.life = 0;
            mission.randomMaps.Clear();

			File.WriteAllTextAsync($"{p}/missiondata", JsonConvert.SerializeObject(mission));

			return true;
		}

        public static bool TryLikeMap(cmsg_map_like pckDat)
        {
            var map = GetMapInfo(GetPlayingMap(pckDat.common.sig));
            map.map.like++;
			File.WriteAllTextAsync(MapDataPath + "/" + map.map.id.ToString(), JsonConvert.SerializeObject(map));
            return true;
		}
    }
}

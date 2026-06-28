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

        public static bool MapCheck(string userp)
        {
            string MapPath = UserMapPath(userp);
            string MapDataPath = UserMapDataPath(userp);
            if (!System.IO.File.Exists(MapPath))
            {
                using (System.IO.File.Create(MapPath)) { }
            }
            if (!System.IO.File.Exists(MapDataPath))
            {
                using (System.IO.File.Create(MapDataPath)) { }
            }
            if (System.IO.File.ReadAllText(MapPath).Length == 0)
            {
                var d = new List<edit_data>();
                for (int i = 0; i < 12; i++)
                {
                    d.Add(new edit_data { id = 0, name = "", date = "", upload = 0, url = null });
                }
                System.IO.File.WriteAllText(MapPath,JsonConvert.SerializeObject(d));
            }
            if (System.IO.File.ReadAllText(MapDataPath).Length == 0)
            {
                var d = new List<MapDataHelper>();
                for (int i = 0; i < 12; i++)
                {
                    d.Add(new MapDataHelper { id = i});
                }
                System.IO.File.WriteAllText(MapDataPath, JsonConvert.SerializeObject(d));
            }
            if (!System.IO.File.Exists(UserClearListPath(userp)))
            {
                System.IO.File.WriteAllText(UserClearListPath(userp), JsonConvert.SerializeObject(new List<int>()));
            }
            if (!System.IO.File.Exists(UserRecentPlayPath(userp)))
            {
                System.IO.File.WriteAllText(UserRecentPlayPath(userp), JsonConvert.SerializeObject(new List<int>()));
            }
            if (!System.IO.File.Exists(UserFavoritePath(userp)))
            {
                System.IO.File.WriteAllText(UserFavoritePath(userp), JsonConvert.SerializeObject(new List<int>()));
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
            string d = System.IO.File.ReadAllText(UserMapPath(userp));
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
            string d = System.IO.File.ReadAllText(UserMapDataPath(userp));
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
            string MapPath = UserMapPath(userp);

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
            System.IO.File.WriteAllText(MapPath,JsonConvert.SerializeObject(list));
            map = mapD;
            return true;
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
            System.IO.File.WriteAllText(UserMapPath(p),JsonConvert.SerializeObject(EditD));
            System.IO.File.WriteAllText(UserMapDataPath(p), JsonConvert.SerializeObject(InfoD));
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
            System.IO.File.WriteAllText(UserMapPath(userp), JsonConvert.SerializeObject(EditD));
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
            System.IO.File.WriteAllText(UserMapPath(userp), JsonConvert.SerializeObject(EditD));
            System.IO.File.WriteAllText(UserMapDataPath(userp), JsonConvert.SerializeObject(InfoD));
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
                id = GetNextServerMapId(),
                head = userD.head,
                owner_name = userD.name,
                owner_id = userD.userid,
                country = userD.nationality,
                url = currentUpload.url,
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                name = currentUpload.name,
            };
            map.mapData = InfoD[pckData.id - 1];
            AddInitialRankReplay(map, userD, pckData.time, pckData.video);
            SaveServerMap(map);
            RecordPlayerMapUpload(userid, map);
            currentUpload.upload = 1;
            System.IO.File.WriteAllText(UserMapPath(userp), JsonConvert.SerializeObject(EditD));
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

            System.IO.File.WriteAllText(UserMapPath(p), JsonConvert.SerializeObject(EditD));
            System.IO.File.WriteAllText(UserMapDataPath(p), JsonConvert.SerializeObject(InfoD));
            return 0;
        }
    }
}

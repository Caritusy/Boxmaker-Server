using Newtonsoft.Json;
using protocol.game;

namespace BoxMaker_Server
{
    public class ServerMap
    {
        public class RECInfo
        {
            public int video_id;

            public byte[] recData;
        }
        public map_info map;
        public MapDataHelper mapData;
        public List<comment> comments = new List<comment>();
        public List<map_point_rank> ranks = new List<map_point_rank>();
        public List<RECInfo> recs = new List<RECInfo>();
        public List<int> FailureX = new List<int>();
        public List<int> FailureY = new List<int>();

        public static map_show ToMap_Show(ServerMap map,int userid)
        {
            map_info m = map.map;
            string userP = "";
            if (!AccountManager.TryGetUserPath(userid, out userP))
            {
    
            }
            AccountManager.MapCheck(userP);
            var ClearList = JsonConvert.DeserializeObject<List<int>>(File.ReadAllTextAsync(userP +"/clearlist").Result);
            return new map_show
            {
                amount = m.amount,
                collect = m.collect,
                difficulty = m.difficulty,
                finish = ClearList.Contains(m.id) ? 1 : 0,
                id = m.id,
                like = m.like,
                name = m.name,
                pas = m.pas,
                url = m.url,
            };
        }
    }
}

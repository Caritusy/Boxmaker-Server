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
        private const int MapPageSize = 10;
        private const int RecentPlayLimit = 200;

        private static readonly string ReplayRepairBackupRoot = Path.Combine(MapDataPath, "ReplayRepairBackups");
        private static readonly object ServerMapCacheLock = new object();
        private static readonly object UserMapIdListCacheLock = new object();
        private static readonly Dictionary<int, ServerMap> ServerMapById = new Dictionary<int, ServerMap>();
        private static readonly Dictionary<int, List<ServerMap>> MapListCacheByType = new Dictionary<int, List<ServerMap>>();
        private static readonly Dictionary<string, List<int>> UserMapIdListCache = new Dictionary<string, List<int>>();
        private static bool ServerMapCacheLoaded;
        private static string ReplayRepairBackupPath = string.Empty;

        private sealed class RankReplayResult
        {
            public int Rank { get; set; }
        }

        public static List<ServerMap> GetServerMapList()
        {
            return GetServerMapSnapshot();
        }

        public static ServerMap GetMapInfo(int mapid)
        {
            if (TryGetCachedServerMap(mapid, out ServerMap map))
            {
                return map;
            }

            string mapPath = ServerMapPath(mapid);
            if (!System.IO.File.Exists(mapPath))
            {
                return null;
            }

            map = ReadServerMapFile(mapPath);
            if (map == null)
            {
                return null;
            }

            UpsertServerMapCache(map);
            return map;
        }

        public static int GetMapExp(int mapid)
        {
            ServerMap map = GetMapInfo(mapid);
            return Utils.get_map_exp(map.map.pas,map.map.amount);
        }

        public static bool TryComment(int mapid, cmsg_comment pckDat)
        {
            try
            {
                Init();
                ServerMap map = GetMapInfo(mapid);
                if (map == null)
                {
                    return false;
                }

                if (!TryGetAccount(pckDat.common.userid, out smsg_login userD))
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
                QueueSaveServerMap(map, "comment map");
                RecordPlayerComment(pckDat.common.userid);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryCompleteMap(cmsg_complete_map pckDat,out smsg_complete_map retDat)
        {
            int mapid = GetPlayingMap(pckDat.common.sig);
            ServerMap map = GetMapInfo(mapid);
            if (map == null)
            {
                retDat = null;
                return false;
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

                int exp;
                RankReplayResult replayResult;
                lock (MapFileLock)
                {
                    map.map.amount += 1;
                    map.map.pas += 1;
                    replayResult = UpsertRankReplay(map, acc, pckDat.time, pckDat.video);
                    exp = Utils.get_map_exp(map.map.pas, map.map.amount);
                    SaveServerMap(map);

                    List<int> clearList = JsonConvert.DeserializeObject<List<int>>(System.IO.File.ReadAllText(UserClearListPath(userP))) ?? new List<int>();
                    if (!clearList.Contains(mapid))
                    {
                        clearList.Add(mapid);
                        System.IO.File.WriteAllText(UserClearListPath(userP), JsonConvert.SerializeObject(clearList));
                    }
                }

                retDat = new smsg_complete_map()
                {
                    exp = exp,
                    mapid = mapid,
                    rank = replayResult.Rank,
                };

                if (mapid == 10010001) retDat.testify = 4;

                if (map.map.id == 10010001)
                {
                    if (acc.guide == 200)
                    {
                        acc.guide = 666;
                    }
                    if (acc.testify == 0)
                    {
                        acc.testify = 4;
                    }
                }

                acc.exp += exp;
                TrySaveAccount(pckDat.common.userid,acc);

                CheckExp(pckDat.common.userid);

                RecordPlayerMapSuccess(pckDat.common.userid, map, retDat.rank);

            }
            else
            {
                lock (MapFileLock)
                {
                    map.map.amount += 1;
                    EnsureServerMapCollections(map);
                    map.FailureX.Add(pckDat.x);
                    map.FailureY.Add(pckDat.y);
                    QueueSaveServerMap(map, "complete map failure");
                }

                retDat = new smsg_complete_map()
                {
                    mapid = mapid,
                };
            }

            RecordPlayerMapAttempt(pckDat.common.userid, map);
            return true;
        }

        public static bool TryReplayMap(cmsg_replay_map pckDat)
        {
            int mapid = GetPlayingMap(pckDat.common.sig);
            ServerMap map = GetMapInfo(mapid);
            if (map == null)
            {
                return false;
            }

            lock (MapFileLock)
            {
                map.map.amount += 1;
                QueueSaveServerMap(map, "replay map");
            }

            RecordPlayerMapAttempt(pckDat.common.userid, map);
            return true;
        }

        public static bool TryViewMapReplay(cmsg_view_video pckDat, out smsg_view_video retDat)
        {
            retDat = null;
            ServerMap map = GetMapInfo(pckDat.map_id);
            if (map == null)
            {
                return false;
            }

            lock (MapFileLock)
            {
                EnsureServerMapCollections(map);
                ServerMap.RECInfo rec = map.recs.FirstOrDefault(x => x.video_id == pckDat.video_id && x.recData != null && x.recData.Length > 0);
                if (rec == null)
                {
                    return false;
                }

                retDat = new smsg_view_video
                {
                    map_data = map.mapData.data,
                    video_data = rec.recData,
                };
            }

            RecordVideoWatched(map, pckDat.video_id);
            return true;
        }

        public static void RepairServerMapReplayIndexes()
        {
            foreach (ServerMap map in GetServerMapSnapshot())
            {
                if (map?.map == null)
                {
                    continue;
                }

                bool changed;
                lock (MapFileLock)
                {
                    changed = NormalizeReplayStorage(map);
                    if (changed)
                    {
                        BackupServerMapBeforeReplayRepair(map.map.id);
                        SaveServerMap(map);
                    }
                }
            }
        }

        public static bool TrackRecentPlay(int userid, int mapid)
        {
            try
            {
                int normalizedMapId = NormalizeServerMapId(mapid);
                ServerMap map = GetMapInfo(normalizedMapId);
                if (map == null)
                {
                    return false;
                }

                EnqueueIo("track recent play", () =>
                {
                    if (!TryGetUserPath(userid, out string userPath))
                    {
                        return;
                    }

                    MapCheck(userPath);
                    string recentPlayPath = UserRecentPlayPath(userPath);
                    List<int> recentPlay = GetCachedUserMapIdList(recentPlayPath);
                    recentPlay.RemoveAll(id => id == normalizedMapId);
                    recentPlay.Insert(0, normalizedMapId);
                    if (recentPlay.Count > RecentPlayLimit)
                    {
                        recentPlay.RemoveRange(RecentPlayLimit, recentPlay.Count - RecentPlayLimit);
                    }

                    QueueWriteUserMapIdList(recentPlayPath, recentPlay, "recent play");
                    UpsertRecentPlayerMap(userid, map);
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryToggleFavoriteMap(cmsg_favorite_map pckDat, out smsg_favorite_map retDat)
        {
            retDat = new smsg_favorite_map();

            try
            {
                int mapid = NormalizeServerMapId(pckDat.id);
                if (!TryGetUserPath(pckDat.common.userid, out string userPath))
                {
                    return false;
                }

                ServerMap map = GetMapInfo(mapid);
                if (map?.map == null)
                {
                    return false;
                }

                MapCheck(userPath);
                string favoritePath = UserFavoritePath(userPath);
                lock (MapFileLock)
                {
                    List<int> favoriteMaps = GetCachedUserMapIdList(favoritePath);
                    bool alreadyFavorite = favoriteMaps.Contains(mapid);
                    if (alreadyFavorite)
                    {
                        favoriteMaps.RemoveAll(id => id == mapid);
                        map.map.favorite = Math.Max(0, map.map.favorite - 1);
                    }
                    else
                    {
                        favoriteMaps.Insert(0, mapid);
                        map.map.favorite++;
                    }

                    QueueWriteUserMapIdList(favoritePath, favoriteMaps, "favorite map");
                    retDat.num = map.map.favorite;
                    QueueSaveServerMap(map, "favorite map count");
                }

                return true;
            }
            catch
            {
                retDat = null;
                return false;
            }
        }

        public static bool IsFavoriteMap(int userid, int mapid)
        {
            return GetUserFavoriteMapIds(userid).Contains(NormalizeServerMapId(mapid));
        }

        public static map_info BuildMapInfoForUser(ServerMap map, int userid)
        {
            map_info info = CloneMapInfo(map.map);
            info.finish = GetUserClearMapIds(userid).Contains(info.id) ? 1 : 0;
            info.collect = IsFavoriteMap(userid, info.id) ? 1 : 0;
            return info;
        }

        public static List<map_show> SearchMapsForWeb(string keyword, int limit = 20)
        {
            keyword ??= "";
            IEnumerable<ServerMap> maps = GetServerMapSnapshot()
                .Where(x => x?.map != null);

            if (int.TryParse(keyword, out int mapid))
            {
                int normalizedMapId = NormalizeServerMapId(mapid);
                maps = maps.Where(x => x.map.id == normalizedMapId);
            }
            else if (!string.IsNullOrWhiteSpace(keyword))
            {
                maps = maps.Where(x => x.map.name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || x.map.owner_name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            return maps
                .OrderByDescending(x => x.map.id)
                .Take(Math.Clamp(limit, 1, 50))
                .Select(map => BuildMapShow(map, 0, new HashSet<int>(), new HashSet<int>()))
                .ToList();
        }

        public static bool TrySearchMap(cmsg_search_map pckDat,out smsg_view_map retDat)
        {
            int mapid = 0;
            if (int.TryParse(pckDat.name, out mapid) && mapid > ServerMapIdBase)
            {
                ServerMap map = GetMapInfo(mapid);
                if (map != null)
                {
                    retDat = new smsg_view_map();
                    retDat.page = 0;
                    retDat.infos.Add(BuildMapShow(
                        map,
                        pckDat.common.userid,
                        GetUserClearMapIds(pckDat.common.userid),
                        GetUserFavoriteMapIds(pckDat.common.userid)));
                    return true;
                }
            }
            else
            {
                string keyword = pckDat.name ?? "";
                HashSet<int> clearMapIds = GetUserClearMapIds(pckDat.common.userid);
                HashSet<int> favoriteMapIds = GetUserFavoriteMapIds(pckDat.common.userid);
                List<ServerMap> maps = GetServerMapSnapshot()
                    .Where(x => x?.map?.name != null && x.map.name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                retDat = new smsg_view_map();
                retDat.page = GetPageCount(maps.Count);
                retDat.infos = maps
                    .Take(MapPageSize)
                    .Select(map => BuildMapShow(map, pckDat.common.userid, clearMapIds, favoriteMapIds))
                    .ToList();
                return true;
            }

            retDat = null;
            return false;
        }

        static bool isWantedDif(int Now, int Target,int difStatic)
        {
            if (difStatic == 0) return Now == Target;
            return Now == Target && Target == difStatic;
        }

        public static bool TryGetMapList(cmsg_view_map pckDat,out smsg_view_map retDat)
        {
            List<ServerMap> mapinfo = GetMapsForListType(pckDat.type, pckDat.common.userid);
            HashSet<int> clearMapIds = GetUserClearMapIds(pckDat.common.userid);
            HashSet<int> favoriteMapIds = GetUserFavoriteMapIds(pckDat.common.userid);

            retDat = new smsg_view_map();
            retDat.page = GetPageCount(mapinfo.Count);
            retDat.infos = mapinfo
                .Skip(pckDat.index * MapPageSize)
                .Take(MapPageSize)
                .Select(map => BuildMapShow(map, pckDat.common.userid, clearMapIds, favoriteMapIds))
                .ToList();
            return true;
        }

        public static bool TryLikeMap(cmsg_map_like pckDat)
        {
            ServerMap map = GetMapInfo(GetPlayingMap(pckDat.common.sig));
            if (map == null)
            {
                return false;
            }

            lock (MapFileLock)
            {
                map.map.like++;
                QueueSaveServerMap(map, "like map");
            }
            return true;
        }

        private static List<ServerMap> GetServerMapSnapshot()
        {
            EnsureServerMapCache();
            lock (ServerMapCacheLock)
            {
                return serverMaps.ToList();
            }
        }

        private static void EnsureServerMapCache()
        {
            if (ServerMapCacheLoaded)
            {
                return;
            }

            lock (ServerMapCacheLock)
            {
                if (ServerMapCacheLoaded)
                {
                    return;
                }

                Init();
                ServerMapById.Clear();
                serverMaps.Clear();

                foreach (string file in Directory.GetFiles(MapDataPath))
                {
                    ServerMap map = ReadServerMapFile(file);
                    if (map?.map == null)
                    {
                        continue;
                    }

                    ServerMapById[map.map.id] = map;
                    serverMaps.Add(map);
                }

                ServerMapCacheLoaded = true;
                MapListCacheByType.Clear();
            }
        }

        private static bool TryGetCachedServerMap(int mapid, out ServerMap map)
        {
            EnsureServerMapCache();
            lock (ServerMapCacheLock)
            {
                return ServerMapById.TryGetValue(mapid, out map);
            }
        }

        private static void SaveServerMap(ServerMap map)
        {
            lock (MapFileLock)
            {
                EnsureServerMapCollections(map);
                System.IO.File.WriteAllText(ServerMapPath(map.map.id), JsonConvert.SerializeObject(map));
                UpsertServerMapCache(map);
            }
        }

        private static void QueueSaveServerMap(ServerMap map, string name)
        {
            if (map?.map == null)
            {
                return;
            }

            MarkServerMapCacheDirty();
            EnqueueIo(name, () => SaveServerMap(map));
        }

        private static void MarkServerMapCacheDirty()
        {
            lock (ServerMapCacheLock)
            {
                MapListCacheByType.Clear();
            }
        }

        private static RankReplayResult UpsertRankReplay(ServerMap map, smsg_login account, int time, byte[] replayData)
        {
            EnsureServerMapCollections(map);
            NormalizeReplayStorage(map);

            List<map_point_rank> userRanks = map.ranks.Where(x => x.user_id == account.userid).ToList();
            map_point_rank bestExistingRank = userRanks
                .OrderBy(x => x.player_point)
                .ThenBy(x => x.video_id)
                .FirstOrDefault();

            bool hasReplay = HasReplayPayload(replayData);
            bool shouldReplace = hasReplay && (bestExistingRank == null || time < bestExistingRank.player_point);

            if (shouldReplace)
            {
                HashSet<int> oldVideoIds = userRanks.Select(x => x.video_id).ToHashSet();
                map.ranks.RemoveAll(x => x.user_id == account.userid);
                RemoveReplaysWithoutRankReference(map, oldVideoIds);

                int videoId = GetNextReplayVideoId(map);
                map_point_rank rank = CreateMapPointRank(account, time, videoId);
                map.ranks.Add(rank);
                map.recs.Add(new ServerMap.RECInfo { video_id = videoId, recData = replayData });
            }

            NormalizeReplayStorage(map);
            SortRanksForStorage(map);

            return new RankReplayResult
            {
                Rank = GetUserMapRank(map, account.userid),
            };
        }

        private static void AddInitialRankReplay(ServerMap map, smsg_login account, int time, byte[] replayData)
        {
            if (!HasReplayPayload(replayData))
            {
                return;
            }

            EnsureServerMapCollections(map);
            int videoId = GetNextReplayVideoId(map);
            map.ranks.Add(CreateMapPointRank(account, time, videoId));
            map.recs.Add(new ServerMap.RECInfo { video_id = videoId, recData = replayData });
            SortRanksForStorage(map);
        }

        private static map_point_rank CreateMapPointRank(smsg_login account, int time, int videoId)
        {
            return new map_point_rank
            {
                player_country = account.nationality,
                player_level = account.level,
                player_name = account.name,
                player_point = time,
                user_id = account.userid,
                video_id = videoId,
                visitor = account.visitor,
            };
        }

        private static bool NormalizeReplayStorage(ServerMap map)
        {
            EnsureServerMapCollections(map);
            bool changed = false;
            HashSet<int> replayVideoIds = map.recs
                .Where(x => x != null && HasReplayPayload(x.recData))
                .Select(x => x.video_id)
                .ToHashSet();

            List<map_point_rank> replayableRanks = map.ranks
                .Where(x => x != null && replayVideoIds.Contains(x.video_id))
                .ToList();

            if (replayableRanks.Count != map.ranks.Count)
            {
                changed = true;
            }

            List<map_point_rank> guestRanks = replayableRanks
                .Where(x => x != null && x.user_id <= 0)
                .ToList();
            List<map_point_rank> bestUserRanks = replayableRanks
                .Where(x => x != null && x.user_id > 0)
                .GroupBy(x => x.user_id)
                .Select(group => group
                    .OrderBy(x => x.player_point)
                    .ThenBy(x => x.video_id)
                    .First())
                .ToList();
            List<map_point_rank> bestRanks = guestRanks.Concat(bestUserRanks).ToList();

            if (bestRanks.Count != map.ranks.Count)
            {
                map.ranks = bestRanks;
                changed = true;
            }

            HashSet<int> rankVideoIds = map.ranks.Select(x => x.video_id).ToHashSet();
            HashSet<int> seenReplayIds = new HashSet<int>();
            List<ServerMap.RECInfo> compactReplays = new List<ServerMap.RECInfo>();

            foreach (ServerMap.RECInfo rec in map.recs.Where(x => x != null))
            {
                if (!rankVideoIds.Contains(rec.video_id) || !HasReplayPayload(rec.recData) || !seenReplayIds.Add(rec.video_id))
                {
                    changed = true;
                    continue;
                }

                compactReplays.Add(rec);
            }

            if (compactReplays.Count != map.recs.Count)
            {
                changed = true;
            }

            map.recs = compactReplays;
            SortRanksForStorage(map);
            return changed;
        }

        private static void RemoveReplaysWithoutRankReference(ServerMap map, HashSet<int> candidateVideoIds)
        {
            if (candidateVideoIds.Count == 0)
            {
                return;
            }

            HashSet<int> referencedVideoIds = map.ranks.Select(x => x.video_id).ToHashSet();
            map.recs.RemoveAll(x => candidateVideoIds.Contains(x.video_id) && !referencedVideoIds.Contains(x.video_id));
        }

        private static int GetNextReplayVideoId(ServerMap map)
        {
            EnsureServerMapCollections(map);
            HashSet<int> usedIds = map.ranks.Select(x => x.video_id)
                .Concat(map.recs.Select(x => x.video_id))
                .Where(x => x > 0)
                .ToHashSet();

            int maxId = usedIds.Count == 0 ? 0 : usedIds.Max();
            if (maxId < int.MaxValue)
            {
                return maxId + 1;
            }

            for (int id = 1; id < int.MaxValue; id++)
            {
                if (!usedIds.Contains(id))
                {
                    return id;
                }
            }

            throw new InvalidOperationException("No replay video id is available for this map.");
        }

        private static void SortRanksForStorage(ServerMap map)
        {
            map.ranks.Sort((x, y) => y.player_point.CompareTo(x.player_point));
        }

        private static bool HasReplayPayload(byte[] replayData)
        {
            return replayData != null && replayData.Length > 0;
        }

        private static void EnsureServerMapCollections(ServerMap map)
        {
            map.comments ??= new List<comment>();
            map.ranks ??= new List<map_point_rank>();
            map.recs ??= new List<ServerMap.RECInfo>();
            map.FailureX ??= new List<int>();
            map.FailureY ??= new List<int>();
        }

        private static int GetNextServerMapId()
        {
            EnsureServerMapCache();
            lock (ServerMapCacheLock)
            {
                return serverMaps.Count == 0
                    ? ServerMapIdBase + 1
                    : serverMaps.Max(map => map.map.id) + 1;
            }
        }

        private static void UpsertServerMapCache(ServerMap map)
        {
            if (map?.map == null)
            {
                return;
            }

            lock (ServerMapCacheLock)
            {
                ServerMapById[map.map.id] = map;

                int index = serverMaps.FindIndex(existing => existing?.map?.id == map.map.id);
                if (index >= 0)
                {
                    serverMaps[index] = map;
                }
                else
                {
                    serverMaps.Add(map);
                }

                ServerMapCacheLoaded = true;
                MapListCacheByType.Clear();
            }
        }

        private static ServerMap ReadServerMapFile(string filePath)
        {
            try
            {
                return JsonConvert.DeserializeObject<ServerMap>(System.IO.File.ReadAllText(filePath));
            }
            catch
            {
                return null;
            }
        }

        private static void BackupServerMapBeforeReplayRepair(int mapid)
        {
            try
            {
                string sourcePath = ServerMapPath(mapid);
                if (!System.IO.File.Exists(sourcePath))
                {
                    return;
                }

                if (string.IsNullOrEmpty(ReplayRepairBackupPath))
                {
                    ReplayRepairBackupPath = Path.Combine(ReplayRepairBackupRoot, DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Directory.CreateDirectory(ReplayRepairBackupPath);
                }

                string backupPath = Path.Combine(ReplayRepairBackupPath, mapid.ToString());
                if (!System.IO.File.Exists(backupPath))
                {
                    System.IO.File.Copy(sourcePath, backupPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"备份地图录像修复数据失败 mapid={mapid}: {ex.Message}");
            }
        }

        private static List<ServerMap> GetMapsForListType(int type, int userid)
        {
            if (type == 100)
            {
                return GetUserMapList(userid, UserFavoritePath);
            }

            if (type == 102)
            {
                return GetUserMapList(userid, UserRecentPlayPath);
            }

            if (type == 101)
            {
                return GetServerMapSnapshot()
                    .Where(x => x?.map != null && x.map.owner_id == userid)
                    .OrderByDescending(x => x.map.id)
                    .ToList();
            }

            lock (ServerMapCacheLock)
            {
                EnsureServerMapCache();
                if (MapListCacheByType.TryGetValue(type, out List<ServerMap> cached))
                {
                    return cached;
                }

                List<ServerMap> result = BuildMapListForType(type);
                MapListCacheByType[type] = result;
                return result;
            }
        }

        private static List<ServerMap> BuildMapListForType(int type)
        {
            IEnumerable<ServerMap> maps = serverMaps.Where(x => x?.map != null);

            return type switch
            {
                1 => maps.OrderByDescending(x => x.map.amount).ToList(),
                2 => maps.OrderByDescending(x => x.map.id).ToList(),
                4 => FilterByDifficulty(maps, 1).OrderByDescending(x => x.map.id).ToList(),
                5 => FilterByDifficulty(maps, 2).OrderByDescending(x => x.map.id).ToList(),
                6 => FilterByDifficulty(maps, 3).OrderByDescending(x => x.map.id).ToList(),
                7 => FilterByDifficulty(maps, 4).OrderByDescending(x => x.map.id).ToList(),
                _ => maps.ToList(),
            };
        }

        private static IEnumerable<ServerMap> FilterByDifficulty(IEnumerable<ServerMap> maps, int difficulty)
        {
            return maps.Where(map =>
            {
                int calculatedDifficulty = Utils.get_map_nd(map.map.pas, map.map.amount);
                return (calculatedDifficulty == difficulty && isWantedDif(calculatedDifficulty, difficulty, map.map.difficulty))
                    || map.map.difficulty == difficulty;
            });
        }

        private static HashSet<int> GetUserClearMapIds(int userid)
        {
            if (!TryGetUserPath(userid, out string userPath))
            {
                return new HashSet<int>();
            }

            MapCheck(userPath);
            string clearListPath = UserClearListPath(userPath);
            if (!System.IO.File.Exists(clearListPath))
            {
                return new HashSet<int>();
            }

            List<int> clearList = JsonConvert.DeserializeObject<List<int>>(System.IO.File.ReadAllText(clearListPath));
            return clearList == null ? new HashSet<int>() : clearList.ToHashSet();
        }

        private static HashSet<int> GetUserFavoriteMapIds(int userid)
        {
            if (!TryGetUserPath(userid, out string userPath))
            {
                return new HashSet<int>();
            }

            MapCheck(userPath);
            return GetCachedUserMapIdList(UserFavoritePath(userPath)).ToHashSet();
        }

        private static List<ServerMap> GetUserMapList(int userid, Func<string, string> getListPath)
        {
            if (!TryGetUserPath(userid, out string userPath))
            {
                return new List<ServerMap>();
            }

            MapCheck(userPath);
            List<int> mapIds = GetCachedUserMapIdList(getListPath(userPath));

            List<ServerMap> maps = new List<ServerMap>();
            HashSet<int> seenMapIds = new HashSet<int>();
            foreach (int mapId in mapIds)
            {
                int normalizedMapId = NormalizeServerMapId(mapId);
                if (!seenMapIds.Add(normalizedMapId))
                {
                    continue;
                }

                ServerMap map = GetMapInfo(normalizedMapId);
                if (map != null)
                {
                    maps.Add(map);
                }
            }

            return maps;
        }

        private static List<int> ReadUserMapIdList(string filePath)
        {
            EnsureUserMapIdListFile(filePath);

            try
            {
                List<int> ids = JsonConvert.DeserializeObject<List<int>>(System.IO.File.ReadAllText(filePath));
                return ids == null
                    ? new List<int>()
                    : ids.Select(NormalizeServerMapId).Where(id => id > ServerMapIdBase).ToList();
            }
            catch
            {
                WriteUserMapIdList(filePath, new List<int>());
                return new List<int>();
            }
        }

        private static void WriteUserMapIdList(string filePath, List<int> mapIds)
        {
            System.IO.File.WriteAllText(filePath, JsonConvert.SerializeObject(mapIds));
        }

        private static List<int> GetCachedUserMapIdList(string filePath)
        {
            lock (UserMapIdListCacheLock)
            {
                if (UserMapIdListCache.TryGetValue(filePath, out List<int>? cached) && cached != null)
                {
                    return cached.ToList();
                }
            }

            List<int> mapIds;
            lock (MapFileLock)
            {
                mapIds = ReadUserMapIdList(filePath);
            }

            lock (UserMapIdListCacheLock)
            {
                UserMapIdListCache[filePath] = mapIds.ToList();
            }

            return mapIds;
        }

        private static void QueueWriteUserMapIdList(string filePath, List<int> mapIds, string name)
        {
            List<int> snapshot = mapIds.Select(NormalizeServerMapId).Where(id => id > ServerMapIdBase).ToList();
            lock (UserMapIdListCacheLock)
            {
                UserMapIdListCache[filePath] = snapshot.ToList();
            }

            EnqueueIo(name, () =>
            {
                lock (MapFileLock)
                {
                    WriteUserMapIdList(filePath, snapshot);
                }
            });
        }

        private static void EnsureUserMapIdListFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.WriteAllText(filePath, JsonConvert.SerializeObject(new List<int>()));
            }
        }

        private static int NormalizeServerMapId(int mapid)
        {
            if (mapid <= 0)
            {
                return mapid;
            }

            return mapid < ServerMapIdBase ? ServerMapIdBase + mapid : mapid;
        }

        private static map_info CloneMapInfo(map_info source)
        {
            return new map_info
            {
                amount = source.amount,
                collect = source.collect,
                country = source.country,
                date = source.date,
                difficulty = source.difficulty,
                favorite = source.favorite,
                finish = source.finish,
                head = source.head,
                id = source.id,
                like = source.like,
                name = source.name,
                owner_id = source.owner_id,
                owner_name = source.owner_name,
                pas = source.pas,
                url = source.url,
            };
        }

        private static map_show BuildMapShow(ServerMap map, int userid, HashSet<int> clearMapIds, HashSet<int> favoriteMapIds)
        {
            map_info m = map.map;
            return new map_show
            {
                amount = m.amount,
                collect = favoriteMapIds.Contains(m.id) ? 1 : 0,
                difficulty = m.difficulty,
                finish = clearMapIds.Contains(m.id) ? 1 : 0,
                id = m.id,
                like = m.like,
                name = m.name,
                pas = m.pas,
                url = m.url,
            };
        }

        private static int GetPageCount(int count)
        {
            if (count <= 0)
            {
                return 0;
            }

            return (count + MapPageSize - 1) / MapPageSize;
        }
    }
}

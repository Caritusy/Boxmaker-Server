using Newtonsoft.Json;
using protocol.game;

namespace BoxMaker_Server
{
    public partial class AccountManager
    {
        private const string OldPlayerRegisterText = "i'm old player";
        private const int PlayerStateListLimit = 10;
        private static readonly object PlayerStateCacheLock = new object();
        private static readonly Dictionary<string, PlayerState> PlayerStateCacheByPath = new Dictionary<string, PlayerState>();

        public static bool TryGetPlayerInfo(int userid, out smsg_view_player retDat)
        {
            retDat = new smsg_view_player();

            try
            {
                PlayerState state = EnsurePlayerState(userid);
                retDat.data = state.data;
                retDat.recent.AddRange(state.recent.Take(PlayerStateListLimit));
                retDat.upload.AddRange(state.upload.Take(PlayerStateListLimit));
                retDat.top.AddRange(state.top.Take(PlayerStateListLimit));
                retDat.play.AddRange(state.play.Take(PlayerStateListLimit));
                return true;
            }
            catch
            {
                retDat = null!;
                return false;
            }
        }

        private static void CreatePlayerStateForNewAccount(string userPath, smsg_login account)
        {
            PlayerState state = CreateEmptyPlayerState(account, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            SavePlayerState(userPath, state);
        }

        private static void UpdatePlayerStateAccount(smsg_login account)
        {
            if (!TryGetUserPath(account.userid, out string userPath))
            {
                return;
            }

            PlayerState state = LoadPlayerState(userPath) ?? BuildPlayerStateFromHistory(account.userid, OldPlayerRegisterText);
            ApplyAccountToPlayerState(state, account);
            SavePlayerState(userPath, state);
        }

        private static void RecordPlayerMapUpload(int userid, ServerMap map)
        {
            EnqueueIo("playerstate map upload", () =>
            {
                PlayerState state = EnsurePlayerState(userid);
                UpsertUploadMap(state, map);
                SavePlayerState(userid, state);
            });
        }

        private static void RecordPlayerMapAttempt(int userid, ServerMap map)
        {
            EnqueueIo("playerstate map attempt", () =>
            {
                PlayerState state = EnsurePlayerState(userid);
                state.data.amount++;
                IncrementPlayCount(state, map);
                SavePlayerState(userid, state);
            });
        }

        private static void RecordPlayerMapSuccess(int userid, ServerMap map, int rank)
        {
            EnqueueIo("playerstate map success", () =>
            {
                PlayerState state = EnsurePlayerState(userid);
                state.data.pas++;
                UpsertTopMap(state, map, rank);
                SavePlayerState(userid, state);
            });
        }

        private static void RecordPlayerComment(int userid)
        {
            EnqueueIo("playerstate comment", () =>
            {
                PlayerState state = EnsurePlayerState(userid);
                state.data.comment++;
                SavePlayerState(userid, state);
            });
        }

        public static void RebuildAllPlayerStates()
        {
            Init();
            foreach (string userPath in Directory.GetDirectories(AccDataPath))
            {
                string directoryName = Path.GetFileName(userPath);
                string[] parts = directoryName.Split(AccountPathSeparator);
                if (parts.Length == 0 || !int.TryParse(parts[0], out int userid))
                {
                    continue;
                }

                PlayerState existingState = LoadPlayerState(userPath);
                string registerText = string.IsNullOrWhiteSpace(existingState?.data?.register)
                    ? OldPlayerRegisterText
                    : existingState.data.register;
                PlayerState rebuiltState = BuildPlayerStateFromHistory(userid, registerText);
                SavePlayerState(userPath, rebuiltState);
            }
        }

        public static void RecordVideoWatched(ServerMap map, int videoId)
        {
            if (map == null || map.map == null)
            {
                return;
            }

            map_point_rank? rank = map.ranks.FirstOrDefault(x => x.video_id == videoId);
            if (rank == null || rank.user_id <= 0)
            {
                return;
            }

            int ownerUserId = rank.user_id;
            EnqueueIo("playerstate video watched", () =>
            {
                PlayerState state = EnsurePlayerState(ownerUserId);
                state.data.watched++;
                SavePlayerState(ownerUserId, state);
            });
        }

        private static void UpsertRecentPlayerMap(int userid, ServerMap map)
        {
            PlayerState state = EnsurePlayerState(userid);
            int rank = GetUserMapRank(map, userid);
            state.recent.RemoveAll(x => x.id == map.map.id);
            state.recent.Insert(0, new map_recent
            {
                id = map.map.id,
                name = map.map.name,
                rank = rank,
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                url = map.map.url,
            });
            TrimPlayerStateLists(state);
            SavePlayerState(userid, state);
        }

        private static PlayerState EnsurePlayerState(int userid)
        {
            if (!TryGetUserPath(userid, out string userPath))
            {
                throw new InvalidOperationException($"User {userid} not found.");
            }

            PlayerState state = LoadPlayerState(userPath);
            if (state == null)
            {
                state = BuildPlayerStateFromHistory(userid, OldPlayerRegisterText);
                SavePlayerState(userPath, state);
            }
            else if (TryGetAccount(userid, out smsg_login account))
            {
                ApplyAccountToPlayerState(state, account);
                SavePlayerState(userPath, state);
            }

            return state;
        }

        private static PlayerState BuildPlayerStateFromHistory(int userid, string registerText)
        {
            if (!TryGetAccount(userid, out smsg_login account))
            {
                throw new InvalidOperationException($"User {userid} not found.");
            }

            PlayerState state = CreateEmptyPlayerState(account, registerText);
            if (!TryGetUserPath(userid, out string userPath))
            {
                return state;
            }

            MapCheck(userPath);
            HashSet<int> clearMapIds = GetUserClearMapIds(userid);
            List<int> recentMapIds = ReadUserMapIdList(UserRecentPlayPath(userPath));
            List<ServerMap> maps = GetServerMapSnapshot();
            HashSet<int> confirmedPlayedMapIds = clearMapIds.ToHashSet();

            foreach (ServerMap map in maps.Where(x => x?.map != null))
            {
                if (map.map.owner_id == userid)
                {
                    UpsertUploadMap(state, map);
                }

                state.data.comment += map.comments.Count(x => x.userid == userid);

                int rank = GetUserMapRank(map, userid);
                if (rank > 0)
                {
                    confirmedPlayedMapIds.Add(map.map.id);
                    UpsertTopMap(state, map, rank);
                    state.data.video += map.ranks.Count(x => x.user_id == userid && map.recs.Any(rec => rec.video_id == x.video_id));
                }
            }

            foreach (int mapId in recentMapIds)
            {
                ServerMap map = GetMapInfo(mapId);
                if (map == null)
                {
                    continue;
                }

                state.recent.Add(new map_recent
                {
                    id = map.map.id,
                    name = map.map.name,
                    rank = GetUserMapRank(map, userid),
                    time = "",
                    url = map.map.url,
                });
                IncrementPlayCount(state, map);
            }

            state.data.amount = confirmedPlayedMapIds.Count;
            state.data.pas = clearMapIds.Count;
            state.data.point = 0;
            state.data.watched = 0;
            TrimPlayerStateLists(state);
            return state;
        }

        private static PlayerState CreateEmptyPlayerState(smsg_login account, string registerText)
        {
            PlayerState state = new PlayerState();
            state.data.register = registerText;
            ApplyAccountToPlayerState(state, account);
            return state;
        }

        private static void ApplyAccountToPlayerState(PlayerState state, smsg_login account)
        {
            string register = string.IsNullOrEmpty(state.data.register) ? OldPlayerRegisterText : state.data.register;
            state.data.userid = account.userid;
            state.data.name = account.name;
            state.data.country = account.nationality;
            state.data.head = account.head;
            state.data.level = account.level;
            state.data.exp = account.exp;
            state.data.visitor = account.visitor;
            state.data.register = register;
            state.data.point = 0;
        }

        private static PlayerState LoadPlayerState(string userPath)
        {
            lock (PlayerStateCacheLock)
            {
                if (PlayerStateCacheByPath.TryGetValue(userPath, out PlayerState? cached) && cached != null)
                {
                    return cached;
                }
            }

            string playerStatePath = UserPlayerStatePath(userPath);
            if (!System.IO.File.Exists(playerStatePath))
            {
                return null!;
            }

            try
            {
                PlayerState? state = JsonConvert.DeserializeObject<PlayerState>(System.IO.File.ReadAllText(playerStatePath));
                if (state != null)
                {
                    lock (PlayerStateCacheLock)
                    {
                        PlayerStateCacheByPath[userPath] = state;
                    }
                }

                return state ?? null!;
            }
            catch
            {
                return null!;
            }
        }

        private static void SavePlayerState(int userid, PlayerState state)
        {
            if (TryGetUserPath(userid, out string userPath))
            {
                SavePlayerState(userPath, state);
            }
        }

        private static void SavePlayerState(string userPath, PlayerState state)
        {
            TrimPlayerStateLists(state);
            string json = JsonConvert.SerializeObject(state);
            lock (PlayerStateCacheLock)
            {
                PlayerStateCacheByPath[userPath] = state;
            }

            EnqueueIo("playerstate save", () => System.IO.File.WriteAllText(UserPlayerStatePath(userPath), json));
        }

        private static void UpsertUploadMap(PlayerState state, ServerMap map)
        {
            state.upload.RemoveAll(x => x.id == map.map.id);
            state.upload.Insert(0, new map_upload
            {
                id = map.map.id,
                name = map.map.name,
                time = map.map.date,
                url = map.map.url,
            });
        }

        private static void UpsertTopMap(PlayerState state, ServerMap map, int rank)
        {
            state.top.RemoveAll(x => x.id == map.map.id);
            state.top.Add(new map_top
            {
                id = map.map.id,
                name = map.map.name,
                rank = rank,
                url = map.map.url,
            });
            state.top.Sort((x, y) => x.rank.CompareTo(y.rank));
        }

        private static void IncrementPlayCount(PlayerState state, ServerMap map)
        {
            int mapId = map.map.id;
            state.playCountByMapId.TryGetValue(mapId, out int count);
            state.playCountByMapId[mapId] = count + 1;

            state.play.RemoveAll(x => x.id == mapId);
            state.play.Add(new map_play
            {
                id = mapId,
                name = map.map.name,
                play = count + 1,
                url = map.map.url,
            });
            state.play.Sort((x, y) => y.play.CompareTo(x.play));
        }

        private static int GetUserMapRank(ServerMap map, int userid)
        {
            int index = map.ranks.FindIndex(x => x.user_id == userid);
            return index < 0 ? 0 : map.ranks.Count - index;
        }

        private static void TrimPlayerStateLists(PlayerState state)
        {
            TrimList(state.recent);
            TrimList(state.upload);
            TrimList(state.top);
            TrimList(state.play);
        }

        private static void TrimList<T>(List<T> list)
        {
            if (list.Count > PlayerStateListLimit)
            {
                list.RemoveRange(PlayerStateListLimit, list.Count - PlayerStateListLimit);
            }
        }

        private class PlayerState
        {
            public player_data data { get; set; } = new player_data();
            public List<map_recent> recent { get; set; } = new List<map_recent>();
            public List<map_upload> upload { get; set; } = new List<map_upload>();
            public List<map_top> top { get; set; } = new List<map_top>();
            public List<map_play> play { get; set; } = new List<map_play>();
            public Dictionary<int, int> playCountByMapId { get; set; } = new Dictionary<int, int>();
        }
    }
}

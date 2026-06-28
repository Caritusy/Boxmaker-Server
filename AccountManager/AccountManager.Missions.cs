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
        private const int MissionEasyMapCount = 8;
        private const int MissionHardMapCount = 16;
        private const int MissionInitialLife = 100;
        private const int MissionMinimumPlayCount = 100;

        public static bool TryGetMissionData(cmsg_mission_view pckDat, out smsg_mission_view retDat)
        {
            retDat = new smsg_mission_view();

            if (!TryGetUserPath(pckDat.common.userid, out string userPath))
            {
                return false;
            }

            MissionData? mission = LoadMissionData(userPath);
            if (mission == null)
            {
                return false;
            }

            retDat = mission.missionData;
            return true;
        }

        public static bool TryStartMission(cmsg_mission_start pckDat, out smsg_mission_play retDat)
        {
            retDat = new();

            if (!TryGetUserPath(pckDat.common.userid, out string userPath))
            {
                return false;
            }

            MissionData? mission = LoadMissionData(userPath);
            if (mission == null)
            {
                return false;
            }

            if (!TryBuildMissionMapList(pckDat.hard, out List<ServerMap> missionMaps))
            {
                return false;
            }

            mission.randomMaps = missionMaps;
            mission.missionData.hard = pckDat.hard;
            mission.missionData.start = 1;
            mission.missionData.life = MissionInitialLife;
            mission.missionData.index = 0;

            SaveMissionData(userPath, mission);
            FillMissionPlayData(retDat, mission.randomMaps[0]);
            TrackRecentPlay(pckDat.common.userid, mission.randomMaps[0].map.id);
            return true;
        }

        public static bool TryContinueMission(cmsg_mission_continue pckDat, out smsg_mission_play retDat)
        {
            retDat = new();

            if (!TryGetUserPath(pckDat.common.userid, out string userPath))
            {
                return false;
            }

            MissionData? mission = LoadMissionData(userPath);
            if (!TryGetCurrentMissionMap(mission, out ServerMap map))
            {
                return false;
            }

            FillMissionPlayData(retDat, map);
            TrackRecentPlay(pckDat.common.userid, map.map.id);
            return true;
        }

        public static bool TryReplayMission(cmsg_mission_replay pckDat)
        {
            if (!TryGetUserPath(pckDat.common.userid, out string userPath))
            {
                return false;
            }

            MissionData? mission = LoadMissionData(userPath);
            if (mission == null)
            {
                return false;
            }

            mission.missionData.life = Math.Max(0, mission.missionData.life - 1);
            SaveMissionData(userPath, mission);
            return true;
        }

        public static bool TryClearMapMission(cmsg_mission_success pckDat, out object retDat, out int m_res)
        {
            retDat = new();
            m_res = 0;

            if (!TryGetUserPath(pckDat.common.userid, out string userPath))
            {
                return false;
            }

            MissionData? mission = LoadMissionData(userPath);
            if (mission == null || mission.randomMaps.Count == 0)
            {
                return false;
            }

            if (IsLastMissionMap(mission))
            {
                smsg_mission_finish finish = BuildMissionFinish(mission, suc: 1);

                if (!TryGetAccount(pckDat.common.userid, out smsg_login acc))
                {
                    return false;
                }

                acc.exp += finish.exp;
                TrySaveAccount(pckDat.common.userid, acc);
                CheckExp(pckDat.common.userid);

                m_res = -1;
                retDat = finish;
                CompleteMission(mission);
            }
            else
            {
                mission.missionData.index++;
                smsg_mission_play nextMap = new();
                ServerMap map = mission.randomMaps[mission.missionData.index];
                FillMissionPlayData(nextMap, map);
                TrackRecentPlay(pckDat.common.userid, map.map.id);
                retDat = nextMap;
            }

            SaveMissionData(userPath, mission);
            return true;
        }

        public static bool TryFailMission(cmsg_mission_fail pckDat, out object retDat, out int m_res)
        {
            retDat = new();
            m_res = 0;

            if (!TryGetUserPath(pckDat.common.userid, out string userPath))
            {
                return false;
            }

            MissionData? mission = LoadMissionData(userPath);
            if (mission == null)
            {
                return false;
            }

            mission.missionData.life = Math.Max(0, mission.missionData.life - 1);
            if (mission.missionData.life == 0)
            {
                retDat = BuildMissionFinish(mission, suc: 0);
                m_res = -1;
                ResetMission(mission);
            }
            else
            {
                retDat = null;
            }

            SaveMissionData(userPath, mission);
            return true;
        }

        public static bool TryDropMission(cmsg_mission_continue pckDat)
        {
            if (!TryGetUserPath(pckDat.common.userid, out string userPath))
            {
                return false;
            }

            MissionData? mission = LoadMissionData(userPath);
            if (mission == null)
            {
                return false;
            }

            ResetMission(mission);
            SaveMissionData(userPath, mission);
            return true;
        }

        private static MissionData? LoadMissionData(string userPath)
        {
            return LoadMissionDataAsync(userPath).GetAwaiter().GetResult();
        }

        private static async Task<MissionData?> LoadMissionDataAsync(string userPath)
        {
            string missionPath = UserMissionPath(userPath);
            if (!System.IO.File.Exists(missionPath))
            {
                await SaveMissionDataAsync(userPath, new MissionData());
            }

            string json = await System.IO.File.ReadAllTextAsync(missionPath);
            return JsonConvert.DeserializeObject<MissionData>(json);
        }

        private static void SaveMissionData(string userPath, MissionData mission)
        {
            SaveMissionDataAsync(userPath, mission).GetAwaiter().GetResult();
        }

        private static Task SaveMissionDataAsync(string userPath, MissionData mission)
        {
            return System.IO.File.WriteAllTextAsync(UserMissionPath(userPath), JsonConvert.SerializeObject(mission));
        }

        private static bool TryBuildMissionMapList(int hard, out List<ServerMap> missionMaps)
        {
            missionMaps = new List<ServerMap>();

            MissionPassRateRange? range = GetMissionPassRateRange(hard);
            if (range == null)
            {
                return false;
            }

            List<ServerMap> candidates = GetServerMapSnapshot()
                .Where(map => IsMissionCandidate(map, range.Value))
                .ToList();

            if (candidates.Count == 0)
            {
                return false;
            }

            int mapCount = GetMissionMapCount(hard);
            for (int i = 0; i < mapCount; i++)
            {
                missionMaps.Add(candidates[Random.Shared.Next(candidates.Count)]);
            }

            return true;
        }

        private static bool IsMissionCandidate(ServerMap map, MissionPassRateRange range)
        {
            if (map?.map == null || map.map.amount <= MissionMinimumPlayCount)
            {
                return false;
            }

            float passRate = GetMapPassRate(map);
            return passRate > range.MinExclusive && passRate <= range.MaxInclusive;
        }

        private static float GetMapPassRate(ServerMap map)
        {
            if (map.map.amount <= 0)
            {
                return 0f;
            }

            return (float)map.map.pas / map.map.amount;
        }

        private static MissionPassRateRange? GetMissionPassRateRange(int hard)
        {
            return hard switch
            {
                1 => new MissionPassRateRange(0.35f, 0.80f),
                2 => new MissionPassRateRange(0.10f, 0.35f),
                3 => new MissionPassRateRange(0.01f, 0.10f),
                4 => new MissionPassRateRange(float.NegativeInfinity, 0.01f),
                _ => null,
            };
        }

        private static int GetMissionMapCount(int hard)
        {
            return hard <= 2 ? MissionEasyMapCount : MissionHardMapCount;
        }

        private static bool TryGetCurrentMissionMap(MissionData? mission, out ServerMap map)
        {
            map = null;
            if (mission == null || mission.randomMaps.Count == 0)
            {
                return false;
            }

            if (mission.missionData.index < 0 || mission.missionData.index >= mission.randomMaps.Count)
            {
                return false;
            }

            map = mission.randomMaps[mission.missionData.index];
            return map != null;
        }

        private static bool IsLastMissionMap(MissionData mission)
        {
            return mission.missionData.index + 1 >= mission.randomMaps.Count;
        }

        private static smsg_mission_finish BuildMissionFinish(MissionData mission, int suc)
        {
            return new smsg_mission_finish
            {
                exp = suc == 1 ? CalculateMissionRewardExp(mission) : 0,
                authors = mission.GetAuthorList(),
                suc = suc,
            };
        }

        private static int CalculateMissionRewardExp(MissionData mission)
        {
            int hard = Math.Clamp(mission.missionData.hard, 1, 4);
            int divisor = 5 - hard;
            int baseExp = Random.Shared.Next(20 / divisor, 100 / divisor);
            int power = Random.Shared.Next(1, hard);
            return (int)Math.Pow(baseExp, power) + mission.missionData.life;
        }

        private static void CompleteMission(MissionData mission)
        {
            if (mission.missionData.br_max + 1 <= 4)
            {
                mission.missionData.br_max++;
            }

            ResetMission(mission);
        }

        private static void ResetMission(MissionData mission)
        {
            mission.missionData.index = 0;
            mission.missionData.start = 0;
            mission.missionData.hard = 0;
            mission.missionData.life = 0;
            mission.randomMaps.Clear();
        }

        private readonly struct MissionPassRateRange
        {
            public MissionPassRateRange(float minExclusive, float maxInclusive)
            {
                MinExclusive = minExclusive;
                MaxInclusive = maxInclusive;
            }

            public float MinExclusive { get; }

            public float MaxInclusive { get; }
        }
    }
}

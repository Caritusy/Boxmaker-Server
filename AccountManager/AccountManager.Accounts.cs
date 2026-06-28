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
        private static readonly object AccountCacheLock = new object();
        private static readonly Dictionary<int, string> AccountPathByUserId = new Dictionary<int, string>();
        private static readonly Dictionary<string, string> AccountPathByOpenId = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<int, smsg_login> AccountByUserId = new Dictionary<int, smsg_login>();
        private static readonly Dictionary<string, smsg_login> AccountByOpenId = new Dictionary<string, smsg_login>(StringComparer.OrdinalIgnoreCase);
        private static bool AccountPathCacheLoaded;

        public static bool TryGetUserPath(int uid, out string path)
        {
            EnsureAccountPathCache();
            lock (AccountCacheLock)
            {
                if (AccountPathByUserId.TryGetValue(uid, out string? cachedPath) && cachedPath != null)
                {
                    path = cachedPath;
                    return true;
                }
            }

            path = "";
            return false;
        }

        public static bool TryGetUserPath(string userName, out string path)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                path = "";
                return false;
            }

            EnsureAccountPathCache();
            lock (AccountCacheLock)
            {
                if (AccountPathByOpenId.TryGetValue(userName, out string? cachedPath) && cachedPath != null)
                {
                    path = cachedPath;
                    return true;
                }
            }

            path = "";
            return false;
        }

        public static bool TryGetAccount(int uid, out smsg_login loginData)
        {
            Init();
            lock (AccountCacheLock)
            {
                if (AccountByUserId.TryGetValue(uid, out smsg_login? cachedAccount) && cachedAccount != null)
                {
                    loginData = cachedAccount;
                    return true;
                }
            }

            string p = "";
            if (!TryGetUserPath(uid, out p))
            {
                loginData = null!;
                return false;
            }
            smsg_login? loadedAccount = JsonConvert.DeserializeObject<smsg_login>(System.IO.File.ReadAllText(UserInfoPath(p)));
            if (loadedAccount == null)
            {
                loginData = null!;
                return false;
            }

            loginData = loadedAccount;
            CacheAccount(loginData, p);
            return true;

        }

        public static bool TryGetAccount(string openid, out smsg_login loginData)
        {
            Init();
            if (string.IsNullOrWhiteSpace(openid))
            {
                loginData = null!;
                return false;
            }

            lock (AccountCacheLock)
            {
                if (AccountByOpenId.TryGetValue(openid, out smsg_login? cachedAccount) && cachedAccount != null)
                {
                    loginData = cachedAccount;
                    return true;
                }
            }

            string p = "";
            if (!TryGetUserPath(openid, out p))
            {
                loginData = null!;
                return false;
            }
            smsg_login? loadedAccount = JsonConvert.DeserializeObject<smsg_login>(System.IO.File.ReadAllText(UserInfoPath(p)));
            if (loadedAccount == null)
            {
                loginData = null!;
                return false;
            }

            loginData = loadedAccount;
            CacheAccount(loginData, p);
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
            QueueSaveAccount(p, loginData);
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
                QueueSaveAccount(p, loginData);
                return true;
            }
            catch { return false; }
        }

        public static bool TryWebLogin(string openid, string openkey, out smsg_login loginData)
        {
            loginData = null!;
            if (string.IsNullOrWhiteSpace(openid) || string.IsNullOrWhiteSpace(openkey))
            {
                return false;
            }

            if (!TryGetAccount(openid, out smsg_login account) || account.openkey != openkey)
            {
                return false;
            }

            loginData = account;
            return true;
        }

        public static bool TryUpdateWebPassword(int userid, string oldPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || !IsValidString(newPassword))
            {
                return false;
            }

            if (!TryGetAccount(userid, out smsg_login account) || account.openkey != oldPassword)
            {
                return false;
            }

            account.openkey = newPassword;
            return TrySaveAccount(userid, account);
        }

        public static bool TryUpdateWebProfile(int userid, string nickname, string nationality, int head)
        {
            if (string.IsNullOrWhiteSpace(nickname))
            {
                return false;
            }

            if (!TryGetAccount(userid, out smsg_login account))
            {
                return false;
            }

            account.name = nickname.Trim();
            account.nationality = string.IsNullOrWhiteSpace(nationality) ? account.nationality : nationality.Trim();
            account.head = Math.Max(0, head);
            bool saved = TrySaveAccount(userid, account);
            if (saved)
            {
                UpdatePlayerStateAccount(account);
            }

            return saved;
        }

        public static int GetExpToNextLevel(smsg_login account)
        {
            int nextLevel = Math.Min(account.level + 1, 60);
            if (!Utils.levelToExpNeed.TryGetValue(nextLevel, out int need))
            {
                return 0;
            }

            return Math.Max(0, need - account.exp);
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
            string accountPath = AccountDirectoryPath(accD.userid, accD.openid);
            Directory.CreateDirectory(accountPath);
            System.IO.File.WriteAllText(UserInfoPath(accountPath), JsonConvert.SerializeObject(accD));
            CacheAccount(accD, accountPath);
            CreatePlayerStateForNewAccount(accountPath, accD);
            return accD;
        }

        public static smsg_login RegisterAccount(int userid, cmsg_register regData)
        {
            Init();
            string userp = "";
            if (!TryGetUserPath(userid, out userp))
            {
                return null!;
            }
            var loginData = JsonConvert.DeserializeObject<smsg_login>(System.IO.File.ReadAllText(UserInfoPath(userp)));
            if (loginData == null)
            {
                return null!;
            }

            string newDire = AccountDirectoryPath(userid, regData.openid);
            if (!Directory.Exists(newDire))
            {
                Directory.CreateDirectory(newDire);
            }
            foreach (string d in Directory.GetFiles(userp))
            {
                string FileName = Path.GetFileName(d);
                System.IO.File.Copy(d, Path.Combine(newDire, FileName));
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
            System.IO.File.WriteAllText(UserInfoPath(newDire), JsonConvert.SerializeObject(loginData));
            RemoveCachedAccountPath(userp);
            CacheAccount(loginData, newDire);
            if (!System.IO.File.Exists(UserPlayerStatePath(newDire)))
            {
                CreatePlayerStateForNewAccount(newDire, loginData);
            }
            else
            {
                UpdatePlayerStateAccount(loginData);
            }
            return loginData;
        }

        public static void CheckExp(int userid)
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
            UpdatePlayerStateAccount(acc);
        }

        private static void EnsureAccountPathCache()
        {
            if (AccountPathCacheLoaded)
            {
                return;
            }

            lock (AccountCacheLock)
            {
                if (AccountPathCacheLoaded)
                {
                    return;
                }

                Init();
                AccountPathByUserId.Clear();
                AccountPathByOpenId.Clear();

                foreach (string accountPath in Directory.GetDirectories(AccDataPath))
                {
                    string directoryName = Path.GetFileName(accountPath);
                    string[] parts = directoryName.Split(AccountPathSeparator);
                    if (parts.Length == 0 || !int.TryParse(parts[0], out int userid))
                    {
                        continue;
                    }

                    AccountPathByUserId[userid] = accountPath;
                    if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
                    {
                        AccountPathByOpenId[parts[1]] = accountPath;
                    }
                }

                AccountPathCacheLoaded = true;
            }
        }

        private static void CacheAccount(smsg_login account, string accountPath)
        {
            if (account == null)
            {
                return;
            }

            lock (AccountCacheLock)
            {
                AccountPathByUserId[account.userid] = accountPath;
                if (!string.IsNullOrWhiteSpace(account.openid))
                {
                    AccountPathByOpenId[account.openid] = accountPath;
                    AccountByOpenId[account.openid] = account;
                }

                AccountByUserId[account.userid] = account;
                AccountPathCacheLoaded = true;
            }
        }

        private static void RemoveCachedAccountPath(string accountPath)
        {
            lock (AccountCacheLock)
            {
                foreach (int userid in AccountPathByUserId.Where(x => x.Value == accountPath).Select(x => x.Key).ToList())
                {
                    AccountPathByUserId.Remove(userid);
                    AccountByUserId.Remove(userid);
                }

                foreach (string openid in AccountPathByOpenId.Where(x => x.Value == accountPath).Select(x => x.Key).ToList())
                {
                    AccountPathByOpenId.Remove(openid);
                    AccountByOpenId.Remove(openid);
                }
            }
        }

        private static void QueueSaveAccount(string accountPath, smsg_login account)
        {
            CacheAccount(account, accountPath);
            string json = JsonConvert.SerializeObject(account);
            EnqueueIo("account save", () => System.IO.File.WriteAllText(UserInfoPath(accountPath), json));
        }
    }
}

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
        public static bool TryGetUserPath(int uid, out string path)
        {
            string[] d = Directory.GetDirectories(AccDataPath);
            foreach (string p in d)
            {
                string pathE = Path.GetFileName(p);
                string[] a = pathE.Split(AccountPathSeparator);
                if (a.Length > 0 && a[0] == uid.ToString())
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
                string[] a = pathE.Split(AccountPathSeparator);
                if (a.Length > 1 && a[1] == userName)
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
            loginData = JsonConvert.DeserializeObject<smsg_login>(System.IO.File.ReadAllText(UserInfoPath(p)));
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
            loginData = JsonConvert.DeserializeObject<smsg_login>(System.IO.File.ReadAllText(UserInfoPath(p)));
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
            System.IO.File.WriteAllText(UserInfoPath(p),JsonConvert.SerializeObject(loginData));
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
                System.IO.File.WriteAllText(UserInfoPath(p), JsonConvert.SerializeObject(loginData));
                return true;
            }
            catch { return false; }
        }

        public static bool TryWebLogin(string openid, string openkey, out smsg_login loginData)
        {
            loginData = null;
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
            CreatePlayerStateForNewAccount(accountPath, accD);
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
            var loginData = JsonConvert.DeserializeObject<smsg_login>(System.IO.File.ReadAllText(UserInfoPath(userp)));
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
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using protocol.game;

namespace BoxMaker_Server.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private const string WebUserSessionKey = "WebUserId";
        private const string CountryConfigRelativePath = @"Resources\config\t_guojia.txt";
        private const string LegacyCountryConfigPath = @"D:\UnityProject\Boxmaker\ExportedProject\Assets\Resources\config\t_guojia.txt";

        public string InitialUserJson { get; private set; } = "null";

        public string CountryOptionsJson { get; private set; } = "[]";

        public string AvatarOptionsJson { get; private set; } = "[]";

        public void OnGet()
        {
            InitialUserJson = JsonConvert.SerializeObject(BuildCurrentUserPayload());
            CountryOptionsJson = JsonConvert.SerializeObject(LoadCountryOptions());
            AvatarOptionsJson = JsonConvert.SerializeObject(GetAvatarOptions());
        }

        public IActionResult OnGetMe()
        {
            return new JsonResult(BuildCurrentUserPayload());
        }

        public IActionResult OnGetSearch(string q)
        {
            List<ServerMap> maps = AccountManager.SearchServerMapsForWeb(q, 24);
            return new JsonResult(new
            {
                ok = true,
                maps = maps.Select(ToMapPayload).ToList(),
            });
        }

        public IActionResult OnPostLogin([FromBody] LoginRequest request)
        {
            if (!AccountManager.TryWebLogin(request.openid, request.openkey, out smsg_login account))
            {
                return new JsonResult(new { ok = false, message = "账号或密码错误。" });
            }

            HttpContext.Session.SetInt32(WebUserSessionKey, account.userid);
            return new JsonResult(new { ok = true, user = BuildUserPayload(account) });
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Remove(WebUserSessionKey);
            return new JsonResult(new { ok = true });
        }

        public IActionResult OnPostProfile([FromBody] ProfileRequest request)
        {
            int? userid = HttpContext.Session.GetInt32(WebUserSessionKey);
            if (userid == null)
            {
                return new JsonResult(new { ok = false, message = "未登录。" });
            }

            HashSet<string> countryCodes = LoadCountryOptions()
                .Select(x => x.code)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!countryCodes.Contains(request.nationality))
            {
                return new JsonResult(new { ok = false, message = "请选择有效的国家/地区。" });
            }

            if (!GetAvatarOptions().Any(x => x.id == request.head))
            {
                return new JsonResult(new { ok = false, message = "请选择有效的头像。" });
            }

            if (!AccountManager.TryUpdateWebProfile(userid.Value, request.nickname, request.nationality, request.head))
            {
                return new JsonResult(new { ok = false, message = "资料保存失败。" });
            }

            AccountManager.TryGetAccount(userid.Value, out smsg_login account);
            return new JsonResult(new { ok = true, user = BuildUserPayload(account) });
        }

        public IActionResult OnPostPassword([FromBody] PasswordRequest request)
        {
            int? userid = HttpContext.Session.GetInt32(WebUserSessionKey);
            if (userid == null)
            {
                return new JsonResult(new { ok = false, message = "未登录。" });
            }

            bool ok = AccountManager.TryUpdateWebPassword(userid.Value, request.oldPassword, request.newPassword);
            return new JsonResult(new { ok, message = ok ? "密码已更新。" : "密码更新失败。" });
        }

        private object BuildCurrentUserPayload()
        {
            int? userid = HttpContext.Session.GetInt32(WebUserSessionKey);
            if (userid == null || !AccountManager.TryGetAccount(userid.Value, out smsg_login account))
            {
                return new { ok = false };
            }

            return new { ok = true, user = BuildUserPayload(account) };
        }

        private static object BuildUserPayload(smsg_login account)
        {
            AccountManager.TryGetPlayerInfo(account.userid, out smsg_view_player player);
            player_data data = player?.data ?? new player_data();

            return new
            {
                userid = account.userid,
                openid = account.openid,
                name = account.name,
                country = account.nationality,
                head = account.head,
                level = account.level,
                exp = account.exp,
                nextExp = AccountManager.GetExpToNextLevel(account),
                visitor = account.visitor,
                register = data.register,
                amount = data.amount,
                pas = data.pas,
                comment = data.comment,
                video = data.video,
                watched = data.watched,
            };
        }

        private static object ToMapPayload(ServerMap serverMap)
        {
            map_info map = serverMap.map;
            string title = string.IsNullOrWhiteSpace(map.name) ? "未命名地图" : map.name.Trim();
            int difficulty = map.difficulty > 0 ? map.difficulty : Utils.get_map_nd(map.pas, map.amount);
            int exp = Utils.get_map_exp(map.pas, map.amount);
            double passRate = map.amount <= 0 ? 0 : Math.Round((double)map.pas / map.amount * 100, 1);

            return new
            {
                id = map.id,
                name = title,
                icon = GetTitleIcon(title),
                ownerId = map.owner_id,
                ownerName = map.owner_name,
                country = map.country,
                date = map.date,
                amount = map.amount,
                pas = map.pas,
                passRate,
                difficulty,
                forcedDifficulty = map.difficulty > 0,
                exp,
                like = map.like,
                favorite = map.favorite,
                url = map.url == null || map.url.Length == 0 ? "" : Convert.ToBase64String(map.url),
            };
        }

        private static string GetTitleIcon(string title)
        {
            foreach (char value in title)
            {
                if (!char.IsWhiteSpace(value))
                {
                    return value.ToString();
                }
            }

            return "#";
        }

        private static List<CountryOption> LoadCountryOptions()
        {
            CountryOption unsetOption = new CountryOption { code = "--", name = "未设置", flag = "gq_000" };
            string? countryConfigPath = ResolveCountryConfigPath();
            if (countryConfigPath == null)
            {
                return new List<CountryOption>
                {
                    unsetOption,
                    new CountryOption { code = "CN", name = "中国" },
                };
            }

            List<CountryOption> options = System.IO.File.ReadLines(countryConfigPath)
                .Skip(2)
                .Select(line => line.Split('\t'))
                .Where(parts => parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[0]) && !string.IsNullOrWhiteSpace(parts[1]))
                .Select(parts => new CountryOption
                {
                    code = parts[0].Trim(),
                    name = parts[1].Trim(),
                    flag = parts.Length >= 3 ? parts[2].Trim() : "",
                })
                .ToList();

            if (!options.Any(x => string.Equals(x.code, unsetOption.code, StringComparison.OrdinalIgnoreCase)))
            {
                options.Insert(0, unsetOption);
            }

            return options;
        }

        private static string? ResolveCountryConfigPath()
        {
            string? configuredPath = Environment.GetEnvironmentVariable("BOXMAKER_COUNTRY_CONFIG");
            string[] candidates =
            {
                configuredPath ?? "",
                Path.Combine(AppContext.BaseDirectory, CountryConfigRelativePath),
                Path.Combine(Directory.GetCurrentDirectory(), CountryConfigRelativePath),
                LegacyCountryConfigPath,
            };

            return candidates.FirstOrDefault(path => !string.IsNullOrWhiteSpace(path) && System.IO.File.Exists(path));
        }

        private static List<AvatarOption> GetAvatarOptions()
        {
            return new List<AvatarOption>
            {
                new AvatarOption { id = 0, name = "游客" },
                new AvatarOption { id = 1, name = "兔子" },
                new AvatarOption { id = 2, name = "小猪" },
                new AvatarOption { id = 3, name = "白鸟" },
                new AvatarOption { id = 4, name = "魔花" },
                new AvatarOption { id = 5, name = "刺猬" },
                new AvatarOption { id = 6, name = "乌龟" },
                new AvatarOption { id = 7, name = "空白" },
            };
        }

        public class LoginRequest
        {
            public string openid { get; set; } = "";

            public string openkey { get; set; } = "";
        }

        public class ProfileRequest
        {
            public string nickname { get; set; } = "";

            public string nationality { get; set; } = "";

            public int head { get; set; }
        }

        public class PasswordRequest
        {
            public string oldPassword { get; set; } = "";

            public string newPassword { get; set; } = "";
        }

        public class CountryOption
        {
            public string code { get; set; } = "";

            public string name { get; set; } = "";

            public string flag { get; set; } = "";
        }

        public class AvatarOption
        {
            public int id { get; set; }

            public string name { get; set; } = "";
        }
    }
}

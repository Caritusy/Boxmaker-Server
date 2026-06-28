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

        public string InitialUserJson { get; private set; } = "null";

        public void OnGet()
        {
            InitialUserJson = JsonConvert.SerializeObject(BuildCurrentUserPayload());
        }

        public IActionResult OnGetMe()
        {
            return new JsonResult(BuildCurrentUserPayload());
        }

        public IActionResult OnGetSearch(string q)
        {
            List<map_show> maps = AccountManager.SearchMapsForWeb(q, 20);
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
                return new JsonResult(new { ok = false, message = "账号或密码错误" });
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
                return new JsonResult(new { ok = false, message = "未登录" });
            }

            if (!AccountManager.TryUpdateWebProfile(userid.Value, request.nickname, request.nationality, request.head))
            {
                return new JsonResult(new { ok = false, message = "资料保存失败" });
            }

            AccountManager.TryGetAccount(userid.Value, out smsg_login account);
            return new JsonResult(new { ok = true, user = BuildUserPayload(account) });
        }

        public IActionResult OnPostPassword([FromBody] PasswordRequest request)
        {
            int? userid = HttpContext.Session.GetInt32(WebUserSessionKey);
            if (userid == null)
            {
                return new JsonResult(new { ok = false, message = "未登录" });
            }

            bool ok = AccountManager.TryUpdateWebPassword(userid.Value, request.oldPassword, request.newPassword);
            return new JsonResult(new { ok, message = ok ? "密码已更新" : "密码更新失败" });
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

        private static object ToMapPayload(map_show map)
        {
            return new
            {
                id = map.id,
                name = map.name,
                amount = map.amount,
                pas = map.pas,
                difficulty = map.difficulty,
                like = map.like,
                url = map.url == null || map.url.Length == 0 ? "" : Convert.ToBase64String(map.url),
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
    }
}

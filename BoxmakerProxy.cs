using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoBuf;
using protocol.game;
using System.Collections;
using System.Net;
using System.Text;

namespace BoxMaker_Server
{
    public class BoxmakerProxy : Controller
    {
        public static BoxmakerProxy Instance { get; private set; }

        public BoxmakerProxy()
        {
            if (Instance == null) Instance = this;
        }

        private static Dictionary<int,string> KeepAliveDict = new Dictionary<int,string>();

        public static int GetOnlinePlayerCount()
        {
            return KeepAliveDict.Count;
        }

        public static string ServerVerD = "1.121";

        public bool UserTokenVerify(int userid,string sig)
        {
            string token = "";
            if (!KeepAliveDict.TryGetValue(userid, out token))
            {
                Console.WriteLine($"[red][服务端] 由于重启/客户端token已过期太久,无法找到UID:{userid}的token，下发过期提示。");
                return false;
            }
            if (token != sig)
            {
                Console.WriteLine($"[red][服务端] {userid}的Token{sig}已过期，下发过期提示。");
                return false;
            }
            return true;

        }

        [HttpPost("/10001")]
        public async Task<IActionResult> OnLogin()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_login loginData = net_http.parse_packet_client<cmsg_login>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 登录 ：{JsonConvert.SerializeObject(loginData)}");

                    smsg_login loginRet = new smsg_login();

                    loginData.ver = loginData.ver.Replace("yymoon_","").Replace("_","");

                    var ServerVer = new VersionCon(ServerVerD);
					var ClientVer = new VersionCon(loginData.ver);
                    if (ServerVer > ClientVer)
                    {
                       return net_http.ret_msg(null, -5);
					}

					if (loginData.openid == "" && loginData.openkey == "" && loginData.nationality == "")
                    {
                        loginRet = AccountManager.NewGuest();
                    }
                    else if (AccountManager.TryGetAccount(loginData.openid, out loginRet))
                    {
                        if (loginRet.openkey != loginData.openkey)
                        {
                            loginRet = AccountManager.NewGuest();
                            return net_http.ret_msg(loginRet);
                        }
                        loginRet.sig = $"{Guid.NewGuid().ToString().Replace("-","")}";
                        if (KeepAliveDict.ContainsKey(loginRet.userid))
                        {
                            KeepAliveDict.Remove(loginRet.userid);
                        }
                        KeepAliveDict.Add(loginRet.userid, loginRet.sig);
                        AccountManager.CheckExp(loginRet.userid);
                    }
                    else
                    {
                        loginRet = AccountManager.NewGuest();
                        return net_http.ret_msg(loginRet);
                    }



                    return net_http.ret_msg(loginRet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解密失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10002")]
        public async Task<IActionResult> OnRegister()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_register loginData = net_http.parse_packet_client<cmsg_register>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 注册 ：{JsonConvert.SerializeObject(loginData)}");

                    smsg_login loginRet = new smsg_login();

                    if (AccountManager.TryGetAccount(loginData.openid, out loginRet))
                    {
                        return net_http.ret_msg(null, -11);
                    }

                    if (!AccountManager.IsValidString(loginData.openid) || !AccountManager.IsValidString(loginData.openkey))
                    {
                        return net_http.ret_msg(null,-23);
                    }

                    loginRet = AccountManager.RegisterAccount(loginData.common.userid, loginData);

                    loginRet.sig = $"{Guid.NewGuid().ToString().Replace("-", "")}";
                    if (KeepAliveDict.ContainsKey(loginRet.userid))
                    {
                        KeepAliveDict.Remove(loginRet.userid);
                    }
                    KeepAliveDict.Add(loginRet.userid, loginRet.sig);

                    //else
                    //{
                    //    return Content(net_http.ret_msg(loginRet, -8), "application/x-www-form-urlencoded");
                    //}

                    return net_http.ret_msg(loginRet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"注册失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10003")]
        public async Task<IActionResult> OnChangeAccount()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_change_account loginData = net_http.parse_packet_client<cmsg_change_account>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 切换账户 ：{JsonConvert.SerializeObject(loginData)}");

                    smsg_login loginRet = new smsg_login();

                    if (AccountManager.TryGetAccount(loginData.openid, out loginRet))
                    {
                        if (loginRet.openkey != loginData.openkey)
                        {
                            return net_http.ret_msg(null, -10);
                        }
                    }
                    else
                    {
                        return net_http.ret_msg(null, -8);
                    }

                    loginRet.sig = $"{Guid.NewGuid().ToString().Replace("-", "")}";
                    if (KeepAliveDict.ContainsKey(loginRet.userid))
                    {
                        KeepAliveDict.Remove(loginRet.userid);
                    }
                    KeepAliveDict.Add(loginRet.userid, loginRet.sig);

                    return net_http.ret_msg(loginRet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"切换账户失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10004")]
        public async Task<IActionResult> OnGetMapListInfo()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    AccountManager.serverMaps = AccountManager.GetServerMapList();

                    cmsg_view_map loginData = net_http.parse_packet_client<cmsg_view_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 获取地图列表 ：{JsonConvert.SerializeObject(loginData)}");

                    smsg_view_map retDat;

                    if (!AccountManager.TryGetMapList(loginData, out retDat))
                    {
                        net_http.ret_msg(null, -2);
                    }

                    return net_http.ret_msg(retDat);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取地图列表失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10005")]
        public async Task<IActionResult> OnGetMapInfo()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_view_comment loginData = net_http.parse_packet_client<cmsg_view_comment>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 获取地图信息 ：{JsonConvert.SerializeObject(loginData)}");

                    smsg_view_comment retDat = new smsg_view_comment();
                    int id = 0;
                    if (loginData.id < 10010000)
                    {
                        if (loginData.id == 0) loginData.id += 1;
                        id = 10010000 + loginData.id;
                    }
                    else
                    {
                        id = loginData.id;
                    }
                    ServerMap map = AccountManager.GetMapInfo(id);

                    List<comment> cmts = map.comments.ToList();
                    cmts.Reverse();
                    string userP = "";
                    if (!AccountManager.TryGetUserPath(loginData.common.userid, out userP))
                    {

                    }
                    AccountManager.MapCheck(userP);
                    var ClearList = JsonConvert.DeserializeObject<List<int>>(BoxMaker_Server.File.ReadAllTextAsync(userP + "/clearlist").Result);
                    map.map.finish = ClearList.Contains(map.map.id) ? 1 : 0;
                    retDat.infos = map.map;
                    retDat.comments = cmts;
                    //retDat.infos.

                    return net_http.ret_msg(retDat);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取地图信息失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10006")]
        public async Task<IActionResult> OnPlayMap()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_play_map loginData = net_http.parse_packet_client<cmsg_play_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 游玩地图 ：{JsonConvert.SerializeObject(loginData)}");

                    smsg_play_map retDat = new smsg_play_map();
                    int id = 0;
                    if (loginData.id < 10010000)
                    {
                        id = 10010000 + loginData.id;
                    }
                    else
                    {
                        id = loginData.id;
                    }
                    ServerMap map = AccountManager.GetMapInfo(id);
                    retDat.map_data = map.mapData.data;
                    retDat.y = map.FailureY;
                    retDat.x = map.FailureX;

                    AccountManager.SetPlayingMap(loginData.common.sig, id);

                    return net_http.ret_msg(retDat);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"游玩地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10007")]
        public async Task<IActionResult> OnGetMapPlayResult()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_complete_map loginData = net_http.parse_packet_client<cmsg_complete_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 完成地图 ：{JsonConvert.SerializeObject(loginData)}");

                    smsg_complete_map retDat = new smsg_complete_map();

                    if (!AccountManager.TryCompleteMap(loginData, out retDat))
                    {
                        return net_http.ret_msg(null,-2);
                    }

                    return net_http.ret_msg(retDat);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"完成地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10008")]
        public async Task<IActionResult> OnSearch()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_search_map loginData = net_http.parse_packet_client<cmsg_search_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 搜索地图 ：{JsonConvert.SerializeObject(loginData)}");

                    smsg_view_map retDat;

                    if (!AccountManager.TrySearchMap(loginData, out retDat))
                    {
                        return net_http.ret_msg(null,-5);
                    }

                    return net_http.ret_msg(retDat);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"搜索地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10009")]
        public async Task<IActionResult> OnFavoriteMap()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_favorite_map loginData = net_http.parse_packet_client<cmsg_favorite_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 重玩地图 ：{JsonConvert.SerializeObject(loginData)}");

                    if (true)
                    {
                        return net_http.ret_msg(null, -2);
                    }

                    return net_http.ret_msg(null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"重玩地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10010")]
        public async Task<IActionResult> OnReplayMap()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_replay_map loginData = net_http.parse_packet_client<cmsg_replay_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 重玩地图 ：{JsonConvert.SerializeObject(loginData)}");


                    if (!AccountManager.TryReplayMap(loginData))
                    {
                        return net_http.ret_msg(null, -2);
                    }

                    return net_http.ret_msg(null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"重玩地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10011")]
        public async Task<IActionResult> OnGetEditMaps()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_view_edit loginData = net_http.parse_packet_client<cmsg_view_edit>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 获取编辑地图 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    List<edit_data> maps = new List<edit_data>();
                    if (!AccountManager.TryGetEditList(loginData.common.userid, out maps))
                    {
                        return net_http.ret_msg(null, -2);;
                    }


                    smsg_view_edit retMsg = new smsg_view_edit();
                    retMsg.exp = 114514;
                    retMsg.level = 10;
                    retMsg.infos = maps;

                    return net_http.ret_msg(retMsg);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取编辑地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10012")]
        public async Task<IActionResult> OnCreateMap()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_create_map loginData = net_http.parse_packet_client<cmsg_create_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 创建地图 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    smsg_create_map retDat = new smsg_create_map();

                    edit_data map;
                    if (!AccountManager.TryCreateMap(loginData.common.userid,loginData.index + 1, new byte[0], DateTime.Now.ToString("yyyy-MM-dd HH:mm"), out map))
                    {
                        return net_http.ret_msg(null, -2);
                    }
                    retDat.map = map;

                    return net_http.ret_msg(retDat);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10013")]
        public async Task<IActionResult> OnEditOrPlayMap()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_play_edit_map loginData = net_http.parse_packet_client<cmsg_play_edit_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 编辑或游玩地图 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    smsg_play_edit_map retDat = new smsg_play_edit_map();

                    List<MapDataHelper> maps;
                    if (!AccountManager.TryGetEditInfoList(loginData.common.userid, out maps))
                    {
                        return net_http.ret_msg(null,-2);
                    }
                    MapDataHelper mapdata = maps[loginData.id-1];
                    if (mapdata.data == null)
                    {
                        mapdata.data = AccountManager.DefaultMapArray;
                    }
                    retDat.mapdata = mapdata.data;

                    return net_http.ret_msg(retDat);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"编辑或游玩失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10014")]
        public async Task<IActionResult> OnSaveMap()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_save_map loginData = net_http.parse_packet_client<cmsg_save_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 保存地图 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    if (!AccountManager.TrySaveMap(loginData.common.userid, loginData))
                    {
                        return net_http.ret_msg(null,-2);
                    }
                        //return net_http.ret_msg(null, -2);;
                    

                    return net_http.ret_msg(null);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10015")]
        public async Task<IActionResult> OnChangeMapName()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_change_map_name loginData = net_http.parse_packet_client<cmsg_change_map_name>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 地图改名 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    if (!AccountManager.TryChangeMapName(loginData.common.userid, loginData))
                    {
                        return net_http.ret_msg(null, -2);
                    }

                    return net_http.ret_msg(null);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"地图改名失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10016")]
        public async Task<IActionResult> OnUploadMap()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_upload_map loginData = net_http.parse_packet_client<cmsg_upload_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 上传地图 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }


                    if (!AccountManager.TryUploadMap(loginData.common.userid, loginData))
                    {
                        return net_http.ret_msg(null, -2);
                    }

                    return net_http.ret_msg(null);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"上传地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10017")]
        public async Task<IActionResult> OnDeleteMap()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_delete_map loginData = net_http.parse_packet_client<cmsg_delete_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 删除地图 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    if (!AccountManager.TryDeleteMap(loginData.common.userid, loginData))
                    {
                        return net_http.ret_msg(null, -2);
                    }

                    return net_http.ret_msg(null);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10018")]
        public async Task<IActionResult> OnComment()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_comment loginData = net_http.parse_packet_client<cmsg_comment>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 评论 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    

                    smsg_comment retDat = new smsg_comment();

                    if (!AccountManager.TryComment(loginData.id, loginData))
                    {
                        return net_http.ret_msg(null, -2);
                    }
                    ServerMap map = AccountManager.GetMapInfo(loginData.id);
                    retDat.comment = map.comments[0];

                    return net_http.ret_msg(retDat);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"评论失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10019")]
        public async Task<IActionResult> OnCheckRanking()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_view_map_point_rank loginData = net_http.parse_packet_client<cmsg_view_map_point_rank>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 查看排行榜 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    ServerMap map = AccountManager.GetMapInfo(loginData.map_id);

                    smsg_view_map_point_rank retDat = new smsg_view_map_point_rank();

                    var ret = map.ranks.ToList();
                    ret.Reverse();
                    retDat.ranks = ret;
                    //if (!AccountManager.TryUploadMap(loginData.common.userid, loginData))
                    //{
                    //    return net_http.ret_msg(null, -2);
                    //}

                    return net_http.ret_msg(retDat);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查看排行榜失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10020")]
        public async Task<IActionResult> OnCheckRecord()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_view_video loginData = net_http.parse_packet_client<cmsg_view_video>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 查看录像 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    ServerMap map = AccountManager.GetMapInfo(loginData.map_id);

                    smsg_view_video retDat = new smsg_view_video();

                    retDat.map_data = map.mapData.data;
                    retDat.video_data = map.recs.Where(x => x.video_id == loginData.video_id).First().recData;
                    //if (!AccountManager.TryUploadMap(loginData.common.userid, loginData))
                    //{
                    //    return net_http.ret_msg(null, -2);
                    //}

                    return net_http.ret_msg(retDat);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查看录像失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10021")]
        public async Task<IActionResult> OnGetEditMapSingle()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_view_edit_single loginData = net_http.parse_packet_client<cmsg_view_edit_single>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 获取编辑地图 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    List<edit_data> maps = new List<edit_data>();
                    if (!AccountManager.TryGetEditList(loginData.common.userid, out maps))
                    {
                        return net_http.ret_msg(null, -2);;
                    }


                    smsg_view_edit_single retMsg = new smsg_view_edit_single();
                    retMsg.info = maps.Where(m => m.id == loginData.map_id).FirstOrDefault();

                    return net_http.ret_msg(retMsg);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取编辑地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10023")]
        public async Task<IActionResult> OnGetPlayerInfo()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_view_player loginData = net_http.parse_packet_client<cmsg_view_player>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 获取玩家信息 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    smsg_login acc;
                    if (!AccountManager.TryGetAccount(loginData.userid,out acc))
                    {
                        return net_http.ret_msg(null, -2);
                    }

                    smsg_view_player retDat = new smsg_view_player();
                    retDat.data = new player_data() 
                    {
                        country = acc.nationality,
                        exp = acc.exp,
                        head = acc.head,
                        level = acc.level,
                        mexp = 114514,
                        mlevel = 10,
                        name = acc.name,
                        userid = acc.userid,
                        point = 0,
                        visitor = acc.visitor,
                        video = 200,
                        register = "2018-08-09 99:99:99",
                        watched = 100,
                        amount = 11912,
                        comment = 455,
                        pas = 884554
                    };

                    return net_http.ret_msg(retDat);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取玩家信息失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10024")]
        public async Task<IActionResult> OnCompleteEditGuide()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_complete_guide loginData = net_http.parse_packet_client<cmsg_complete_guide>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 通过编辑教程 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    if (!AccountManager.CompleteEditGuide(loginData.common.userid, loginData))
                    {
                        return net_http.ret_msg(null, -2);
                    }
                    
                    return net_http.ret_msg(null);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"通过编辑教程失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10025")]
        public async Task<IActionResult> OnLoadMission()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    var loginData = net_http.parse_packet_client<cmsg_mission_view>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 加载百人团信息 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    //if (true)
                    //{
                    //    return net_http.ret_msg(null, -2);
                    //}
                    smsg_mission_view retDat;
                    if (!AccountManager.TryGetMissionData(loginData, out retDat))
                    {
						return net_http.ret_msg(null, -2);
					}
                    
                    return net_http.ret_msg(retDat);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载百人团信息失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

		[HttpPost("/10026")]
		public async Task<IActionResult> OnStartMission()
		{
			try
			{
				// 读取请求的Body数据
				using (MemoryStream ms = new MemoryStream())
				{
					await Request.Body.CopyToAsync(ms);
					byte[] requestBody = ms.ToArray();

					var loginData = net_http.parse_packet_client<cmsg_mission_start>(requestBody);

					Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 开打百人团 ：{JsonConvert.SerializeObject(loginData)}");

					if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
					{
						return net_http.ret_msg(null, -4);
					}

					//if (true)
					//{
					//    return net_http.ret_msg(null, -2);
					//}
					smsg_mission_play retDat;
					if (!AccountManager.TryStartMission(loginData, out retDat))
					{
						return net_http.ret_msg(null, -2);
					}

					return net_http.ret_msg(retDat);

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"开打百人团失败: {ex}");
				return net_http.ret_msg(null, -2); ;
			}
		}

		[HttpPost("/10027")]
		public async Task<IActionResult> OnContiuneMission()
		{
			try
			{
				// 读取请求的Body数据
				using (MemoryStream ms = new MemoryStream())
				{
					await Request.Body.CopyToAsync(ms);
					byte[] requestBody = ms.ToArray();

					var loginData = net_http.parse_packet_client<cmsg_mission_continue>(requestBody);

					Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 继续百人团 ：{JsonConvert.SerializeObject(loginData)}");

					if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
					{
						return net_http.ret_msg(null, -4);
					}

					//if (true)
					//{
					//    return net_http.ret_msg(null, -2);
					//}
					smsg_mission_play retDat;
					if (!AccountManager.TryContinueMission(loginData, out retDat))
					{
						return net_http.ret_msg(null, -2);
					}

					return net_http.ret_msg(retDat);

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"继续百人团失败: {ex}");
				return net_http.ret_msg(null, -2); ;
			}
		}

		[HttpPost("/10028")]
		public async Task<IActionResult> OnReplayMission()
		{
			try
			{
				// 读取请求的Body数据
				using (MemoryStream ms = new MemoryStream())
				{
					await Request.Body.CopyToAsync(ms);
					byte[] requestBody = ms.ToArray();

					var loginData = net_http.parse_packet_client<cmsg_mission_replay>(requestBody);

					Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 重玩百人团 ：{JsonConvert.SerializeObject(loginData)}");

					if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
					{
						return net_http.ret_msg(null, -4);
					}

					//if (true)
					//{
					//    return net_http.ret_msg(null, -2);
					//}
					if (!AccountManager.TryReplayMission(loginData))
					{
						return net_http.ret_msg(null, -2);
					}

                    return net_http.ret_msg(null);

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"重玩百人团失败: {ex}");
				return net_http.ret_msg(null, -2); ;
			}
		}

		[HttpPost("/10029")]
		public async Task<IActionResult> OnFailMission()
		{
			try
			{
				// 读取请求的Body数据
				using (MemoryStream ms = new MemoryStream())
				{
					await Request.Body.CopyToAsync(ms);
					byte[] requestBody = ms.ToArray();

					var loginData = net_http.parse_packet_client<cmsg_mission_fail>(requestBody);

					Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 失败百人团 ：{JsonConvert.SerializeObject(loginData)}");

					if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
					{
						return net_http.ret_msg(null, -4);
					}

					//if (true)
					//{
					//    return net_http.ret_msg(null, -2);
					//}
					object retDat = new object();
					int m_res = 0;
					if (!AccountManager.TryFailMission(loginData, out retDat,out m_res))
					{
						return net_http.ret_msg(null, -2);
					}

					return net_http.ret_msg(retDat, m_res);

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"失败百人团失败: {ex}");
				return net_http.ret_msg(null, -2); ;
			}
		}

		[HttpPost("/10030")]
		public async Task<IActionResult> OnClearMission()
		{
			try
			{
				// 读取请求的Body数据
				using (MemoryStream ms = new MemoryStream())
				{
					await Request.Body.CopyToAsync(ms);
					byte[] requestBody = ms.ToArray();

					var loginData = net_http.parse_packet_client<cmsg_mission_success>(requestBody);

					Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 通过百人团 ：{JsonConvert.SerializeObject(loginData)}");

					if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
					{
						return net_http.ret_msg(null, -4);
					}

                    //if (true)
                    //{
                    //    return net_http.ret_msg(null, -2);
                    //}
                    object retDat = new object();
                    int m_res = 0;
					if (!AccountManager.TryClearMapMission(loginData,out retDat,out m_res))
					{
						return net_http.ret_msg(null, -2);
					}

					return net_http.ret_msg(retDat,m_res);

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"通过百人团失败: {ex}");
				return net_http.ret_msg(null, -2); ;
			}
		}

		[HttpPost("/10044")]
        public async Task<IActionResult> OnLikeMap()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_map_like loginData = net_http.parse_packet_client<cmsg_map_like>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 点赞地图 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!AccountManager.TryLikeMap(loginData))
                    {
                        return net_http.ret_msg(null, -2);
                    }

					return net_http.ret_msg(null);
				}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"点赞地图失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

        [HttpPost("/10045")]
        public async Task<IActionResult> OnDownloadMap()
        {
            try
            {
                // 读取请求的Body数据
                using (MemoryStream ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] requestBody = ms.ToArray();

                    cmsg_download_map loginData = net_http.parse_packet_client<cmsg_download_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 查看录像 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    return net_http.ret_msg(null, AccountManager.TryDownloadMap(loginData));

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查看录像失败: {ex}");
                return net_http.ret_msg(null, -2);;
            }
        }

		[HttpPost("/10047")]
		public async Task<IActionResult> OnDropMission()
		{
			try
			{
				// 读取请求的Body数据
				using (MemoryStream ms = new MemoryStream())
				{
					await Request.Body.CopyToAsync(ms);
					byte[] requestBody = ms.ToArray();

					var loginData = net_http.parse_packet_client<cmsg_mission_continue>(requestBody);

					Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 中断百人团 ：{JsonConvert.SerializeObject(loginData)}");

					if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
					{
						return net_http.ret_msg(null, -4);
					}

					//if (true)
					//{
					//    return net_http.ret_msg(null, -2);
					//}
					if (!AccountManager.TryDropMission(loginData))
					{
						return net_http.ret_msg(null, -2);
					}

                    return net_http.ret_msg(null);

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"中断百人团失败: {ex}");
				return net_http.ret_msg(null, -2); ;
			}
		}

	}
}

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoBuf;
using protocol.game;
using System.Collections;
using System.Net;
using System.Text;


namespace BoxMaker_Server
{
    public partial class BoxmakerProxy
    {
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

                    cmsg_view_map loginData = net_http.parse_packet_client<cmsg_view_map>(requestBody);

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 获取地图列表 ：{JsonConvert.SerializeObject(loginData)}");

                    smsg_view_map retDat;

                    if (!AccountManager.TryGetMapList(loginData, out retDat))
                    {
                        return net_http.ret_msg(null, -2);
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
                    if (map == null)
                    {
                        return net_http.ret_msg(null, -2);
                    }

                    List<comment> cmts = map.comments.ToList();
                    cmts.Reverse();
                    retDat.infos = AccountManager.BuildMapInfoForUser(map, loginData.common.userid);
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
                    if (map == null)
                    {
                        return net_http.ret_msg(null, -2);
                    }

                    retDat.map_data = map.mapData.data;
                    retDat.y = map.FailureY;
                    retDat.x = map.FailureX;

                    AccountManager.SetPlayingMap(loginData.common.sig, id);
                    AccountManager.TrackRecentPlay(loginData.common.userid, id);

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

                    Console.WriteLine($"[green][客户端 - {base.HttpContext.Connection.RemoteIpAddress}:{base.HttpContext.Connection.RemotePort}] 收藏地图 ：{JsonConvert.SerializeObject(loginData)}");

                    if (!UserTokenVerify(loginData.common.userid, loginData.common.sig))
                    {
                        return net_http.ret_msg(null, -4);
                    }

                    if (!AccountManager.TryToggleFavoriteMap(loginData, out smsg_favorite_map retDat))
                    {
                        return net_http.ret_msg(null, -2);
                    }

                    return net_http.ret_msg(retDat);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"收藏地图失败: {ex}");
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

                    if (!AccountManager.TryViewMapReplay(loginData, out smsg_view_video retDat))
                    {
                        return net_http.ret_msg(null, -2);
                    }
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
    }
}

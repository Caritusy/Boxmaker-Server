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

                    smsg_view_player retDat;
                    if (!AccountManager.TryGetPlayerInfo(loginData.userid, out retDat))
                    {
                        return net_http.ret_msg(null, -2);
                    }

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
    }
}

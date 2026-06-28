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
    }
}

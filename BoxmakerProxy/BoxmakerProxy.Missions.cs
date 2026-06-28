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

                    // The client treats m_res == -1 as "mission finished" and reads msg_response.error.
                    return net_http.ret_msg(retDat,m_res);

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"通过百人团失败: {ex}");
				return net_http.ret_msg(null, -2); ;
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

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoBuf;
using protocol.game;
using System.IO;
using System.Text;

namespace BoxMaker_Server
{
    public class net_http
    {
        private static string ByteArrayToHexString(byte[] byteArray)
        {
            // 创建一个字符串构建器来构建十六进制字符串
            var sb = new System.Text.StringBuilder(byteArray.Length * 2);

            // 将每个字节转换为两位十六进制表示，并添加到字符串构建器中
            foreach (byte b in byteArray)
            {
                sb.Append(b.ToString("X2"));
            }

            // 返回十六进制字符串
            return sb.ToString();
        }

        public static T parse_packet<T>(byte[] bytes)
        {
            MemoryStream source = new MemoryStream(bytes);
            object obj = new object();
            obj = Serializer.Deserialize<T>(source);
            return (T)obj;
        }

        public static T parse_packet_client<T>(byte[] bytes)
        {
            byte[] decryptedData = decrypt_des.decode(bytes);
            MemoryStream source = new MemoryStream(decryptedData);
            object obj = new object();
            obj = Serializer.Deserialize<T>(source);
            return (T)obj;
        }

        public static FileContentResult ret_msg(object obj,int res = 0,byte[] error = null)
        {
            MemoryStream memoryStream = new MemoryStream();
            Serializer.Serialize(memoryStream, obj);

            msg_response msg = new msg_response();
            msg.res = res;
            msg.msg = memoryStream.ToArray();
            msg.error = error;

            MemoryStream retMS = new MemoryStream();
            Serializer.Serialize(retMS, msg);
            var buffer = retMS.ToArray();
            if (res >= -1)
            {
                Console.WriteLine($"[green][服务器] 服务器回送类型：{(obj != null ? obj.GetType().Name : "null")} 状态码：{res} 数据大小：{buffer.Length}");
            }
            else
            {
                Console.WriteLine($"[red][服务器] 服务器回送类型：{(obj != null ? obj.GetType().Name : "null")} 状态码：{res} 数据：{JsonConvert.SerializeObject(obj)}");
            }
            return BoxmakerProxy.Instance.File(buffer, "application/x-www-form-urlencoded");

        }
    }
}

using Renci.SshNet.Security.Cryptography.Ciphers;
using Renci.SshNet.Security.Cryptography.Ciphers.Modes;
using Renci.SshNet.Security.Cryptography.Ciphers.Paddings;
using System.Text;

namespace BoxMaker_Server
{
    public class decrypt_des
    {
        private static string ekey = "tsjhtsjh";

        private static string eiv = "51478543";

        public static byte[] encode(byte[] inputByteArray)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(ekey);
            byte[] bytes2 = Encoding.UTF8.GetBytes(eiv);
            PKCS5Padding pKCS5Padding = new PKCS5Padding();
            byte[] data = pKCS5Padding.Pad(8, inputByteArray);
            DesCipher desCipher = new DesCipher(bytes, new CbcCipherMode(bytes2), null);
            return desCipher.Encrypt(data);
        }

        public static byte[] decode(byte[] inputByteArray)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(ekey);
            byte[] bytes2 = Encoding.UTF8.GetBytes(eiv);
            PKCS5Padding pKCS5Padding = new PKCS5Padding();
            DesCipher desCipher = new DesCipher(bytes, new CbcCipherMode(bytes2), null);
            byte[] decryptedData = desCipher.Decrypt(inputByteArray);

            // 解除填充
            int paddingLength = decryptedData[decryptedData.Length - 1];
            int unpaddedLength = decryptedData.Length - paddingLength;

            byte[] unpaddedData = new byte[unpaddedLength];
            Array.Copy(decryptedData, 0, unpaddedData, 0, unpaddedLength);

            return unpaddedData;
        }
    }
}

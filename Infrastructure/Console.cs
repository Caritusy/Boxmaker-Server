using System.Text.RegularExpressions;

namespace BoxMaker_Server
{
    public class Console
    {
        public static void WriteLine(string text)
        {
            int startIndex = 0;

            while (startIndex < text.Length)
            {
                int openBracket = text.IndexOf('[', startIndex);
                int closeBracket = text.IndexOf(']', openBracket);

                // 如果没有找到更多的标记，输出剩余的文本
                if (openBracket == -1 || closeBracket == -1)
                {
                    System.Console.Write(text.Substring(startIndex));
                    break;
                }

                // 输出标记之前的文本
                if (openBracket > startIndex)
                {
                    System.Console.Write(text.Substring(startIndex, openBracket - startIndex));
                }

                // 获取颜色
                string color = text.Substring(openBracket + 1, closeBracket - openBracket - 1).ToLower();

                // 设置控制台颜色，如果颜色匹配则改变颜色，否则输出原始标记
                if (IsValidColor(color))
                {
                    SetConsoleColor(color);
                    startIndex = closeBracket + 1;
                }
                else
                {
                    System.Console.Write(text.Substring(openBracket, closeBracket - openBracket + 1));
                    startIndex = closeBracket + 1;
                }

                // 找到下一个标记
                int nextOpenBracket = text.IndexOf('[', startIndex);

                // 如果没有更多的标记，则输出剩余文本并退出循环
                if (nextOpenBracket == -1)
                {
                    System.Console.WriteLine(text.Substring(startIndex));
                    break;
                }

                // 输出当前颜色标记后的文本
                System.Console.Write(text.Substring(startIndex, nextOpenBracket - startIndex));

                // 更新起始位置
                startIndex = nextOpenBracket;
            }

            // 重置颜色
            System.Console.ResetColor();
        }

        static bool IsValidColor(string color)
        {
            return color == "green" ||
                color == "white" ||
                color == "red"; // 可以根据需要添加更多颜色
        }

        static void SetConsoleColor(string color)
        {
            switch (color)
            {
                case "green":
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "white":
                    System.Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "red":
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    break;
                // 可以根据需要添加更多颜色
                default:
                    System.Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }
    }
}

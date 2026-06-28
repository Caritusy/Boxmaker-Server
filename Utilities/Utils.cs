namespace BoxMaker_Server
{
    public class Utils
    {
        public static Dictionary<int, int> levelToExpNeed = new Dictionary<int, int>()
        {
            { 1, 0 },
            { 2, 6 },
            { 3, 14 },
            { 4, 23 },
            { 5, 33 },
            { 6, 45 },
            { 7, 58 },
            { 8, 72 },
            { 9, 88 },
            { 10, 105 },
            { 11, 123 },
            { 12, 143 },
            { 13, 162 },
            { 14, 182 },
            { 15, 201 },
            { 16, 221 },
            { 17, 240 },
            { 18, 260 },
            { 19, 279 },
            { 20, 299 },
            { 21, 318 },
            { 22, 338 },
            { 23, 357 },
            { 24, 377 },
            { 25, 396 },
            { 26, 416 },
            { 27, 435 },
            { 28, 455 },
            { 29, 474 },
            { 30, 494 },
            { 31, 513 },
            { 32, 533 },
            { 33, 552 },
            { 34, 572 },
            { 35, 591 },
            { 36, 611 },
            { 37, 630 },
            { 38, 650 },
            { 39, 669 },
            { 40, 689 },
            { 41, 708 },
            { 42, 728 },
            { 43, 747 },
            { 44, 767 },
            { 45, 786 },
            { 46, 806 },
            { 47, 825 },
            { 48, 845 },
            { 49, 864 },
            { 50, 884 },
            { 51, 903 },
            { 52, 923 },
            { 53, 942 },
            { 54, 962 },
            { 55, 981 },
            { 56, 1001 },
            { 57, 1020 },
            { 58, 1040 },
            { 59, 1059 },
            { 60, 1079 }
        };


        public static int get_map_exp(int tr, int rs)
        {
            if (rs < 100)
            {
                return 2;
            }
            float num = (float)tr / (float)rs;
            if (num < 0.001f)
            {
                return 11;
            }
            if (num < 0.01f)
            {
                return 9;
            }
            if (num < 0.05f)
            {
                return 7;
            }
            if (num < 0.1f)
            {
                return 6;
            }
            if (num < 0.2f)
            {
                return 5;
            }
            if (num < 0.4f)
            {
                return 4;
            }
            if (num < 0.5f)
            {
                return 3;
            }
            return 2;
        }

        public static int get_map_nd(int tr, int rs)
        {
            float num = (float)tr / (float)rs;
            if (rs > 10000)
            {
                if (num < 0.0005f)
                {
                    return 4;
                }
                if (num < 0.005f)
                {
                    return 3;
                }
                if (num < 0.05f)
                {
                    return 2;
                }
            }
            else if (rs > 1000)
            {
                if (num < 0.005f)
                {
                    return 3;
                }
                if (num < 0.05f)
                {
                    return 2;
                }
            }
            else
            {
                if (rs <= 100)
                {
                    return 0;
                }
                if (num < 0.05f)
                {
                    return 2;
                }
            }
            return 1;
        }
    }
}

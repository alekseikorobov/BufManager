using System.Collections.Generic;

namespace BufManager
{
    public class MyData
    {
        public MyData()
        {
            listData1 = new List<MyDataFindReplace>();
            listData2 = new List<MyDataFindReplace>();
            boff = new List<MyDataBuff>();
            countHistory = 25;
        }
        public List<MyDataFindReplace> listData1 { get; set; }
        public List<MyDataFindReplace> listData2 { get; set; }

        public List<MyDataBuff> boff { get; set; }

        public int countHistory { get; set; }


        public string PathImage { get; set; }
    }
}

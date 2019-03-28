namespace BufManager
{
    public class MyDataFindReplace
    {
        private string _findPole;
        public string FindPole
        {
            get { return _findPole; }
            set
            {
                //PrioritetFind++;
                _findPole = value.IndexOf("///") != -1 ? value.Substring(0, value.IndexOf("///")) : value;
                shwodataFind = value.IndexOf("///") != -1 ? value.Substring(value.IndexOf("///") + 2) : "";
            }
        }
        private string shwodataFind;
        public string FindPoleShwo
        {
            get
            {
                return _findPole +
                (!string.IsNullOrEmpty(shwodataFind) ? " (" + shwodataFind + ")" : "");
            }
        }

        private string _replacePole;
        public string ReplacePole
        {
            get { return _replacePole; }
            set
            {
                //PrioritetReplace++;
                _replacePole = value.IndexOf("///") != -1 ? value.Substring(0, value.IndexOf("///")) : value;
                shwodataReplace = value.IndexOf("///") != -1 ? value.Substring(value.IndexOf("///") + 2) : "";
            }
        }
        private string shwodataReplace;
        public string ReplacePoleShwo
        {
            get
            {
                return _replacePole +
                    (!string.IsNullOrEmpty(shwodataReplace) ? " (" + shwodataReplace + ")" : "");
            }
        }
        public int PrioritetFind { get; set; }
        public int PrioritetReplace { get; set; }
    }
}

namespace BufManager
{
    public class MyDataBuff
    {
        string ReplaceText()
        {
            string s = _text.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");

            return s.Length > 60 ? s.Substring(0, 60) + "..." : s;
        }

        public string _text;
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                _showText = ReplaceText();
            }
        }

        public string _showText;
        public string ShowText
        {
            get
            {
                return _showText;

            }
        }

        public int id { get; set; }

        public bool IsChecked { get; set; }
    }
}

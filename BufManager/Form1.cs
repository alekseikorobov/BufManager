using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BufManager.Util;
using myConfig;

namespace BufManager
{
    [DefaultEvent("ClipboardChanged")]
    public partial class Form1 : Form
    {
        Config<MyData> config = new Config<MyData>("Config.xml");
        private string text, textRepl;
        private string[] error = new string[2];
        private ToolStripItem tem1, tem2, tem3, tem0, tem4, tem5;
        bool isItem = false;
        IntPtr nextClipboardViewer;
        int countBuff = 0;
        private string lastFind, lastRep;
        bool isClosing = false;

        public Form1()
        {
            InitializeComponent();
            tem5 = contextMenuStrip1.Items[contextMenuStrip1.Items.Count - 6];
            tem4 = contextMenuStrip1.Items[contextMenuStrip1.Items.Count - 5];
            tem0 = contextMenuStrip1.Items[contextMenuStrip1.Items.Count - 4];
            tem1 = contextMenuStrip1.Items[contextMenuStrip1.Items.Count - 3];
            tem2 = contextMenuStrip1.Items[contextMenuStrip1.Items.Count - 2];
            tem3 = contextMenuStrip1.Items[contextMenuStrip1.Items.Count - 1];

            updateList();
            ShowContextMenu();
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
            this.Resize += new System.EventHandler(this.Form1_Resize);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }
        #region dll
        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    OnClipboardChanged();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        #endregion
        void OnClipboardChanged()
        {
            label5.Text = "Обращение - " + (++countBuff).ToString();
            try
            {
                Image image = Clipboard.GetImage();
                if (image != null)
                {
                    var n = DateTime.Now;
                    string name = string.Format("im_{0}{1}", n.ToString("yyyyMMdd_HHmmss"), ".jpg");
                    string path = Path.Combine(config.config.PathImage, name);
                    image.Save(path);
                    return;
                }
            }
            catch
            {
                return;
            }
            try
            {
                text = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(text)) return;
            }
            catch
            {
                return;
            }

            int count = config.config.boff.Count > 0 ? config.config.boff.Max(c => c.id) + 1 : 0;

            if (config.config.boff.Any(c => c.text == text))
            {
                var r = config.config.boff.Single(c => c.text == text);
                r.id = count;
            }
            else
            {
                config.config.boff.Add(new MyDataBuff() { text = text, id = count });

                while (config.config.boff.Count > config.config.countHistory)
                {
                    int i = 0;
                    /*while (config.config.boff[i].IsChecked)
                    {
                        i++;
                    }*/
                    config.config.boff.RemoveAt(i);
                }
            }
            ShowContextMenu();
        }
        void ShowContextMenu()
        {
            contextMenuStrip1.Items.Clear();
            foreach (var buff in config.config.boff.OrderByDescending(c => c.id))
            {
                ToolStripMenuItem i = new ToolStripMenuItem();
                i.Text = buff.ShowText;
                i.Tag = buff.id;
                i.Click += new EventHandler(i_Click);
                contextMenuStrip1.Items.Add(i);
            }
            contextMenuStrip1.Items.Add(tem3);
            contextMenuStrip1.Items.Add(tem2);
            contextMenuStrip1.Items.Add(tem1);
            contextMenuStrip1.Items.Add(tem0);
            contextMenuStrip1.Items.Add(tem4);
            contextMenuStrip1.Items.Add(tem5);
        }
        void i_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (sender as ToolStripMenuItem);
            if (item != null)
            {
                Clipboard.SetText(config.config.boff.Single(c => c.id == (int)item.Tag).text);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateReturn();
        }
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            updateReturn();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateReturn();
        }
        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            updateReturn();
        }
        private void linkLabel1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(error[0]);
        }
        private void linkLabel2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(error[1]);
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            updateReturn();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            updateReturn();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            lastFind = comboBox1.Text;
            lastRep = comboBox2.Text;
            addHistory(comboBox1.Text, comboBox2.Text);

            updateList();
            SetText();
            updateReturn1();
        }
        private void addHistory(string str, string str1)
        {
            string s1 = str.IndexOf("///") != -1 ? str.Substring(0, str.IndexOf("///")) : str;
            string s2 = str1.IndexOf("///") != -1 ? str1.Substring(0, str1.IndexOf("///")) : str1;

            if (config.config.listData1.Any(c => c.FindPole == s1))
            {
                var r = config.config.listData1.Single(c => c.FindPole == s1);
                r.PrioritetFind++;
            }
            else
            {
                config.config.listData1.Add(new MyDataFindReplace() { FindPole = str, PrioritetFind = 1 });
            }

            if (config.config.listData2.Any(c => c.ReplacePole == s2))
            {
                var r = config.config.listData2.Single(c => c.FindPole == s2);
                r.PrioritetFind++;
            }
            else
            {
                config.config.listData2.Add(new MyDataFindReplace() { ReplacePole = str1, PrioritetReplace = 1 });
            }

            config.writeConfig(config.config);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isClosing)
            {
                e.Cancel = true;
                this.Hide();
                WindowState = FormWindowState.Minimized;
                notifyIcon1.Visible = true;
            }
            //Application.Exit();
        }
        private void notifyIcon1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isClosing = true;
            config.writeConfig(config.config);
            Application.Exit();
            OnClosed(e);
        }
        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            config.config.boff.Clear();
            ShowContextMenu();
        }
        private void последняяЗаменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateReturn1();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            FormConfig f = new FormConfig(ref config);
            f.ShowDialog();
        }
        private void уникальныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(text)) return;
            List<string> textMessiv = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (textMessiv.Count < 2) return;

            text = string.Join(Environment.NewLine, textMessiv.Distinct());
            Clipboard.SetText(text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BuffList bl = new BuffList();
            bl.textList.Lines = config.config.boff.Select(c => c.text).ToArray();
            bl.Show();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void сортироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(text)) return;
            List<string> textMessiv = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (textMessiv.Count < 2) return;

            text = string.Join(Environment.NewLine, textMessiv.OrderBy(c => c));
            Clipboard.SetText(text);
        }

        private void сортироватьЧислаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(text)) return;
            List<string> textMessiv = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (textMessiv.Count < 2) return;

            text = string.Join(Environment.NewLine, textMessiv.OrderBy(c =>
            {
                if (Regex.IsMatch(c, "^\\d+"))
                    return Regex.Match(c, "^\\d+").Value;
                return c;
            }));
            Clipboard.SetText(text);
        }

        private void убратьКонечныеПробелыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(text)) return;
            List<string> textMessiv = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (textMessiv.Count < 2) return;

            text = string.Join(Environment.NewLine, textMessiv.Select(c => c.Trim()));
            Clipboard.SetText(text);
        }


        private string myReplace(string s)
        {
            string r = "\r";
            string n = "\n";
            string t = "\t";

            Regex reg_r = new Regex("\\\\r");
            Regex reg_n = new Regex("\\\\n");
            Regex reg_t = new Regex("\\\\t");

            var m = reg_r.Replace(s, r);
            var m1 = reg_n.Replace(m, n);
            var m2 = reg_t.Replace(m1, t);

            return m2;
        }
        private void updateResult()
        {
            try
            {
                Regex reg = new Regex(comboBox1.Text);
                Match m = reg.Match(textBox3.Text);
                textBox1.Text = m.Value;
                linkLabel1.Text = "";
            }
            catch (Exception e)
            {
                linkLabel1.Text = "Ошибка";
                error[0] = e.Message;
            }
        }
        private void updateReturn()
        {
            try
            {
                if (radioButton2.Checked)
                {
                    Regex reg = new Regex(comboBox1.Text);
                    var m = reg.Replace(textBox3.Text, comboBox2.Text);
                    var m1 = reg.Replace(text ?? "", comboBox2.Text);
                    textBox1.Text = reg.Match(textBox3.Text).Value;
                    textBox2.Text = m;
                    textRepl = m1;
                    linkLabel2.Text = "";
                }
                else
                {
                    var s = myReplace(comboBox1.Text);
                    var s1 = myReplace(comboBox2.Text);
                    textBox2.Text = textBox3.Text.Replace(s, s1);
                    textRepl = text != null ? text.Replace(s, s1) : "";
                }
            }
            catch (Exception e)
            {
                linkLabel2.Text = "Ошибка";
                error[1] = e.Message;
            }
        }
        private void updateReturn1()
        {
            try
            {
                if (radioButton2.Checked)
                {

                    Regex reg = new Regex(lastFind);
                    var m = reg.Replace(text, lastRep);
                    var m1 = reg.Replace(text ?? "", lastRep);
                    textBox2.Text = m;
                    textRepl = m1;
                    linkLabel2.Text = "";
                }
                else
                {
                    var s = myReplace(lastFind);
                    var s1 = myReplace(lastRep);
                    textRepl = text != null ? text.Replace(s, s1) : "";
                }
                Clipboard.SetText(string.IsNullOrEmpty(textRepl) ? " " : textRepl);
            }
            catch (Exception e)
            {
                linkLabel2.Text = "Ошибка";
                error[1] = e.Message;
            }
        }
        void SetText()
        {

        }
        void updateList()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            foreach (MyDataFindReplace r in config.config.listData1.OrderByDescending(c => c.PrioritetFind).ThenByDescending(c => c.PrioritetReplace))
            {
                comboBox1.Items.Add(r.FindPoleShwo);
            }
            foreach (MyDataFindReplace r in config.config.listData2.OrderByDescending(c => c.PrioritetReplace))
            {
                comboBox2.Items.Add(r.ReplacePoleShwo);
            }
        }



        /*----start--------------------------------------*/

        static bool Visible = true;
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void SetConsoleWindowVisibility(bool visible)
        {
            //IntPtr hWnd = FindWindow(null, Console.Title);
            //if (hWnd != IntPtr.Zero)
            //{
            //    if (visible) ShowWindow(hWnd, 1); //1 = SW_SHOWNORMAL           
            //    else ShowWindow(hWnd, 0); //0 = SW_HIDE
            //}
        }




        private void скринToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible = false;
            SetConsoleWindowVisibility(Visible);

            //Application.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            //{
            try
            {
                using (var bmp = SnippingTool.Snip())
                {
                    //_main.Visibility = Visibility.Visible;
                    if (bmp != null)
                    {
                        try
                        {
                            Clipboard.SetImage(bmp);
                        }
                        catch (Exception)
                        {
                            // ignore
                        }
                    }
                }
            }
            finally
            {
                //_main.Visibility = Visibility.Visible;
            }
        }

        /*-----end---------------------------------------*/
    }
}

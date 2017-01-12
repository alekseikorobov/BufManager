using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BufManager
{
    public partial class FormConfig : Form
    {
        private myConfig.Config<MyData> config;

        public FormConfig()
        {
            InitializeComponent();
        }

        public FormConfig(ref myConfig.Config<MyData> config):this()
        {
            // TODO: Complete member initialization
            this.config = config;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int i = int.Parse(textBox4.Text);
                if(i<0)
                {
                    MessageBox.Show("Значение для количества не может быть меньше нуля");
                    return;
                }
                config.config.countHistory = i;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось присвоить значение количеству в истории, изменить нестройки или отмените их");
                return;
            }

            if (!Path.IsPathRooted(textBox1.Text))
            {
                MessageBox.Show("Указаный путь к картинкам не допустим");
                return;
            }
            if (!Directory.Exists(textBox1.Text))
            {
                var r = MessageBox.Show("Разрешение","Указанная папка для картинок не сущесту, хотите ли вы ее созадть?",MessageBoxButtons.YesNo);
                if (r == System.Windows.Forms.DialogResult.Yes)
                {
                    Directory.CreateDirectory(textBox1.Text);
                }
                else
                {
                    return;
                }
            }
            config.config.PathImage = textBox1.Text;
            config.writeConfig(config.config);
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormConfig_Load(object sender, EventArgs e)
        {
            textBox4.Text = config.config.countHistory.ToString();
            textBox1.Text = config.config.PathImage;
        }
    }
}

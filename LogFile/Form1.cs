using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var lines = FilterFile(File.ReadAllLines(openFileDialog1.FileName));
            var list = new List<string>();
            listBox1.DataSource = null;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].ToString();
                try
                {
                    var kind = line.Substring(18, 6);

                    if (kind.ToUpper().Contains(toolStripComboBox1.SelectedItem.ToString().ToUpper()))
                    {
                        list.Add(line);
                    }
                }
                catch
                {
                }
            }
            listBox1.DataSource = list;
            listBox1.ResumeLayout();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.ItemHeight = 25;
            listBox1.DrawMode = DrawMode.OwnerDrawVariable;
        }


        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            //          17
            //11-10 12:17:39.450 INFO  App           Windows version is older than Windows 8.1 and using DirectSound as audio frameworks [ [System thread], Initialize, VidyoClient.cpp:571 ]
            listBox1.DataSource = FilterFile(File.ReadAllLines(openFileDialog1.FileName));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
        }
        string[] FilterFile(string[] rrrrr)
        {
            var list = new List<string>();
            foreach (var item in rrrrr.Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                DateTime f;
                if (item.Length>19 && DateTime.TryParse(item.Substring(0, 19), out f))
                {
                    list.Add(item);
                }
                else if (list.Count > 0)
                {
                    list[list.Count - 1] += item;
                }
            }
            return list.ToArray();

        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            fileSystemWatcher1.Path = string.Join("\\", openFileDialog1.FileName.Split(new string[] { "\\" }, StringSplitOptions.None).Take(openFileDialog1.FileName.Split(new string[] { "\\" }, StringSplitOptions.None).Length - 1));
            //C:\Users\ntato\Documents\Visual Studio 2015\Projects\DeviceP2R\DeviceP2R\bin\Debug\logs




            listBox1.DataSource = FilterFile(File.ReadAllLines(openFileDialog1.FileName));


            //listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.TopIndex = listBox1.Items.Count - 1;

        }

        private void ListBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 50;
            e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.AliceBlue)), 0, 0, 1300, 100);
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {

            try
            {


                e.DrawBackground();

                Graphics g = e.Graphics;

                g.FillRectangle(new SolidBrush(Color.White), e.Bounds);


                ListBox lb = (ListBox)sender;

                var font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 12, FontStyle.Bold);
                e.DrawFocusRectangle();
                g.DrawRectangle(new Pen(Color.Black), new Rectangle(new Point(e.Bounds.X, e.Bounds.Y), new Size(e.Bounds.Width, e.Bounds.Height)));
                var kind = lb.Items[e.Index].ToString().Substring(18, 6);



                g.DrawRectangle(new Pen(Color.Black), new Rectangle(new Point(e.Bounds.X, e.Bounds.Y), new Size(e.Bounds.Width, e.Bounds.Height * 90)));





                if (kind.ToUpper().Contains("ERROR"))
                {
                    g.DrawString(lb.Items[e.Index].ToString(), font, new SolidBrush(Color.DarkRed), new RectangleF(new PointF(e.Bounds.X, e.Bounds.Y), new SizeF(e.Bounds.Width, e.Bounds.Height)));

                }
                else if (kind.ToUpper().Contains("FATAL"))
                {
                    g.DrawString(lb.Items[e.Index].ToString(), font, new SolidBrush(Color.Red), new RectangleF(new PointF(e.Bounds.X, e.Bounds.Y), new SizeF(e.Bounds.Width, e.Bounds.Height)));


                }
                else if (kind.ToUpper().Contains("DEBUG"))
                {
                    g.DrawString(lb.Items[e.Index].ToString(), font, new SolidBrush(Color.Blue), new RectangleF(new PointF(e.Bounds.X, e.Bounds.Y), new SizeF(e.Bounds.Width, e.Bounds.Height)));


                }
                else if (kind.ToUpper().Contains("WARNING"))
                {
                    g.DrawString(lb.Items[e.Index].ToString(), font, new SolidBrush(Color.Yellow), new RectangleF(new PointF(e.Bounds.X, e.Bounds.Y), new SizeF(e.Bounds.Width, e.Bounds.Height)));


                }
                else if (kind.ToUpper().Contains("INFO"))
                {
                    g.DrawString(lb.Items[e.Index].ToString(), font, new SolidBrush(Color.Green), new RectangleF(new PointF(e.Bounds.X, e.Bounds.Y), new SizeF(e.Bounds.Width, e.Bounds.Height)));
                }

            }
            catch
            {

            }

        }
    }
}

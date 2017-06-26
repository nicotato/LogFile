using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogFile2
{
    public partial class Form1 : Form
    {
        const string FILENAMEADRESS = "state.bin";


        // The name of the key must include a valid root.
        const string userRoot = "HKEY_CURRENT_USER";
        const string subkey = "SOFTWARE";
        const string keyName = userRoot + "\\" + subkey;

        private string adress;
        LevelLog type = LevelLog.All;
        public Form1()
        {
            InitializeComponent();
            drpLeveLog.DataSource = Enum.GetValues(typeof(LevelLog));
            drpLeveLog.SelectedItem = LevelLog.All;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog1.ShowDialog(this);
        }

        private async void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            fileSystemWatcher1.Path = Path.GetDirectoryName(openFileDialog1.FileName);
            fileSystemWatcher1.Filter = Path.GetFileName(openFileDialog1.SafeFileName);
            fileSystemWatcher1.EnableRaisingEvents = true;
            fileSystemWatcher1.NotifyFilter = NotifyFilters.LastWrite;

            adress = openFileDialog1.FileName;

            textBox1.Text = await GetDataFile();

            ScrollToBottom(textBox1);

            textBox2.Text = openFileDialog1.FileName;


            var reg = Registry.CurrentUser.OpenSubKey("SOFTWARE").OpenSubKey("LogFileText", true);
            reg.SetValue("Adress", openFileDialog1.FileName);
        }
        private string lastLineText;
        private async void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            if (adress == e.FullPath && e.ChangeType == WatcherChangeTypes.Changed)
            {

                string tex = await Task.Run(() =>
                {
                    string res = null;
                    try
                    {
                        res = File.ReadAllLines(adress).Where(T => type == LevelLog.All || T.Contains($"[{type}]")).LastOrDefault();
                    }
                    catch (Exception)
                    {
                        System.Threading.Thread.Sleep(50);
                        res = File.ReadAllLines(adress).Where(T => type == LevelLog.All || T.Contains($"[{type}]")).LastOrDefault();
                    }
                    if (res.Equals(lastLineText))
                        return null;
                    else
                    {
                        lastLineText = res;
                        return res;
                    }

                });

                if (!string.IsNullOrEmpty(tex))
                {
                    textBox1.Invoke(new MethodInvoker(() =>
                   {
                       textBox1.AppendText(tex);
                       textBox1.AppendText(Environment.NewLine);
                       //textBox1.Select(textBox1.Text.Length, 0);
                       textBox1.ScrollToCaret();
                   }));
                }
            }
        }
        private async void Form1_Load(object sender, EventArgs e)
        {

            var reg = Registry.CurrentUser.OpenSubKey("SOFTWARE").OpenSubKey("LogFileText");
            if (reg != null)
            {
                adress = reg.GetValue("Adress") as string;
                if (!string.IsNullOrEmpty(adress))
                {
                    textBox2.Text = adress;
                    fileSystemWatcher1.Path = Path.GetDirectoryName(adress);
                    fileSystemWatcher1.Filter = Path.GetFileName(adress);
                    fileSystemWatcher1.EnableRaisingEvents = true;
                    fileSystemWatcher1.NotifyFilter = NotifyFilters.LastWrite;
                    fileSystemWatcher1.EnableRaisingEvents = chkRun.Checked;
                    textBox1.Text = await GetDataFile();
                    ScrollToBottom(textBox1);

                }
            }
        }
        private async void chkRun_CheckedChanged(object sender, EventArgs e)
        {
            fileSystemWatcher1.EnableRaisingEvents = chkRun.Checked;
            if (chkRun.Checked)
            {
                textBox1.Text = await GetDataFile();
                ScrollToBottom(textBox1);
            }
        }

        #region Utils

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int SendMessage(System.IntPtr hWnd, int wMsg, System.IntPtr wParam, System.IntPtr lParam);

        private const int WM_VSCROLL = 0x115;
        private const int SB_BOTTOM = 7;

        /// <summary>
        /// Scrolls the vertical scroll bar of a multi-line text box to the bottom.
        /// </summary>
        /// <param name="tb">The text box to scroll</param>
        public static void ScrollToBottom(System.Windows.Forms.TextBox tb)
        {
            if (System.Environment.OSVersion.Platform != System.PlatformID.Unix)
                SendMessage(tb.Handle, WM_VSCROLL, new System.IntPtr(SB_BOTTOM), System.IntPtr.Zero);
        }
        public async Task<string> GetDataFile()
        {
            try
            {
                return await Task.Run(() =>
                {
                    var text = File.ReadAllLines(adress).Where(T => type == LevelLog.All || T.Contains($"[{type}]"));
                    lastLineText = text.LastOrDefault();
                    return string.Join(Environment.NewLine, text) + Environment.NewLine;
                });
            }
            catch 
            {
                await Task.Delay(200);
                return await GetDataFile();
            }
        }

        enum LevelLog
        {
            WARM,
            DEBUG,
            ERROR,
            INFO,
            All
        }
        #endregion

        private async void drpLeveLog_SelectionChangeCommitted(object sender, EventArgs e)
        {
            type = (LevelLog)drpLeveLog.SelectedItem;
            fileSystemWatcher1.EnableRaisingEvents = chkRun.Checked;
            if (drpLeveLog.Enabled && chkRun.Checked)
            {
                textBox1.Text = await GetDataFile();
                ScrollToBottom(textBox1);
            }
        }
    }
}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace FF13FullLauncher
{
    public partial class MainForm : Form
    {
        Form cfgForm = new SettingsForm();
        string gamePath = Path.Combine(Application.StartupPath, @"white_data\prog\win\bin\ffxiiiimg.exe");

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Length > 1)
                this.Close();
            if (File.Exists(gamePath) || SilDev.Log.DebugEnabled())
            {
                SilDev.Initialization.File(Application.StartupPath, "FFXiiiLauncher.ini");

                if (!SilDev.Elevation.IsAdministrator())
                {
                    bool WriteRights = SilDev.Elevation.WritableLocation();
                    bool GeDoSaToClose = (SilDev.Initialization.ReadValue("Settings", "GeDoSaToClose").ToLower() == "true");
                    if (!WriteRights || GeDoSaToClose)
                        SilDev.Elevation.RestartAsAdministrator();
                }

                if (!File.Exists(SilDev.Initialization.File()))
                    SilDev.Initialization.CreateFile(new string[] { "[Settings]", "DispMode=", string.Format("ResWidth={0}", Screen.PrimaryScreen.Bounds.Width), string.Format("ResHeight={0}", Screen.PrimaryScreen.Bounds.Height), "GeDoSaToPath=", "GeDoSaToLaunch=True", "GeDoSaToModify=False", "GeDoSaToClose=False", "SubtLang=" }, true);
                
                SilDev.XmlFile.File(Application.StartupPath, "setup.xml");
                if (!File.Exists(SilDev.XmlFile.File()))
                    File.WriteAllText(SilDev.XmlFile.File(), SilDev.Crypt.BaseExtrem.DecryptArray(new string[] { "PT", "1n", "UG", "hSWF", "lF", "Vm1", "k", "aE", "4x", "T", "Dhv", "UUQr", "VU", "dad", "jEy", "WH", "RK", "WGFtN", "T", "Jiaj", "lD", "UG", "xO", "SGJo", "Wm1", "QbF", "Iy", "Yn", "Q5", "VmJ5", "bG1a", "dT", "ky", "WTh", "BQ0", "lL", "M", "Gd", "Q", "bFI", "y", "YnQ5V", "lpq", "bD", "Ji", "MjlD", "UHg0", "VFp", "rOV", "di", "Zl", "Yy", "WXA5", "bWQ4", "QU", "NJS", "zBn", "UG", "xS", "MmJ", "0", "OTF", "k", "dlJt", "YnBk", "M0w", "4", "RWp", "Q", "bFIy", "YnQ5", "M", "W", "R2Um", "1i", "cG", "RI", "UG", "dBa", "UNO", "NGpJ", "aDFX", "Wm9O", "MlV", "N", "MUVXd", "kVE", "TX", "dJ", "ek", "xu", "Sj", "Ni", "dU16Z", "H", "VjM", "2", "Qz", "OXl", "M", "Nk", "FI", "ZD", "Bo", "bUk", "5", "UT", "JjNHB", "6", "Y3", "V4", "V2", "I0Q", "mlJbE", "5t", "Ym", "hS", "M", "2N1", "bFdMa", "D", "FX", "Wm9O", "MlVN", "MUVX", "dkVE", "TXd", "Jek", "xu", "SjNi", "dU", "1", "6", "ZH", "Vj", "M2Qz", "OXl", "MNkF", "IZDBo", "bUk", "5a", "zJj", "NHB6", "Y3", "V4", "V2I0", "Q", "lNZ", "MEZH", "Umxa", "WFl", "U", "eGpD", "TjR6", "UG", "ln", "VEx", "t", "Ul", "h", "kaTB6", "WnVs", "R1", "p2Tm", "1ibEJ", "p", "SXc0", "U01p", "MGpi", "dmwy", "Y3l", "W", "bWR", "nd1", "di", "N", "DlE", "UA==" }, 64));
                
                if (string.IsNullOrWhiteSpace(SilDev.Initialization.ReadValue("Settings", "DispMode")))
                {
                    string value = SilDev.XmlFile.GetXmlValue("window_mode");
                    SilDev.Initialization.WriteValue("Settings", "DispMode", value == "0" ? "Windowed" : "Fullscreen");
                }
                
                if (string.IsNullOrWhiteSpace(SilDev.Initialization.ReadValue("Settings", "VoiceLang")))
                {
                    string value = SilDev.XmlFile.GetXmlValue("voice_mode");
                    SilDev.Initialization.WriteValue("Settings", "VoiceLang", value == "0" ? "Japanese" : "English");
                }
                
                string SubtLang = string.Empty;
                foreach (string file in Directory.GetFiles(Application.StartupPath, "*.ini", SearchOption.AllDirectories))
                {
                    if (file != SilDev.Initialization.File())
                    {
                        if (!string.IsNullOrWhiteSpace(SilDev.Initialization.ReadValue("Settings", "Language", file)))
                        {
                            if (string.IsNullOrEmpty(SubtLang))
                                SubtLang = SilDev.Initialization.ReadValue("Settings", "Language", file);
                            else
                            {
                                if (SubtLang != SilDev.Initialization.ReadValue("Settings", "Language", file))
                                    SilDev.Initialization.WriteValue("Settings", "Language", SubtLang, file);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(SilDev.Initialization.ReadValue("Settings", "UserName", file)))
                            ProfileName.Text = SilDev.Initialization.ReadValue("Settings", "UserName", file);
                    }
                }

                if (string.IsNullOrWhiteSpace(ProfileName.Text))
                {
                    string SteamPath = string.Empty;
                    if (SilDev.Reg.SubKeyExist("HKLM", @"SOFTWARE\Valve\Steam"))
                        SteamPath = SilDev.Reg.ReadValue("HKLM", @"SOFTWARE\Valve\Steam", "InstallPath");
                    if (Directory.Exists(SteamPath))
                    {
                        string SteamProfile = Path.Combine(SteamPath, @"config\loginusers.vdf");
                        if (File.Exists(SteamProfile))
                        {
                            foreach (string line in File.ReadLines(SteamProfile))
                            {
                                bool found = false;
                                foreach (string split in line.Split('\"'))
                                {
                                    if (!string.IsNullOrWhiteSpace(split))
                                    {
                                        if (split.ToLower() == "personaname")
                                        {
                                            found = true;
                                            continue;
                                        }
                                        if (found)
                                        {
                                            ProfileName.Text = split;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                if (string.IsNullOrWhiteSpace(SilDev.Initialization.ReadValue("Settings", "SubtLang")) && !string.IsNullOrWhiteSpace(SubtLang))
                    SilDev.Initialization.WriteValue("Settings", "SubtLang", string.Format("{0}{1}", char.ToUpper(SubtLang[0]), SubtLang.Substring(1)));
            }
            else
            {
                MessageBox.Show(string.Format("Open the root folder of the game to replace the \"FFXiiiLauncher.exe\".{0}{0}Don't forget the backup of the original file!", Environment.NewLine), "How To Use", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (File.Exists(SilDev.Log.File))
                Process.Start(SilDev.Log.File);
        }

        private void playBtnPan_Click(object sender, EventArgs e)
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f.Name != this.Name)
                {
                    f.Close();
                    break;
                }
            }
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            PlayBw.RunWorkerAsync();
        }

        private void PlayBw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                bool IsAdmin = SilDev.Elevation.IsAdministrator();
                if (SilDev.XmlFile.GetXmlValue(SilDev.XmlFile.GetXmlContent(), "window_mode") == "1")
                    SilDev.XmlFile.SetXmlValue("window_mode", "0");

                string dMode = SilDev.Initialization.ReadValue("Settings", "DispMode").ToLower();

                string TempValue = SilDev.Initialization.ReadValue("Settings", "ResWidth");
                int width = (!string.IsNullOrWhiteSpace(TempValue) ? Convert.ToInt32(TempValue) : Screen.PrimaryScreen.Bounds.Width);
                TempValue = SilDev.Initialization.ReadValue("Settings", "ResHeight");
                int height = (!string.IsNullOrWhiteSpace(TempValue) ? Convert.ToInt32(TempValue) : Screen.PrimaryScreen.Bounds.Height);

                string GeDoSaToPath = SilDev.Initialization.ReadValue("Settings", "GeDoSaToPath");
                bool GeDoSaToLaunch = (SilDev.Initialization.ReadValue("Settings", "GeDoSaToLaunch").ToLower() == "true");
                bool GeDoSaToModify = (SilDev.Initialization.ReadValue("Settings", "GeDoSaToModify").ToLower() == "true");
                bool GeDoSaToClose = (SilDev.Initialization.ReadValue("Settings", "GeDoSaToClose").ToLower() == "true");

                string vLang = SilDev.Initialization.ReadValue("Settings", "VoiceLang").ToLower();

                string arg = string.Empty;
                if (width < 320 || height < 240)
                {
                    width = Screen.PrimaryScreen.Bounds.Width;
                    height = Screen.PrimaryScreen.Bounds.Height;
                }

                if (GeDoSaToLaunch && !GeDoSaToModify && width > 1280 && height > 720)
                {
                    string GeDoSaToCfgPath = Path.Combine(GeDoSaToPath, @"config\ffxiiiimg\GeDoSaTo.ini");
                    if (File.Exists(GeDoSaToCfgPath))
                    {
                        string FullContent = File.ReadAllText(GeDoSaToCfgPath);
                        string ToChange = string.Empty;
                        foreach (string line in File.ReadAllLines(GeDoSaToCfgPath))
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                if (!line.ToLower().StartsWith("#renderresolution") && line.ToLower().StartsWith("renderresolution") && line.EndsWith("@60"))
                                {
                                    ToChange = line;
                                    break;
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(ToChange))
                        {
                            File.Delete(GeDoSaToCfgPath);
                            File.WriteAllText(GeDoSaToCfgPath, FullContent.Replace(ToChange, string.Format("renderResolution {0}x{1}@60", width, height)));
                        }
                    }
                }

                if (GeDoSaToLaunch)
                    GeDoSaToLaunch = (File.Exists(Path.Combine(GeDoSaToPath, "GeDoSaToTool.exe")) && File.Exists(Path.Combine(GeDoSaToPath, "GeDoSaTo.dll")));

                if (GeDoSaToLaunch && Process.GetProcessesByName("GeDoSaToTool").Length <= 0)
                    SilDev.Run.App(GeDoSaToPath, "GeDoSaToTool.exe", (IsAdmin && GeDoSaToClose ? SilDev.Run.WindowStyle.Normal : SilDev.Run.WindowStyle.Minimized), -1, -1);

                IntPtr GeDoSaTo = IntPtr.Zero;
                if (IsAdmin && GeDoSaToLaunch && GeDoSaToClose)
                {
                    int ForceCount = 250;
                    while (ForceCount > 0)
                    {
                        Thread.Sleep(10);
                        foreach (Process p in Process.GetProcessesByName("GeDoSaToTool"))
                        {
                            if (p.MainWindowHandle != IntPtr.Zero)
                            {
                                if (p.MainWindowTitle.ToLower().Contains("warning"))
                                {
                                    p.CloseMainWindow();
                                    continue;
                                }
                                ForceCount = 0;
                                GeDoSaTo = p.MainWindowHandle;
                                SilDev.WndHook.ShowWindow(GeDoSaTo, 0);
                                break;
                            }
                        }
                        ForceCount--;
                    }
                }

                if (dMode == "fullscreen")
                    arg = "-FullScreenMode=Force";
                if (vLang == "japanese")
                    arg = string.Format("{0}{1}{2}", arg, !string.IsNullOrWhiteSpace(arg) ? " " : string.Empty, "-VoiceJPMode");

                Process[] game = Process.GetProcessesByName("ffxiiiimg");
                if (game.Length <= 0)
                    SilDev.Run.App(Path.GetDirectoryName(gamePath), Path.GetFileName(gamePath), arg);

                if (dMode.Contains("windowed"))
                {
                    int usage = 0;
                    game = Process.GetProcessesByName("ffxiiiimg");
                    while (usage < 40000 && game.Length > 0)
                    {
                        Thread.Sleep(10);
                        game = Process.GetProcessesByName("ffxiiiimg");
                        foreach (Process ff13 in game)
                            usage = (int)ff13.NonpagedSystemMemorySize64;
                    }
                    Thread.Sleep(1000);
                    game = Process.GetProcessesByName("ffxiiiimg");
                    foreach (Process ff13 in game)
                    {
                        if (dMode.Contains("borderless"))
                            SilDev.WndHook.RemoveWindowBorders(ff13.MainWindowHandle);
                        SilDev.WndHook.SetWindowSize(ff13.MainWindowHandle, width, height);
                    }
                }

                if (IsAdmin && GeDoSaToLaunch && GeDoSaToClose)
                {
                    game = Process.GetProcessesByName("ffxiiiimg");
                    foreach (Process ff13 in game)
                        ff13.WaitForExit();

                    if (GeDoSaTo != IntPtr.Zero)
                    {
                        SilDev.WndHook.ShowWindow(GeDoSaTo, 9);
                        SilDev.WndHook.PostMessage(GeDoSaTo, 0x10, 0, 0);
                        Thread.Sleep(200);
                    }
                    foreach (Process p in Process.GetProcessesByName("GeDoSaToTool"))
                        p.Kill();
                }
                else
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex.Message, "PlayBw_DoWork");
                e.Cancel = true;
            }
        }

        private void PlayBw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                this.Opacity = 1;
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
                this.Focus();
            }
            else
                this.Close();
        }

        private void cfgBtnPan_Click(object sender, EventArgs e)
        {
            try
            {
                cfgForm = new SettingsForm();
                cfgForm.Location = cfgFormLocation();
                cfgForm.TopLevel = false;
                cfgForm.TopMost = true;
                cfgForm.Show();

                playBtnPan.Visible = false;
                playBtnTx.Visible = false;
                cfgBtnPan.Visible = false;
                cfgBtnTx.Visible = false;
                creditsBtn.Visible = false;

                cfgPanel.Controls.Add(cfgForm);
                cfgPanel.Visible = true;

                CfgFormCloseCheck.Enabled = true;
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex.Message, "cfgBtnPan_Click");
            }
        }

        private void CfgFormCloseCheck_Tick(object sender, EventArgs e)
        {
            bool IsOpen = false;
            foreach (Form f in Application.OpenForms)
            {
                IsOpen = (f.Name == cfgForm.Name);
                if (IsOpen)
                    break;
            }
            if (!IsOpen)
            {
                cfgPanel.Visible = false;

                playBtnPan.Visible = true;
                playBtnTx.Visible = true;
                cfgBtnPan.Visible = true;
                cfgBtnTx.Visible = true;
                creditsBtn.Visible = true;

                CfgFormCloseCheck.Enabled = false;
            }
        }

        private void creditsBtn_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Hand.Play();
            SilDev.MsgBox.Show(this, string.Format("Program developed by $î13ñ7™{0}{0}support@si13n7.com{0}{0}www.si13n7.com", Environment.NewLine), "Credits", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        #region CUSTOM LAYOUT

        public MainForm()
        {
            InitializeComponent();
            cfgPanel.Visible = false;
            cfgPanel.Location = new Point(1, 56);
            cfgPanel.Size = new Size(650, 444);
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.SizeAll;
            SilDev.WndHook.MoveWindow_Mouse(this, e);
            this.Cursor = Cursors.Default;
        }

        private void MainForm_Move(object sender, EventArgs e)
        {
            foreach (Form f in Application.OpenForms)
                if (f.Name != this.Name)
                    f.Location = cfgFormLocation();
        }

        private void playBtnPan_MouseEnter(object sender, EventArgs e)
        {
            playBtnPan.BackgroundImage = (Image)Properties.Resources.btn_big_mo_13;
        }

        private void playBtnPan_MouseLeave(object sender, EventArgs e)
        {
            playBtnPan.BackgroundImage = (Image)Properties.Resources.btn_big;
        }

        private void cfgBtnPan_MouseEnter(object sender, EventArgs e)
        {
            cfgBtnPan.BackgroundImage = (Image)Properties.Resources.btn_small_mo_13;
        }

        private void cfgBtnPan_MouseLeave(object sender, EventArgs e)
        {
            cfgBtnPan.BackgroundImage = (Image)Properties.Resources.btn_small;
        }

        private void closeBtnPan_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closeBtnPan_MouseEnter(object sender, EventArgs e)
        {
            closeBtnPan.BackgroundImage = (Image)Properties.Resources.btn_close_mo_13;
        }

        private void closeBtnPan_MouseLeave(object sender, EventArgs e)
        {
            closeBtnPan.BackgroundImage = (Image)Properties.Resources.btn_close;
        }

        private void minBtnPan_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void minBtnPan_MouseEnter(object sender, EventArgs e)
        {
            minBtnPan.BackgroundImage = (Image)Properties.Resources.btn_min_mo_13;
        }

        private void minBtnPan_MouseLeave(object sender, EventArgs e)
        {
            minBtnPan.BackgroundImage = (Image)Properties.Resources.btn_min;
        }

        private Point cfgFormLocation()
        {
            return new Point(1, 0);
        }

        #endregion
    }
}

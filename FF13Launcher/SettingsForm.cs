using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FF13FullLauncher
{
    public partial class SettingsForm : Form
    {
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            string TipText = string.Format("This option is only importanten for windowed or{0}fullscreen using GeDoSaTo, if the modify config{0}checkbox is enabled.", Environment.NewLine);
            HintToolTip.SetToolTip(ResLabel, TipText);
            HintToolTip.SetToolTip(ResWidth, TipText);
            HintToolTip.SetToolTip(ResXLabel, TipText);
            HintToolTip.SetToolTip(ResHeight, TipText);

            TipText = string.Format("Usefull for windowed mode. So it's possible to use{0}the definied resolution for the window without to{0}disable GeDoSaTo downsampling.", Environment.NewLine);
            HintToolTip.SetToolTip(GeDoSaToModify, TipText);

            TipText = "This option is only for the cracked game version.";
            HintToolTip.SetToolTip(SubtLangLabel, TipText);
            HintToolTip.SetToolTip(SubtLang, TipText);

            if (!SilDev.Elevation.IsAdministrator())
            {
                TipText = "Administration rights required.";
                WarnToolTip.SetToolTip(GeDoSaToClose, TipText);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(SilDev.Initialization.ReadValue("Settings", "DispMode")))
                {
                    switch (SilDev.Initialization.ReadValue("Settings", "DispMode").ToLower())
                    {
                        case "fullscreen":
                            DispMode.SelectedIndex = 0;
                            break;
                        case "windowed":
                            DispMode.SelectedIndex = 1;
                            break;
                        case "windowed (borderless)":
                            DispMode.SelectedIndex = 2;
                            break;
                    }
                }

                string TempValue = SilDev.Initialization.ReadValue("Settings", "ResWidth");
                ResWidth.Value = (!string.IsNullOrWhiteSpace(TempValue) ? Convert.ToInt32(TempValue) : Screen.PrimaryScreen.Bounds.Width);
                TempValue = SilDev.Initialization.ReadValue("Settings", "ResHeight");
                ResHeight.Value = (!string.IsNullOrWhiteSpace(TempValue) ? Convert.ToInt32(TempValue) : Screen.PrimaryScreen.Bounds.Height);

                if (!string.IsNullOrWhiteSpace(SilDev.Initialization.ReadValue("Settings", "VoiceLang")))
                {
                    if (SilDev.Initialization.ReadValue("Settings", "VoiceLang") == "Japanese")
                        VoiceLang.SelectedIndex = 0;
                    else
                        VoiceLang.SelectedIndex = 1;
                }

                if (!string.IsNullOrWhiteSpace(SilDev.Initialization.ReadValue("Settings", "SubtLang")))
                {
                    switch (SilDev.Initialization.ReadValue("Settings", "SubtLang").ToLower())
                    {
                        case "english":
                            SubtLang.SelectedIndex = 0;
                            break;
                        case "french":
                            SubtLang.SelectedIndex = 1;
                            break;
                        case "german":
                            SubtLang.SelectedIndex = 2;
                            break;
                        case "italian":
                            SubtLang.SelectedIndex = 3;
                            break;
                        case "spanish":
                            SubtLang.SelectedIndex = 4;
                            break;
                    }
                }
                else
                    SubtLang.Enabled = false;

                if (!string.IsNullOrWhiteSpace(SilDev.Initialization.ReadValue("Settings", "GeDoSaToPath")))
                {
                    if (Directory.Exists(SilDev.Initialization.ReadValue("Settings", "GeDoSaToPath")))
                    {
                        GeDoSaToPath.Enabled = true;
                        GeDoSaToPath.Text = SilDev.Initialization.ReadValue("Settings", "GeDoSaToPath");
                        
                        GeDoSaToLaunch.Enabled = true;
                        GeDoSaToLaunch.Checked = (SilDev.Initialization.ReadValue("Settings", "GeDoSaToLaunch").ToLower() == "true");
                        
                        GeDoSaToModify.Enabled = true;
                        GeDoSaToModify.Checked = (SilDev.Initialization.ReadValue("Settings", "GeDoSaToModify").ToLower() == "true");

                        GeDoSaToClose.Enabled = true;
                        GeDoSaToClose.Checked = (SilDev.Initialization.ReadValue("Settings", "GeDoSaToClose").ToLower() == "true");
                    }
                }
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex.Message, "SettingsForm_Load");
            }
        }

        private void OpenDirBtn_Click(object sender, EventArgs e)
        {
            SilDev.Initialization.ReadValue("Settings", "GeDoSaToPath");
            while (true)
            {
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.ShowNewFolderButton = false;
                    if (File.Exists(GeDoSaToPath.Text))
                        dialog.SelectedPath = GeDoSaToPath.Text;
                    dialog.ShowDialog(new Form() { ShowIcon = false, TopMost = true });
                    if (File.Exists(Path.Combine(dialog.SelectedPath, "GeDoSaToTool.exe")) && File.Exists(Path.Combine(dialog.SelectedPath, "GeDoSaTo.dll")))
                    {
                        GeDoSaToPath.Enabled = true;
                        GeDoSaToPath.Text = dialog.SelectedPath;
                        GeDoSaToLaunch.Enabled = true;
                        GeDoSaToModify.Enabled = true;
                        if (GeDoSaToLaunch.Checked)
                            GeDoSaToClose.Enabled = true;
                    }
                    else
                    {
                        if (SilDev.MsgBox.Show(this, "GeDoSaToTool not found!", "Not Found", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Retry)
                            continue;
                    }
                    break;
                }
            }
        }

        private void GeDoSaToLaunch_CheckedChanged(object sender, EventArgs e)
        {
            if (GeDoSaToLaunch.Checked)
                GeDoSaToClose.Enabled = true;
            else
                GeDoSaToClose.Enabled = false;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                switch (DispMode.SelectedIndex)
                {
                    case 0:
                        SilDev.Initialization.WriteValue("Settings", "DispMode", "Fullscreen");
                        SilDev.XmlFile.SetXmlValue("window_mode", "1");
                        break;
                    case 1:
                        SilDev.Initialization.WriteValue("Settings", "DispMode", "Windowed");
                        SilDev.XmlFile.SetXmlValue("window_mode", "0");
                        break;
                    case 2:
                        SilDev.Initialization.WriteValue("Settings", "DispMode", "Windowed (Borderless)");
                        SilDev.XmlFile.SetXmlValue("window_mode", "0");
                        break;
                }

                SilDev.Initialization.WriteValue("Settings", "ResWidth", ResWidth.Value.ToString());
                SilDev.Initialization.WriteValue("Settings", "ResHeight", ResHeight.Value.ToString());

                if (File.Exists(Path.Combine(GeDoSaToPath.Text, "GeDoSaToTool.exe")) && File.Exists(Path.Combine(GeDoSaToPath.Text, "GeDoSaToTool.exe")))
                {
                    SilDev.Initialization.WriteValue("Settings", "GeDoSaToPath", GeDoSaToPath.Text);
                    SilDev.Initialization.WriteValue("Settings", "GeDoSaToLaunch", GeDoSaToLaunch.Checked.ToString());
                    SilDev.Initialization.WriteValue("Settings", "GeDoSaToModify", GeDoSaToModify.Checked.ToString());
                    SilDev.Initialization.WriteValue("Settings", "GeDoSaToClose", GeDoSaToClose.Checked.ToString());
                }

                if (VoiceLang.SelectedIndex == 0)
                {
                    SilDev.Initialization.WriteValue("Settings", "VoiceLang", "Japanese");
                    SilDev.XmlFile.SetXmlValue("voice_mode", "0");
                }
                else
                {
                    SilDev.Initialization.WriteValue("Settings", "VoiceLang", "English");
                    SilDev.XmlFile.SetXmlValue("voice_mode", "1");
                }
                SilDev.Initialization.WriteValue("Settings", "SubtLang", SubtLang.SelectedItem.ToString());
                foreach (string file in Directory.GetFiles(Application.StartupPath, "*.ini", SearchOption.AllDirectories))
                    if (file != SilDev.Initialization.File())
                        SilDev.Initialization.WriteValue("Settings", "Language", SubtLang.SelectedItem.ToString().ToLower(), file);

                if (GeDoSaToClose.Checked && !SilDev.Elevation.IsAdministrator())
                    SilDev.Elevation.RestartAsAdministrator();
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex.Message, "saveBtn_Click");
            }
            this.Close();
        }

        #region CUSTOM LAYOUT

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void saveBtn_MouseEnter(object sender, EventArgs e)
        {
            saveBtn.BackgroundImage = (Image)Properties.Resources.btn_small_mo_13;
        }

        private void saveBtn_MouseLeave(object sender, EventArgs e)
        {
            saveBtn.BackgroundImage = (Image)Properties.Resources.btn_small;
        }

        private void cancelBtn_MouseClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        private void cancelBtn_MouseEnter(object sender, EventArgs e)
        {
            cancelBtn.BackgroundImage = (Image)Properties.Resources.btn_small_mo_13;
        }

        private void cancelBtn_MouseLeave(object sender, EventArgs e)
        {
            cancelBtn.BackgroundImage = (Image)Properties.Resources.btn_small;
        }

        #endregion
    }
}

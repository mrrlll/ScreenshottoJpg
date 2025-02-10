using System.Drawing.Imaging;
using IWshRuntimeLibrary;

namespace ScreenshotToJpg
{
    public partial class Form1 : Form
    {
        private FileSystemWatcher watcher;
        private string screenshotFolder;
        private NotifyIcon notifyIcon;

        public Form1()
        {
            InitializeComponent();

            screenshotFolder = Properties.Settings.Default.ScreenshotFolderPath;

            while (string.IsNullOrEmpty(screenshotFolder) || !Directory.Exists(screenshotFolder))
            {
                if (!SelectScreenshotFolder())
                {
                    DialogResult result = MessageBox.Show(
                        "�A�v���P�[�V���������삷�邽�߂ɂ́A�X�N���[���V���b�g�t�H���_��I������K�v������܂��B�������t�H���_��I�����܂����H�u�������v��I������ƃA�v���P�[�V�������I�����܂��B",
                        "�X�N���[���V���b�g�t�H���_�̑I��",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.No)
                    {
                        Application.Exit();
                        return;
                    }
                }
                else
                {
                    break;
                }
                screenshotFolder = Properties.Settings.Default.ScreenshotFolderPath;
            }

            // ����N�����̃X�^�[�g�A�b�v�o�^�m�F
            if (!Properties.Settings.Default.IsStartupRegistered)
            {
                DialogResult result = MessageBox.Show(
                    "Windows �N�����ɂ��̃A�v���P�[�V�����������I�ɊJ�n���܂����H",
                    "�X�^�[�g�A�b�v�o�^",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    RegisterStartup();
                }
            }

            // �X�^�[�g�A�b�v�o�^��Ԃ����j���[�ɔ��f
            startupcheckbox.Checked = Properties.Settings.Default.IsStartupRegistered;

            InitializeFileSystemWatcher();
            InitializeNotifyIcon();
        }

        private void InitializeFileSystemWatcher()
        {
            watcher = new FileSystemWatcher
            {
                Path = screenshotFolder,
                Filter = "*.png",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };
            watcher.Created += OnScreenshotCreated;
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon()
            {
                Icon = new Icon(GetType(), "logo.ico"),
                Visible = true,
                Text = "Screenshot to JPG Converter",
                ContextMenuStrip = notifyIconMenu
            };
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            notifyIcon.BalloonTipClicked += NotifyIcon_BalloonTipClicked;

            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }

        private bool SelectScreenshotFolder()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    screenshotFolder = folderDialog.SelectedPath;
                    Properties.Settings.Default.ScreenshotFolderPath = screenshotFolder;
                    Properties.Settings.Default.Save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void OnScreenshotCreated(object sender, FileSystemEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                try
                {
                    string jpgFilePath = Path.ChangeExtension(e.FullPath, ".jpg");
                    System.Threading.Thread.Sleep(500);

                    using (var image = new Bitmap(e.FullPath))
                    {
                        image.Save(jpgFilePath, ImageFormat.Jpeg);

                        using (Image jpgImage = Image.FromFile(jpgFilePath))
                        {
                            DataObject dataObject = new DataObject();
                            dataObject.SetImage(jpgImage);

                            string[] files = new string[] { jpgFilePath };
                            dataObject.SetData(DataFormats.FileDrop, files);

                            Clipboard.SetDataObject(dataObject, true);
                        }
                    }
                    notifyIcon.ShowBalloonTip(3000, "�X�N���[���V���b�g�ϊ�����", $"JPG�ɕϊ����A�N���b�v�{�[�h�ɃR�s�[���܂���:\n{jpgFilePath}", ToolTipIcon.Info);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"�G���[���������܂���: {ex.Message}", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            SelectScreenshotFolder();
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            if (!string.IsNullOrEmpty(screenshotFolder) && Directory.Exists(screenshotFolder))
            {
                InitializeFileSystemWatcher();
                notifyIcon.ShowBalloonTip(3000, "�t�H���_�ύX", $"�Ď��t�H���_���ȉ��ɕύX���܂���:\n{screenshotFolder}", ToolTipIcon.Info);
            }
        }

        private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(screenshotFolder) && Directory.Exists(screenshotFolder))
            {
                System.Diagnostics.Process.Start("explorer.exe", screenshotFolder);
            }
            else
            {
                MessageBox.Show("�X�N���[���V���b�g�t�H���_���ݒ肳��Ă��Ȃ����A���݂��܂���B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripMenuItemSelectFolder_Click(object sender, EventArgs e)
        {
            if (SelectScreenshotFolder())
            {

                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }

                if (!string.IsNullOrEmpty(screenshotFolder) && Directory.Exists(screenshotFolder))
                {
                    InitializeFileSystemWatcher();
                    notifyIcon.ShowBalloonTip(3000, "�t�H���_�ύX", $"�Ď��t�H���_���ȉ��ɕύX���܂���:\n{screenshotFolder}", ToolTipIcon.Info);
                }
            }
        }

        private void notifyIconMenuItemSelectFolder(object sender, EventArgs e)
        {
            if (SelectScreenshotFolder())
            {

                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }

                if (!string.IsNullOrEmpty(screenshotFolder) && Directory.Exists(screenshotFolder))
                {
                    InitializeFileSystemWatcher();
                    notifyIcon.ShowBalloonTip(3000, "�t�H���_�ύX", $"�Ď��t�H���_���ȉ��ɕύX���܂���:\n{screenshotFolder}", ToolTipIcon.Info);
                }
            }
        }

        private void notifyIconMenuItemExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void RegisterStartup()
        {
            try
            {
                string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(startupFolderPath, Application.ProductName + ".lnk"); // �V���[�g�J�b�g�̃p�X
                string exePath = Application.ExecutablePath; // ���s�t�@�C���̃p�X

                // �V���[�g�J�b�g���쐬
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = exePath;
                shortcut.WorkingDirectory = Application.StartupPath; // ��ƃf�B���N�g��
                //shortcut.IconLocation = exePath + ",0"; // �A�C�R��
                shortcut.Save();

                // �A�v���P�[�V�����ݒ���X�V
                Properties.Settings.Default.IsStartupRegistered = true;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�X�^�[�g�A�b�v�o�^���ɃG���[���������܂���: {ex.Message}", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UnregisterStartup()
        {
            try
            {
                string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(startupFolderPath, Application.ProductName + ".lnk");

                // �V���[�g�J�b�g�����݂���΍폜
                if (System.IO.File.Exists(shortcutPath))
                {
                    System.IO.File.Delete(shortcutPath);
                }

                // �A�v���P�[�V�����ݒ���X�V
                Properties.Settings.Default.IsStartupRegistered = false;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�X�^�[�g�A�b�v�o�^�������ɃG���[���������܂���: {ex.Message}", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void startupcheckbox_click(object sender, EventArgs e)
        {
            // �`�F�b�N��Ԃ𔽓]�����Ă��珈��
            startupcheckbox.Checked = !startupcheckbox.Checked;

            if (startupcheckbox.Checked)
            {
                RegisterStartup();
            }
            else
            {
                UnregisterStartup();
            }
        }
    }
}
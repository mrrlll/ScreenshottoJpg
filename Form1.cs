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
                        "アプリケーションが動作するためには、スクリーンショットフォルダを選択する必要があります。今すぐフォルダを選択しますか？「いいえ」を選択するとアプリケーションを終了します。",
                        "スクリーンショットフォルダの選択",
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

            // 初回起動時のスタートアップ登録確認
            if (!Properties.Settings.Default.IsStartupRegistered)
            {
                DialogResult result = MessageBox.Show(
                    "Windows 起動時にこのアプリケーションを自動的に開始しますか？",
                    "スタートアップ登録",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    RegisterStartup();
                }
            }

            // スタートアップ登録状態をメニューに反映
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
                    notifyIcon.ShowBalloonTip(3000, "スクリーンショット変換完了", $"JPGに変換し、クリップボードにコピーしました:\n{jpgFilePath}", ToolTipIcon.Info);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                notifyIcon.ShowBalloonTip(3000, "フォルダ変更", $"監視フォルダを以下に変更しました:\n{screenshotFolder}", ToolTipIcon.Info);
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
                MessageBox.Show("スクリーンショットフォルダが設定されていないか、存在しません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    notifyIcon.ShowBalloonTip(3000, "フォルダ変更", $"監視フォルダを以下に変更しました:\n{screenshotFolder}", ToolTipIcon.Info);
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
                    notifyIcon.ShowBalloonTip(3000, "フォルダ変更", $"監視フォルダを以下に変更しました:\n{screenshotFolder}", ToolTipIcon.Info);
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
                string shortcutPath = Path.Combine(startupFolderPath, Application.ProductName + ".lnk"); // ショートカットのパス
                string exePath = Application.ExecutablePath; // 実行ファイルのパス

                // ショートカットを作成
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = exePath;
                shortcut.WorkingDirectory = Application.StartupPath; // 作業ディレクトリ
                //shortcut.IconLocation = exePath + ",0"; // アイコン
                shortcut.Save();

                // アプリケーション設定を更新
                Properties.Settings.Default.IsStartupRegistered = true;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"スタートアップ登録中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UnregisterStartup()
        {
            try
            {
                string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(startupFolderPath, Application.ProductName + ".lnk");

                // ショートカットが存在すれば削除
                if (System.IO.File.Exists(shortcutPath))
                {
                    System.IO.File.Delete(shortcutPath);
                }

                // アプリケーション設定を更新
                Properties.Settings.Default.IsStartupRegistered = false;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"スタートアップ登録解除中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void startupcheckbox_click(object sender, EventArgs e)
        {
            // チェック状態を反転させてから処理
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
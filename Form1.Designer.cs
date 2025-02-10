namespace ScreenshotToJpg
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            notifyIconMenu = new ContextMenuStrip(components);
            フォルダーの選択SToolStripMenuItem = new ToolStripMenuItem();
            startupcheckbox = new ToolStripMenuItem();
            終了XToolStripMenuItem = new ToolStripMenuItem();
            notifyIconMenu.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIconMenu
            // 
            notifyIconMenu.Items.AddRange(new ToolStripItem[] { フォルダーの選択SToolStripMenuItem, startupcheckbox, 終了XToolStripMenuItem });
            notifyIconMenu.Name = "contextMenuStrip1";
            notifyIconMenu.Size = new Size(181, 92);
            // 
            // フォルダーの選択SToolStripMenuItem
            // 
            フォルダーの選択SToolStripMenuItem.Name = "フォルダーの選択SToolStripMenuItem";
            フォルダーの選択SToolStripMenuItem.Size = new Size(180, 22);
            フォルダーの選択SToolStripMenuItem.Text = "フォルダーの選択(&S)";
            フォルダーの選択SToolStripMenuItem.Click += notifyIconMenuItemSelectFolder;
            // 
            // startupcheckbox
            // 
            startupcheckbox.Checked = true;
            startupcheckbox.CheckState = CheckState.Checked;
            startupcheckbox.Name = "startupcheckbox";
            startupcheckbox.Size = new Size(180, 22);
            startupcheckbox.Text = "スタートアップ登録 (&R";
            startupcheckbox.Click += startupcheckbox_click;
            // 
            // 終了XToolStripMenuItem
            // 
            終了XToolStripMenuItem.Name = "終了XToolStripMenuItem";
            終了XToolStripMenuItem.Size = new Size(180, 22);
            終了XToolStripMenuItem.Text = "終了(&X)";
            終了XToolStripMenuItem.Click += notifyIconMenuItemExit;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "Form1";
            Text = "Form1";
            notifyIconMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ContextMenuStrip notifyIconMenu;
        private ToolStripMenuItem フォルダーの選択SToolStripMenuItem;
        private ToolStripMenuItem 終了XToolStripMenuItem;
        private ToolStripMenuItem startupcheckbox;
    }
}

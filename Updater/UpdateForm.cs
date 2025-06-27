namespace Updater
{
    public partial class UpdateForm : Form
    {
        private const string MasterName = "EasyTemplate.Ava.Desktop";
        public UpdateForm()
        {
            InitializeComponent();
            CopyTo();
        }

        private void CopyTo()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string updateTmpDir = Path.Combine(baseDir, "UpdateTmp");
            string desktopDir = Path.Combine(baseDir, MasterName);
            string[] excludeFiles = { "Updater.exe", "Updater.dll", "result.txt" };

            if (!Directory.Exists(updateTmpDir) || !Directory.Exists(desktopDir))
                return;

            foreach (var file in Directory.GetFiles(updateTmpDir, "*", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(file);
                if (excludeFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                    continue;

                // 计算目标路径
                string relativePath = Path.GetRelativePath(updateTmpDir, file);
                string destPath = Path.Combine(desktopDir, relativePath);

                // 确保目标目录存在
                string? destDir = Path.GetDirectoryName(destPath);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                // 如果目标文件已存在，先删除
                if (File.Exists(destPath))
                    File.Delete(destPath);

                File.Move(file, destPath);
            }

            // 删除UpdateTmp目录
            Directory.Delete(updateTmpDir, true);

            // 启动主程序
            string exePath = Path.Combine(desktopDir, $"{MasterName}.exe");
            if (File.Exists(exePath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = exePath,
                    WorkingDirectory = desktopDir,
                    UseShellExecute = true
                });
            }

            // 关闭自身
            Application.Exit();
        }
    }
}

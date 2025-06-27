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

                // ����Ŀ��·��
                string relativePath = Path.GetRelativePath(updateTmpDir, file);
                string destPath = Path.Combine(desktopDir, relativePath);

                // ȷ��Ŀ��Ŀ¼����
                string? destDir = Path.GetDirectoryName(destPath);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                // ���Ŀ���ļ��Ѵ��ڣ���ɾ��
                if (File.Exists(destPath))
                    File.Delete(destPath);

                File.Move(file, destPath);
            }

            // ɾ��UpdateTmpĿ¼
            Directory.Delete(updateTmpDir, true);

            // ����������
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

            // �ر�����
            Application.Exit();
        }
    }
}

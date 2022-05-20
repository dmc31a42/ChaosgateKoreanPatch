using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChaosgateKoreanPatch.Helper;

namespace ChaosgateKoreanPatch.Models
{
    public class UnityDataDirWindows: IUnityDataDir
    {
        private string baseDir = "";

        private string DataExeCommonName = "";

        public UnityDataDirWindows(string path)
        {
            baseDir = Path.TrimEndingDirectorySeparator(path);

            string dirFolderName = Path.GetFileName(baseDir);
            if (!dirFolderName.EndsWith("_Data")) return;

            DataExeCommonName = dirFolderName.Substring(0, dirFolderName.LastIndexOf("_Data"));

            if (
                !File.Exists(this.BinaryPath)
                || !File.Exists(this.ResourcesAssetPath)
                || !File.Exists(this.GlobalgamemanagersPath)
                || !File.Exists(this.GlobalgamemanagersAssetsPath)
                || !File.Exists(this.ResourcesUnityDefaultResourcesPath)
                || !File.Exists(this.ResourcesUnity_builtin_extraPath)
                || !File.Exists(this.GlobalgamemanagersPath)
            ) return;

            IsValid = true;
        }

        public Platform Platform => Platform.Windows;
        public bool IsValid { get; internal set; } = false;

        public string AppInfoPath => Path.Combine(baseDir, @"app.info");
        public string ResourcesAssetPath => Path.Combine(baseDir, @"resources.assets");
        public string GlobalgamemanagersPath => Path.Combine(baseDir, @"globalgamemanagers");
        public string GlobalgamemanagersAssetsPath => Path.Combine(baseDir, @"globalgamemanagers.assets");
        public string BinaryPath => Path.GetFullPath(Path.Combine(baseDir, $@"..\{DataExeCommonName}.exe"));
        public string ResourcesUnityDefaultResourcesPath => Path.Combine(baseDir, @"Resources\unity default resources");
        public string ResourcesUnity_builtin_extraPath => Path.Combine(baseDir, @"Resources\unity_builtin_extra");
        public string ManagedFolderPath => Path.Combine(baseDir, @"Managed");
        public string StreamingAssetsFolderPath => Path.Combine(baseDir, @"StreamingAssets");
        public string il2cpp_dataFolderPath => Path.Combine(baseDir, @"il2cpp_data");

        public static IUnityDataDir? Near(string path) {
            if (!Directory.Exists(path)) return null;

            if (path.Contains("_Data"))
            {
                string? localPath = Path.TrimEndingDirectorySeparator(path);

                while (localPath != null && !localPath.EndsWith("_Data"))
                {
                    localPath = Path.GetDirectoryName(localPath);
                }
                if (localPath == null) return null;

                var dataDir = new UnityDataDirWindows(localPath);
                if (dataDir.IsValid) return dataDir;
            } 
            else
            {
                var dirs = Directory.GetDirectories(path);
                var found = dirs.FirstOrDefault((dir) => dir.EndsWith("_Data"));
                if (found == null) return null;

                var dataDir = new UnityDataDirWindows(Path.Combine(path, found));
                if (dataDir.IsValid) return dataDir;
            }
            return null;
        }
    }
}

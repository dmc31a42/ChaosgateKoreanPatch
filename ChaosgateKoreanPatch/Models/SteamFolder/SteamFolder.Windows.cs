using Gameloop.Vdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gameloop.Vdf.Linq;

namespace ChaosgateKoreanPatch.Models
{
#pragma warning disable CA1416 // 플랫폼 호환성 유효성 검사
    public class SteamFolderWindows
    {
        private string? binRoot;
        private string[] folders = new string[] { };
        public string[] Folders => folders;

        public SteamFolderWindows()
        {
            binRoot = GetBinRootFromURLWindows() ?? GetBinRootByBruteForce();

            if (binRoot != null && GetLibraryFolderFromVDF()) return;
            
        }

        private bool GetLibraryFolderFromVDF()
        {
            if (this.binRoot == null) return false;
            folders = SteamFolderWindows.GetLibraryFolderFromVDF(this.binRoot);
            if (folders.Length == 0) return false;
            return true;
        }

        public static string[] GetLibraryFolderFromVDF(string binRoot)
        {
            string[] empty = new string[] { };

            if (binRoot == null) return empty;
            var vdfPath = Path.Combine(binRoot, @"steamapps\libraryfolders.vdf");
            if (!File.Exists(vdfPath)) return empty;

            Gameloop.Vdf.Linq.VProperty libraryfolders = VdfConvert.Deserialize(File.ReadAllText(vdfPath));

#pragma warning disable CS8620 // 참조 형식의 null 허용 여부 차이로 인해 매개 변수에 대해 인수를 사용할 수 없습니다.
            string[] tempFolders = libraryfolders.Value
                .Select<VToken, string?>((child) =>
                {
                    var vObject = child as VProperty;
                    if (vObject == null) return "";
                    if (vObject.Value.Type != VTokenType.Object) return null;
                    if (vObject == null || vObject.Value["path"] == null) return null;

#pragma warning disable CS8600 // null 리터럴 또는 가능한 null 값을 null을 허용하지 않는 형식으로 변환하는 중입니다.
                    var vObjPath = (VValue)vObject.Value["path"];
                    if (vObjPath == null) return null;
                    string? path = (string?)vObjPath.Value;
#pragma warning restore CS8600 // null 리터럴 또는 가능한 null 값을 null을 허용하지 않는 형식으로 변환하는 중입니다.
                    if (path == null) return null;

                    string tempPath = Path.Combine((string)path, @"steamapps\common");
                    if (!Directory.Exists(tempPath)) return null;

                    return tempPath;
                }).Where<string>(result => result != null).ToArray<string>() ?? new string[] { }; ;
#pragma warning restore CS8620 // 참조 형식의 null 허용 여부 차이로 인해 매개 변수에 대해 인수를 사용할 수 없습니다.


            if (tempFolders.Length == 0) return empty;

            return tempFolders;
        }


        public static string[] GetLibraryCommonByBruteForce()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
#pragma warning disable CS8620 // 참조 형식의 null 허용 여부 차이로 인해 매개 변수에 대해 인수를 사용할 수 없습니다.
            return allDrives
                .Select(d => Path.Combine(d.Name, @"SteamLibrary\libraryfolder.vdf"))
                .Where(File.Exists)
                .Select(Path.GetDirectoryName)
                .Where<string>(path => path != null)
                .Select(path => Path.Combine(path, @"steamapps\common"))
                .ToArray();
#pragma warning restore CS8620 // 참조 형식의 null 허용 여부 차이로 인해 매개 변수에 대해 인수를 사용할 수 없습니다.
        }

        private static string? GetBinRootByBruteForce()
        {
            string[] candidatePaths =
            {
                @"Program Files(x86)\Steam\steam.exe",
                @"Program Files\Steam\steam.exe",
                @"Steam\steam.exe",
            };
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                foreach (string candidatePath in candidatePaths)
                {
                    var tempPath = Path.Combine(d.Name, candidatePath);
                    if (File.Exists(tempPath)) return Path.GetDirectoryName(tempPath);
                }
            }
            return null;
        }

        private static string? GetBinRootFromURLWindows()
        {

            string? key = (string?)Registry.GetValue(@"HKEY_CLASSES_ROOT\steam\Shell\Open\Command", "", null);

            if (key == null) return null;

            Match match = Regex.Match(key, @"\""(.*?)\""");
            if (!match.Success) return null;

            string fullPath = match.Groups[1].Value;
            string? DirPath = System.IO.Path.GetDirectoryName(fullPath);
            if (DirPath == null) return null;
            if (!Directory.Exists(DirPath)) return null;

            return DirPath;
        }
    }
#pragma warning restore CA1416 // 플랫폼 호환성 유효성 검사
}

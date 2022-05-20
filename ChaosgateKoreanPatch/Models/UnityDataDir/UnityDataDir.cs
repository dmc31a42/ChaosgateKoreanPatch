using ChaosgateKoreanPatch.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosgateKoreanPatch.Models
{
    public interface IUnityDataDir
    {
        public Platform Platform { get; }
        public bool IsValid { get; }

        public string AppInfoPath { get; }
        public string ResourcesAssetPath { get; }
        public string GlobalgamemanagersPath { get; }
        public string GlobalgamemanagersAssetsPath { get; }

        public string BinaryPath { get; }

        public string ResourcesUnityDefaultResourcesPath { get; }
        public string ResourcesUnity_builtin_extraPath { get; }

        public string ManagedFolderPath { get; }
        public string StreamingAssetsFolderPath { get; }

        public string il2cpp_dataFolderPath { get; }

        public static IUnityDataDir? Near(string path) { return null; }

    }
}

using ChaosgateKoreanPatch.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosgateKoreanPatch.Models
{
    public class UnityFolder
    {
        private IUnityDataDir unityDataDir;
        public IUnityDataDir UnityDataDir => unityDataDir;

        public Platform Platform => unityDataDir.Platform;

        private string? productName;
        public string? ProductName => productName;

        private string? companyName;
        public string? CompanyName => companyName;

        public UnityFolder(IUnityDataDir unityDataDir)
        {
            if (!unityDataDir.IsValid)
            {
                throw new ArgumentException("Not valid UnityDataDir");
            }
            this.unityDataDir = unityDataDir;

            if (File.Exists(unityDataDir.AppInfoPath))
            {
                var appInfoText = File.ReadAllText(unityDataDir.AppInfoPath).Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.TrimEntries);
                if (appInfoText.Length >= 2)
                {
                    companyName = appInfoText[0];
                    productName = appInfoText[1];
                }
            }
        }
    }
}

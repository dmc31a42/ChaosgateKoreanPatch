using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reactive;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using AssetsTools.NET.Extra;
using Avalonia.Metadata;
using ChaosgateKoreanPatch.Models;
using ReactiveUI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AssetsTools.NET;

namespace ChaosgateKoreanPatch.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            InitGameDirectory();
        }

        static private UnityFolder? tryGetGameFolder(string path)
        {
            var unityDataDir = UnityDataDirWindows.Near(path);
            if (unityDataDir == null || !unityDataDir.IsValid) return null;
            try
            {
                var unityFolder = new UnityFolder(unityDataDir);
                if (unityFolder.CompanyName == "Complex Games Inc."
                   && unityFolder.ProductName == "GreyKnights")
                {
                    return unityFolder;
                }
            }
            catch { }
            return null;
        }

        private bool InitGameDirectory()
        {

            SteamFolderWindows steamFolder = new SteamFolderWindows();

            foreach (string folder in steamFolder.Folders)
            {
                var candidateGameFolder = Path.Combine(folder, @"ChaosGate");
                var candidateUnityFolder = tryGetGameFolder(candidateGameFolder);
                if (candidateUnityFolder != null)
                {
                    GameDirectory = candidateGameFolder;
                    return true;
                }
            }
            foreach (string folder in steamFolder.Folders)
            {
                foreach (string gameFolder in Directory.GetDirectories(folder))
                {
                    var candidateUnityFolder = tryGetGameFolder(gameFolder);
                    if (candidateUnityFolder != null)
                    {
                        GameDirectory = gameFolder;
                        return true;
                    }
                }
            }

            return false;
        }

        private UnityFolder? unityFolder;

        private string gameDirectory = "";
        public string GameDirectory {
            get => gameDirectory;
            set
            {
                if (value == gameDirectory) return;
                gameDirectory = value;
                unityFolder = tryGetGameFolder(value);

                this.RaisePropertyChanged();

                if (gameDirectory == "")
                {
                    IsValid = false;
                    Message = "ī��������Ʈ ��������� ��ġ�� ������ �������ּ���.";
                } 
                else if (unityFolder == null)
                {
                    IsValid = false;
                    Message = "�ùٸ� ī��������Ʈ ���������� ������ �ƴմϴ�.";
                } 
                else
                {
                    gameDirectory = Path.GetDirectoryName(unityFolder.UnityDataDir.BinaryPath) ?? unityFolder.UnityDataDir.BinaryPath;
                    this.RaisePropertyChanged();
                    IsValid = true;
                    Message = "";
                }
            }
        }

        private bool isValid = false;
        public bool IsValid {
            get => isValid;
            internal set { this.RaiseAndSetIfChanged(ref isValid, value); this.RaisePropertyChanged(nameof(PatchAvailable)); }
            }

        private string message = "";
        public string Message
        {
            get => message;
            internal set => this.RaiseAndSetIfChanged(ref message, value);
        }

        private string log = "";
        public string Log
        {
            get => log;
            internal set => this.RaiseAndSetIfChanged(ref log, value);
        }

        private int progress = 0;
        public int Progress
        {
            get => progress;
            internal set => this.RaiseAndSetIfChanged(ref progress, value);
        }

        public bool CheckFileAvailable()
        {
            if (unityFolder == null) return false;

            var enBundlePath = Path.Combine(unityFolder.UnityDataDir.StreamingAssetsFolderPath, @"bundles\localization\english");
            var tcBundlePath = Path.Combine(unityFolder.UnityDataDir.StreamingAssetsFolderPath, @"bundles\localization\traditionalchinese");

            try
            {
                using (FileStream fs1 = File.Open(enBundlePath, FileMode.Open, FileAccess.Read))
                using (FileStream fs2 = File.Open(tcBundlePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    fs1.Close();
                    fs2.Close();
                }
            }
            catch (IOException)
            {
                Message = "���� ������ �ٸ� ���μ����� ���� ����ֽ��ϴ�. ������ �����ϰ� �ٽ� ��ġ�� �õ��ϼ���.";
                return false;
            }
            Message = "";
            return true;
        }

        private bool isPatching = false;
        public bool IsPatching
        {
            get => isPatching;
            internal set { this.RaiseAndSetIfChanged(ref isPatching, value); this.RaisePropertyChanged(nameof(PatchAvailable)); }
        }

        public async void Patch()
        {
            if (!CheckFileAvailable()) return;
            IsPatching = true;
            Progress = 0;

            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);

            Log += "�ӽ� ���� ����.\n"; Progress++;

            await Task.Run(() =>
            {
                WebClient myWebClient = new();
                var translatedFile = Path.Combine(tempDir, "translated.zip");
                Log += "���� ������ �ٿ�ε� ��...\n"; Progress++;
                bool downloadSuccess = false;
                int failedCount = 0;
                while (!downloadSuccess)
                {
                    try
                    {
                        myWebClient.DownloadFile("https://weblate.nakwonelec.com/download-language/ko/chaosgate/?format=zip", translatedFile);
                        downloadSuccess = true;
                    }
                    catch
                    {
                        if (File.Exists(translatedFile)) File.Delete(translatedFile);
                        if (failedCount > 3)
                        {
                            Log += $"��ġ ����.\n";
                            IsPatching = false;
                            return;
                        }
                        Log += $"���� ������ �ٿ�ε� ����({++failedCount}/3).\n";
                    }
                }
                
                Log += "���� ������ �ٿ�ε� �Ϸ�.\n"; Progress++;
                var translatedFolder = Path.Combine(tempDir, "translated");
                Directory.CreateDirectory(translatedFolder);
                ZipFile.ExtractToDirectory(translatedFile, translatedFolder);
                Log += "���� ������ ���� ����.\n"; Progress++;

                var jsonFolder = Path.Combine(translatedFolder, @"chaosgate\abilityloc_english\ko");

                var enBundlePath = Path.Combine(unityFolder.UnityDataDir.StreamingAssetsFolderPath, @"bundles\localization\english");
                var tcBundlePath = Path.Combine(unityFolder.UnityDataDir.StreamingAssetsFolderPath, @"bundles\localization\traditionalchinese");
                var tcBundleCopyPath = Path.Combine(tempDir, Path.GetRandomFileName());
                File.Copy(tcBundlePath, tcBundleCopyPath, true);
                Log += "���� ���� �ӽ� ������ ����.\n"; Progress++;
                var am = new AssetsManager();
                var tcBundle = am.LoadBundleFile(tcBundleCopyPath, true);
                var tcInst = am.LoadAssetsFileFromBundle(tcBundle, 0);
                var enBundle = am.LoadBundleFile(enBundlePath, true);
                var enInst = am.LoadAssetsFileFromBundle(enBundle, 0);
                Log += "���� ���� �ҷ�����.\n"; Progress++;

                var tcLocTablesInf = tcInst.table.assetFileInfo
                  .Where(inf => !am.GetTypeInstance(tcInst, inf).GetBaseField().Get("localizationData").IsDummy());
                var enLocTablesInf = enInst.table.assetFileInfo
                  .Where(inf => !am.GetTypeInstance(enInst, inf).GetBaseField().Get("localizationData").IsDummy());
                var replacers = new List<AssetsReplacer>() { };

                Log += "���� ������ ���� ��...\n"; Progress++;
                foreach (var inf in tcLocTablesInf)
                {
                    var baseField = am.GetTypeInstance(tcInst, inf).GetBaseField();
                    var name = baseField.Get("m_Name").GetValue().AsString();
                    var enBaseField = am.GetTypeInstance(enInst, enInst.table.GetAssetInfo(name.Replace("TraditionalChinese", "English"))).GetBaseField();
                    var targetJsonPath = Path.Combine(jsonFolder, $"{name.Replace("TraditionalChinese", "English")}.json");
                    if (!File.Exists(targetJsonPath))
                        continue;

                    var keyValueEnglish = new Dictionary<string, string>(enBaseField.Get("localizationData").Get("Array").GetChildrenList().Select(item =>
                    {
                        return new KeyValuePair<string, string>(item.Get("id").GetValue().AsString(), item.Get("translation").GetValue().AsString());
                    }));
                    using (StreamReader reader = File.OpenText(targetJsonPath))
                    {
                        JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                        foreach (var item in baseField.Get("localizationData").Get("Array").GetChildrenList())
                        {
                            var id = item.Get("id").GetValue().AsString();
                            item.Get("translation").GetValue().Set((o[id]?.Value<string>() ?? keyValueEnglish[id]) as string);

                        }
                    }

                    var newGoBytes = baseField.WriteToByteArray();
                    var repl = new AssetsReplacerFromMemory(0, inf.index, (int)inf.curFileType, AssetHelper.GetScriptIndex(tcInst.file, inf), newGoBytes);
                    replacers.Add(repl);
                }
                Log += "���� ������ ���� �Ϸ�.\n"; Progress++;
                //write changes to memory
                byte[] newAssetData;
                using (var stream = new MemoryStream())
                using (var writer = new AssetsFileWriter(stream))
                {
                    tcInst.file.Write(writer, 0, replacers, 0);
                    newAssetData = stream.ToArray();
                }

                //rename this asset name from boring to cool when saving
                var bunRepl = new BundleReplacerFromMemory(tcInst.name, tcInst.name, true, newAssetData, -1);
                Log += "��ġ�� ���� ����.\n"; Progress++;
                if (File.Exists(tcBundlePath)) File.Delete(tcBundlePath);
                using (var bunWriter = new AssetsFileWriter(File.OpenWrite(tcBundlePath)))
                    tcBundle.file.Write(bunWriter, new List<BundleReplacer>() { bunRepl });
                Log += "��ġ�� ���� ��ũ�� ���.\n"; Progress++;
                tcBundle.BundleStream.Dispose();
                enBundle.BundleStream.Dispose();
                Log += "�ҷ��� ���� ������ ������.\n"; Progress++;
                Console.WriteLine("task end");

                Log += "�ӽ����� ����.\n"; Progress++;
                Directory.Delete(tempDir, true);
                Log += "��ġ �Ϸ�.\n"; Progress++;
                IsPatching = false;
            });

        }

        public bool PatchAvailable => IsValid && !IsPatching;

    }
}

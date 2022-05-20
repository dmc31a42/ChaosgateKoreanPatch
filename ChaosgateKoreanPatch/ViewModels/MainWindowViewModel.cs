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
                    Message = "카오스게이트 데몬헌터즈가 설치된 폴더를 선택해주세요.";
                } 
                else if (unityFolder == null)
                {
                    IsValid = false;
                    Message = "올바른 카오스게이트 데몬헌터즈 폴더가 아닙니다.";
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
                Message = "게임 에셋이 다른 프로세서에 의해 잠겨있습니다. 게임을 종료하고 다시 패치를 시도하세요.";
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

            Log += "임시 폴더 생성.\n"; Progress++;

            await Task.Run(() =>
            {
                WebClient myWebClient = new();
                var translatedFile = Path.Combine(tempDir, "translated.zip");
                Log += "번역 데이터 다운로드 중...\n"; Progress++;
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
                            Log += $"패치 실패.\n";
                            IsPatching = false;
                            return;
                        }
                        Log += $"번역 데이터 다운로드 실패({++failedCount}/3).\n";
                    }
                }
                
                Log += "번역 데이터 다운로드 완료.\n"; Progress++;
                var translatedFolder = Path.Combine(tempDir, "translated");
                Directory.CreateDirectory(translatedFolder);
                ZipFile.ExtractToDirectory(translatedFile, translatedFolder);
                Log += "번역 데이터 압축 해제.\n"; Progress++;

                var jsonFolder = Path.Combine(translatedFolder, @"chaosgate\abilityloc_english\ko");

                var enBundlePath = Path.Combine(unityFolder.UnityDataDir.StreamingAssetsFolderPath, @"bundles\localization\english");
                var tcBundlePath = Path.Combine(unityFolder.UnityDataDir.StreamingAssetsFolderPath, @"bundles\localization\traditionalchinese");
                var tcBundleCopyPath = Path.Combine(tempDir, Path.GetRandomFileName());
                File.Copy(tcBundlePath, tcBundleCopyPath, true);
                Log += "게임 에셋 임시 폴더에 복사.\n"; Progress++;
                var am = new AssetsManager();
                var tcBundle = am.LoadBundleFile(tcBundleCopyPath, true);
                var tcInst = am.LoadAssetsFileFromBundle(tcBundle, 0);
                var enBundle = am.LoadBundleFile(enBundlePath, true);
                var enInst = am.LoadAssetsFileFromBundle(enBundle, 0);
                Log += "게임 에셋 불러오기.\n"; Progress++;

                var tcLocTablesInf = tcInst.table.assetFileInfo
                  .Where(inf => !am.GetTypeInstance(tcInst, inf).GetBaseField().Get("localizationData").IsDummy());
                var enLocTablesInf = enInst.table.assetFileInfo
                  .Where(inf => !am.GetTypeInstance(enInst, inf).GetBaseField().Get("localizationData").IsDummy());
                var replacers = new List<AssetsReplacer>() { };

                Log += "번역 데이터 적용 중...\n"; Progress++;
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
                Log += "번역 데이터 적용 완료.\n"; Progress++;
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
                Log += "패치된 에셋 생성.\n"; Progress++;
                if (File.Exists(tcBundlePath)) File.Delete(tcBundlePath);
                using (var bunWriter = new AssetsFileWriter(File.OpenWrite(tcBundlePath)))
                    tcBundle.file.Write(bunWriter, new List<BundleReplacer>() { bunRepl });
                Log += "패치된 에셋 디스크에 기록.\n"; Progress++;
                tcBundle.BundleStream.Dispose();
                enBundle.BundleStream.Dispose();
                Log += "불러온 에셋 데이터 정리중.\n"; Progress++;
                Console.WriteLine("task end");

                Log += "임시폴더 삭제.\n"; Progress++;
                Directory.Delete(tempDir, true);
                Log += "패치 완료.\n"; Progress++;
                IsPatching = false;
            });

        }

        public bool PatchAvailable => IsValid && !IsPatching;

    }
}

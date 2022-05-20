using ChaosgateKoreanPatch.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ChaosgateKoreanPatch.Test
{
    [TestClass]
    public class UnityDataDirWindowsTest
    {
        [TestMethod]
        public void Constructor()
        {
            Assert.IsTrue((new UnityDataDirWindows(@"D:\SteamLibrary\steamapps\common\ChaosGate\ChaosGate_Data\")).IsValid);
            Assert.IsTrue((new UnityDataDirWindows(@"D:\SteamLibrary\steamapps\common\ChaosGate\ChaosGate_Data")).IsValid);
            Assert.IsFalse((new UnityDataDirWindows(@"D:\SteamLibrary\steamapps\common\ChaosGate\")).IsValid);
            Assert.IsFalse((new UnityDataDirWindows(@"D:\SteamLibrary\steamapps\common\ChaosGate")).IsValid);
        }

        [TestMethod]
        public void Near()
        {
            Assert.IsTrue(UnityDataDirWindows.Near(@"D:\SteamLibrary\steamapps\common\ChaosGate\ChaosGate_Data\")?.IsValid);
            Assert.IsTrue(UnityDataDirWindows.Near(@"D:\SteamLibrary\steamapps\common\ChaosGate\ChaosGate_Data\StreamingAssets")?.IsValid);
            Assert.IsTrue(UnityDataDirWindows.Near(@"D:\SteamLibrary\steamapps\common\ChaosGate")?.IsValid);
            Assert.IsNull(UnityDataDirWindows.Near(@"D:\SteamLibrary\steamapps\common"));
            Assert.IsNull(UnityDataDirWindows.Near(@"D:\SteamLibrary\steamapps\comm"));
        }
    }
}
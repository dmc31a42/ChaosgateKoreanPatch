using ChaosgateKoreanPatch.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosgateKoreanPatch.Test
{
    [TestClass]
    public class UnityFolderTest
    {
        [TestMethod]
        public void Constructor()
        {
            Assert.IsNotNull((new UnityFolder(new UnityDataDirWindows(@"D:\SteamLibrary\steamapps\common\ChaosGate\ChaosGate_Data\"))));
        }

        [TestMethod]
        public void CompanyProductName()
        {
            var unityFolder = new UnityFolder(new UnityDataDirWindows(@"D:\SteamLibrary\steamapps\common\ChaosGate\ChaosGate_Data\"));
            Assert.AreEqual(unityFolder.CompanyName, "Complex Games Inc.");
            Assert.AreEqual(unityFolder.ProductName, "GreyKnights");
        }
    }
}

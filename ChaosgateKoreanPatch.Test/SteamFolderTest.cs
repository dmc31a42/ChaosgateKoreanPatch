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
    public class SteamFolderTest
    {
        [TestMethod]
        public void Constructor()
        {
            var steamFolder = new SteamFolderWindows();
        }

        [TestMethod]
        public void StaticMethod()
        {
            Assert.AreNotEqual(SteamFolderWindows.GetLibraryFolderFromVDF(@"C:\Program Files (x86)\Steam").Length, 0);
            Assert.AreEqual(SteamFolderWindows.GetLibraryFolderFromVDF(@"C:\Program Files (x86)").Length, 0);

            Assert.AreNotEqual(SteamFolderWindows.GetLibraryCommonByBruteForce().Length, 0);
        }
    }
}

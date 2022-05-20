using ChaosgateKoreanPatch.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosgateKoreanPatch.Test.ViewModels
{
    [TestClass]
    public class MainWindowViewModelTest
    {
        [TestMethod]
        public void Constructor()
        {
            var mainWindowVM = new MainWindowViewModel();
        }

        [TestMethod]
        public void Patch()
        {
            var mainWindowVM = new MainWindowViewModel();
            mainWindowVM.Patch();
        }

        [TestMethod]
        public void CheckFileAvailable()
        {
            var mainWindowVM = new MainWindowViewModel();
            mainWindowVM.CheckFileAvailable();
        }
    }
}

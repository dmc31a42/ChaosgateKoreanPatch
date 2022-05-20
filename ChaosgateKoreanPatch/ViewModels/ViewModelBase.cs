using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChaosgateKoreanPatch.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        private string test = "";
        private string Test
        {
            get => "";
            set => this.RaiseAndSetIfChanged(ref test, value);
        }
    }
}

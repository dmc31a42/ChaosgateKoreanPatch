using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ChaosgateKoreanPatch.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace ChaosgateKoreanPatch
{
    public class MyCoolObservableExceptionHandler : IObserver<Exception>
    {
        public void OnNext(Exception value)
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var exceptionWindow = new ExceptionWindow(value);
                exceptionWindow.ShowDialog(desktop.MainWindow);
                return;
            }
            RxApp.MainThreadScheduler.Schedule(() => { throw value; });

        }

        public void OnError(Exception error)
        {
            RxApp.MainThreadScheduler.Schedule(() => { throw error; });
        }

        public void OnCompleted()
        {
        }
    }
}

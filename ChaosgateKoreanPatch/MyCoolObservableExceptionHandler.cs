﻿using ReactiveUI;
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
            if (Debugger.IsAttached) Debugger.Break();

            //Analytics.Current.TrackEvent("MyRxHandler", new Dictionary<string, string>()
            //                                    {
            //                                        {"Type", value.GetType().ToString()},
            //                                        {"Message", value.Message},
            //                                    });

            RxApp.MainThreadScheduler.Schedule(() => { throw value; });
        }

        public void OnError(Exception error)
        {
            if (Debugger.IsAttached) Debugger.Break();

        //    Analytics.Current.TrackEvent("MyRxHandler Error", new Dictionary<string, string>()
        //{
        //    {"Type", error.GetType().ToString()},
        //    {"Message", error.Message},
        //});

            RxApp.MainThreadScheduler.Schedule(() => { throw error; });
        }

        public void OnCompleted()
        {
            if (Debugger.IsAttached) Debugger.Break();
            RxApp.MainThreadScheduler.Schedule(() => { throw new NotImplementedException(); });
        }
    }
}

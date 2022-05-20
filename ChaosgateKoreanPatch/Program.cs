using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ChaosgateKoreanPatch.Views;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ChaosgateKoreanPatch
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs eventArgs) =>
            //{
            //    Trace.WriteLine("UnobservedTaskException2");
            //    eventArgs.SetObserved();
            //    ((AggregateException)eventArgs.Exception).Handle(ex =>
            //    {
            //        Trace.WriteLine("Exception type: {0}" + ex.GetType());
            //        return true;
            //    });
            //};
            try
            {
                RxApp.DefaultExceptionHandler = new MyCoolObservableExceptionHandler() { };
                BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);

            }
            catch (Exception e)
            {
                BuildAvaloniaApp()
                .Start(AppMain, args);

            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionWindow exceptionWindow = new ExceptionWindow(e.ExceptionObject as Exception);
            exceptionWindow.Show();
        }

        private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Trace.WriteLine("UnobservedTaskException1");
            ExceptionWindow exceptionWindow = new ExceptionWindow(e.Exception);
            exceptionWindow.Show();

        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        // Application entry point. Avalonia is completely initialized.
        static void AppMain(Application app, string[] args)
        {
            // A cancellation token source that will be used to stop the main loop
            var cts = new CancellationTokenSource();

            // Do you startup code here
            new ExceptionWindow().Show();

            // Start the main loop
            app.Run(cts.Token);
        }
    }
}

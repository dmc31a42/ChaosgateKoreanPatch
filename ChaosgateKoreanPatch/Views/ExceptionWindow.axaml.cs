using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections;

namespace ChaosgateKoreanPatch.Views
{
    public partial class ExceptionWindow : Window
    {
        public ExceptionWindow()
        {
            InitializeComponent();
        }

        Exception? exception;

        public ExceptionWindow(Exception e)
        {
            InitializeComponent();
            exception = e;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            Exception? myE = exception;
            string text = "";
            while (myE != null)
            {
                text += "------------------------------------------------------\n";
                text += "Message: \n" + myE?.Message + "\n------------------------------------------------------\n";
                text += "Source: \n" + myE?.Source + "\n------------------------------------------------------\n";
                text += "TargetSite: \n" + myE?.TargetSite + "\n------------------------------------------------------\n";
                text += "Target in class: \n" + myE?.TargetSite?.DeclaringType + "\n------------------------------------------------------\n";
                text += "Member type: \n" + myE?.TargetSite?.MemberType + "\n------------------------------------------------------\n";
                text += "StackTrace: \n" + myE?.StackTrace + "\n------------------------------------------------------\n";
                text += "Data: \n------------------------------------------------------\n";
                if (myE?.Data != null)
                {
                    foreach (DictionaryEntry de in myE.Data)
                    {
                        text += de.Key + ":\n------------------------------------------------------\n" +
                            de.Value + "\n------------------------------------------------------\n";
                    }
                }
                text += "\n------------------------------------------------------\n";
                myE = myE.InnerException;
            }
            this.FindControl<TextBox>("textBox1").Text = text;
        }
    }
}

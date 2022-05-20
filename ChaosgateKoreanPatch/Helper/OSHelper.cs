using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ChaosgateKoreanPatch.Helper
{
    public enum Platform
    {
        None,
        Windows,
        Linux,
        macOS,
        Android,
        iOS,
        Unknown
    }

    public class OSHelper
    {
        
        static Platform RuntimeOS
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    return Platform.Windows;
                else if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                    return Platform.Linux;
                else if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                    return Platform.macOS;
                else
                    return Platform.Unknown;
            }
        }
    }
    
}

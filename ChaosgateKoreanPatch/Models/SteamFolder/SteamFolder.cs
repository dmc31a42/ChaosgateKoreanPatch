using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosgateKoreanPatch.Models
{
    public interface SteamFolder
    {
        public string[] folders { get; }

        static SteamFolder Get()
        {
            throw new NotImplementedException();
        }
    }
}

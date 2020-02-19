using System.Collections.Generic;
using System.Linq;
using Core.Managers;

namespace Core
{
    public static class Foundation
    {
        public static List<GlobalAccess> Globals = new List<GlobalAccess>();
        public static GameManager GameManager;

        public static T GetGlobalClass<T>() where T : GlobalClass
        {
            return Globals.OfType<T>().FirstOrDefault();
        }
    }
}
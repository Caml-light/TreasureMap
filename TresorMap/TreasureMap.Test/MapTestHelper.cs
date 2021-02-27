using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreasureMap.Test
{
    public static class MapTestHelper
    {
        public static Map GetMap()
        {
            Map map = new();
            map.ParseFromLine("C-3-2");
            return map;
        }
    }
}

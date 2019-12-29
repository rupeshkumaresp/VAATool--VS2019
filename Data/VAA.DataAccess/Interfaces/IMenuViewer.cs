using System;
using System.Collections.Generic;
using System.Linq;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess.Interfaces
{
    interface IMenuViewer
    {
        List<MenuData> GetListForMenuViewer(long cycle, int menuclass, int menutype, int departure, int arrival, string flightno);
    }
}

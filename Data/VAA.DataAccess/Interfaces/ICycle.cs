using System;
using System.Collections.Generic;
using System.Linq;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess.Interfaces
{
    /// <summary>
    /// Cycle related operations
    /// </summary>
    public interface ICycle
    {
        List<Cycle> GetCycles();
        Cycle GetCycle(long cycleId);
        List<string> GetCycleWeek(long cycleId);
        Cycle CreateNewCycle(Cycle cycle);
        Cycle GetActiveCycle();
        bool DeleteCycle(long cycleId);
        bool UpdateCycle(Cycle cycle);
        bool IsCycleActive(long cycleId);
        bool IsCycleArchived(long cycleId);
        bool IsCycleLocked(long cycleId);
        List<tCycleWeek> GetWeeksAndDates(long cycleId);
        bool IsCycleHasLiveOrders(long cycleId);
        bool UpdateCyclesActiveState(long cycleId);
    }

}

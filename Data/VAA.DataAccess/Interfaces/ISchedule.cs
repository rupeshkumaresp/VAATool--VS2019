using System;
using System.Collections.Generic;
using System.Linq;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess.Interfaces
{
    interface ISchedule
    {
        List<tSchedules> GetAllSchedules();
        Schedules GetScheduleById(int scheduleid);
        long CreateNewSchedule(Schedules schedule);
        long UpdateSchedules(Schedules schedule);
        void UpdateStatus(Schedules schedule);
    }
}

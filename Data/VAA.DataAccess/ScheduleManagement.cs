using System;
using System.Collections.Generic;
using System.Linq;
using VAA.DataAccess.Interfaces;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess
{
    public class ScheduleManagement : ISchedule
    {
        readonly VAAEntities _context = new VAAEntities();

        public List<tSchedules> GetAllSchedules()
        {
            var schdulesData = (from data in _context.tSchedules
                                select new
                                {
                                    ID = data.ID,
                                    Subject = data.Subject,
                                    Start = data.Start,
                                    End = data.End,
                                    UserID = data.UserID,
                                    RecurrenceRule = data.RecurrenceRule,
                                    RecurrenceParentID = data.RecurrenceParentID,
                                    Annotations = data.Annotations,
                                    Description = data.Description,
                                    Remainder = data.Remainder,
                                    Completed = data.Completed,
                                    ColorId = data.ColorID
                                }).ToList();

            return (from x in schdulesData
                    select new tSchedules
                    {
                        ID = x.ID,
                        Subject = x.Subject,
                        Start = x.Start,
                        End = x.End,
                        UserID = x.UserID,
                        RecurrenceRule = x.RecurrenceRule,
                        RecurrenceParentID = x.RecurrenceParentID,
                        Annotations = x.Annotations,
                        Description = x.Description,
                        Remainder = x.Remainder,
                        Completed = x.Completed,
                        ColorID = x.ColorId
                    }).ToList();
        }
        public Schedules GetScheduleById(int scheduleid)
        {
            var scheduledata = (from schedules in _context.tSchedules where schedules.ID == scheduleid select schedules).FirstOrDefault();

            if (scheduledata != null)
            {
                return new Schedules()
                {
                    ID = scheduledata.ID,
                    Subject = scheduledata.Subject,
                    Start = scheduledata.Start,
                    End = scheduledata.End,
                    UserID = scheduledata.UserID,
                    RecurrenceRule = scheduledata.RecurrenceRule,
                    RecurrenceParentID = scheduledata.RecurrenceParentID,
                    Annotations = scheduledata.Annotations,
                    Description = scheduledata.Description,
                    Remainder = scheduledata.Remainder,
                    Completed = scheduledata.Completed,
                    ColorID = scheduledata.ColorID
                };
            }
            return null;
        }
        public long CreateNewSchedule(Schedules schedule)
        {
            try
            {
                tSchedules newschedule = new tSchedules
                {
                    Subject = schedule.Subject,
                    Start = schedule.Start,
                    End = schedule.End,
                    UserID = schedule.UserID,
                    RecurrenceRule = schedule.RecurrenceRule,
                    RecurrenceParentID = schedule.RecurrenceParentID,
                    Annotations = schedule.Annotations,
                    Description = schedule.Description,
                    Remainder = schedule.Remainder,
                    Completed = schedule.Completed,
                    ColorID = schedule.ColorID
                };
                _context.tSchedules.Add(newschedule);
                _context.SaveChanges();

                var newScheduleId = newschedule.ID;
                var scheduleUpdate = (from tSchedules in _context.tSchedules where tSchedules.ID == newScheduleId select tSchedules).FirstOrDefault();

                if (scheduleUpdate.RecurrenceParentID == 0)
                {
                    scheduleUpdate.RecurrenceParentID = null;
                    _context.SaveChanges();
                }

                return newschedule.ID;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public long UpdateSchedules(Schedules schedule)
        {
            try
            {
                var scheduleUpdate = (from tSchedules in _context.tSchedules where tSchedules.ID == schedule.ID select tSchedules).FirstOrDefault();

                if (scheduleUpdate != null)
                {
                    scheduleUpdate.Subject = schedule.Subject;
                    scheduleUpdate.Start = schedule.Start;
                    scheduleUpdate.End = schedule.End;
                    scheduleUpdate.UserID = schedule.UserID;
                    scheduleUpdate.RecurrenceRule = schedule.RecurrenceRule;
                    scheduleUpdate.RecurrenceParentID = schedule.RecurrenceParentID;
                    scheduleUpdate.Annotations = schedule.Annotations;
                    scheduleUpdate.Description = schedule.Description;
                    scheduleUpdate.Remainder = schedule.Remainder;
                    scheduleUpdate.Completed = schedule.Completed;
                    scheduleUpdate.ColorID = schedule.ColorID;
                    _context.SaveChanges();

                    var newScheduleId = scheduleUpdate.ID;
                    var parentIdUpdate = (from tSchedules in _context.tSchedules where tSchedules.ID == newScheduleId select tSchedules).FirstOrDefault();

                    if (parentIdUpdate.RecurrenceParentID == 0)
                    {
                        parentIdUpdate.RecurrenceParentID = null;
                        _context.SaveChanges();
                    }
                    return newScheduleId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public bool DeleteSchedule(int scheduleid)
        {
            try
            {
                var scheduleData = (from data in _context.tSchedules where data.ID == scheduleid select data).FirstOrDefault();
                if (scheduleData != null)
                {
                    _context.tSchedules.Remove(scheduleData);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public void UpdateStatus(Schedules schedule)
        {
            var scheduleUpdate = (from tSchedules in _context.tSchedules where tSchedules.ID == schedule.ID select tSchedules).FirstOrDefault();

            if (scheduleUpdate != null)
            {
                scheduleUpdate.Completed = schedule.Completed;
                _context.SaveChanges();
            }
        }
    }
}

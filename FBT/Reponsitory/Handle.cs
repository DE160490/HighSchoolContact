using FBT.Models;
using System.Xml.Linq;

namespace FBT.Reponsitory
{
    public class Handle
    {
        public static string CreateScheduleID()
        {
            try {
                using(var contextdb = new FbtContext())
                {
                    var scheduleID = contextdb.Schedules.Max(item => item.ScheduleId);
                    string part1 = scheduleID.Substring(0, 4);
                    string part2 = scheduleID.Substring(4);
                    int convertInt = Convert.ToInt32(part2);
                    convertInt++;
                    if (convertInt <= 9)
                    {
                        return part1 + "00000" + convertInt;
                    }
                    else if (convertInt <= 99)
                    {
                        return part1 + "0000" + convertInt;
                    }
                    else if (convertInt <= 999)
                    {
                        return part1 + "000" + convertInt;
                    }
                    else if (convertInt <= 9999)
                    {
                        return part1 + "00" + convertInt;
                    }
                    else if (convertInt <= 99999)
                    {
                        return part1 + "0" + convertInt;
                    }
                    else if (convertInt <= 999999)
                    {
                        return part1 + convertInt;
                    }
                    else
                    {
                        string character = part1.Substring(3);
                        char inc = Convert.ToChar(Convert.ToInt32(character.ToCharArray()[0]) + 1);
                        return part1.Substring(0, 3) + inc + "000001";
                    }
                }
            }catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }

            return "";
        }

        public static bool checkValidateSchedule(Schedule schedule)
        {
            if(schedule == null)
            {
                return false;
            }else if(schedule != null)
            {
                using (var dbContext = new FbtContext())
                {
                    if(!dbContext.Teachers.Any(item => item.TeacherId == schedule.TeacherId))
                    {
                        Console.WriteLine("Don't have Teacher have ID: " + schedule.TeacherId);
                        return false;
                    }
                    if(!dbContext.Classes.Any(item => item.ClassId == schedule.ClassId))
                    {
                        Console.WriteLine("Don't have ClassID: " + schedule.ClassId);
                        return false;
                    }
                    if(!dbContext.SubjectTeachers.Any(item => item.TeacherId == schedule.TeacherId &&  item.ClassId == schedule.ClassId))
                    {
                        Console.WriteLine("Teacher have ID: " + schedule.TeacherId + " don't teaching class have ID: " + schedule.ClassId);
                        return false;
                    }
                    if(!dbContext.Subjects.Any(item => item.SubjectId == schedule.SubjectId))
                    {
                        Console.WriteLine("Don't have SubjectID: " + schedule.SubjectId);
                        return false;
                    }
                    if (dbContext.Schedules.Any(item => item.ClassId == schedule.ClassId &&
                                                item.WeekEnds > schedule.WeekBegins &&
                                                item.DayOfWeek == schedule.DayOfWeek &&
                                                item.Lecture == schedule.Lecture &&
                                                item.TimeEnd > schedule.TimeStart &&
                                                item.WeekEnds >= schedule.DayOfWeek))
                    {
                        Console.WriteLine("Current time: " + schedule.TimeStart + " " + schedule.DayOfWeek + " schedule had exist!");
                        return false;
                    }
                }
            }

            return true;
        }

        public static List<Schedule> SplitScheduleInput(string inputString)
        {
            List<Schedule> list = new List<Schedule>();

            string[] scheduleinput = inputString.Split("$@");
            scheduleinput = scheduleinput.Take(scheduleinput.Length - 1).ToArray();
            foreach (string schedule in scheduleinput)
            {
                Schedule scheduleinsert = new Schedule();
                //scheduleinsert.ScheduleId = CreateScheduleID();
                scheduleinsert.ScheduleId = " ";
                string[] element = schedule.Split(", ");
                foreach (var item in element)
                {
                    string[] key_value = item.Split(": ");
                    if (key_value[0].Trim() == "teacherID") { scheduleinsert.TeacherId = key_value[1]; }
                    else if (key_value[0].Trim() == "classID") { scheduleinsert.ClassId = key_value[1]; }
                    else if (key_value[0].Trim() == "subjectID") { scheduleinsert.SubjectId = key_value[1]; }
                    else if (key_value[0].Trim() == "weekBegins") { scheduleinsert.WeekBegins = DateTime.Parse(key_value[1]); }
                    else if (key_value[0].Trim() == "weekEnds") { scheduleinsert.WeekEnds = DateTime.Parse(key_value[1]); }
                    else if (key_value[0].Trim() == "dayofweek") { scheduleinsert.DayOfWeek = DateTime.Parse(key_value[1]); }
                    else if (key_value[0].Trim() == "lecture") { scheduleinsert.Lecture = key_value[1]; }
                    else if (key_value[0].Trim() == "timeStart") 
                    { 
                        DateTime convert = DateTime.Parse(key_value[1]);
                        scheduleinsert.TimeStart = convert.TimeOfDay;
                    }
                    else if (key_value[0].Trim() == "timeEnd")
                    {
                        DateTime convert = DateTime.Parse(key_value[1]);
                        scheduleinsert.TimeEnd = convert.TimeOfDay;
                    }
                }

                var getSchedule = scheduleinsert.ScheduleId + " " + scheduleinsert.TeacherId + " " + scheduleinsert.ClassId + " " + scheduleinsert.SubjectId + " " +
                scheduleinsert.WeekBegins + " " + scheduleinsert.WeekEnds + " " + scheduleinsert.DayOfWeek + " " + scheduleinsert.Lecture + " " + scheduleinsert.TimeStart + " " + scheduleinsert.TimeEnd;
                Console.WriteLine(getSchedule);

                if (checkValidateSchedule(scheduleinsert))
                {
                    //using (var dbContext = new FbtContext())
                    //{
                    //    dbContext.Schedules.Add(scheduleinsert);
                    //    dbContext.SaveChanges();
                    //}
                    list.Add(scheduleinsert);
                }
                else
                {
                    Console.WriteLine("Day: " + scheduleinsert.DayOfWeek + " Database insert not success!");
                }

            }
            return list;
        }
    }
}

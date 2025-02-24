using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_sheduler
{
    class TaskItem
    {
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public TimeSpan DueTime { get; set; }
        public int Priority { get; set; }

        public TaskItem(string name, DateTime dueDate, TimeSpan dueTime, int priority)
        {
            Name = name;
            DueDate = dueDate;
            DueTime = dueTime;
            Priority = priority;
        }

        public override string ToString() =>
            $"Task: {Name} | Due Date: {DueDate:yyyy-MM-dd} | Time: {DueTime:hh\\:mm} | Priority: {Priority}";
    }
}

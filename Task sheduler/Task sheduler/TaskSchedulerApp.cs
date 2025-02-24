using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Task_sheduler
{
    class TaskSchedulerApp
    {
        static List<TaskItem> taskList = new List<TaskItem>();
        static string filePath = "tasks.json"; // File to store tasks
        static bool isRunning = true; // Control background notifications

        static void Main()
        {
            LoadTasks(); // Load tasks from file
            Thread notificationThread = new Thread(CheckNotifications);
            notificationThread.Start(); // Start background notification checking

            while (true)

            {
                Console.WriteLine("\nTask Scheduler - Choose an option:");
                Console.WriteLine("1. Add Task");
                Console.WriteLine("2. Update Task");
                Console.WriteLine("3. Delete Task");
                Console.WriteLine("4. View Sorted Tasks");
                Console.WriteLine("5. Exit");
                Console.Write("Enter your choice: ");

                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    isRunning = false; // Stop background notifications
                    SaveTasks();
                    return;
                }

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Invalid input! Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1: AddTask(); break;
                    case 2: UpdateTask(); break;
                    case 3: DeleteTask(); break;
                    case 4: ViewTasks(); break;
                    case 5:
                        isRunning = false; // Stop notification thread
                        SaveTasks();
                        return;
                    default: Console.WriteLine("Invalid choice! Try again."); break;
                }
            }
        }

        static void AddTask()
        {
            Console.Write("Enter Task Name: ");
            string name = Console.ReadLine();
            if (CheckExit(name)) return;

            Console.Write("Enter Due Date (yyyy-MM-dd): ");
            string dueDateInput = Console.ReadLine();
            if (CheckExit(dueDateInput)) return;

            if (!DateTime.TryParse(dueDateInput, out DateTime dueDate))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.Write("Enter Due Time (HH:mm): ");
            string dueTimeInput = Console.ReadLine();
            if (CheckExit(dueTimeInput)) return;

            if (!TimeSpan.TryParse(dueTimeInput, out TimeSpan dueTime))
            {
                Console.WriteLine("Invalid time format.");
                return;
            }

            Console.Write("Enter Priority (1-High, 2-Medium, 3-Low): ");
            string priorityInput = Console.ReadLine();
            if (CheckExit(priorityInput)) return;

            if (!int.TryParse(priorityInput, out int priority) || priority < 1 || priority > 3)
            {
                Console.WriteLine("Invalid priority input.");
                return;
            }

            taskList.Add(new TaskItem(name, dueDate, dueTime, priority));
            SaveTasks();
            Console.WriteLine("Task added successfully!");
        }

        static void UpdateTask()
        {
            Console.Write("Enter the Task Name to Update: ");
            string name = Console.ReadLine();
            if (CheckExit(name)) return;

            TaskItem task = taskList.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                Console.WriteLine("Task not found!");
                return;
            }

            Console.Write("Enter New Task Name (or press Enter to keep the same): ");
            string newName = Console.ReadLine();
            if (!string.IsNullOrEmpty(newName)) task.Name = newName;

            Console.Write("Enter New Due Date (yyyy-MM-dd) or press Enter to keep the same: ");
            string newDateStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDateStr) && DateTime.TryParse(newDateStr, out DateTime newDueDate))
                task.DueDate = newDueDate;

            Console.Write("Enter New Due Time (HH:mm) or press Enter to keep the same: ");
            string newTimeStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(newTimeStr) && TimeSpan.TryParse(newTimeStr, out TimeSpan newDueTime))
                task.DueTime = newDueTime;

            Console.Write("Enter New Priority (1-High, 2-Medium, 3-Low) or press Enter to keep the same: ");
            string newPriorityStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPriorityStr) && int.TryParse(newPriorityStr, out int newPriority))
                task.Priority = newPriority;

            SaveTasks();
            Console.WriteLine("Task updated successfully!");
        }

        static void DeleteTask()
        {
            Console.Write("Enter the Task Name to Delete: ");
            string name = Console.ReadLine();
            if (CheckExit(name)) return;

            TaskItem task = taskList.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                Console.WriteLine("Task not found!");
                return;
            }

            taskList.Remove(task);
            SaveTasks();
            Console.WriteLine("Task deleted successfully!");
        }

        static void ViewTasks()
        {
            if (taskList.Count == 0)
            {
                Console.WriteLine("No tasks available.");
                return;
            }

            // Sorting tasks
            BubbleSortByPriority();
            MergeSortByTime(taskList, 0, taskList.Count - 1);
            QuickSortByDate(taskList, 0, taskList.Count - 1);

            Console.WriteLine("\nTask List:");
            foreach (var task in taskList)
            {
                Console.WriteLine(task);
            }
        }

        static void SaveTasks()
        {
            string json = JsonSerializer.Serialize(taskList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        static void LoadTasks()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                taskList = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
            }
        }

        static void CheckNotifications()
        {
            while (isRunning)
            {
                DateTime now = DateTime.Now;
                foreach (var task in taskList)
                {
                    DateTime taskTime = task.DueDate.Date + task.DueTime;

                    if (taskTime > now && taskTime <= now.AddMinutes(5))
                    {
                        Console.WriteLine($"\n⏰ Reminder: Task '{task.Name}' is due at {task.DueDate:yyyy-MM-dd} {task.DueTime:hh\\:mm}");
                    }
                }
                Thread.Sleep(30000); // Check every 30 seconds
            }
        }

        static bool CheckExit(string input) => input.ToLower() == "exit";

        // Sorting Methods

        // Bubble Sort by Priority (High > Medium > Low)
        static void BubbleSortByPriority()
        {
            for (int i = 0; i < taskList.Count - 1; i++)
            {
                for (int j = 0; j < taskList.Count - i - 1; j++)
                {
                    if (taskList[j].Priority > taskList[j + 1].Priority)
                    {
                        var temp = taskList[j];
                        taskList[j] = taskList[j + 1];
                        taskList[j + 1] = temp;
                    }
                }
            }
        }

        // Merge Sort by Due Time
        static void MergeSortByTime(List<TaskItem> list, int left, int right)
        {
            if (left < right)
            {
                int middle = (left + right) / 2;
                MergeSortByTime(list, left, middle);
                MergeSortByTime(list, middle + 1, right);
                MergeByTime(list, left, middle, right);
            }
        }

        static void MergeByTime(List<TaskItem> list, int left, int middle, int right)
        {
            int n1 = middle - left + 1;
            int n2 = right - middle;
            var leftList = new List<TaskItem>();
            var rightList = new List<TaskItem>();

            for (int i = 0; i < n1; i++)
                leftList.Add(list[left + i]);
            for (int j = 0; j < n2; j++)
                rightList.Add(list[middle + 1 + j]);

            int k = left, i1 = 0, i2 = 0;

            while (i1 < n1 && i2 < n2)
            {
                if (leftList[i1].DueTime <= rightList[i2].DueTime)
                    list[k++] = leftList[i1++];
                else
                    list[k++] = rightList[i2++];
            }

            while (i1 < n1) list[k++] = leftList[i1++];
            while (i2 < n2) list[k++] = rightList[i2++];
        }

        // Quick Sort by Due Date
        static void QuickSortByDate(List<TaskItem> list, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(list, low, high);
                QuickSortByDate(list, low, pivotIndex - 1);
                QuickSortByDate(list, pivotIndex + 1, high);
            }
        }

        static int Partition(List<TaskItem> list, int low, int high)
        {
            DateTime pivot = list[high].DueDate;
            int i = low - 1;
            for (int j = low; j < high; j++)
            {
                if (list[j].DueDate < pivot)
                {
                    i++;
                    var temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }
            var swap = list[i + 1];
            list[i + 1] = list[high];
            list[high] = swap;
            return i + 1;
        }
    }
}

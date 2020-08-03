using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Mongo
{
    public class TaskManagerDatabaseSettings : ITaskManagerDatabaseSettings
    {
        public string TasksCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public string DatabaseName { get; set; }
    }


    public interface ITaskManagerDatabaseSettings
    {
        string TasksCollectionName { get; set; }
        string UsersCollectionName { get; set; }
        string DatabaseName { get; set; }
    }
}

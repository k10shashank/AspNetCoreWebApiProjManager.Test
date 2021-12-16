using System;

namespace AspNetCoreWebApiProjManager.Test.Entities
{
    public class TaskModel
    {
        public int ID_TASK { get; set; }
        
        public string DETAILS { get; set; }
        
        public DateTime CREATED_ON { get; set; }

        public string STATUS { get; set; }
        
        public ProjectModel PROJECT { get; set; }
        
        public UserModel USER { get; set; }
    }
}

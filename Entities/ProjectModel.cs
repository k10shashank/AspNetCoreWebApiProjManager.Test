using System;

namespace AspNetCoreWebApiProjManager.Test.Entities
{
    public class ProjectModel
    {
        public int ID_PROJECT { get; set; }
        public string NAME { get; set; }
        public string DETAILS { get; set; }
        public DateTime CREATED_ON { get; set; }
    }
}

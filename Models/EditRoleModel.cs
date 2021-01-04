using System.Collections.Generic;

namespace LogInWeb.Models
{
    public class EditRoleModel
    {
        public EditRoleModel()
        {
            Users=new List<string>();
        }
        public string RoleId { get; set; }
        public string RoleName{get;set;}
        public List<string> Users{get;set;}

    }
}
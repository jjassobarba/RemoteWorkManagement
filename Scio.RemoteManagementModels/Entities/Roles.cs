using System.Collections.Generic;

namespace Scio.RemoteManagementModels.Entities
{
    public class Roles
    {
        public virtual int Id { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string ApplicationName { get; set; }
        public virtual IList<Users> UsersInRole { get; set; }

        public Roles()
        {
            UsersInRole = new List<Users>();
        }
    }
}

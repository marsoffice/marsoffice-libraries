using System;
using System.Collections.Generic;
using System.Text;

namespace MarsOffice.Dto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsDisabled { get; set; }
        public IEnumerable<string> RoleIds { get; set; }
        public IEnumerable<string> GroupIds{ get; set; }
    }
}

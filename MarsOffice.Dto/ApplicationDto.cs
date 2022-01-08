using System;
using System.Collections.Generic;
using System.Text;

namespace MarsOffice.Dto
{
    public class ApplicationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<RoleDto> Roles { get; set; }
        public bool IsDisabled { get; set; }
    }
}

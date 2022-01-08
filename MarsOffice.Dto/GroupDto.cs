using System;
using System.Collections.Generic;
using System.Text;

namespace MarsOffice.Dto
{
    public class GroupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public IEnumerable<string> ChildrenIds { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework_Task.Entities
{
    internal class Assignment : BaseEntity
    {
        public string Content { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}

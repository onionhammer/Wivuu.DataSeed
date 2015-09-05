using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wivuu.DataSeed.Tests.Domain
{
    public class Class
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid SchoolId { get; set; }

        public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();
        public virtual School School { get; set; }
    }
}

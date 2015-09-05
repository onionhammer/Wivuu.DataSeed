using System;
using System.Collections.Generic;

namespace Wivuu.DataSeed.Tests.Domain
{
    public class Class
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();
    }
}

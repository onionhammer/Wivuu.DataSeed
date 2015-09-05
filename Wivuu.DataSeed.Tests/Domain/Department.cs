using System;
using System.Collections.Generic;

namespace Wivuu.DataSeed.Tests.Domain
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual School School { get; set; }
        public ICollection<Class> Classes { get; set; } = new HashSet<Class>();
    }
}

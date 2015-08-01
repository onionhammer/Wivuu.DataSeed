using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wivuu.DataSeed.Tests.Domain
{
    public class School
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Class> Classes { get; set; } = new HashSet<Class>();
        public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();
    }
}

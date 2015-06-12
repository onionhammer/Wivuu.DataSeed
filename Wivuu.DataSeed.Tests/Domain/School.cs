using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wivuu.DataSeed.Tests.Domain
{
    public class School
    {
        public School()
        {
            this.Classes  = new HashSet<Class>();
            this.Students = new HashSet<Student>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}

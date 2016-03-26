using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wivuu.DataSeed.Tests.Domain
{
    public class ProtectedEntity
    {
        public int Id { get; protected set; }

        public string Name { get; protected set; }

        public int Age { get; protected set; }

        public ProtectedEntity(int id, string name = "", int age = 0)
        {
            this.Id   = id;
            this.Name = name;
            this.Age  = age;
        }

        internal ProtectedEntity() { }
    }
}
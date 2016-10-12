using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wivuu.DataSeed
{
    [Table("__DataMigrationHistory")]
    internal class DataMigrationHistory
    {
        [Key]
        public string MigrationId { get; set; }

        public string ContextKey { get; set; }
    }
}

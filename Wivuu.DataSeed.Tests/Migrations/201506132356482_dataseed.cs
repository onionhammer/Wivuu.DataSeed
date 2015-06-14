namespace Wivuu.DataSeed.Tests.Migrations
{
    using System;
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;

    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public partial class dataseed : InitialDataMigration, IMigrationMetadata
    {
        string IMigrationMetadata.Id
        {
            get { return "201506132356482_dataseed"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return "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"; }
        }
    }
}

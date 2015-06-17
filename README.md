# Purpose

The purpose of DataSeed is to help you manage the data that gets pre-filled
in your database when using EntityFramework's code-first model. DataSeed adds
'migration' rows to a __DataMigrations table to keep track of which data 
migrations have already been performed, similarly to how EntityFramework tracks
schema changes.

# Usage

Using DataSeed is simple, after installing the package to your code-first 
enabled project, open your Configuration.cs file, in the `Seed` method, add
the following call:

```C#
protected override void Seed(MyDbContext context)
{
    this.Execute(context);
}
```

This is DataSeed's hook into the standard EntityFramework seeding process. Now
DataSeed will automatically be looped in when you invoke `update-database`.

## Your first Data Migration
Now you can create a new class extending from DataMigration&lt;T&gt;, where T 
is your DbContext class.

```C#
public class AddClasses : DataMigration<MyDbContext>
{
    protected override void Apply(MyDbContext context)
    {
        // Add classes
        context.Classes.Add(new Class
        {
            Id   = Guid.NewGuid(),
            Name = "Biology 101"
        });

        context.Classes.Add(new Class
        {
            Id   = Guid.NewGuid(),
            Name = "Physics 201"
        });
    }

    public override int Order
    {
        get { return 1; }
    }
}
```

# Advanced usage

## Always Run

Enable `AlwaysRun` if you want your data seed migration class to run every
time `update-database` is invoked. Note that you will frequently pair this with
a `Cleanup` method. An example where you might use this is if you are seeding
a database table with an XML or JSON file that's stored in your project.

```C#
public override bool AlwaysRun
{
    get { return true; }
}
```

## Cleanup

If your seed needs to perform cleanup in between running, override the `Cleanup`
command.

```C#
public override void Apply(MyDbContext context)
{
    // Load data from file
    var data = ReadFromFile("file.json");
    
    foreach (var item in data)
        context.MyTable.Add(item);
}

public override void Cleanup(MyDbContext context)
{
    // Remove data
    foreach (var item in context.MyTable.ToList())
        context.MyTable.Remove(item);
}
```

# Future

- Add powershell snippet: enable-data-migrations
- Add powershell snippet: add-data-migration
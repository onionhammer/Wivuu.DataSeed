# Update 1.1.10
- Contains breaking changes


# Purpose

The purpose of DataSeed is to help you manage the data that gets pre-filled
in your database when using EntityFramework's code-first model. DataSeed adds
'migration' rows to a __DataMigrations table to keep track of which data 
migrations have already been performed, similarly to how EntityFramework tracks
schema changes.

# Usage

## Invoke the dataseed
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

## Setting up the DataMigrationHistory table
The next step is to update your DbContext class to extend the SeededDbContext class 
(which in turn extends the DbContext). This adds a __DataMigrationHistory table
to your database to help track which data migrations have already been applied. 
That done, invoke 'add-migration dataseed' from your Package Manager Console, this 
will add a migration for your new table.

## Your first Data Migration
Now you can create a new class extending from DataMigration&lt;T&gt;, where T 
is your DbContext class.

```C#
public class AddClasses : DataMigration<MyDbContext>
{
    protected Random Random { get; } = new Random(0x01);
    
    protected override void Apply(MyDbContext context)
    {
        // Add classes
        context.Classes.Add(new Class
        {
            Id   = Random.NextGuid(),
            Name = "Biology 101"
        });

        context.Classes.Add(new Class
        {
            Id   = Random.NextGuid(),
            Name = "Physics 201"
        });
    }

    public override int Order => 1;
}
```

# Advanced usage

## Adding or Updating a model

Often times when a migration is running, you want it to effectively be 'stateless', in
this case meaning it doesn't matter whether the migration has been performed before or not,
you just want the data to look like "this". As of 1.0.7, DataSeed has a new method of mapping
your seed data onto your database, called an `Update`. There are three primary types of `Update`.

### Domain Model to Domain Model
```C#
// Find a class (by key)
db.Classes.Find(biologyId)
    // Update the class with the following values
    .Update(new Class
    {
        Id         = biologyId,
        Name       = "Biology 101",
        Department = scienceDept
    })
    // If the model didn't exist, do the following
    .Default(c => db.Classes.Add(c));
```

### Other object to Domain Model
```C#
db.Classes.Find(physicsId)
    // Note the use of an anonymous type
    .Update(new
    {
        Id         = physicsId,
        Name       = "Physics 201",
        Department = scienceDept
    })
    .Default(c => db.Classes.Add(c));
```

### Dictionary to Domain Model
```C#
db.Classes.Find(physicsId)
    .Update(new Dictionary<string, object>
    {
        ["Id"]         = physicsId,
        ["Name"]       = "Physics 201",
        ["Department"] = scienceDept
    })
    .Default(c => db.Classes.Add(c));
```

The above methods are available for any instance of a class when using the Wivuu.DataSeed namespace. They
are built on top of Reflection.Emit, any mapping performed is cached in an in-memory assembly at runtime.

## Always Run

Enable `AlwaysRun` if you want your data seed migration class to run every
time `update-database` is invoked. Note that you will frequently pair this with
a `Cleanup` method. An example where you might use this is if you are seeding
a database table with an XML or JSON file that's stored in your project.

```C#
public override bool AlwaysRun => true;
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

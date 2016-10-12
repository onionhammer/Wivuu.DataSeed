# Update 1.0.11
- Updated seed mechanism, DbContext no longer needs to extend SeededDbContext, which has been deprecated. 
- 'View' creation system for creating queries of readonly objects

(10/12/2016) The below document has been updated to reflect intended usecase.

# Purpose

The purpose of DataSeed is to help you manage the data that gets pre-filled
in your database when using EntityFramework's code-first model. DataSeed helps
you pre-fill your database with lookup tables and testing data.

# Usage

## Create a seed
Create a new Seed class (by convention, in a /DataMigrations folder) 
to inject data into your database with `update-database`.

```C#
public class AddClassesSample : Seed<MyDbContext>
{
    protected Random Random { get; } = new Random(0x01);

	// Only run this seed once
	public override bool ShouldRun(MyDbContext context) =>
		context.Classes.Any() == false;
    
	// Apply the seed
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
}
```

## Invoke the dataseed
Using DataSeed is simple, after installing the package to your code-first 
enabled project, open your Configuration.cs file, in the `Seed` method, add
the following call:

```C#
protected override void Seed(MyDbContext context)
{
    this.Execute(context, new [] {
		new AddClassesSample(),
		// ... Add more seeds here
	});
}
```

This is DataSeed's hook into the standard EntityFramework seeding process. Now
DataSeed will automatically be looped in when you invoke `update-database`. The 
input Seed classes are run in-order, inside a transaction. If any of the seeds failed,
the transaction is rolled back.

## Future
- Further documentation is coming to address usage of 'DbViews'.
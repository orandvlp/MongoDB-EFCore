I'm trying to work with MongoDB and Entity Framework Core. 

I'm following DDD where the business logic is implemented in rich domain entities (see Entity folder).

The persistence is done using POCO classes (see DbModel folder). These classes have no business logic and only have annotations related to MongoDB and a mapping method.

The main principle of this approach is that all operations are done at the domain entity level. The POCO classes are only used to map the domain entities to the database.

Due to how Entity Framework Core works, I need to make sure that all the changes are tracked by the context, which I find a bit cumbersome. 

The version in the `main` branch is the version that uses domain entities for the operations but I couldn't get it to work.

I made a few attempts as you will see in the commented code in `AddOrderLineToOrderAsync` (the comments are marked with `<<<<<<<<`)

The version in the `This is working` branch operates directly on the POCO classes and it works.

Is there a clean way to achieve what I'm trying to do?


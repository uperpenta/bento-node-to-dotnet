Observations:

* Most class properties should be initalized, string could/can be empty at first with string.Empty
* LINQ C# native query language as ORM
* very easy to DI
* types at compilation, much stricter
* Task - represents an operation that can be cancelled/faulted/completed - doesnt return anything, Task<T> it's the same
  but returns something of type T
* IResult - minimal API - NO MVC, IActionResult for MVC Api controllers, class based APIs.
* DbContext is scoped, not to be used in looping logic like a poller.
* Use IHttpClientFactory DI for many http requests
* For background services, in .NET we can use BackgroundService (AddHostedService), that runs automatically. Das nice
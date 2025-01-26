This is a simple MVC app with 1 view (Admin) and 2 Controllers, Admin (MVC Style) and UserLink (API controller). Generation of Links is done via UserLink Service which is injected using DI in the Program.cs file.
I decided to go with in-memory cache where the link is stored via its GUID. There are a couple of pros and cons to this.

First, the pros:

- In-memory is a lot simpler for me to implement for this project.
- In-memory cache also cleans up after the time has expired. This way, I don't need to manage my memory manually like I would with a traditional DB.
- In-memory is faster.

The cons:

- In-memory is volatile; however, for storing one-time links, this is probably fine.
- In-memory is good till about 1 million users.
- In-memory is not distributed, meaning it can only be accessed by this instance of the application. Hence, if this was scaled horizontally, it could pose a problem. But for that, we can use Redis.

For testing, I have added a couple of test classes and Moq'ed the memory cache. I believe the test coverage I currently have is more than sufficient for a PoC application.

As future improvements for this application, I would consider looking into Redis. Distributed cache would allow the application to be scalable, as in-memory cache is only limited to one instance of the app. 
In the requirements, it was mentioned that the client needs to generate more than 1 million links per month. This would definitely be a use case for Redis, as in-memory cache would begin to struggle at that load. 
A database could potentially be used in that situation; however, the aforementioned problem of data management comes back, i.e., who will be cleaning up the database from all the leftover links? 
Furthermore, while a database is safer due to not being volatile, it will not be as fast as cache when it comes to read speeds.
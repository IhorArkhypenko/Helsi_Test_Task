# Helsi_Test_Task
Simple notes management API.

You can find the task in the project files.

## How to run
1. **Install Postgres.**
2. **Clone this repository.**
3. **Apply migrations:** dotnet ef database update --project ./Helsi.Todo.Infrastructure --startup-project ./Helsi.Todo.Api
4. **Run the application and go to the Swagger:** https://localhost:5001/swagger/index.html
5. **For testing, you can use the Guid generator:** https://go.lightnode.com/resources/guid-uuid-generator

## Comments for reviewer
1. Oleg said that I can choose any DB, so I used Postgres instead of Mongo.
2. Some decisions may look like overhead(separate entity configuration files, extensions for IServiceCollection, FluentValidation etc.). I just wanted to demonstrate how I prefer to do things in real-life projects.
3. Firstly, I tried to do something like CQRS, but without MediatR. Then I understood that it would be hard and overwhelming without MediatR, so I did the test task using TaskListService + Repository.
4. Repository without async/await to avoid creation of redundant state machines and allocations.
5. In real life, we are getting currentUserId from JWT token, but for this test task, without authentication, I tried to simulate authorization by passing the userId in the header to separate it from other parameters.
6. I prefer to avoid magic, so I wrote attributes for the controller and methods explicitly ([Route("tasklists")], [FromBody], [FromHeader]).
7. It would be good to use a library for mapping, like Mapster, but it looks redundant for such a small project.
8. It would be good to add logging, but it looks redundant for the test task.

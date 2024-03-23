using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

var todoItems = app.MapGroup("/todoitems");

//con expresiones lambda

// todoItems.MapGet("/", async (TodoDb db) =>
//     await db.Todos.ToListAsync());

// todoItems.MapGet("/complete", async (TodoDb db) =>
//     await db.Todos.Where(t => t.IsComplete).ToListAsync());

// todoItems.MapGet("/{id}", async (int id, TodoDb db) =>
//     await db.Todos.FindAsync(id)
//         is Todo todo
//             ? Results.Ok(todo)
//             : Results.NotFound());

// todoItems.MapPost("/", async (Todo todo, TodoDb db) =>
//     {
//         db.Todos.Add(todo);
//         await db.SaveChangesAsync();

//         return Results.Created($"/todoitems/{todo.Id}", todo);
//     });

// todoItems.MapPut("/{id}", async (int id, Todo inputTodo, TodoDb db) =>
// {
//     var todo = await db.Todos.FindAsync(id);
    
//     if(todo is null) return Results.NotFound();

//     todo.Name = inputTodo.Name;
//     todo.IsComplete = inputTodo.IsComplete;

//     await db.SaveChangesAsync();

//     return Results.NoContent();
// });

// todoItems.MapDelete("/{id}", async (int id, TodoDb db) =>
// {
//     if( await db.Todos.FindAsync(id) is Todo todo)
//         {
//             db.Todos.Remove(todo);
//             await db.SaveChangesAsync();
//             return Results.NoContent();
//         }
//     return Results.NotFound();
// });

//Con metodos y la API TYpedResults

todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/complete", GetCompleteTodos);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPost("/", CreateTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);

app.Run();

static async Task<IResult> GetAllTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.ToArrayAsync());
}

static async Task<IResult> GetCompleteTodos (TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Where(todo => todo.IsComplete).ToListAsync());
}

static async Task<IResult> GetTodo (TodoDb db, int id)
{
    return await db.Todos.FindAsync(id)
         is Todo todo
             ? TypedResults.Ok(todo)
             : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo (TodoDb db, Todo todo)
{
       db.Todos.Add(todo);
       await db.SaveChangesAsync();
       return TypedResults.Created($"/todoitems/{todo.Id}", todo);
}

static async Task<IResult> UpdateTodo (TodoDb db, Todo inputTodo, int id)
{
    var todo = await db.Todos.FindAsync(id);
    
    if(todo is null) return TypedResults.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo (TodoDb db, int id)
{
    if( await db.Todos.FindAsync(id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
    return TypedResults.NotFound();
}
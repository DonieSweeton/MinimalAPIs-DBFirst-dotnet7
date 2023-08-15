using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalDBFirst.Data;
using MinimalDBFirst.Models;
using MinimalDBFirst.Models.DTO;
using Npgsql;
using System.Data;

namespace MiniDB0731.EndPoints
{
    public static class UsersEndpointClass // Define a class to contain the method
    {
        public static void UsersEndPoint(this WebApplication app)
        {
            app.MapGet("/users", List);
            app.MapGet("/users/{user_id}", Get);
            app.MapPost("/users", Create);
            app.MapPut("/users/{user_id}", Update);
            app.MapDelete("/users/{user_id}", Delete);
        }

        public static async Task<IResult> List(DotnetContext db, IMapper mapper)
        {
            var query = "SELECT * FROM users WHERE is_deleted = false;";

            var result = await db.Users.FromSqlRaw(query).ToListAsync();

            var getAllDtos = mapper.Map<List<GetAllDTO>>(result);

            return Results.Ok(getAllDtos);
        }

        public static async Task<IResult> Get(DotnetContext db, IMapper mapper, int user_id)
        {
            // Find the user by user_id and check if it exists
            var user = await db.Users
                .Where(u => u.UserId == user_id && !u.IsDeleted) // Filter by user_id and ensure it's not deleted
                .FirstOrDefaultAsync();

            if (user != null)
            {
                // User found, map it to the GetSmDTO using AutoMapper
                var getSmDtos = mapper.Map<GetSmDTO>(user);
                return Results.Ok(getSmDtos);
            }
            else
            {
                return Results.NotFound("User not found.");
            }
        }
        
        public static async Task<IResult> Create(DotnetContext db, IMapper mapper, CreateUserDTO createUserDTO)
        {
            var newUser = mapper.Map<User>(createUserDTO);

            // Set default values for CreatedDate and IsDeleted
            newUser.CreatedDate = DateTime.Now;

            // Use raw SQL query to insert the new user into the database
            var query = "INSERT INTO users (user_name, user_email, created_by, created_date) " +
                        "VALUES (@userName, @userEmail, @createdBy, @createdDate) " +
                        "RETURNING user_id;";

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@userName", newUser.UserName),
                new NpgsqlParameter("@userEmail", newUser.UserEmail),
                new NpgsqlParameter("@createdBy", newUser.CreatedBy),
                new NpgsqlParameter("@createdDate", newUser.CreatedDate),
            };


            var task = db.Database.ExecuteSqlRawAsync(query, parameters.ToArray());

            // Get the created user_id from the database
            var userId = await db.Users
                        .Where(u => u.UserName == newUser.UserName && u.UserEmail == newUser.UserEmail)
                        .Select(u => u.UserId)
                        .FirstOrDefaultAsync();
            newUser.UserId = userId;


            // Return the created user with the user_id in the response
            //return Results.Created($"/users/{userId}", newUser);
            return Results.Created($"/users/{newUser.UserId}", newUser);
        }

        public static async Task<IResult> Update(DotnetContext db, int user_id, IMapper mapper, EditUserDTO editUserDTO)
        {
            var parameters = new List<NpgsqlParameter> { new NpgsqlParameter("@userId", user_id) };
            var user = await db.Users
                    .Where(u => u.UserId == user_id && !u.IsDeleted)
                    .FirstOrDefaultAsync();
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            // Map properties from EditUserDTO to the existing User entity.
            mapper.Map(editUserDTO, user);

            user.ModifiedDate = DateTime.Now;

            // Use raw SQL query to update the user in the database
            var updateQuery = "UPDATE users " +
                              "SET user_name = @userName, user_email = @userEmail, modified_by = @modifiedBy " +
                              "WHERE user_id = @userId;";

            var updateParameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@userName", user.UserName),
                new NpgsqlParameter("@userEmail", user.UserEmail),
                new NpgsqlParameter("@modifiedBy", user.ModifiedBy), // Check this property
                new NpgsqlParameter("@userId", user_id),
            };

            await db.Database.ExecuteSqlRawAsync(updateQuery, updateParameters.ToArray());

            return Results.Ok("User has been updated.");
        }

        public static async Task<IResult> Delete(DotnetContext db, int user_id)
        {
            // Use raw SQL query to update the user's IsDeleted status in the database
            var query = "UPDATE users SET is_deleted = true WHERE user_id = @userId AND is_deleted = false;";
            var parameters = new List<NpgsqlParameter> { new NpgsqlParameter("@userId", user_id) };

            // Execute the raw SQL query
            int rowsAffected = await db.Database.ExecuteSqlRawAsync(query, parameters.ToArray());

            if (rowsAffected > 0)
            {
                return Results.Ok("User has been deleted.");
            }
            else
            {
                return Results.NotFound("User not found.");
            }
        }
    }
}

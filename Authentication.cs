using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZapMobileApi.Models;

namespace ZapMobileApi
{
    public static class Authentication

    {
        [FunctionName("SignUp")]
        public static async Task<IActionResult> SignUp([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "SignUp/email/password")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            try
            {
                using (var conn = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    int id = 0;
                    string email = req.Query["email"];
                    string password = req.Query["password"];
                    var passwordHashed = PasswordHelper.EncodePasswordToBase64(password);
                    conn.Open();
                    SqlCommand command = new SqlCommand("Select userId from UserTable where email=@email", conn);
                    command.Parameters.AddWithValue("@email", email);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = Convert.ToInt32(reader["id"]);
                        }
                    }
                    if (id > 0)
                    {
                        return new BadRequestObjectResult("User already exist with this email");
                    }

                    string query = "INSERT INTO UserTable (email,password) VALUES (@email,@password)";
                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", email);

                    cmd.Parameters.AddWithValue("@password", passwordHashed);
                    await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            return new OkResult();

        }

        [FunctionName("Login")]
        public static async Task<IActionResult> Login([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Login/email/password")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            try
            {
                using (var conn = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {

                    string email = req.Query["email"];
                    string password = req.Query["password"];
                    var passwordHashed = PasswordHelper.EncodePasswordToBase64(password);
                    conn.Open();
                    SqlCommand command = new SqlCommand("Select userId from UserTable where email=@email and password=@password", conn);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", passwordHashed);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var userObj = new UserModel
                            {
                                UserId = Convert.ToInt32(reader["userId"]),
                                Email = reader["email"].ToString(),
                                Location = reader["location"].ToString(),
                               // LocationId =(int)reader["locationId"]
                            };
                            conn.Close();
                            return new OkObjectResult(userObj);
                        }
                        return new NotFoundObjectResult("Invalid credentials");
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            return new OkResult();

        }

    }
}
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
using System.Threading.Tasks;
using ZapMobileApi.Models;

namespace ZapMobileApi
{
    public static class GetMachines

    {
        [FunctionName("GetMachines")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "machines")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            List<MachineModel> machineList = new List<MachineModel>();
          //  List<LocationModel> llist = new List<LocationModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var query = @"Select Location.locationName, Location.locationId, MachineTable.machineName, MachineTable.machineId , MachineTable.locationId from Location inner join MachineTable on Location.locationId = MachineTable.locationId ";
                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                 //   System.DateTime dbDateTime = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

                    while (reader.Read())
                    {
                        
                        LocationModel locations = new LocationModel()
                        {
                            LocationId= (int) reader["locationId"],
                            LocationName= (string) reader ["locationName"]

                        };
                        MachineModel machines = new MachineModel()
                        {
                            MachineId = (int)reader["machineId"],

                            MachineName = (string)reader["machineName"],
                            Status = true,
                            LocationId = (int)reader["locationId"],
                            Location = locations
                        };
                        machineList.Add(machines);
                        
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }


            if (machineList.Count > 0)
            {
                log.LogError($"{machineList.Count} results found");
                 return new OkObjectResult(machineList);
             
            }
            else
            {
                log.LogError("No results found!");
                return new OkObjectResult(machineList);
            }
        }
        /*
        [FunctionName("GetMachineById")]
        public static async Task<IActionResult> GetMachineById([HttpTrigger(AuthorizationLevel.Anonymous, "get","post", Route = "machines/id")] HttpRequest req, ILogger log)
        {

            log.LogError(req.Method.ToString());
            List<MachineModel> machineList = new List<MachineModel>();
            List<MeasurementsModel> mlist = new List<MeasurementsModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    string id = req.Query["id"];

                    var query = @"select top (3) MachineTable.machineName,Location.locationName, Sensor.sensorName
                                    ,Measurement.value, Measurement.timestamp,Location.locationId, MachineTable.machineId,Sensor.sensorId, Measurement.measurementId
                                    from Location inner join MachineTable on Location.locationId = MachineTable.locationId inner join
                                    Sensor on MachineTable.machineId = Sensor.machineId 
                                    join Measurement on Sensor.sensorId= Measurement.sensorId where MachineTable.machineId ="+ id +" order by Measurement.timestamp desc ";
                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    System.DateTime dbDateTime = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

                    while (reader.Read())
                    {

                        LocationModel locations = new LocationModel()
                        {
                            LocationId = (int)reader["locationId"],
                            LocationName = (string)reader["locationName"]

                        };


                        MachineModel machines = new MachineModel()
                        {
                            MachineId = (int)reader["machineId"],
                            MachineName = (string)reader["machineName"],
                            Status = true,
                            LocationId = (int)reader["locationId"],
                            Location = locations

                        };
                        SensorModel sensors = new SensorModel()
                        {
                            SensorId = (int)reader["sensorId"],
                            SensorName = (string)reader["sensorName"],
                            MachineId = (int)reader["machineId"],
                            Machine = machines
                        };
                        MeasurementsModel measurements = new MeasurementsModel()
                        {
                            MeasurementId = (int)reader["measurementId"],
                            Value = (long)reader["value"],
                            Timestamp = dbDateTime.AddMilliseconds((long)reader["timestamp"]).ToLocalTime(),
                            SensorId = (int)reader["sensorId"],
                            Sensor = sensors

                        };



                        machineList.Add(machines);
                        mlist.Add(measurements);

                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }


            if (machineList.Count > 0)
            {
                log.LogError($"{machineList.Count} results found");
                return new OkObjectResult(mlist);

            }
            else
            {
                log.LogError("No results found!");
                return new OkObjectResult(mlist);
            }
        }*/

        [FunctionName("GetMachineById")]
        public static async Task<IActionResult> GetMachineById([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "machine/id")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            List<MachineInfo> machineList = new List<MachineInfo>();
            List<Sensor> sl = new List<Sensor>();
            MachineInfo details = new MachineInfo();
            try
            {               
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    string id = req.Query["id"];

                    var query = @"select top (3) MachineTable.machineName,Location.locationName, Sensor.sensorName
                                    ,Measurement.value, Measurement.timestamp,Location.locationId, MachineTable.machineId,Sensor.sensorId, Measurement.measurementId, Sensor.unit, MachineTable.status
                                    from Location inner join MachineTable on Location.locationId = MachineTable.locationId inner join
                                    Sensor on MachineTable.machineId = Sensor.machineId 
                                    join Measurement on Sensor.sensorId= Measurement.sensorId where MachineTable.machineId =" + id + " order by Measurement.timestamp desc,sensorName asc ";
                   
                    System.DateTime dbDateTime = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        Sensor sensors = new Sensor()
                        {
                            type = (string)reader["sensorName"],
                            value = (long)reader["value"],
                            unit = (string)reader["unit"]
                        };
                        sl.Add(sensors);
                        details = new MachineInfo() 
                        { 
                           machineId= (int)reader["machineId"],
                           machineName=(string)reader["machineName"],
                           status=true,
                           locationId=(int)reader["locationId"],
                           location= new Location()
                           {
                               locationId = (int)reader["locationId"],
                               locationName = (string)reader["locationName"]
                           },
                           measure = new Measure()
                           {
                               timestamp = dbDateTime.AddMilliseconds((long)reader["timestamp"]).ToLocalTime(),
                               sensorList = sl
                           }
                        };                 
                    }
                    machineList.Add(details);
                    connection.Close();
                }                
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (machineList.Count > 0)
            {
                log.LogError($"{machineList.Count} results found");
                return new OkObjectResult(machineList);
            }
            else
            {
                log.LogError("No results found!");
                return new OkObjectResult(machineList);
            }
        }


        /**
        [FunctionName("Subscribe")]
        public static async Task<IActionResult> AddLocation([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "location")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<UserModel>(requestBody);
          
            try
            {
                using (var connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();

                    if (!String.IsNullOrEmpty(input.Email))
                    {
                        Console.WriteLine("email not null");

                        var query = @"select UserTable.userId from UserTable where UserTable.email = '" + input.Email + "'";
                        SqlCommand command1 = new SqlCommand(query, connection);
                        var reader = await command1.ExecuteReaderAsync();
                        int id = 0;
                        while (reader.Read())
                        {
                            id = (int)reader["userId"];
                            Console.WriteLine(id);
                        }
                        reader.Close();
                        if (id != 0)
                        {
                            var updateQuery = $"Update  UserTable set location= '{input.Location}' where userId='{id}'";
                            SqlCommand command2 = new SqlCommand(updateQuery, connection);
                            command2.ExecuteNonQuery();
                            Console.WriteLine("Location updated");
                        }
                        else
                        {
                            Console.WriteLine("Cannot find user");
                        }
                    }
                    connection.Close();
                }
           
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            return new OkResult();

        }*/


        [FunctionName("AddComment")]
        public static async Task<IActionResult> AddComment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "comment")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<Comment>(requestBody);
            
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    if (!String.IsNullOrEmpty(input.Message))
                    {
                        Console.WriteLine("Message not null");

                        var query = @"select UserTable.userId from UserTable where UserTable.email = '"+input.UserName +"'" ;
                        SqlCommand command1 = new SqlCommand(query, connection);
                        var reader = await command1.ExecuteReaderAsync();
                        int id = 0;
                        while (reader.Read())
                        {
                            id = (int)reader["userId"];
                        }
                        reader.Close();
                        if (id != 0)
                        {
                            var insertQuery = $"INSERT INTO Comment(message,userId,machineId,mTime) VALUES('{input.Message}', '{id}' , '{input.MachineId}', '{input.Timestamp}')";
                            SqlCommand command2 = new SqlCommand(insertQuery, connection);
                            command2.ExecuteNonQuery();
                            Console.WriteLine("Value inserted");
                        }
                        else
                        {
                            Console.WriteLine("Can't find user");
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
                return new BadRequestResult();
            }
            
            return new OkResult();

         }

        [FunctionName("SubscribeLocation")]
        public static async Task<IActionResult> Subscribe([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "subscribe")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<UserModel>(requestBody);
            int uId = 0;
            int lId = 0;
            try
            {
                using (var connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();

                    if (!String.IsNullOrEmpty(input.Email))
                    {
                        Console.WriteLine("email not null");

                        var userIdQuery = @"select UserTable.userId from UserTable where UserTable.email = '" + input.Email + "'";
                        SqlCommand userCommand = new SqlCommand(userIdQuery, connection);
                        var userReader = await userCommand.ExecuteReaderAsync();

                        while (userReader.Read())
                        {
                            uId = (int)userReader["userId"];
                        }
                        userReader.Close();               

                        var locationIdQuery = @"select locationId from Location where locationName = '" + input.Location + "'";
                        SqlCommand locationCommand = new SqlCommand(locationIdQuery, connection);
                        var locationReader = await locationCommand.ExecuteReaderAsync();
                        while (locationReader.Read())
                        {
                            lId = (int)locationReader["locationId"];
                        }
                        locationReader.Close();


                        if (uId != 0 && lId != 0)
                        {
                            var updateQuery = $"Update  UserTable set locationId= '{lId}' where userId='{uId}'";
                            SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                            updateCommand.ExecuteNonQuery();
                            Console.WriteLine("Location updated");
                        }
                        else
                        {
                            Console.WriteLine("Cannot find user");
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            return new OkResult();

        }
        [FunctionName("GetLocations")]
        public static async Task<IActionResult> GetLocations([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "locations")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            List<LocationModel> locationList = new List<LocationModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var query = @"Select Distinct Location.locationName, Location.locationId from Location ";
                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        LocationModel locations = new LocationModel()
                        {
                            LocationId = (int)reader["locationId"],
                            LocationName = (string)reader["locationName"]

                        };
                        locationList.Add(locations);                       
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (locationList.Count > 0)
            {
                return new OkObjectResult(locationList);
            }
            else
            {
                log.LogError("No results found!");
                return new OkObjectResult(locationList);
            }
        }
       
        [FunctionName("GetSubscribedLocation")]
        public static async Task<IActionResult> GetSubscribedLocation([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "user/location")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            UserInfo user = new UserInfo();    
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    string username = req.Query["username"];

                    var query = @"select Location.locationName,Location.locationId,UserTable.email 
                                from Location join UserTable on UserTable.locationId=Location.locationId where UserTable.email ='" +  username +"'"  ;
                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {                                         
                        user = new UserInfo()
                        {
                            userName = (string)reader["email"],
                            location = new Location()
                            {
                                locationId = (int)reader["locationId"],
                                locationName = (string)reader["locationName"]
                            }                            
                        };
                    }
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (user!=null)
            {
                //log.LogError($"{machineList.Count} results found");
                return new OkObjectResult(user);
            }
            else
            {
                log.LogError("No results found!");
                return new OkObjectResult(user);
            }
        }

        [FunctionName("GetMachineSensor")]
        public static async Task<IActionResult> GetMachineBySensor([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "machine/sensors")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            List<SensorInfo> machineList = new List<SensorInfo>();
            List<SensorList> sensorlist = new List<SensorList>();
            SensorInfo details = new SensorInfo();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    string id = req.Query["id"];

                    var query = @"select MachineTable.machineId,MachineTable.machineName, Sensor.sensorName
                                    ,Sensor.sensorId, Sensor.unit, Sensor.min_value, Sensor.max_value
                                    from  MachineTable Join Sensor on MachineTable.machineId = Sensor.machineId
                                     where MachineTable.machineId = " + id ;

                    System.DateTime dbDateTime = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        SensorList sensors = new SensorList()
                        {
                            sensorId = (int)reader["sensorId"],
                            sensorName = (string)reader["sensorName"],
                            value_max = (int)reader["max_value"],
                            value_min = (int)reader["min_value"],
                            unit = (string)reader["unit"]

                        };
                        sensorlist.Add(sensors);
                        details = new SensorInfo()
                        {
                            machineId = (int)reader["machineId"],
                            machineName = (string)reader["machineName"],
                            sensorList= sensorlist
                            
                        };
                    }
                    machineList.Add(details);
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (machineList.Count > 0)
            {
                log.LogError($"{machineList.Count} results found");
                return new OkObjectResult(machineList);
            }
            else
            {
                log.LogError("No results found!");
                return new OkObjectResult(machineList);
            }
        }

        [FunctionName("GetNotifications")]
        public static async Task<IActionResult> GetNotifications([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "user/notifications")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            List<UserNotification> user = new List<UserNotification>();
            List<Notification> nList = new List<Notification>();
            UserNotification userNotification = new UserNotification();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    System.DateTime dbDateTime = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

                    connection.Open();
                    string username = req.Query["username"];                                      
                    var query = @"select UserTable.email, UserTable.userId, UserTable.locationId,
                                Alarm.alarmId,Alarm.alarmType, Alarm.measurementId,
                                Measurement.value, Measurement.timestamp, Measurement.sensorId,
                                Sensor.sensorName, Sensor.MachineId,MachineTable.machineName,
                                Location.locationId, Location.locationName
                                from UserTable inner join Location on UserTable.locationId= Location.locationId
                                inner join MachineTable on MachineTable.locationId= Location.locationId
                                inner join Sensor on Sensor.machineId=MachineTable.machineId
                                inner join Measurement on Measurement.sensorId=Sensor.sensorId
                                join Alarm on Measurement.measurementId= Alarm.measurementId  where UserTable.email='" + username + "'";
                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                       Notification notification = new Notification() 
                       { 
                           machineId =(int)reader["machineId"],
                           machineName = (string)reader["machineName"],
                           sensorName = (string)reader["sensorName"],
                           sensorValue =(long) reader["value"],
                           type = (string)reader["alarmType"],
                           timestamp= dbDateTime.AddMilliseconds((long)reader["timestamp"]).ToLocalTime()

                       };
                        nList.Add(notification);

                        userNotification = new UserNotification()
                        {
                            userName = (string)reader["email"],
                            location = new Location()
                            {
                                locationId = (int)reader["locationId"],
                                locationName = (string)reader["locationName"]
                            },
                            notifications= nList
                        };
                    }
                    user.Add(userNotification);
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (user.Count > 0)
            {
                log.LogError($"{user.Count} results found");
                return new OkObjectResult(user);
            }
            else
            {
                log.LogError("No results found!");
                return new OkObjectResult(user);
            }
        }
               

        [FunctionName("GetMachineComments")]
        public static async Task<IActionResult> GetMachineComments([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "machine/comments")] HttpRequest req, ILogger log)
        { log.LogError(req.Method.ToString());
            CommentInfo commentDetail = new CommentInfo();
            List<CommentInfo> comments = new List<CommentInfo>();
            List<MachineComment> mc = new List<MachineComment>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    System.DateTime dbDateTime = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

                    connection.Open();
                    string id = req.Query["id"];

                    var query = @"select Comment.message,MachineTable.machineId,Comment.mTime
                                 from MachineTable join Comment on Comment.machineId=MachineTable.machineId 
                                 where MachineTable.machineId=" + id +"order by Comment.mTime desc";
                    Console.WriteLine(query);
                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {

                        MachineComment comment = new MachineComment()
                        { 
                            message= (string)reader["message"],
                            timestamp = dbDateTime.AddMilliseconds((long)reader["mTime"]).ToLocalTime()

                        };
                        mc.Add(comment);

                        commentDetail = new CommentInfo()
                        {
                            machineId = (int)reader["machineId"],
                            comments=mc
                        };
                    }
                    comments.Add(commentDetail);
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (comments.Count>0)
            {
                return new OkObjectResult(comments);
            }
            else
            {
                log.LogError("No results found!");
                return new OkObjectResult(comments);
            }
        }
        

        [FunctionName("GetSensorStat")]
        public static async Task<IActionResult> GetSensorStatById([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "machine/sensor/stat")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            SensorAverage stat = new SensorAverage();

            long averageVal = 0;
            int validVal = 0;
            int invalidVal =0;
            try
            {                
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    
                    connection.Open();
                    string id = req.Query["id"];
                    

                    var query = @"declare @week bigint
                                  declare @current bigint;
                                  
                                  set @week = 604800;
                                  set @current = (select top 1 timestamp from Measurement where sensorId =" + id + " order by timestamp desc); select AVG(Measurement.value) AS Average from Measurement where sensorId =" + id + " and timestamp > (@current -@week);";

                    var queryValid = @"SELECT TOP (1000) count(value) as valid FROM Measurement where sensorId ="+id+" and value between (select min_value from Sensor "+
                                     "where sensorId ="+id+") and (select max_value from Sensor where sensorId ="+id+");";
                    var queryInvalid = @"SELECT TOP (1000) count(value) as invalid FROM Measurement where sensorId ="+id+ "and value  not between (select min_value from Sensor "+
                                        "where sensorId ="+id+") and (select max_value from Sensor where sensorId ="+id+")";

                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {

                        averageVal = (long)reader["Average"];
                    }
                    reader.Close();

                    SqlCommand command2 = new SqlCommand(queryValid, connection);
                    var reader2 = await command2.ExecuteReaderAsync();

                    while (reader2.Read())
                    {
                        validVal = (int)reader2["valid"];
                    }
                    reader2.Close();

                    SqlCommand command3 = new SqlCommand(queryInvalid, connection);
                    var reader3 = await command3.ExecuteReaderAsync();

                    while (reader3.Read())
                    {
                        invalidVal = (int)reader3["invalid"];
                    }
                    reader3.Close();


                    stat = new SensorAverage
                    {
                        SensorId = id,
                        AverageValue = averageVal,
                        Valid = validVal,
                        Invalid = invalidVal

                    };

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (stat !=null)
            {
              
                return new OkObjectResult(stat);
            }
            else
            {
                log.LogError("No results found!");
                return new OkObjectResult(stat);
            }
        }


    }
}

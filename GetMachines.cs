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
    public static class GetMachines

    {
        [FunctionName("GetMachines")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "machines")] HttpRequest req, ILogger log)
        {
            log.LogError(req.Method.ToString());
            List<MachineModel> machineList = new List<MachineModel>();
            List<LocationModel> llist = new List<LocationModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var query = @"Select Location.locationName, Location.locationId, MachineTable.machineName, MachineTable.machineId , MachineTable.locationId from Location inner join MachineTable on Location.locationId = MachineTable.locationId ";
                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    System.DateTime dbDateTime = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

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
        }
        /*****************************/

        [FunctionName("GetMachinesById")]
        public static async Task<IActionResult> GetMachinesById([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "machine/id")] HttpRequest req, ILogger log)
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
                                    join Measurement on Sensor.sensorId= Measurement.sensorId where MachineTable.machineId =" + id + " order by Measurement.timestamp desc ";
                   
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
/*******************************************************************************/


        [FunctionName("Test")]
        public static async Task<IActionResult> GetName(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);


        }


    }
}

﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebRole.Logging;

namespace WebRole.Controllers
{
    public class MonoglotController : ApiController
    {
        public string Get(int id)
        {
            string result = "";
            try
            {
                MonoglotEventSource.Log.Startup();
                MonoglotEventSource.Log.PageStart(id, this.Url.Request.RequestUri.AbsoluteUri.ToString());
                string sqlServerConectionString = ConfigurationManager.ConnectionStrings["sqlServerConectionString"].ConnectionString;
                string queryString = "SELECT ProductID, Name from Production.Product WHERE ProductID=@productId";
                using (SqlConnection connection = new SqlConnection(sqlServerConectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    MonoglotEventSource.Log.ReadDataStart();
                    connection.Open();
                    command.Parameters.AddWithValue("@productId", id);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = String.Format("ProductID={0}, Name={1}", reader[0], reader[1]);
                    }
                    reader.Close();
                    watch.Stop();
                    long elapsed = watch.ElapsedMilliseconds;
                    MonoglotEventSource.Log.ReadDataFinish(elapsed);
                }
                MonoglotEventSource.Log.PageEnd();
            }
            catch (Exception ex)
            {
                //SQL Server Store is probably not available, log to table storage
                PersistenceErrorEventSource.Log.Failure(ex.Message);  
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return result;
        }
    }
}

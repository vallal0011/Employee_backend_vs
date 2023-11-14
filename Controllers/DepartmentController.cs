using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using asp_net.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;


using Microsoft.AspNetCore.Authentication.Google;
namespace asp_net.Controllers



{

    
   
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + GoogleDefaults.AuthenticationScheme)]
    public class DepartmentController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration Get_configuration()
        {
            return _configuration;


        }

        [HttpGet]
        
        public JsonResult Get(IConfiguration _configuration)
        {
            string query = @"
                            select DepartmentId, DepartmentName from
                            dbo.Departments
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ConnStr");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using SqlCommand myCommand = new SqlCommand(query, myCon);
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);
                myReader.Close();
                myCon.Close();
            }

            return new JsonResult(table);
        }



        [HttpPost]
        public JsonResult Post(Department dep)
        {
            string query = @"
                           insert into dbo.Departments
                           values (@DepartmentName)
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ConnStr");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@DepartmentName", dep.DepartmentName);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Added Successfully");
        }


        [HttpPut]
        public JsonResult Put(Department dep)
        {
            string query = @"
                           update dbo.Departments
                           set DepartmentName= @DepartmentName
                            where DepartmentId=@DepartmentId
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ConnStr");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@DepartmentId", dep.DepartmentId);
                    myCommand.Parameters.AddWithValue("@DepartmentName", dep.DepartmentName);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Updated Successfully");
        }



        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public JsonResult Delete(int id)
        {
            string query = @"
                           delete from dbo.Departments
                            where DepartmentId=@DepartmentId
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ConnStr");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@DepartmentId", id);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Deleted Successfully");
        }


    }
}
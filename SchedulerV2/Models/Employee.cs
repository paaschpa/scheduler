﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;
using SchedulerV2.DataAccess.DapperLibrary;

namespace SchedulerV2.Models
{
    public class Employee
    {
        public Employee()
        {
            Locations = new List<Location>();
        }

        public int EmployeeID { get; set; }
        public String UserName { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public bool IsGM { get; set; }
        public bool IsDO { get; set; }
        public String Email { get; set; }
        public String DisplayName { get; set; }
        public bool IsDeleted { get; set; }
        public int HomeLocationId { get; set; }

        public List<Location> Locations { get; set; }

        public static IEnumerable<Employee> ListAllDOs()
        {
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();
                var sql = @"Select EmployeeId, UserName, FirstName, LastName, IsGM, IsDO, e.Email, DisplayName, IsDeleted, HomeLocationId, '' as SplitOn,
                            LocationID, BrandID, Number, Name, City, DOEmployeeID, GMEmployeeID, OpenTime, CloseTime, l.Email, ParentID
                            From Employees e Left Outer Join Locations l On e.EmployeeID = l.DOEmployeeID Where e.IsDO = 1";
                var employees = new List<Employee>();

                var locations = conn.Query<Employee, Location, Location>(sql,
                        (employee, location) =>
                        {
                            if (employees.Where(x => x.EmployeeID == employee.EmployeeID).Count() > 0)
                                employees.Where(x => x.EmployeeID == employee.EmployeeID).FirstOrDefault().Locations.Add(location);
                            else
                            {
                                employee.Locations.Add(location);
                                employees.Add(employee);
                            }

                            return location;
                        }
                    , null, null, true, "SplitOn", null, null);

                return employees.ToList();
            }
        }

        public static Employee FindByUserName(String userName)
        {
            using (DbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CalendarConnectionString"].ToString()))
            {
                conn.Open();
                var sql = @"Select * From Employees Where UserName = @userName";

                var employee = conn.Query<Employee>(sql, new { userName = userName });

                return employee.SingleOrDefault();
            }
        }
    }
}
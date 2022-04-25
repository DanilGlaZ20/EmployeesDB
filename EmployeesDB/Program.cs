using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using EmployeesDB.Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Z.EntityFramework.Plus;

namespace EmployeesDB
{
    class Program
    {
        static private EmployeesContext _context = new EmployeesContext();



        static string GetEmployeesSalary()//1
        {
            var employees = from Employees in _context.Employees
                where Employees.Salary > 48000
                orderby Employees.LastName
                select new {FirstName = Employees.FirstName, LastName = Employees.LastName, Salary = Employees.Salary};

            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($" {e.FirstName} {e.LastName} {e.Salary}");

            }

            return sb.ToString().TrimEnd();

        }
        static string GETBrownLinq()//2
        {
            var newAddress = new Addresses()
            {
                TownId =27,
                AddressText = "Severnogorsk station"
            };
            _context.Addresses.Add(newAddress);
            var employees = from e in _context.Employees
                where e.LastName.Equals("Brown")
                select e;
            foreach (var e in employees)
            {
                e.Address = newAddress;
            }

            _context.SaveChanges();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine(
                    $"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.DepartmentId} {e.ManagerId} {e.HireDate} {e.Salary} {e.AddressId}");

            }

            return sb.ToString().TrimEnd();
        }


        static string GetEmployeerID()//4
        {
            int ID = 0;
            Console.Write("ID сотрудника:");
            ID = Convert.ToInt16(Console.ReadLine());
            var employees = from Employees in _context.Employees
                where Employees.EmployeeId == ID
                join EmployeesProjects in _context.EmployeesProjects on Employees.EmployeeId equals EmployeesProjects
                    .EmployeeId
                join Projects in _context.Projects on EmployeesProjects.ProjectId equals Projects.ProjectId
                select new
                {
                    FirstName = Employees.FirstName, LastName = Employees.LastName, JobTitle = Employees.JobTitle,
                    Name = Projects.Name
                };
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.JobTitle}");
                break;
            }

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.Name}");
            }

            return sb.ToString().TrimEnd();
        }
        static string DateTimeLinq()//3
        {
            var employees = (from e in _context.Employees
                    join ep in _context.EmployeesProjects
                        on e.EmployeeId equals ep.EmployeeId
                    where (ep.Project.StartDate.Year >= 2002 && ep.Project.StartDate.Year <= 2005)
                    select new
                    {
                        e.EmployeeId,
                        e.ManagerId,
                        ep.Project
                    }).Take(5)
                .ToList();

            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.Append($"Employee id = {e.EmployeeId}," + $"{e.Project.StartDate}");
                if (e.Project.EndDate != null) sb.AppendLine($" {e.Project.EndDate}");
                else sb.AppendLine(" Проект не окончен");
            }

            return sb.ToString().TrimEnd();
        }
        static string Department5EmployeesIDLinq()//5
        {
            var departments =
                (from d in _context.Departments
                    where (d.Employees.Count < 5)
                    select new
                    {
                        d.Name,
                        d.Employees.Count
                    }).ToList();
            var sb = new StringBuilder();
            foreach (var d in departments)
            {
                sb.Append($"{d.Name} {d.Count}\n");
            }

            return sb.ToString().TrimEnd();
        }
        static string UpdateSalaryLINQ()//6
        {
            var departmentId = Convert.ToInt32(Console.ReadLine());
            var percentIncrease = Convert.ToDouble(Console.ReadLine());
            percentIncrease += 1;
            var employees =
                from e in _context.Employees
                where (e.DepartmentId == departmentId)
                select e;
            foreach (var e in employees)
            {
                e.Salary *= (decimal) percentIncrease;
            }

            _context.SaveChanges();
            return "Salary Update!";
        }
        static void DeleteTownLinq()
        {
            Console.WriteLine("Enter town name");
            var townId = Convert.ToInt32(Console.ReadLine());
            var town =
                (from t in _context.Towns
                    where (t.Name.Equals(townId))
                    select t).ToList();
            if (town.Count != 0)
            {
                foreach (var t in town)
                {
                    Console.WriteLine($"{t.TownId} {t.Name}");
                    foreach (var a in t.Addresses)
                    {
                        var employees =
                            (from e in _context.Employees
                                where (e.AddressId == townId)
                                select e).ToList();
                        foreach (var e in employees)
                        {
                            e.Address = null;
                        }

                        _context.Remove(a);
                    }

                    _context.Remove(t);
                }

                _context.SaveChanges();
            }
            else Console.WriteLine("Города с таким Id нет в базе");
        }

        

        static void Main(string[] args)
        {
            //Console.WriteLine(GetEmployeesSalary()); //ищем богатых работников YES! #1LINQ !!!
            Console.WriteLine(GETBrownLinq()); //Выводим Браунов Yes! #2LINQ !!!
            //Console.WriteLine(DateTimeLinq() ); //Выводит  проеты 2002-2005 YES! #3LINQ !!!
            // Console.WriteLine(GetEmployeerID());//Вывод сотрудника и его проектов по ID  Yes! #4LINQ !!!
            //Console.WriteLine(Department5EmployeesIDLinq());//Вывод отделов где менее 5 сотрудников  Yes! #5LINQ !!!
            Console.WriteLine(UpdateSalaryLINQ());//Увеличение зарплаты YES #6LINQ !!!
            //Console.WriteLine(Delete())//Удаление департамента  Yes #7 
            DeleteTownLinq(); //Удаление города по названию YES #8LINQ !!!

            

        }

        

        

            

        }
    }

         


           

       






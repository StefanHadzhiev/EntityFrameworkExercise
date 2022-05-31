using System;
using System.Linq;
using System.Text;
using SoftUni;
using SoftUni.Data;
using SoftUni.Models;


namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            Console.WriteLine(RemoveTown(context));
        }

        //3.Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Employee[] employees = context
                .Employees
                .OrderBy(x => x.EmployeeId)
                .ToArray();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} {emp.MiddleName} {emp.JobTitle} {emp.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //4.Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Employee[] employees = context
                .Employees
                .Where(x => x.Salary > 50000)
                .OrderBy(x => x.FirstName)
                .ToArray();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }


            return sb.ToString().TrimEnd();
        }

        //5.Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesFromResearchAndDev = context
                .Employees
                .Where(x => x.Department.Name == "Research and Development")
                .OrderBy(x => x.Salary)
                .ThenByDescending(x => x.FirstName)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DepartmentName = e.Department.Name,
                    Salary = e.Salary
                })
                .ToArray();

            foreach (var emp in employeesFromResearchAndDev)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} from {emp.DepartmentName} - ${emp.Salary:f2}");
            }

            return sb.ToString().TrimEnd();

        }

        //6.Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4

            };

            context.Addresses.Add(address);

            var nakovEmployee = context
                .Employees
                .FirstOrDefault(x => x.LastName == "Nakov");

            nakovEmployee.Address = address;

            context.SaveChanges();


            var employeesAddresses = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Select(e => new
                {
                    AddressText = e.Address.AddressText
                })
                .Take(10)
                .ToArray();

            foreach (var emplAdd in employeesAddresses)
            {
                sb.AppendLine(emplAdd.AddressText);
            }

            return sb.ToString().TrimEnd();
        }


        //7.Employees and Projects

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            DateTime initialStartDate = new DateTime(2001, 01, 01);
            DateTime initialEndDate = new DateTime(2003, 12, 31);

            var employees = context
                .Employees
                .Where(e => e.EmployeesProjects.Any
                (ep => ep.Project.StartDate >= initialStartDate && ep.Project.StartDate <= initialEndDate))
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects.Select(ep => new
                    {
                        ProjectName = ep.Project.Name,
                        ProjectStartDate = ep.Project.StartDate,
                        ProjectEndDate = ep.Project.EndDate
                    })
                })
                .Take(10)
                .ToArray();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} - Manager: {emp.ManagerFirstName} {emp.ManagerLastName}");

                foreach (var project in emp.Projects)
                {

                    var startDate = project.ProjectStartDate.ToString("M/d/yyyy h:mm:ss tt");
                    var endDate = project.ProjectEndDate.HasValue ? project.ProjectEndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished";

                    sb.AppendLine($"--{project.ProjectName} - {startDate} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //8.Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var addresses = context
                .Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Select(a => new
                {
                    AddressText = a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = a.Employees.Count
                })
                .Take(10)
                .ToArray();

            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.EmployeesCount} employees");
            }

            return sb.ToString().TrimEnd();

        }

        //9.Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {

            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    Projects = e.EmployeesProjects.Select(ep => new
                    {
                        ProjectName = ep.Project.Name
                    })
                    .OrderBy(ep => ep.ProjectName)
                    .ToArray()
                })
                .ToArray();


            var emp147 = employees.FirstOrDefault(e => e.EmployeeId == 147);

            sb.AppendLine($"{emp147.FirstName} {emp147.LastName} - {emp147.JobTitle}");

            foreach (var project in emp147.Projects)
            {
                sb.AppendLine($"{project.ProjectName}");
            }

            return sb.ToString().TrimEnd();

        }
        //10.Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departments = context
                .Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepName = d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employees = d.Employees.Select(e => new
                    {
                        EmployeeFirstName = e.FirstName,
                        EmployeeLastName = e.LastName,
                        EmployeeJobTitle = e.JobTitle,
                    })
                    .OrderBy(e => e.EmployeeFirstName)
                    .ThenBy(e => e.EmployeeLastName)
                    .ToArray()
                })
                .ToList();

            foreach (var dep in departments)
            {
                sb.AppendLine($"{dep.DepName} - {dep.ManagerFirstName} - {dep.ManagerLastName}");
                foreach (var emp in dep.Employees)
                {
                    sb.AppendLine($"{emp.EmployeeFirstName} {emp.EmployeeLastName} - {emp.EmployeeJobTitle}");
                }
            }


            return sb.ToString().TrimEnd();

        }


        //11.Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    Name = p.Name,
                    Desc = p.Description,
                    StartDate = p.StartDate
                })
                .OrderBy(p => p.Name)
                .ToArray();

            foreach (var project in projects)
            {
                sb.AppendLine(project.Name);
                sb.AppendLine(project.Desc);
                sb.AppendLine(project.StartDate.ToString("M/d/yyyy h:mm:ss tt"));
            }

            return sb.ToString().TrimEnd();



        }

        //12.Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design" || e.Department.Name == "Marketing" || e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var emp in employees)
            {
                emp.Salary += 0.12M * emp.Salary;
            }

            context.SaveChanges();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} (${emp.Salary:f2})");
            }



            return sb.ToString().TrimEnd();
        }

        //13.Find Employees by First Name Starting with "Sa"
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName, 
                    JobTitle = e.JobTitle,
                    Salary = e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle} - (${emp.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        //14.Delete Project by Id 
        //doesnt work as expected
      
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();


            Project projectToDelete = context
                .Projects
                .First(p => p.ProjectId == 2);

            var employeesWorkingOnProjectToDelete = context
                .EmployeesProjects
                .Where(e => e.ProjectId == projectToDelete.ProjectId)
                .ToList();

            foreach (var employee in employeesWorkingOnProjectToDelete)
            {
                employee.Project = null;
            }


            context.SaveChanges(); 
            context.Projects.Remove(projectToDelete);
           
            var first10Projects = context.Projects.Take(10).ToArray() ;

            foreach (var pro in first10Projects)
            {
                sb.AppendLine(pro.Name);
            }

            context.Projects.Add(projectToDelete);

            foreach (var employee in employeesWorkingOnProjectToDelete)
            {
                employee.Project = projectToDelete;
            }

            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        //15.Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var seattleAddresses = context
                .Addresses
                .Where(a => a.Town.Name == "Seattle")
                .ToArray();

            var employeesFromSeattle = context
                .Employees
                .ToArray()
                .Where(e => seattleAddresses.Any(a => a.AddressId == e.AddressId))
                .ToArray();

            foreach (var emp in employeesFromSeattle)
            {
                emp.AddressId = null;
            }

            context.Addresses.RemoveRange(seattleAddresses);

            Town seattleTown = context
                .Towns
                .First(t => t.Name == "Seattle");

            context.Towns.Remove(seattleTown);
            context.SaveChanges();

            sb.AppendLine($"{seattleAddresses.Length} addresses in Seattle were deleted");

            return sb.ToString().TrimEnd();

        }
    }
}

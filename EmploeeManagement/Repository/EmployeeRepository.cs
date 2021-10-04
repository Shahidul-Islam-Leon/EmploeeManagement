using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext context;

        public EmployeeRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Employee Create(Employee employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();
            return employee;
        }

        public Employee Delete(int id)
        {
            var employee = context.Employees.Find(id);
            if (employee!=null)
            {
                context.Employees.Remove(employee);
                context.SaveChanges();
            }
            return employee;

           

            //context.Employees.Remove(GetEmployee(id));
            //context.SaveChanges();
            

        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return context.Employees.ToList();
        }

        public Employee GetEmployee(int id)
        {
            return context.Employees.Find(id);
           
        }

        public Employee Update(Employee employee)
        {
            var Change = context.Employees.Attach(employee);
            Change.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return employee;
        }
    }
}

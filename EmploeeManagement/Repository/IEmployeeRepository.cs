using EmployeeManagement.Models;
//using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Repository
{
    public interface IEmployeeRepository
    {

        public IEnumerable<Employee> GetAllEmployee();
        public Employee GetEmployee(int id);
        public Employee Create(Employee employee);
        public Employee Update(Employee employee);
        public Employee Delete(int id);
       
       

    }
       
}

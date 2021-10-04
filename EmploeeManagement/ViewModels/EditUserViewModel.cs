using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class EditUserViewModel
    {
        [Required, Display(Name = "Username")]
        public string UserName { get; set; }

        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailUsed", controller: "Account")]
        public string Email { get; set; }
    }
}

using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = ("Admin"))]

    public class AdministratorController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public AdministratorController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [HttpGet]

        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel createRoleViewModel)
        {
            if (ModelState.IsValid)
            {

                var role = new IdentityRole
                {
                    Name = createRoleViewModel.RoleName
                };
                var result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList", "Administrator");
                }
            }

            return View(createRoleViewModel);
        }

        [HttpGet]
        public IActionResult RoleList()
        {
            var list = roleManager.Roles;
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var Role = await roleManager.FindByIdAsync(id);

            if (Role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            EditRoleViewModel model = new()
            {
                Id = Role.Id,
                RoleName = Role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, Role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel editRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(editRoleViewModel.Id);
                if (role == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {editRoleViewModel.Id} cannot be found";
                    return View("NotFound");
                }
                else
                {
                    role.Name = editRoleViewModel.RoleName;

                    // Update the Role using UpdateAsync
                    var result = await roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                }

            }
            return View(editRoleViewModel);
        }


        [HttpGet]
        public async Task<ActionResult> EditUserRole(string Id)
        {
            ViewBag.RoleId = Id;

            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"RoleId with id={Id} can not be found";
                return View("Notfound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);

            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditUserRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"RoleId with id={roleId} can not be found";
                return View("Notfound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }


        [HttpGet]
        public IActionResult ListUsers()
        {
            return View( userManager.Users);
        }

        [HttpGet ]
        public async Task< IActionResult> EditUser(string id)
        {
            var user =await userManager.FindByIdAsync(id);

            if (user==null)
            {
                ViewBag.ErrorMessage = $"UserId with id={id} can not be found";
                return View("Notfound");
            }

            var model = new EditUserViewModel()
            {
              UserName=user.UserName,
              Email=user.Email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            user.UserName = model.UserName;
            user.Email = model.Email;

            //if (user == null)
            //{
            //    ViewBag.ErrorMessage = $"UserId with id={id} can not be found";
            //    return View("Notfound");
            //}

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            return View(model);
        }

    }
}

    

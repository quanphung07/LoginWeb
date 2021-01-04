using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using LogInWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogInWeb.Controllers
{
    [Authorize(Roles="Admin,User")]
    public class AdministrationController:Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public AdministrationController(RoleManager<IdentityRole> roleManager,
        UserManager<AppUser> userManager)
        {
            _roleManager=roleManager;
            _userManager=userManager;
        }
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> ListUsers()
        {
            var users=await _userManager.Users.ToListAsync();
            return View(users);
        }
        public async Task<IActionResult> ListRoles()
        {
            var roles=await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        public async Task<IActionResult> Edit(string RoleId)
        {
            var role=await _roleManager.FindByIdAsync(RoleId);
            if(role==null)
            {
                ViewBag.ErrorMessage=$"Role with id={RoleId} not found";
                return View("EditRole");
            }
            var model=new EditRoleModel()
            {
                RoleId=role.Id,
                RoleName=role.Name,
            };
            foreach(var user in _userManager.Users.ToList())
            {
                if(await _userManager.IsInRoleAsync(user,role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            
            return View(model);
        }
        [HttpPost]
         public async Task<IActionResult> Edit(EditRoleModel model)
        {
            var role=await _roleManager.FindByIdAsync(model.RoleId);
            if(role==null)
            {
                ViewBag.ErrorMessage=$"Role with id={model.RoleId} not found";
                return NotFound();
            }
            else
            {
                role.Name=model.RoleName;
                var result=await _roleManager.UpdateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }
            // var model=new EditRoleModel()
            // {
            //     RoleId=role.Id,
            //     RoleName=role.Name,
            // };

            
            return RedirectToAction("Edit",new {RoleId=model.RoleId});
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleModel model)
        {
            if(ModelState.IsValid)
            {
                var role=new IdentityRole(){Name=model.RoleName};
                var result=await _roleManager.CreateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }

            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.Id=roleId;
            var role= await _roleManager.FindByIdAsync(roleId);
            if(role==null)
            {
                ViewBag.ErrorMessage=$"Role with id={roleId} not found";
                return NotFound();
            }
            var model=new List<UserRoleModel>();
            foreach (var user in _userManager.Users.ToList())
            {
                var userRolemodel=new UserRoleModel()
                {
                    UserId=user.Id,
                    Username=user.UserName

                };
                if(await _userManager.IsInRoleAsync(user,role.Name))
                {
                    userRolemodel.IsSelected=true;
                }
                model.Add(userRolemodel);
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleModel> userRoles,string roleId)
        {
            var role=await _roleManager.FindByIdAsync(roleId);
            if(role==null)
            {
                ViewBag.ErrorMessage=$"Role with id={roleId} not found";
                return NotFound();
            }
            for(int i=0;i<userRoles.Count;i++)
            {   IdentityResult result=null;
                var user=await _userManager.FindByIdAsync(userRoles[i].UserId);
                if(userRoles[i].IsSelected && !await _userManager.IsInRoleAsync(user,role.Name))
                {
                    result=await _userManager.AddToRoleAsync(user,role.Name);
                }
                else if (!userRoles[i].IsSelected && await _userManager.IsInRoleAsync(user,role.Name))
                {
                    result=await _userManager.RemoveFromRoleAsync(user,role.Name);
                }
                else
                {
                    continue;
                }
                if(result.Succeeded)
                {
                    if(i<(userRoles.Count-1))
                    {
                        continue;
                    }else
                    {
                        return RedirectToAction("Edit",new {roleId=role.Id});
                    }

                }

            }
            return RedirectToAction("Edit",new {roleId=role.Id});;
        }
    }
}
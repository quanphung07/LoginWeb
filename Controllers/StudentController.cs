using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogInWeb.Controllers
{
    public class StudentController:Controller
    {
       
        public IActionResult Index()
        {
            return Content("Hello");
        }
    }
}
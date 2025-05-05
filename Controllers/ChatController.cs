using Microsoft.AspNetCore.Mvc;

namespace ImoSphere.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
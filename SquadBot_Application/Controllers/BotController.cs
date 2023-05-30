using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SquadBot_Application.Controllers
{
    [Controller]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class BotController : Controller
    {
        [ActionName("startBot")]
        [HttpGet]
        public ActionResult StartBot()
        {
            return View();
        }

        [ActionName("stopBot")]
        [HttpGet]
        public ActionResult StopBot() 
        { 
            return View(); 
        }

        [ActionName("stats")]
        [HttpGet]
        public ActionResult Statistics() 
        { 
            return View();
        }

    }
}

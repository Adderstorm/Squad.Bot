using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SquadBot_Application.Services;

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
            BotService.StartThread();
            return Ok();
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

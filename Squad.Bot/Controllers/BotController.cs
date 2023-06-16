using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squad.Bot.Logging;
using Squad.Bot.Services;

namespace Squad.Bot.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    //[Authorize]
    public class BotController : ControllerBase
    {
        [ActionName("startBot")]
        [HttpGet]
        public ActionResult StartBot()
        {
            try
            {
                BotService.StartThread();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error has occurred: ", ex);
                return BadRequest(ex.Message);
            }
            return Ok("Thread has started");
        }

        [ActionName("stopBot")]
        [HttpGet]
        public ActionResult StopBot() 
        {
            try
            {
                BotService.StopThread();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error has occurred: ", ex);
                return BadRequest(ex.Message);
            }
            return Ok("Thread has stoped"); 
        }

        [ActionName("getBotThreadStat")]
        [HttpGet]
        public ActionResult Statistics() 
        {
            ThreadState state;
            try
            {
                state = BotService.GetThreadState();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error has occurred: ", ex);
                return BadRequest(ex.Message);
            }
            return Ok(state.ToString());
        }

        [ActionName("setServersToLog")]
        [HttpPost("{serverId}")]
        public ActionResult SetServersToLog(long serverId)
        {

            try
            {
                BotService.SetServerToLog(serverId);
                return Ok("Server has added");
            }
            catch (ArgumentException ex)
            {
                Logger.LogError("", ex);
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                Logger.LogError("Uncaught error: ", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}

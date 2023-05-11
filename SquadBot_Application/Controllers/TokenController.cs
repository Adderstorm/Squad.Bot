using Discord;
using Microsoft.AspNetCore.Mvc;
using SquadBot_Application.Logging;

namespace SquadBot_Application.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        [HttpPost]
        public ActionResult PostToken(string token)
        {
            try
            {
                TokenUtils.ValidateToken(TokenType.Bot, token);
            }
            catch 
            {
                Logger.LogError("Discord Bot token validation error");
                return ValidationProblem("Discord Bot token is invalid, please check it and try send again");
            }
            return Ok();
        }

    }
}

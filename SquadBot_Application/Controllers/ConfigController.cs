using Discord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SquadBot_Application.Logging;
using SquadBot_Application.Models;
using SquadBot_Application.Services;
using System;

namespace SquadBot_Application.Controllers
{
    [Controller]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class ConfigController : Controller
    {
        [ActionName("postToken")]
        [HttpPost]
        public ActionResult PostToken(string token)
        {
            try
            {
                TokenUtils.ValidateToken(TokenType.Bot, token);
                ConfigService.AddToken(token);
            }
            catch (ArgumentException)
            {
                Logger.LogError("Discord Bot token validation error");
                return ValidationProblem("Discord Bot token is invalid, please check it and try send again");
            }
            catch (Exception exception)
            {
                Logger.LogError("Post token error");
                return ValidationProblem($"Uncaught error: {exception.Message}");
            }
            Logger.LogInfo("Token was succesfully added and saved");
            return Ok();
        }

        [ActionName("updateToken")]
        [HttpPut]
        public ActionResult UpdateToken(string token) 
        {
            try
            {
                TokenUtils.ValidateToken(TokenType.Bot, token);
                ConfigService.UpdateToken(token);
            }
            catch (ArgumentException)
            {
                Logger.LogError("Discord Bot token validation error");
                return ValidationProblem("Discord Bot token is invalid, please check it and try send again");
            }
            catch (Exception exception)
            {
                Logger.LogError($"{exception.Message}");
                return ValidationProblem($"Uncaught error: {exception.Message}");
            }
            Logger.LogInfo("Token was succesfully updated");
            return Ok();
        }

        [ActionName("postConfig")]
        [HttpPost]
        public ActionResult PostConfig(Config config = null!)
        {
            try
            {
                ConfigService.AddConfig(config);
            }
            catch(Exception exception)
            {
                return BadRequest($"Uncaught error: {exception.Message}");
            }
            Logger.LogInfo("Config was succesfully added and saved");
            return Ok();
        }

        [ActionName("updateConfig")]
        [HttpPut]
        public ActionResult UpdateConfig(Config config = null!)
        {
            try
            {
                ConfigService.UpdateConfig(config);
            }
            catch(Exception exception)
            {
                return new BadRequestObjectResult($"Uncaught error: {exception.Message}");
            }
            Logger.LogInfo("Config was succesfully updated");
            return Ok();
        }
    }
}

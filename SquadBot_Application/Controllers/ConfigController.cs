using Discord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SquadBot_Application.Logging;
using SquadBot_Application.Models;
using SquadBot_Application.Services;
using System;

namespace SquadBot_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    //[Authorize]
    public class ConfigController : ControllerBase
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
            catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
            {
                Logger.LogError("Discord Bot token validation error ", ex);
                return ValidationProblem("Discord Bot token is invalid or null, please check it and try again");
            }
            catch (Exception ex)
            {
                Logger.LogError("Post token error ", ex);
                return BadRequest($"Uncaught error: {ex.Message}");
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
            catch (Exception ex) when (ex is ArgumentException|| ex is ArgumentNullException)
            {
                Logger.LogError("Discord Bot token validation error ", ex);
                return ValidationProblem("Discord Bot token is invalid or null, please check it and try again");
            }
            catch (Exception ex)
            {
                Logger.LogError("Update token error ", ex);
                return BadRequest($"Uncaught error: {ex.Message}");
            }
            Logger.LogInfo("Token was succesfully updated");
            return Ok();
        }

        [ActionName("postConfig")]
        [HttpPost]
        public ActionResult PostConfig(Config config = null!)
        {
            if (config == null)
                return BadRequest();
            try
            {
                ConfigService.AddConfig(config);
            }
            catch(Exception ex)
            {
                Logger.LogError("Post Config error ", ex);
                return BadRequest($"Uncaught error: {ex.Message}");
            }
            Logger.LogInfo("Config was succesfully added and saved");
            return Ok();
        }

        [ActionName("updateConfig")]
        [HttpPut]
        public ActionResult UpdateConfig(Config config = null!)
        {
            if (config == null)
                return BadRequest();
            try
            {
                ConfigService.UpdateConfig(config);
            }
            catch(Exception ex)
            {
                Logger.LogError("Update Config error ", ex);
                return BadRequest($"Uncaught error: {ex.Message}");
            }
            Logger.LogInfo("Config was succesfully updated");
            return Ok();
        }
    }
}

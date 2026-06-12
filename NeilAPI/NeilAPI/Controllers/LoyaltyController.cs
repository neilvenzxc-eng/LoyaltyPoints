using LoyaltyPoints.Services;
using LoyaltyPoints.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace NeilAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoyaltyController : ControllerBase
    {
        private readonly LoyaltyService _loyaltyService;

        public LoyaltyController(LoyaltyService loyaltyService)
        {
            _loyaltyService = loyaltyService;
        }

        // 1. POST: REGISTER
        [HttpPost("register")]
        public ActionResult CreateAccount([FromBody] User registrationData)
        {
            try
            {
                if (registrationData == null || string.IsNullOrEmpty(registrationData.Username) || string.IsNullOrEmpty(registrationData.Password))
                {
                    return BadRequest("Username and Password are required.");
                }

                _loyaltyService.CreateAccount(registrationData.Username, registrationData.Password);
                return StatusCode(201, new { message = "Account created successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // 2. GET: LOGIN SIMULATION
        [HttpGet("login")]
        public ActionResult Login([FromQuery] string username, [FromQuery] string password)
        {
            try
            {
                bool success = _loyaltyService.Login(username, password);
                if (!success)
                {
                    return Unauthorized(new { message = "Invalid credentials." });
                }
                return Ok(_loyaltyService.CurrentUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // 3. PATCH: ADD POINTS
        [HttpPatch("add-points")]
        public ActionResult AddPoints([FromQuery] string username, [FromQuery] string password, [FromBody] int moneySpent)
        {
            try
            {
                if (!_loyaltyService.Login(username, password))
                {
                    return Unauthorized(new { message = "Authentication failed." });
                }

                int earned = _loyaltyService.AddPoints(moneySpent);
                return Ok(new
                {
                    message = $"Success! Earned {earned} points.",
                    currentBalance = _loyaltyService.GetPoints()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // 4. PUT: REDEEM REWARD
        [HttpPut("redeem")]
        public ActionResult RedeemReward([FromQuery] string username, [FromQuery] string password, [FromBody] int rewardOption)
        {
            try
            {
                if (!_loyaltyService.Login(username, password))
                {
                    return Unauthorized(new { message = "Authentication failed." });
                }

                bool success = _loyaltyService.UsePoints(rewardOption);
                if (!success)
                {
                    return BadRequest(new { message = "Redemption failed. Insufficient points or invalid reward option." });
                }

                return Ok(new
                {
                    message = "Reward redeemed successfully!",
                    remainingPoints = _loyaltyService.GetPoints()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // 5. DELETE: REMOVE ACCOUNT COMPLETELY
        [HttpDelete("delete-account")]
        public ActionResult DeleteAccount([FromQuery] string username, [FromQuery] string password)
        {
            try
            {
                bool isAuthenticated = _loyaltyService.Login(username, password);
                if (!isAuthenticated)
                {
                    return Unauthorized(new { message = "Authentication failed. Cannot delete account." });
                }

                int userIdToDelete = _loyaltyService.CurrentUser.Id;

                bool isDeleted = _loyaltyService.DeleteAccount(userIdToDelete);

                if (!isDeleted)
                {
                    return BadRequest(new { message = "Could not delete account. User may not exist." });
                }

                return Ok(new { message = $"Account '{username}' has been successfully removed from the database and JSON snapshot." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
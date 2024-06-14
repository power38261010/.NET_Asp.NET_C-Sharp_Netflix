using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.Models;
using NetflixClone.Services.Contracts;

namespace NetflixClone.Controllers {
    [Route("api/subscriptions")]
    [ApiController]
    public class SubscriptionController : ControllerBase {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<ISubscriptionService> _logger;

        public SubscriptionController(ISubscriptionService subscriptionService, ILogger<ISubscriptionService> logger) {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllSubscriptions() {
            try {
                var subscriptions = await _subscriptionService.GetAll();
                return Ok(subscriptions);
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _subscriptionService.GetAll: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Subscription>> GetSubscriptionById(int id)  {
            try {
                var subscription = await _subscriptionService.GetById(id);
                if (subscription == null) {
                    return NotFound();
                }
                return Ok(subscription);
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _subscriptionService.GetById: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult> CreateSubscription([FromBody] SubscriptionRequest subscription) {
            try {
                await _subscriptionService.Create(subscription);
                return CreatedAtAction(nameof(GetSubscriptionById), new { id = subscription.Id }, subscription);
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _subscriptionService.GetById: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateSubscription(int id, [FromBody] SubscriptionRequest subscription) {
            try {
                if (id != subscription.Id) {
                    return BadRequest();
                }

                await _subscriptionService.Edit(subscription);
                var updatedSub = await _subscriptionService.GetById(id);
                return Ok(updatedSub);
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _subscriptionService.Edit: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteSubscription(int id) {
            try {
                await _subscriptionService.Delete(id);
                return Ok(new {Message = "Subscription Deleted"});

            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _subscriptionService.Delete: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}

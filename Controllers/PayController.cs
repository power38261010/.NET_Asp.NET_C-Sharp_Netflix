using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.Controllers.ModelRequest;
using NetflixClone.DTO;
using NetflixClone.Models;
using NetflixClone.Services.Contracts;

namespace NetflixClone.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PayController : ControllerBase {
        private readonly IPayService _payService;
        private readonly IUserService _userService;
        private readonly ILogger<IPayService> _logger;

        public PayController(IPayService payService, IUserService userService, ILogger<IPayService> logger) {
            _payService = payService;
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAllPayments() {
            try {
                var payments = await _payService.GetAll();
                return Ok(payments);
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _payService.GetAll: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayById(int id) {
            try {
                var pay = await _payService.GetById(id);
                if (pay == null) {
                    return NotFound();
                }
                return Ok(pay);
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _payService.GetById: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreatePay([FromBody] PayRequest pay) {
            try {
                await _payService.Create(pay);
                return CreatedAtAction(nameof(GetPayById), new { id = pay.Id }, pay);
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _payService.GetById: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("pay-subscriptions")]
        public async Task<IActionResult> GetAllPaySubscription () {
            var username = HttpContext.User.Identity?.Name;
            if (username != "super_admin") return NotFound();
            try {
                var paySubscriptions = await _payService.GetAllPaySubscription();
                if (paySubscriptions == null) {
                    return NotFound();
                }
                return Ok(paySubscriptions);
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _payService.TypesPayToArg: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdatePay(int id, [FromBody]  PayRequest pay)  {
            try {
                if (id != pay.Id) {
                    return BadRequest();
                }
                await _payService.Edit(pay);
                var updatedPay = await _payService.GetById(id);
                return Ok(updatedPay);
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _payService.Edit: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteSubscription(int id) {
            try {
                await _payService.Delete(id);
                return Ok(new {Message = "Pay Deleted"});

            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _payService.Delete: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("ars")]
        public async Task<IActionResult> GetPayByCountry () {
            try {
                var pays = await _payService.TypesPayToArg();
                if (pays == null) {
                    return NotFound();
                }
                return Ok(pays);
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _payService.TypesPayToArg: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePaySubscription([FromBody] PaymentRequest paymentRequest) {
            if (paymentRequest == null) {
                return BadRequest();
            }

            try {
                var username = HttpContext.User.Identity?.Name;
                if (username != null) {
                var userSesion = (User?) await _userService.GetUserByUsername(username);
                var payment = await _payService.CreatePaySubscription(
                    paymentRequest.Amount,
                    paymentRequest.Token,
                    paymentRequest.Description,
                    paymentRequest.PaymentMethodId,
                    paymentRequest.PayerEmail,
                    paymentRequest.PayId,
                    userSesion.Id,
                    paymentRequest.IsAnual
                );
                return Ok(payment);
                }
                return BadRequest(new { Message = "Usuario inexistente" });

            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _payService.CreatePaySubscription: {ex.Message}");
                _logger.LogError($"Error durante el uso de _payService.CreatePaySubscription: {ex}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}

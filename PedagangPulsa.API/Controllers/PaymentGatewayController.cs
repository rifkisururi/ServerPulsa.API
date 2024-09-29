using Microsoft.AspNetCore.Mvc;
using PedagangPulsa.API.Interface;
using PedagangPulsa.API.Model;

namespace PedagangPulsa.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentGatewayController : ControllerBase
    {
        private readonly ILogger<PaymentGatewayController> _logger;
        private readonly IDuitkuService _duitkuService;
        public PaymentGatewayController(ILogger<PaymentGatewayController> logger, IDuitkuService duitkuService)
        {
            _logger = logger;
            _duitkuService = duitkuService;
        }

        [HttpPost("duitku/create-payment")]
        public async Task<IActionResult> CreatePayment(DuitkuPaymentRequest paymentRequest)
        {
            try
            {
                var result = await _duitkuService.CreatePaymentRequestAsync(paymentRequest);
                return Ok(new { message = "Payment created successfully", result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error creating payment", error = ex.Message });
            }
        }
    }
}

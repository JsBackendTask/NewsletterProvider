using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace NewsletterProvider.Functions
{
    public class SubscriptionStatus(ILogger<SubscriptionStatus> logger, DataContext context)
    {
        private readonly ILogger<SubscriptionStatus> _logger = logger;
        private readonly DataContext _context = context;

        [Function("SubscriptionStatus")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(body))
            {
                return new BadRequestObjectResult(new { Status = 400, message = "Invalid email" });
            }

            var email = JsonConvert.DeserializeObject<SubscribeEntity>(body)!.Email;

            var exisistingSubscriber = await _context.Subscribes.FindAsync(email);

            if (exisistingSubscriber != null)
            {
                return new OkObjectResult(new { Status = 200, Message = "Subscribed" });
            }
            else
            {
                return new OkObjectResult(new { Status = 200, Message = "Not subscribed" });
            }
        }
    }
}

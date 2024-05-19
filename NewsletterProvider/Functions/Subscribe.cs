using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NewsletterProvider.Functions
{
    public class Subscribe(ILogger<Subscribe> logger, DataContext context)
    {
        private readonly ILogger<Subscribe> _logger = logger;
        private readonly DataContext _context = context;

        [Function("Subscribe")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (!string.IsNullOrEmpty(body))
            {
                var email = JsonConvert.DeserializeObject<SubscribeEntity>(body)!.Email;
                if (email != null)
                {

                    var exisistingSubscriber = await _context.Subscribes.FindAsync(email);
                    if (exisistingSubscriber != null)
                    {
                        return new BadRequestObjectResult(new { status = 400, message = "Email already subscribed" });
                    } 
                    else
                    {
                        await _context.Subscribes.AddAsync(new SubscribeEntity { Email = email });
                        await _context.SaveChangesAsync();
                        return new OkObjectResult(new { Status = 200, Message = $"Subscribed {email}" });
                    }
                }
                else
                {
                    return new BadRequestObjectResult(new { Status = 400, message = "Invalid email" });
                }
            }
            return new BadRequestObjectResult(new { Status = 400, message = "Invalid email" });
        }
    }
}

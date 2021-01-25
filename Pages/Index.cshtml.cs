using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Mayler.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }


        public void OnGet()
        {
            var services = HttpContext.RequestServices;
            var gmailAPI = (IGmailAPI<EmailMessage>)services.GetService(typeof(IGmailAPI<EmailMessage>)); 
            
            var threads = gmailAPI.getThreads("me","INBOX");
            Console.WriteLine(threads[0][0].subject);
        }
    }
}

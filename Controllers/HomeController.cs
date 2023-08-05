using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;

namespace Controllers
{
    [Route("")]
    [ApiController]
    public class Ipcheck : Controller
    {
        public IActionResult About()
        {
            try
            {

                try
                {

                    var addlist = Dns.GetHostEntry(Dns.GetHostName());
                    //Retrieve client IP address through HttpContext.Connection
                    var allArray = addlist.AddressList.Select(x => x.ToString().Trim());
                    var hostname = addlist.HostName.ToString().Trim();

                    return new JsonResult(new
                    {
                        allHost = allArray,
                        Hostname = hostname
                    });
                }
                catch (Exception e)
                {
                    return new JsonResult(new
                    {
                        errormessage = e.Message,
                        e.InnerException
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + "\n" + e.InnerException);
            }

        }
    }
}
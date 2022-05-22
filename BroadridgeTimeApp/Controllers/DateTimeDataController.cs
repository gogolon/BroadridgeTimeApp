using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BroadridgeTimeApp.Services;
using System.Threading.Tasks;
using BroadridgeTimeApp.Models;

namespace BroadridgeTimeApp.Controllers
{   
    [ApiController]
    public class DateTimeDataController : ControllerBase
    {
        [Route("api/[controller]")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                string timezone = System.IO.File.ReadAllText("timezone.cfg");
                var timeApi = new WorldTimeApiClient(timezone);
                return Ok(await timeApi.GetDateTimeDataAsync());
            }
            catch(Exception ex)
            {
                if (ex is InvalidTimeZoneException || ex is FileNotFoundException)
                {
                    return StatusCode(500);
                }
                else
                    return StatusCode(502);
            }
        }

        [Route("api/[controller]/timezones")]
        [HttpGet]
        public async Task<IActionResult> GetTimezones()
        {
            try
            {
                var timeApi = new WorldTimeApiClient();
                return Ok(await timeApi.GetTimezonesAsync());
            }
            catch
            {
                return StatusCode(502);
            }
        }
    }
}

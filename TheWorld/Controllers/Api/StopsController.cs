using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("/api/trips/{tripName}/stops")]
    public class StopsController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<StopsController> _logger;
        private GeoCoordsService _coordsService;

        public StopsController(IWorldRepository repository, ILogger<StopsController> logger, GeoCoordsService coordsService)
        {
            _repository = repository;
            _logger = logger;
            _coordsService = coordsService;
        }

        [HttpGet("")]
        public IActionResult Get(string tripname)
        {
            try
            {
                var trip = _repository.GetUserTripByName(tripname, User.Identity.Name);

                return Ok(Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.OrderBy(s => s.Arrival).ToList()));
            }
            catch (Exception ex)
            {
                _logger.LogError("failed to get stops: {0}", ex);
            }
            return BadRequest("failed to get stops");
        }

  
        [HttpPost("")]
        public async Task<IActionResult> Post(string tripname, [FromBody]StopViewModel vm)
        {
            try
            {
                // If VM is valid
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);


                    // look up geocodes
                    var result = await _coordsService.GetCoordsAsync(newStop.Name);
                    if (!result.Success)
                    {
                        _logger.LogError(result.Message);
                    }
                    else
                    {
                        newStop.Latitude = result.Latitude;
                        newStop.Longitude = result.Longitude;


                        //save to db

                        _repository.AddStop(tripname, newStop, User.Identity.Name);

                        if (await _repository.SaveChangesAsync())
                        {
                            return Created($"/api/trips/{tripname}/stops/{newStop.Name}",
                           Mapper.Map<StopViewModel>(newStop));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                
                _logger.LogError("failed to save new Stop: {0}", ex);
            }
            return BadRequest("Failed to save new stop");
        }

    }
}

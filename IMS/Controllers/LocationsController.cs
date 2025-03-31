using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IMS.Data;
using IMS.Models;
using IMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace IMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _service;

        public LocationsController(ILocationService service)
        {
            _service = service;
        }

        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<Location>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> Get(int id)
        {
            var location = await _service.GetByIdAsync(id);
            return location == null ? NotFound() : Ok(location);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<Location>> Create(Location location)
        {
            var created = await _service.AddAsync(location);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
    }
}

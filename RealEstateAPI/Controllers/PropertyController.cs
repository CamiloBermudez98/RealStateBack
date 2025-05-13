namespace RealEstateAPI.Controllers
{
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
    using RealEstateAPI.Models;
    using RealEstateAPI.Services;

    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly PropertyService _propertyService;

        public PropertyController(PropertyService propertyService)
        {
            _propertyService = propertyService;
        }


        [HttpGet("filtered")]
        public async Task<IActionResult> Get(
        [FromQuery] string? name,
        [FromQuery] string? address,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice)
        {
            var properties = await _propertyService.GetAsync(name, address, minPrice, maxPrice);
            return Ok(properties);
        }

        [HttpGet]
        public async Task<ActionResult<List<Property>>> Get() =>
            await _propertyService.GetAsync();

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var property = await _propertyService.GetByIdAsync(id);

            if (property == null)
                return NotFound($"No se encontró la propiedad con ID: {id}");

            return Ok(property);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Property newProperty)
        {
            await _propertyService.CreateAsync(newProperty);

            return CreatedAtAction(
                nameof(Get),                
                new { id = newProperty.Id },
                newProperty                  
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, Property updatedProperty)
        {
            var existing = await _propertyService.GetByIdAsync(id);
            if (existing is null) return NotFound();

            updatedProperty.Id = existing.Id;
            await _propertyService.UpdateAsync(id, updatedProperty);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var property = await _propertyService.GetByIdAsync(id);
            if (property is null) return NotFound();

            await _propertyService.DeleteAsync(id);
            return NoContent();
        }
    }
}

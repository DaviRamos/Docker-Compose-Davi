using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projetoTeste.Data;
using projetoTeste.Models;

namespace projetoTeste.Controllers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/[controller]")]
    public class PredictionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PredictionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Prediction
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelInput>>> GetModelInputs()
        {
            return await _context.ModelInputs.ToListAsync();
        }

        // GET: api/Prediction/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModelInput>> GetModelInput(int id)
        {
            var modelInput = await _context.ModelInputs.FindAsync(id);

            if (modelInput == null)
            {
                return NotFound();
            }

            return modelInput;
        }

        // PUT: api/Prediction/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModelInput(int id, ModelInput modelInput)
        {
            if (id != modelInput.Id)
            {
                return BadRequest();
            }

            _context.Entry(modelInput).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModelInputExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Prediction
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ModelInput>> PostModelInput(ModelInput modelInput)
        {
            _context.ModelInputs.Add(modelInput);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModelInput", new { id = modelInput.Id }, modelInput);
        }

        // DELETE: api/Prediction/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModelInput(int id)
        {
            var modelInput = await _context.ModelInputs.FindAsync(id);
            if (modelInput == null)
            {
                return NotFound();
            }

            _context.ModelInputs.Remove(modelInput);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ModelInputExists(int id)
        {
            return _context.ModelInputs.Any(e => e.Id == id);
        }
    }
}

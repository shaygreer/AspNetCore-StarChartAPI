using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id:int}")]
        public IActionResult GetById(int Id)
        {
            try
            {
                CelestialObject cObject = _context.CelestialObjects.Where(c => c.Id == Id).FirstOrDefault();
                if (cObject == null) return NotFound();
                cObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == cObject.Id).ToList();
                return Ok(cObject);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }    
        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            try
            {
                CelestialObject[] cObjects = _context.CelestialObjects.Where(c => c.Name == name).ToArray();
                if (cObjects.Length < 1) return NotFound();
                foreach (CelestialObject cObject in cObjects)
                {
                    cObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == cObject.Id).ToList();
                }
                return Ok(cObjects);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try 
            { 
                CelestialObject[] cObjects = _context.CelestialObjects.ToArray();
                foreach (CelestialObject cObject in cObjects)
                {
                    cObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == cObject.Id).ToList();
                }
                return Ok(cObjects);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            try
            {
                _context.CelestialObjects.Add(celestialObject);
                _context.SaveChanges();

                return CreatedAtRoute("GetById",new{ celestialObject.Id}, celestialObject);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            try
            {
                CelestialObject cObject = _context.CelestialObjects.Where(c => c.Id == id).FirstOrDefault();
                if (cObject == null) return NotFound();
                cObject.Name = celestialObject.Name;
                cObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
                cObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
                _context.CelestialObjects.Update(cObject);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            try
            {
                CelestialObject cObject = _context.CelestialObjects.Where(c => c.Id == id).FirstOrDefault();
                if (cObject == null) return NotFound();
                cObject.Name = name;
                _context.CelestialObjects.Update(cObject);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            List<CelestialObject> cObjects = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id).ToList();
            if (cObjects.Count < 1) return NotFound();
            _context.CelestialObjects.RemoveRange(cObjects);
            _context.SaveChanges();
            return NoContent();

        }




    }
}

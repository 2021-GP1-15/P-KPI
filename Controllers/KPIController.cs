using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IP_KPI.Data;
using IP_KPI.Models;
using IP_KPI.Pagination;

namespace IP_KPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIController : ControllerBase
    {
        private readonly db_a7baa5_ipkpiContext _context;

        public KPIController(db_a7baa5_ipkpiContext context)
        {
            _context = context;
        }

        // GET: api/Colleges
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<College>>> Colleges([FromQuery] Pages filter)
        {
            var validFilter = new Pages(filter.PageNumber, filter.PageSize);
            return await _context.Colleges
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();

        }


        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> Departments([FromQuery] Pages filter)
        {
            var validFilter = new Pages(filter.PageNumber, filter.PageSize);
            return await _context.Departments
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToListAsync();

        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UniProgram>>> Programs([FromQuery] Pages filter)
        {
            var validFilter = new Pages(filter.PageNumber, filter.PageSize);
            return await _context.UniPrograms
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToListAsync();

        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kpi>>> Kpis([FromQuery] Pages filter)
        {
            var validFilter = new Pages(filter.PageNumber, filter.PageSize);
            return await _context.Kpis
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToListAsync();

        }

        [Route("[action]")] //Kpiprograms/1/10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kpiprogram>>> Kpiprograms([FromQuery] Pages filter)
        {
            var validFilter = new Pages(filter.PageNumber, filter.PageSize);
            return await _context.Kpiprograms
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToListAsync();

        }
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicationReport>>> Puplicationreports([FromQuery] Pages filter)
        {
            var validFilter = new Pages(filter.PageNumber, filter.PageSize);
            return await _context.PublicationReports
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToListAsync();

        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacultyStatistic>>> Facultystatistics([FromQuery] Pages filter)
        {
            var validFilter = new Pages(filter.PageNumber, filter.PageSize);
            return await _context.FacultyStatistics
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToListAsync();

        }


        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Survey>>> Surveys([FromQuery] Pages filter)
        {
            var validFilter = new Pages(filter.PageNumber, filter.PageSize);
            return await _context.Surveys
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToListAsync();

        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlumniEmployment>>> Alumniemployments([FromQuery] Pages filter)
        {
            var validFilter = new Pages(filter.PageNumber, filter.PageSize);
            return await _context.AlumniEmployments
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToListAsync();

        }


        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BatchStatistic>>> BatchStatistics([FromQuery] Pages filter)
        {
            var validFilter = new Pages(filter.PageNumber, filter.PageSize);
            return await _context.BatchStatistics
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToListAsync();

        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassSection>>> ClassSections([FromQuery] Pages filter)
        {
            var validFilter = new Pages(filter.PageNumber, filter.PageSize);
            return await _context.ClassSections
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToListAsync();

        }
    }
}

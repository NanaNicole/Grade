using Colegio.Models;
using Colegio.Services;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Colegio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GradeController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<GradeController> _logger;
        private ColegioContext _dbcontext;
        private readonly IGrade _grade;

        public GradeController(ILogger<GradeController> logger, ColegioContext dbcontext, IGrade grade)
        {
            _logger = logger;
            _dbcontext = dbcontext;
            _grade = grade;
        }

        [HttpGet]
        [Route("GetGrades")]
        public IActionResult GetEstudent()
        {

            return Ok(_grade.GetGrades());
        }

        [HttpGet]
        [Route("GetGradeByNumber/{gradeNumber}")]
        public IActionResult GetGradeByNumber(int gradeNumber)
        {
            return Ok(_grade.GetGradeByNumber(gradeNumber));
        }

        [HttpGet]
        [Route("GetGradeByName/{gradeNanme}")]
        public IActionResult GetGradeByName(string gradeNanme)
        {
            return Ok(_grade.GetGradeByName(gradeNanme));
        }

        [HttpPost]
        [Route("CreateGrade")]
        public IActionResult CreateGrade(GradeDto grade)
        {
            return Ok(_grade.CreateGrade(grade));
        }

        [HttpDelete]
        [Route("DeleteGrade/{id}")]
        public IActionResult DeleteGrade(Guid id)
        {
            try
            {
                if (_grade.DeleteGrade(id))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("No se pudo eliminar correctamente la asignatura");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error eliminando asignaturas {ex.Message.ToString()}");
            }
        }


    }
}
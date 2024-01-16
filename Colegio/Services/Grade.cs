using Colegio.Models;
using Colegio.Services.Interfaces;
using Confluent.Kafka;
using Newtonsoft.Json;
using Student.Producer;

namespace Colegio.Services
{
    public class Grade : IGrade
    {

        private readonly ILogger<Grade> _logger;
        private readonly IColegioContext _dbContext;
        private readonly IProducer _producer;

        public Grade(ILogger<Grade> logger, IColegioContext dbContext, IProducer producer)
        {
            _logger = logger;
            _dbContext = dbContext;
            _producer = producer;
        }

        public List<GradeDto> GetGrades()
        {
            List<GradeDto> result = null;
            try
            {
                result = _dbContext.Grades.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data de la asignatura {ex.Message}");
            }
            return result;
        }

        public GradeDto GetGradeByNumber(int gradeNumber)
        {
            GradeDto result = null;
            try
            {
                result = _dbContext.Grades.Where(x => x.Number == gradeNumber)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data de la asignatura {ex.Message.ToString()}");
            }
            return result;
        }

        public GradeDto GetGradeByName(string gradeName)
        {
            GradeDto result = null;
            try
            {
                result = _dbContext.Grades.Where(x => x.Name == gradeName)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data de la asignatura {ex.Message.ToString()}");
            }
            return result;
        }

        public bool CreateGrade(GradeDto grade)
        {
            bool result = false;
            try
            {
                grade.Id = Guid.NewGuid();
                _dbContext.Grades.Add(grade);
                _dbContext.SaveChanges();
                result = true;
                _producer.ProduceMessage(JsonConvert.SerializeObject(grade));
            }
            catch (Exception ex)
            {
                _logger.LogError($"error creando data de la asignatura {ex.Message.ToString()}");
            }
            return result;
        }

        public bool DeleteGrade(Guid id)
        {
            bool result = false;
            try
            {
                GradeDto grade = FindGrade(id);
                if (grade != null)
                {
                    _dbContext.Grades.Remove(grade);
                    _dbContext.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data de la asignatura {ex.Message.ToString()}");
            }
            return result;
        }

        public GradeDto FindGrade(Guid id)
        {
            return _dbContext.Grades.Find(id);
        }
    }
}

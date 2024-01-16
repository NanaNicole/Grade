using Colegio.Models;

namespace Colegio.Services.Interfaces
{
    public interface IGrade
    {
        public List<GradeDto> GetGrades();

        public GradeDto GetGradeByNumber(int gradeNumber);
        public GradeDto GetGradeByName(string gradeName);

        public bool CreateGrade(GradeDto grade);
        public bool DeleteGrade(Guid id);

        public GradeDto FindGrade(Guid id);
    }
}

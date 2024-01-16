using Colegio.Models;
using Microsoft.EntityFrameworkCore;

namespace Colegio
{
    public interface IColegioContext
    {
        DbSet<GradeDto> Grades { get; set; }

        void SaveChanges();
    }
}
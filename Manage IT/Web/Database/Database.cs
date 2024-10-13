using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Web.Database
{
    public class Database : IDisposable
    {
        public List<DbContext> Entities { get; private set; }

        public Database()
        {

        }

        public void Dispose()
        {
            foreach (var entity in Entities)
            {
                entity.Dispose();
            }
        }
    }
}

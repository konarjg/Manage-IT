using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

internal class DatabaseContext : DbContext
{
    public DbSet<Prefix> Prefixes { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMembers> ProjectMembers { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TaskDetails> TaskDetails { get; set; }
    public DbSet<TaskList> TaskLists { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserPermissions> UserPermissions { get; set; }

    public DbSet<T> GetDatabaseSet<T>() where T : class
    {
        switch (typeof(T))
        {
            case Type t when t == typeof(Prefix):
                return (DbSet<T>)Convert.ChangeType(Prefixes, typeof(DbSet<T>));

            case Type t when t == typeof(Project):
                return (DbSet<T>)Convert.ChangeType(Projects, typeof(DbSet<T>));

            case Type t when t == typeof(ProjectMembers):
                return (DbSet<T>)Convert.ChangeType(ProjectMembers, typeof(DbSet<T>));

            case Type t when t == typeof(Task):
                return (DbSet<T>)Convert.ChangeType(Tasks, typeof(DbSet<T>));

            case Type t when t == typeof(TaskList):
                return (DbSet<T>)Convert.ChangeType(TaskLists, typeof(DbSet<T>));

            case Type t when t == typeof(TaskDetails):
                return (DbSet<T>)Convert.ChangeType(TaskDetails, typeof(DbSet<T>));

            case Type t when t == typeof(User):
                return (DbSet<T>)Convert.ChangeType(Users, typeof(DbSet<T>));

            case Type t when t == typeof(UserPermissions):
                return (DbSet<T>)Convert.ChangeType(UserPermissions, typeof(DbSet<T>));
        }

        return null;
    }
}

public class Database : IDisposable
{
    private DatabaseContext DatabaseContext;

    public Database()
    {
        DatabaseContext = new DatabaseContext();
    }

    public void Dispose()
    {
        DatabaseContext.Dispose();
    }

    public bool ExecuteQuery<T>(FormattableString query, out List<T> results) where T : class
    {
        try
        {
            results = DatabaseContext.GetDatabaseSet<T>().FromSqlInterpolated(query).ToList();
            return true;
        }
        catch (Exception exception)
        {
            results = null;
            return false;
        }
    }
}

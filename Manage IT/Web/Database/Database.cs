﻿using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

public class DatabaseContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMembers> ProjectMembers { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TaskDetails> TaskDetails { get; set; }
    public DbSet<TaskList> TaskLists { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserPermissions> UserPermissions { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectMembers>().HasNoKey();
        modelBuilder.Entity<UserPermissions>().HasNoKey();
        modelBuilder.Entity<TaskDetails>().HasNoKey();
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<T> GetDatabaseSet<T>() where T : class
    {
        switch (typeof(T))
        {
            case Type t when t == typeof(Project):
                return Projects as DbSet<T>;

            case Type t when t == typeof(ProjectMembers):
                return ProjectMembers as DbSet<T>;

            case Type t when t == typeof(Task):
                return Tasks as DbSet<T>;

            case Type t when t == typeof(TaskList):
                return TaskLists as DbSet<T>;

            case Type t when t == typeof(TaskDetails):
                return TaskDetails as DbSet<T>;

            case Type t when t == typeof(User):
                return Users as DbSet<T>;

            case Type t when t == typeof(UserPermissions):
                return UserPermissions as DbSet<T>;

            case Type t when t == typeof(Meeting):
                return Meetings as DbSet<T>;

            case Type t when t == typeof(Conversation):
                return Conversations as DbSet<T>;

            case Type t when t == typeof(Message):
                return Messages as DbSet<T>;
        }

        return null;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=sql.bsite.net\\MSSQL2016;Database=manageit_ManageIT;User Id=manageit_ManageIT;Password=manageit;");
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

    public bool ExecuteQueryUnknownType(FormattableString query)
    {
        try
        {
            DatabaseContext.Database.ExecuteSqlInterpolated(query);
            DatabaseContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool ExecuteQuery<T>(FormattableString query, out List<T> results) where T : class
    {
        try
        {
            if (query.Format.Contains("INSERT") || query.Format.Contains("UPDATE") || query.Format.Contains("DELETE"))
            {
                results = null;
                DatabaseContext.Database.ExecuteSqlInterpolated(query);
                DatabaseContext.SaveChanges();
                return true;
            }

            results = DatabaseContext.GetDatabaseSet<T>().FromSqlInterpolated(query).ToList();
            DatabaseContext.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            results = null;
            return false;
        }
    }
}
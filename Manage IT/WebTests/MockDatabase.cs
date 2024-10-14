using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

internal class MockDatabaseContext
{
    public List<Prefix> Prefixes { get; set; }
    public List<Project> Projects { get; set; }
    public List<ProjectMembers> ProjectMembers { get; set; }
    public List<Task> Tasks { get; set; }
    public List<TaskDetails> TaskDetails { get; set; }
    public List<TaskList> TaskLists { get; set; }
    public List<User> Users { get; set; }
    public List<UserPermissions> UserPermissions { get; set; }

    public MockDatabaseContext()
    {
        Prefixes = new List<Prefix>();
        Projects = new List<Project>();
        ProjectMembers = new List<ProjectMembers>();
        Tasks = new List<Task>();
        TaskDetails = new List<TaskDetails>();
        TaskLists = new List<TaskList>();
        Users = new List<User>();
        UserPermissions = new List<UserPermissions>();
    }

    public List<T> GetDatabaseSet<T>() where T : class
    {
        switch (typeof(T))
        {
            case Type t when t == typeof(Prefix):
                return (List<T>)Convert.ChangeType(Prefixes, typeof(List<T>));

            case Type t when t == typeof(Project):
                return (List<T>)Convert.ChangeType(Projects, typeof(List<T>));

            case Type t when t == typeof(ProjectMembers):
                return (List<T>)Convert.ChangeType(ProjectMembers, typeof(List<T>));

            case Type t when t == typeof(Task):
                return (List<T>)Convert.ChangeType(Tasks, typeof(List<T>));

            case Type t when t == typeof(TaskList):
                return (List<T>)Convert.ChangeType(TaskLists, typeof(List<T>));

            case Type t when t == typeof(TaskDetails):
                return (List<T>)Convert.ChangeType(TaskDetails, typeof(List<T>));

            case Type t when t == typeof(User):
                return (List<T>)Convert.ChangeType(Users, typeof(List<T>));

            case Type t when t == typeof(UserPermissions):
                return (List<T>)Convert.ChangeType(UserPermissions, typeof(List<T>));
        }

        return null;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

public class MockDatabase : IDisposable
{
    private MockDatabaseContext DatabaseContext;

    public MockDatabase()
    {
        DatabaseContext = new MockDatabaseContext();
    }

    public void Dispose()
    {
        DatabaseContext.Dispose();
    }

    public bool ExecuteQuery<T>(FormattableString query, out List<T> results) where T : class
    {
        try
        {
            if (!query.Format.Contains("SELECT") && !query.Format.Contains("INSERT") 
                && !query.Format.Contains("DELETE") && !query.Format.Contains("UPDATE"))
            {
                throw new ArgumentException();
            }

            results = DatabaseContext.GetDatabaseSet<T>();
            return true;
        }
        catch (Exception exception)
        {
            throw new ArgumentException();
        }
    }
}
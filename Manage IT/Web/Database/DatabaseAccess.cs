using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

public class DatabaseAccess
{
    private List<FormattableString> QueryBuffer;

    public DatabaseAccess()
    {
        QueryBuffer = new List<FormattableString>();
    }

    private void OnDatabaseUpdateRequested()
    {
        
    }

    public bool ProcessQuery<T>(FormattableString query, out List<T> results) where T : class
    {
        if (query.Format.Contains("INSERT") || query.Format.Contains("UPDATE") || query.Format.Contains("DELETE"))
        {
            QueryBuffer.Add(query);
            results = null;
            return true;
        }

        return ExecuteQuery(query, out results);
    }

    private bool ExecuteQuery<T>(FormattableString query, out List<T> results) where T : class
    {
        bool success;

        using (var database = new Database())
        {
            success = database.ExecuteQuery(query, out results);
        }

        return success;
    }
}

using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;

public class PrefixManager
{
    public List<Prefix> Prefixes { get; private set; }

    public static PrefixManager Instance { get; private set; }

    public PrefixManager()
    {
        Prefixes = GetAllPrefixes();
    }

    public static void Instantiate()
    {
        Instance = new PrefixManager();
    }

    private List<Prefix> GetAllPrefixes()
    {
        List<Prefix> prefixes;
        var query = FormattableStringFactory.Create("SELECT * FROM dbo.Prefixes");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out prefixes);

        if (!success)
        {
            prefixes = new List<Prefix>();
            return prefixes;
        }

        return prefixes;
    }

    public Prefix GetPrefixByCountry(string country)
    {
        return Prefixes.Find(prefix => prefix.Country == country);
    }

    public static bool AddPrefix(int prefixId, string country)
    {
        List<Prefix> prefixes;
        var query = FormattableStringFactory.Create($"INSERT INTO dbo.Prefixes (PrefixId, Country) VALUES ({prefixId}, '{country}')");
        
        return DatabaseAccess.Instance.ExecuteQuery(query, out prefixes);
    }

    public static bool RemovePrefix(int prefixId)
    {
        List<Prefix> prefixes;

        var query = FormattableStringFactory.Create("DELETE FROM dbo.Prefixes WHERE PrefixId = {0}", prefixId);

        return DatabaseAccess.Instance.ExecuteQuery(query, out prefixes);
    }
}
using Dapper;
using Domain.Querys.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

public class QueryService : IQueryService
{
    private readonly string _connectionString;

    public QueryService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<(IEnumerable<T> Data, string Message)> QueryAsync<T>(string sql, object? parameters = null)
    {
        try
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var data = await db.QueryAsync<T>(sql, parameters);
            return (data, "Success");
        }
        catch (Exception ex)
        {
            // Log ex here
            return (Enumerable.Empty<T>(), $"Error: {ex.Message}");
        }
    }

    public async Task<(IEnumerable<T1> Data, string Message)> QueryMultiMapAsync<T1, T2, TReturn>(
    string sql,
    Func<T1, T2, TReturn> map,
    string splitOn = "Id",
    object? parameters = null)
    {
        try
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            // Dapper Multi-Mapping
            var data = await db.QueryAsync(sql, map, parameters, splitOn: splitOn);

            return (data.Cast<T1>(), "Success");
        }
        catch (Exception ex)
        {
            return (Enumerable.Empty<T1>(), $"Error: {ex.Message}");
        }
    }

    public async Task<(T? Data, string Message)> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        try
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var data = await db.QueryFirstOrDefaultAsync<T>(sql, parameters);
            return (data, "Success");
        }
        catch (Exception ex)
        {
            return (default, $"Error: {ex.Message}");
        }
    }
}
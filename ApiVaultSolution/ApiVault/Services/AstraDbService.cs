﻿using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Reflection.Metadata;

public class AstraDbService
{
    private readonly HttpClient _httpClient;
    private readonly string _astraDbId;
    private readonly string _astraDbRegion;
    private readonly string _astraDbKeyspace;
    private readonly string _astraDbApplicationToken;

    public AstraDbService(HttpClient httpClient, string astraDbId, string astraDbRegion, string astraDbKeyspace, string astraDbApplicationToken)
    {
        _httpClient = httpClient;
        _astraDbId = astraDbId;
        _astraDbRegion = astraDbRegion;
        _astraDbKeyspace = astraDbKeyspace;
        _astraDbApplicationToken = astraDbApplicationToken;
    }

    public async Task<bool> DeleteApiKeyAsync(string tableName, string primaryKeyValue)
    {
        try
        {
            SetHeaders();

            string endpoint = $"https://{_astraDbId}-{_astraDbRegion}.apps.astra.datastax.com/api/rest/v2/keyspaces/{_astraDbKeyspace}/{tableName}/{primaryKeyValue}";

            // Send DELETE request
            HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);

            // Check if the delete was successful
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("API key deleted successfully.");
                return true;
            }
            else
            {
                Debug.WriteLine($"Error deleting API key: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return false;
        }
    }

    public async Task<HttpContent?> GetUserCredentials(string tableName, string primaryKeyValue)
    {
        try
        {
            SetHeaders();

            // route and headers
            string endpoint = $"https://{_astraDbId}-{_astraDbRegion}.apps.astra.datastax.com/api/rest/v2/keyspaces/{_astraDbKeyspace}/{tableName}/{primaryKeyValue}";

            // send get request
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }

            else
            {
                Debug.WriteLine($"Error getting credentials: {response.StatusCode}");
                return null;
            }
        }

        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return null;
        }
    }

    private void SetHeaders()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-cassandra-token", _astraDbApplicationToken);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public enum ExperimentServiceError
{
    NotModified,
    BadResponse,
    BadStatus,
    BadJSON
}

public class ExperimentServiceModel
{
    private static string ControlsPath => $"/flights/controls_v{KnownExperiment.ConfigurationVersion}.json";
    private static string ExperimentsPath => $"/flights/experiments_v{KnownExperiment.ConfigurationVersion}.json";

    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(5.0);

    public async Task GetExperimentConfigurationAsync(string currentEtag, Action<Result<Flight, Exception>> completion)
    {
        var url = new Uri($"{ServiceModel.AssetsHostName}{ControlsPath}");
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
        request.Headers.Add("If-None-Match", currentEtag);
        request.Headers.Add("X-App-Version", ServiceModel.GetAppVersion());
        ServiceModel.LogNetworkRequest(request);

        using (var client = new HttpClient())
        {
            client.Timeout = RequestTimeout;
            try
            {
                var response = await client.SendAsync(request);
                ServiceModel.LogNetworkResponse(response, request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var configurations = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExperimentConfiguration>>(content);
                    var newEtag = response.Headers.ETag?.Tag ?? Guid.NewGuid().ToString();
                    completion?.Invoke(Result<Flight, Exception>.Success(new Flight(newEtag, configurations)));
                }
                else if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    completion?.Invoke(Result<Flight, Exception>.Failure(new Exception("ExperimentServiceError.notModified")));
                }
                else
                {
                    completion?.Invoke(Result<Flight, Exception>.Failure(new Exception("ExperimentServiceError.badStatus")));
                }
            }
            catch (Exception ex)
            {
                completion?.Invoke(Result<Flight, Exception>.Failure(ex));
            }
        }
    }

    public async Task GetExperimentDescriptionsAsync(Action<Result<List<ExperimentDescription>, Exception>> completion)
    {
        var url = new Uri($"{ServiceModel.AssetsHostName}{ExperimentsPath}");
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
        request.Headers.Add("X-App-Version", ServiceModel.GetAppVersion());
        ServiceModel.LogNetworkRequest(request);

        using (var client = new HttpClient())
        {
            client.Timeout = RequestTimeout;
            try
            {
                var response = await client.SendAsync(request);
                ServiceModel.LogNetworkResponse(response, request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var configurations = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExperimentDescription>>(content);
                    completion?.Invoke(Result<List<ExperimentDescription>, Exception>.Success(configurations));
                }
                else
                {
                    completion?.Invoke(Result<List<ExperimentDescription>, Exception>.Failure(new Exception("ExperimentServiceError.badStatus")));
                }
            }
            catch (Exception ex)
            {
                completion?.Invoke(Result<List<ExperimentDescription>, Exception>.Failure(ex));
            }
        }
    }
}

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace MunicipalityRegistry.Producer.Snapshot.Oslo
{
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Polly;

    public interface ISnapshotManager
    {
        Task<OsloResult?> FindMatchingSnapshot(string persistentLocalId, Instant eventVersion, bool throwStaleWhenGone, CancellationToken cancellationToken);
    }

    public interface IOsloProxy
    {
        Task<OsloResult> GetSnapshot(string persistentLocal, CancellationToken cancellationToken);
    }

    public sealed class OsloProxy : IOsloProxy
    {
        private readonly HttpClient _httpClient;

        public OsloProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OsloResult> GetSnapshot(string persistentLocal, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/{persistentLocal}");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Accept", "application/ld+json");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var osloResult = JsonConvert.DeserializeObject<OsloResult>(jsonContent, new JsonSerializerSettings().ConfigureDefaultForApi());

            if (osloResult is null)
            {
                throw new JsonSerializationException();
            }

            osloResult.JsonContent = jsonContent;

            return osloResult;
        }
    }

    [Serializable]
    public sealed class StaleSnapshotException : Exception
    {
        public StaleSnapshotException()
        { }

        private StaleSnapshotException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public StaleSnapshotException(string message)
            : base(message)
        { }

        public StaleSnapshotException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    public sealed class SnapshotManagerOptions
    {
        private SnapshotManagerOptions()
        { }

        public int MaxRetryWaitIntervalSeconds { get; private init; }
        public int RetryBackoffFactor { get; private init; }

        public static SnapshotManagerOptions Create(string maxRetryWaitIntervalSeconds, string retryBackoffFactor)
        {
            if (string.IsNullOrEmpty(maxRetryWaitIntervalSeconds) || string.IsNullOrEmpty(retryBackoffFactor))
            {
                throw new ArgumentException("Config settings MaxRetryWaitIntervalSeconds and RetryBackoffFactor need to be set.");
            }

            return new SnapshotManagerOptions
            {
                MaxRetryWaitIntervalSeconds = Convert.ToInt32(maxRetryWaitIntervalSeconds),
                RetryBackoffFactor = Convert.ToInt32(retryBackoffFactor)
            };
        }
    }
    public static class ServiceCollectionExtensions
    {
        public static void AddOsloProxy(this IServiceCollection services, string osloApiUrl)
        {
            if (string.IsNullOrEmpty(osloApiUrl))
            {
                throw new ArgumentNullException(nameof(osloApiUrl), "OsloApiUrl config property not set.");
            }

            services.AddHttpClient<IOsloProxy, OsloProxy>(c =>
            {
                c.BaseAddress = new Uri(osloApiUrl.TrimEnd('/'));
            });
        }
    }

    public class OsloResult
    {
        public OsloResult()
        { }

        public OsloIdentificator Identificator { get; set; }

        [JsonIgnore]
        public string JsonContent { get; set; }
    }

    public class OsloIdentificator : Identificator
    {
        public OsloIdentificator()
            : base(string.Empty, string.Empty, string.Empty)
        { }
    }

    public class LocalSnapshotManager : ISnapshotManager
    {
        private readonly ILogger<LocalSnapshotManager> _logger;
        private readonly IOsloProxy _osloProxy;
        private readonly int _maxRetryWaitIntervalSeconds;
        private readonly int _retryBackoffFactor;

        public LocalSnapshotManager(
            ILoggerFactory loggerFactory,
            IOsloProxy osloProxy,
            SnapshotManagerOptions options)
        {

            _logger = loggerFactory.CreateLogger<LocalSnapshotManager>();
            _osloProxy = osloProxy;
            _maxRetryWaitIntervalSeconds = options.MaxRetryWaitIntervalSeconds;
            _retryBackoffFactor = options.RetryBackoffFactor;
        }

        public async Task<OsloResult?> FindMatchingSnapshot(string persistentLocalId,
            Instant eventVersion,
            bool throwStaleWhenGone,
            CancellationToken cancellationToken)
        {
            return await Policy
                .Handle<Exception>(e =>
                {
                    var shouldHandle = e is StaleSnapshotException or HttpRequestException;

                    if (shouldHandle)
                    {
                        _logger.LogWarning(e, $"Retry getting snapshot for '{persistentLocalId}' because of exception.");
                    }

                    return shouldHandle;
                })
                .WaitAndRetryForeverAsync(retryAttempt =>
                {
                    var waitIntervalSeconds = retryAttempt * _retryBackoffFactor;

                    if (waitIntervalSeconds > _maxRetryWaitIntervalSeconds)
                    {
                        waitIntervalSeconds = _maxRetryWaitIntervalSeconds;
                    }

                    return TimeSpan.FromSeconds(waitIntervalSeconds);
                })
                .ExecuteAsync(async _ => await GetSnapshot(), cancellationToken);

            async Task<OsloResult?> GetSnapshot()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                OsloResult? snapshot;

                try
                {
                    _logger.LogInformation($"Requesting snapshot for '{persistentLocalId}'");
                    snapshot = await _osloProxy.GetSnapshot(persistentLocalId, cancellationToken);
                    _logger.LogInformation("Snapshot received.");
                }
                catch (HttpRequestException e)
                {
                    _logger.LogWarning(e, $"HttpRequestException while getting snapshot for '{persistentLocalId}'.");

                    switch (e.StatusCode)
                    {
                        case HttpStatusCode.Gone when throwStaleWhenGone: throw;
                        case HttpStatusCode.Gone: return null;
                        default:
                            throw;
                    }
                }
                var snapshotDto = DateTimeOffset.Parse(snapshot.Identificator.Versie);
                var snapshotVersion = Instant.FromDateTimeOffset(snapshotDto);

                var versionDeltaInSeconds = Math.Floor(eventVersion.Minus(snapshotVersion).TotalSeconds);

                if (versionDeltaInSeconds > 0)
                {
                    throw new StaleSnapshotException();
                }

                return versionDeltaInSeconds == 0
                    ? snapshot
                    : null;
            }
        }
    }
}

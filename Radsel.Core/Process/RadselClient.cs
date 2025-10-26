using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NewtonsoftJsonHelper;
using Projektanker.ServerSentEvents;
using Projektanker.ServerSentEvents.Client;
using Radsel.Core.Model;
using Radsel.Core.Model.Battery;
using Radsel.Core.Model.Event;

namespace Radsel.Core.Process;
/// <summary>
///     Radsel controller client
/// </summary>
/// <param name="ccuApi">CCU API url</param>
/// <param name="credentials">Credentials to CCU api</param>
/// <param name="logger">Logger</param>
public class RadselClient(Uri ccuApi, RadselCredentials credentials, ILogger logger) : IDisposable {
    private readonly HttpClient _httpClient = new() {
        DefaultRequestHeaders =  {
            Authorization = new AuthenticationHeaderValue("Basic", credentials.Token)
        }
    };
    /// <summary>
    ///     Radsel CCURelay route
    /// </summary>
    public static readonly Uri RadselCCURelayUri = new("https://ccu.su");
    /// <summary>
    ///     Constructor of client with using "https://ccu.su" as source by default
    /// </summary>
    /// <param name="credentials">Credentials to CCU api</param>
    public RadselClient(RadselCredentials credentials, ILogger logger) : this(RadselCCURelayUri, credentials, logger) {}
    
    /// <summary>
    ///     Constructor of client with using "https://ccu.su" as source by default
    /// </summary>
    /// <param name="credentials">Credentials to CCU api</param>
    public RadselClient(RadselCredentials credentials) : this(RadselCCURelayUri, credentials, NullLogger.Instance) {}
    /// <summary>
    ///     Команда к Radsel
    /// </summary>
    /// <param name="Query">Путь запроса</param>
    /// <param name="Json">Содержимое запроса</param>
    public record RadselCommand(Uri Query, string Json);
    /// <summary>
    ///     Radsel sever sent event (SSE) type
    /// </summary>
    public enum RadselSSEventType {
        /// <summary>
        ///     Open connection
        /// </summary>
        Open,
        /// <summary>
        ///     Keep alive message
        /// </summary>
        KeepAlive,
        /// <summary>
        ///     New events
        /// </summary>
        NewEvents,
        /// <summary>
        ///     Close connection
        /// </summary>
        Close
    }
    /// <summary>
    ///     Get state and events in async manner
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task to command execution</returns>
    public async Task<RadselState> GetStateAndEventsAsync(CancellationToken cancellationToken) {
        var query = BuildCommandQuery("GetStateAndEvents");
        var responseJson = await ExecuteJsonAsync(query, cancellationToken);
        var result = ReadState(responseJson);
        return result;
    }
    /// <summary>
    ///     Ack specified events in async manner
    /// </summary>
    /// <param name="eventIdCollection">Collection of event identifiers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task to acking events</returns>
    public async Task AckEventsAsync(ICollection<RadselEventId> eventIdCollection, CancellationToken cancellationToken) {
        var query = BuildCommandQuery("AckEvents", new JObject {
            ["IDs"] = new JArray(eventIdCollection.Select(i => i.Id))
        });
        var responseJson = await ExecuteJsonAsync(query, cancellationToken);
        var response = ReadResponseStatusOrThrow(responseJson);
        if (response.Code != 0) {
            throw new RadselClientResponseException(response);
        }
    }
    /// <summary>
    ///     Set partition state async in async manner
    /// </summary>
    /// <param name="state">Expected partition state</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task to setup partition state</returns>
    public async Task<RadselState> SetPartitionStateAsync(RadselPartitionState state, CancellationToken cancellationToken) {
        var query = BuildCommandQuery("SetPartitionState", new JObject {
            ["State"] = Serialize(state)
        });
        var responseJson = await ExecuteJsonAsync(query, cancellationToken);
        var response = ReadResponseStatus(responseJson);
        if (response != null) {
            throw new RadselClientResponseException(response);
        }
        var result = ReadState(responseJson);
        return result;
    }
    /// <summary>
    ///     Set partition state to all partitions async in async manner
    /// </summary>
    /// <param name="stateArray">Expected states of all partitions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task to setup partition state</returns>
    public async Task<RadselState> SetPartitionsStateAsync(RadselPartitionState?[] stateArray, CancellationToken cancellationToken) {
        var query = BuildCommandQuery("SetPartitionState", new JObject {
            ["State"] = new JArray(stateArray.Select(state => state == null ? string.Empty : Serialize(state.Value)))
        });
        var responseJson = await ExecuteJsonAsync(query, cancellationToken);
        var response = ReadResponseStatus(responseJson);
        if (response != null) {
            throw new RadselClientResponseException(response);
        }
        var result = ReadState(responseJson);
        return result;
    }

    /// <summary>
    ///     Set output state in async manner
    /// </summary>
    /// <param name="output">Output number</param>
    /// <param name="state">Expected state</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task to setup partition state</returns>
    public async Task<RadselState> SetOutputStateAsync(int output, bool state, CancellationToken cancellationToken) {
        var query = BuildCommandQuery("SetOutputState", new JObject {
            ["Number"] = output,
            ["State"] = state ? 1 : 0
        });
        var responseJson = await ExecuteJsonAsync(query, cancellationToken);
        var response = ReadResponseStatus(responseJson);
        if (response != null) {
            throw new RadselClientResponseException(response);
        }
        var result = ReadState(responseJson);
        return result;
    }
    /// <summary>
    ///     Listen radsel events in async manner
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task to listenning radsel events</returns>
    public async IAsyncEnumerable<RadselSSEventType> ListenAsync([EnumeratorCancellation] CancellationToken cancellationToken) {
        var channel = Channel.CreateUnbounded<RadselSSEventType>();
        var subscriber = new RadselEventSubscriber(channel, logger);
        using var source = new EventSource(SSEConnect, subscriber);
        while (cancellationToken.IsCancellationRequested == false) {
            var radselEventEnumerable = channel.Reader.ReadAllAsync(cancellationToken);
            var radselEventCancellable = radselEventEnumerable.WithCancellation(cancellationToken);
            await foreach (var radselEvent in radselEventCancellable) {
                yield return radselEvent;
            }
        }
        async Task<HttpResponseMessage> SSEConnect(HttpCompletionOption option, CancellationToken cancellationToken) {
            var query = new Uri(ccuApi, "events");
            var request = new HttpRequestMessage(HttpMethod.Post, query);
            var response = await SendAsync(request, option, cancellationToken);
            return response;
        }
    }
    /// <summary>
    ///     Send request and parse response in json in async manner
    /// </summary>
    /// <param name="command">Command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task to request sending</returns>
    private async Task<JToken> ExecuteJsonAsync(RadselCommand command, CancellationToken cancellationToken) {
        using var request = new HttpRequestMessage(HttpMethod.Post, command.Query);
        using var content = new StringContent(command.Json, MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded"));
        request.Content = content;
        var result = await ExecuteJsonAsync(request, cancellationToken);
        return result;
    }
    /// <summary>
    ///     Send request and parse response in json in async manner
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task to request sending</returns>
    private async Task<JToken> ExecuteJsonAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        using var response = await SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseJson = JToken.Parse(responseContent);
        return responseJson;
    }
    /// <summary>
    ///     Send request in async manner
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task to request sending</returns>
    private Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        return SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
    }
    /// <summary>
    ///     Send request in async manner
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="option"><see cref="HttpCompletionOption"/></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task to request sending</returns>
    /// <exception cref="RadselException">Unexpected response status code</exception>
    private async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        HttpCompletionOption option,
        CancellationToken cancellationToken
    ) {
        var response = await _httpClient.SendAsync(request, option, cancellationToken);
        if (response.StatusCode != HttpStatusCode.OK) {
            throw new RadselException($"Unexpected response status code = {response.StatusCode}");
        }
        return response;
    }
    /// <summary>
    ///     Dispose unmanaged resources
    /// </summary>
    public void Dispose() {
        _httpClient.Dispose();
    }
    /// <summary>
    ///     Radsel event subscriber
    /// </summary>
    /// <param name="channel">Channel to streaming messages</param>
    /// <param name="logger">Logger</param>
    private class RadselEventSubscriber(Channel<RadselSSEventType> channel, ILogger logger) : IServerSentEventsSubscriber {
        private readonly Channel<RadselSSEventType> _channel = channel;
        /// <summary>
        ///     Process open connection event in async manner
        /// </summary>
        public async Task OnOpen() {
            await _channel.Writer.WriteAsync(RadselSSEventType.Open);
        }
        /// <summary>
        ///     Process message in async manner
        /// </summary>
        /// <param name="serverEvent">Event</param>
        /// <returns>Async task to message processing</returns>
        public async Task OnMessage(ServerSentEvent serverEvent) {
            if (serverEvent.EventType == "message") {
                var radselEvent = ReadSSEvent(serverEvent.Data);
                await _channel.Writer.WriteAsync(radselEvent);
            } else {
                throw new RadselException($"Unexpected radsel event type: {serverEvent.EventType}");
            }
        }
        /// <summary>
        ///     Process exception in async manner
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Async task to exception processing</returns>
        public Task OnError(Exception exception) {
            logger.LogError(exception, "Exception during SSE session");
            return Task.CompletedTask;
        }
    }
    /// <summary>
    ///     Build command query
    /// </summary>
    /// <param name="command">Command</param>
    /// <returns>Query</returns>
    public RadselCommand BuildCommandQuery(string command) {
        var parameters = new JObject();
        return BuildCommandQuery(command, parameters);
    }
    /// <summary>
    ///     Build command query
    /// </summary>
    /// <param name="command">Command</param>
    /// <param name="parameters">Parameters</param>
    /// <returns>Query</returns>
    public RadselCommand BuildCommandQuery(string command, JToken parameters) {
        var commandJson = new JObject {
            ["Command"] = command
        };
        var queryCommand = commandJson.ToString(Formatting.None);
        var commandQuery = new Uri(ccuApi, "data.cgx");
        var commandResultQuery = BuildQuery(commandQuery, new() {
            ["cmd"] = queryCommand
        });
        parameters["Command"] = command;
        return new RadselCommand(commandResultQuery, parameters.ToString(Formatting.None));
    }
    /// <summary>
    ///     Build query with specified query arguments
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="queryStringArgs">Query arguments</param>
    /// <returns>Query presented as <see cref="Uri"/></returns>
    public static Uri BuildQuery(Uri query, Dictionary<string, string?> queryStringArgs) {
        var parameters = BuildQueryString(queryStringArgs);
        return new($"{query}?{parameters}");
    }
    /// <summary>
    ///     Build query with specified query arguments
    /// </summary>
    /// <param name="queryStringArgs">Query arguments</param>
    /// <returns>Query parameters string</returns>
    public static string BuildQueryString(Dictionary<string, string?> queryStringArgs) {
        var variables = new List<string>();
        var ordered = queryStringArgs.OrderBy(item => item.Key);
        foreach (var (key, value) in ordered) {
            if (value == null) {
                continue;
            }
            var encodedKey = Uri.EscapeDataString(key);
            var encodedValue = Uri.EscapeDataString(value);
            var variable = $"{encodedKey}={encodedValue}";
            variables.Add(variable);
        }
        return string.Join("&", variables);
    }
    /// <summary>
    ///     Read a json from string
    /// </summary>
    /// <param name="str">String</param>
    /// <returns>Json</returns>
    /// <exception cref="RadselException">Invalid json syntax</exception>
    public static JToken ReadJson(string str) {
        try {
            return JToken.Parse(str);
        } catch (Exception exception) {
            throw new RadselException($"Invalid json message syntax: '{str}'", exception);
        }
    }
    /// <summary>
    ///     Read <see cref="RadselResponseStatus"/>
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns><see cref="RadselResponseStatus"/></returns>
    public static RadselResponseStatus? ReadResponseStatus(JToken json) {
        var objectToken = json.Select("$.Status", JTokenType.Object);
        if (objectToken == null) {
            return null;
        }
        var code = objectToken.SelectIntOrThrow("$.Code");
        var description = objectToken.SelectStringOrThrow("$.Description");
        return new RadselResponseStatus(code, description);
    }
    /// <summary>
    ///     Read <see cref="RadselResponseStatus"/>
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns><see cref="RadselResponseStatus"/></returns>
    public static RadselResponseStatus ReadResponseStatusOrThrow(JToken json) {
        var result = ReadResponseStatus(json);
        if (result == null) {
            throw new RadselException($"Expected response status response: {json}");
        }
        return result;
    }
    /// <summary>
    ///     Read radsel event from string message
    /// </summary>
    /// <param name.="message">String</param>
    /// <returns><see cref="RadselSSEventType"/></returns>
    public static RadselSSEventType ReadSSEvent(string message) {
        var json = ReadJson(message);
        var type = json.SelectStringOrThrow("$.Type");
        var radselEventType = ReadSSEventType(type);
        return radselEventType;
    }
    /// <summary>
    ///     Read <see cref="RadselSSEventType"/> from <see cref="string"/>
    /// </summary>
    /// <param name="str">String</param>
    /// <returns><see cref="RadselSSEventType"/></returns>
    public static RadselSSEventType ReadSSEventType(string str) {
        var strLowered = str.ToLowerInvariant().Trim();
        if (strLowered == "keepalive") {
            return RadselSSEventType.KeepAlive;
        }
        if (strLowered == "newevents") {
            return RadselSSEventType.NewEvents;
        }
        if (strLowered == "close") {
            return RadselSSEventType.Close;
        }
        throw new RadselException($"Unexpected radsel event type: {str}");
    }
    /// <summary>
    ///     Read <see cref="RadselState"/> from json
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns><see cref="RadselState"/></returns>
    public static RadselState ReadState(JToken json) {
        var inputs = ReadInputArray(json);
        var outputs = ReadOutputArray(json);
        var partitions = ReadPartitionStateArray(json);
        var caseState = ReadCaseState(json);
        var power = ReadPower(json);
        var temp = ReadTemp(json);
        var balance = ReadBalance(json);
        var battery = ReadBatteryObject(json);
        var events = ReadEventArray(json);
        return new(inputs, outputs, partitions, caseState, power, temp, balance, battery, events);
    }
    /// <summary>
    ///     Read array of <see cref="RadselInput"/>
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns>Array of <see cref="RadselInput"/></returns>
    public static RadselInput[] ReadInputArray(JToken json) {
        var inputsJson = json.SelectOrThrow("$.Inputs", JTokenType.Array);
        var inputs = inputsJson.Select(ReadInput).ToArray();
        return inputs;
    }
    /// <summary>
    ///     Read <see cref="RadselInput"/> from json
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns><see cref="RadselInput"/></returns>
    public static RadselInput ReadInput(JToken json) {
        var active = json.SelectIntOrThrow("$.Active");
        var voltage = json.SelectIntOrThrow("$.Voltage");
        var activeBool = active == 1;
        return new(activeBool, voltage);
    }
    /// <summary>
    ///     Read array of <see cref="RadselOutput"/>
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns>Array of <see cref="RadselOutput"/></returns>
    public static RadselOutput[] ReadOutputArray(JToken json) {
        var outputsList = json.SelectListOrThrow<int>("$.Outputs");
        var outputs = outputsList.Select(ReadOutput).ToArray();
        return outputs;
    }
    /// <summary>
    ///     Read <see cref="RadselOutput"/>
    /// </summary>
    /// <param name="value">Integer</param>
    /// <returns><see cref="RadselOutput"/></returns>
    public static RadselOutput ReadOutput(int value) => new(value == 1);
    /// <summary>
    ///     Read array of <see cref="RadselPartitionState"/>
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns>Array of <see cref="RadselPartitionState"/></returns>
    public static RadselPartitionState[] ReadPartitionStateArray(JToken json) {
        var partitionsList = json.SelectListOrThrow<string>("$.Partitions");
        var partitions = partitionsList.Select(ReadPartitionState).ToArray();
        return partitions;
    }
    /// <summary>
    ///     Read <see cref="RadselPartitionState"/>
    /// </summary>
    /// <param name="str">String</param>
    /// <returns><see cref="RadselPartitionState"/></returns>
    public static RadselPartitionState ReadPartitionState(string str) {
        var strPrepared = str.ToLowerInvariant();
        if (strPrepared == "arm") {
            return RadselPartitionState.Arm;
        }
        if (strPrepared == "disarm") {
            return RadselPartitionState.Disarm;
        }
        if (strPrepared == "protect") {
            return RadselPartitionState.Protect;
        }
        throw new RadselException($"Failed to read partition state: {str}");
    }
    /// <summary>
    ///     Read current state of case
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns>Case state</returns>
    public static RadselCaseState? ReadCaseState(JToken json) {
        var powerInt = json.SelectInt("$.Case");
        if (powerInt == null) {
            return null;
        }
        if (powerInt.Value == 1) {
            return RadselCaseState.Opened;
        }
        if (powerInt.Value == 0) {
            return RadselCaseState.Closed;
        }
        throw new RadselException($"Unexpected case state: {powerInt}");
    }
    /// <summary>
    ///     Read current power
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns>Power</returns>
    public static float? ReadPower(JToken json) {
        var powerToken = JsonHelper.SelectOrThrow(json, "$.Power", JTokenType.String, JTokenType.Integer, JTokenType.Float);
        if (powerToken.Type == JTokenType.String) {
            return null;
        }
        if (powerToken.Type == JTokenType.Float) {
            return json.SelectFloatOrThrow("$.Power");
        }
        if (powerToken.Type == JTokenType.Integer) {
            return json.SelectIntOrThrow("$.Power");
        }
        throw new RadselException($"Failed to read power: {json}");
    }
    /// <summary>
    ///     Read current temp
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns>Temp</returns>
    public static int? ReadTemp(JToken json) {
        var powerToken = JsonHelper.SelectOrThrow(json, "$.Temp", JTokenType.String, JTokenType.Integer);
        if (powerToken.Type == JTokenType.String) {
            return null;
        }
        return json.SelectIntOrThrow("$.Temp");
    }
    /// <summary>
    ///     Read current balance
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns>Balance</returns>
    public static float? ReadBalance(JToken json) {
        var powerToken = JsonHelper.SelectOrThrow(json, "$.Balance", JTokenType.String, JTokenType.Integer, JTokenType.Float);
        if (powerToken.Type == JTokenType.String) {
            return null;
        }
        if (powerToken.Type == JTokenType.Float) {
            return json.SelectFloatOrThrow("$.Balance");
        }
        if (powerToken.Type == JTokenType.Integer) {
            return json.SelectIntOrThrow("$.Balance");
        }
        throw new RadselException($"Failed to read balance: {json}");
    }

    /// <summary>
    ///     Read <see cref="RadselBattery"/> object
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns><see cref="RadselBattery"/></returns>
    public static RadselBattery ReadBatteryObject(JToken json) {
        var battery = json.SelectOrThrow("$.Battery", JTokenType.Object);
        return ReadBattery(battery);
    }
    /// <summary>
    ///     Read <see cref="RadselBattery"/>
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns><see cref="RadselBattery"/></returns>
    public static RadselBattery ReadBattery(JToken json) {
        var stateRaw = json.SelectStringOrThrow("$.State");
        var state = ReadBatteryState(stateRaw);
        var charge = json.SelectInt("$.Charge");
        return new(state, charge);
    }
    /// <summary>
    ///     Read <see cref="RadselBatteryState"/>
    /// </summary>
    /// <param name="str">String</param>
    /// <returns><see cref="RadselBatteryState"/></returns>
    public static RadselBatteryState ReadBatteryState(string str) {
        var strPrepared = str.ToLowerInvariant().Trim();
        if (strPrepared == "low2") {
            return RadselBatteryState.Low2;
        }
        if (strPrepared == "low1") {
            return RadselBatteryState.Low1;
        }
        if (strPrepared == "ok") {
            return RadselBatteryState.OK;
        }
        if (strPrepared == "notused") {
            return RadselBatteryState.NotUsed;
        }
        if (strPrepared == "disconnected") {
            return RadselBatteryState.Disconnected;
        }
        throw new RadselException($"Failed to read battery state: {str}");
    }
    /// <summary>
    ///     Read array of <see cref="RadselEvent"/>
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns>Array of <see cref="RadselEvent"/></returns>
    public static RadselEvent[] ReadEventArray(JToken json) {
        var array = json.SelectTokens("$.Events[::]").ToArray();
        var result = new RadselEvent[array.Length];
        var i = 0;
        foreach (var eventJson in array) {
            result[i] = ReadEvent(eventJson);
            i++;
        }
        return result;
    }
    /// <summary>
    ///     Read event <see cref="RadselEvent"/>
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns><see cref="RadselEvent"/></returns>
    /// <exception cref="RadselException">Failed to read event</exception>
    public static RadselEvent ReadEvent(JToken json) {
        var rawEvent = ReadRawEvent(json);
        if (rawEvent.Type == RadselEventType.InputPassive) {
            return ReadEventInputPassive(rawEvent.Id, json);
        }
        if (rawEvent.Type == RadselEventType.InputActive) {
            return ReadEventInputActive(rawEvent.Id, json);
        }
        if (rawEvent.Type == RadselEventType.PowerRecovery) {
            return new RadselEventPowerRecovery(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.PowerFault) {
            return new RadselEventPowerFault(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.BatteryLow1) {
            return new RadselEventBatteryLow1(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.BatteryLow2) {
            return new RadselEventBatteryLow2(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.BalanceLow) {
            return new RadselEventBalanceLow(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.TempLow) {
            return new RadselEventTempLow(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.TempNormal) {
            return new RadselEventTempNormal(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.TempHigh) {
            return new RadselEventTempHigh(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.CaseOpen) {
            return new RadselEventCaseOpen(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.Test) {
            return new RadselEventTest(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.Info) {
            return new RadselEventInfo(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.Arm) {
            return ReadEventArm(rawEvent.Id, json);
        }
        if (rawEvent.Type == RadselEventType.Disarm) {
            return ReadEventDisarm(rawEvent.Id, json);
        }
        if (rawEvent.Type == RadselEventType.Protect) {
            return ReadEventProtect(rawEvent.Id, json);
        }
        if (rawEvent.Type == RadselEventType.ProfileApplied) {
            return ReadProfileApplied(rawEvent.Id, json);
        }
        if (rawEvent.Type == RadselEventType.DeviceOn) {
            return new RadselEventDeviceOn(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.DeviceRestart) {
            return new RadselEventDeviceRestart(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.FirmwareUpgrade) {
            return new RadselEventFirmwareUpgrade(rawEvent.Id);
        }
        if (rawEvent.Type == RadselEventType.ExtRuntimeError) {
            return ReadExtRuntimeError(rawEvent.Id, json);
        }
        if (rawEvent.Type == RadselEventType.NewGateRecords) {
            return new RadselEventNewGateRecords(rawEvent.Id);
        }
        throw new RadselException($"Failed to read json event: {json}");
    }
    /// <summary>
    ///     Read raw event <see cref="RadselEvent"/>
    /// </summary>
    /// <param name="json">Json</param>
    /// <returns><see cref="RadselEvent"/></returns>
    public static RadselEvent ReadRawEvent(JToken json) {
        var eventIdRaw = json.SelectIntOrThrow("$.ID");
        var eventId = new RadselEventId(eventIdRaw);
        var typeRaw = json.SelectStringOrThrow("$.Type");
        var type = ReadEventType(typeRaw);
        return new(eventId, type);
    }
    /// <summary>
    ///     Read <see cref="RadselEventType"/> from string
    /// </summary>
    /// <param name="str">String</param>
    /// <returns><see cref="RadselEventType"/></returns>
    public static RadselEventType ReadEventType(string str) {
        var result = (RadselEventType)Enum.Parse(typeof(RadselEventType), str);
        return result;
    }
    /// <summary>
    ///     Read <see cref="RadselEventInputPassive"/> from json
    /// </summary>
    /// <param name="eventId">Event identifier</param>
    /// <param name="json">Json</param>
    public static RadselEventInputPassive ReadEventInputPassive(RadselEventId eventId, JToken json) {
        var pin = json.SelectIntOrThrow("$.Number");
        var partitionsList = json.SelectList<int>("$.Partitions");
        int[]? partitions = partitionsList == null ? null : [.. partitionsList];
        return new RadselEventInputPassive(eventId, pin, partitions);
    }
    /// <summary>
    ///     Read <see cref="RadselEventInputActive"/> from json
    /// </summary>
    /// <param name="eventId">Event identifier</param>
    /// <param name="json">Json</param>
    public static RadselEventInputActive ReadEventInputActive(RadselEventId eventId, JToken json) {
        var pin = json.SelectIntOrThrow("$.Number");
        var partitionsList = json.SelectList<int>("$.Partitions");
        int[]? partitions = partitionsList == null ? null : [.. partitionsList];
        return new RadselEventInputActive(eventId, pin, partitions);
    }
    /// <summary>
    ///     Read <see cref="RadselEventArm"/s>
    /// </summary>
    /// <param name="id">Event identifier</param>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventArm"/></returns>
    public static RadselEventArm ReadEventArm(RadselEventId id, JToken json) {
        var partition = json.SelectInt("$.Partition");
        var sourceJson = json.SelectOrThrow("$.Source", JTokenType.Object);
        var source = ReadEventSource(sourceJson);
        return new RadselEventArm(id, source, partition);
    }
    /// <summary>
    ///     Read <see cref="RadselEventDisarm"/s>
    /// </summary>
    /// <param name="id">Event identifier</param>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventDisarm"/></returns>
    public static RadselEventDisarm ReadEventDisarm(RadselEventId id, JToken json) {
        var partition = json.SelectInt("$.Partition");
        var sourceJson = json.SelectOrThrow("$.Source", JTokenType.Object);
        var source = ReadEventSource(sourceJson);
        return new RadselEventDisarm(id, source, partition);
    }
    /// <summary>
    ///     Read <see cref="RadselEventProtect"/s>
    /// </summary>
    /// <param name="id">Event identifier</param>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventProtect"/></returns>
    public static RadselEventProtect ReadEventProtect(RadselEventId id, JToken json) {
        var partition = json.SelectInt("$.Partition");
        var sourceJson = json.SelectOrThrow("$.Source", JTokenType.Object);
        var source = ReadEventSource(sourceJson);
        return new RadselEventProtect(id, source, partition);
    }
    /// <summary>
    ///     Read <see cref="RadselEventSource"/>
    /// </summary>
    /// <param name="json">Json of source</param>
    /// <returns><see cref="RadselEventSource"/></returns>
    public static RadselEventSource ReadEventSource(JToken json) {
        var sourceTypeRaw = json.SelectStringOrThrow("$.Type");
        var sourceType = ReadEventSourceType(sourceTypeRaw);
        if (sourceType == RadselEventSourceType.Button) {
            return new RadselEventSourceButton();
        }
        if (sourceType == RadselEventSourceType.Input) {
            return new RadselEventSourceInput();
        }
        if (sourceType == RadselEventSourceType.Scheduler) {
            return new RadselEventSourceScheduler();
        }
        if (sourceType == RadselEventSourceType.Modbus) {
            return new RadselEventSourceModbus();
        }
        if (sourceType == RadselEventSourceType.TouchMemory) {
            return ReadEventSourceTouchMemory(json);
        }
        if (sourceType == RadselEventSourceType.DTMF) {
            return ReadEventSourceDTMF(json);
        }
        if (sourceType == RadselEventSourceType.SMS) {
            return ReadEventSourceSMS(json);
        }
        if (sourceType == RadselEventSourceType.CSD) {
            return ReadEventSourceCSD(json);
        }
        if (sourceType == RadselEventSourceType.Call) {
            return ReadEventSourceCall(json);
        }
        if (sourceType == RadselEventSourceType.GTNet) {
            return new RadselEventSourceGTNet();
        }
        if (sourceType == RadselEventSourceType.UGuardNet) {
            return ReadEventSourceUGuardNet(json);
        }
        if (sourceType == RadselEventSourceType.Shell) {
            return ReadEventSourceShell(json);
        }
        throw new RadselException($"Failed to read event source: {json}");
    }
    /// <summary>
    ///     Read <see cref="RadselEventSourceType"/>
    /// </summary>
    /// <param name="str">String</param>
    /// <returns><see cref="RadselEventSourceType"/></returns>
    public static RadselEventSourceType ReadEventSourceType(string str) {
        if (str == "uGuardNet") {
            return RadselEventSourceType.UGuardNet;
        }
        var result = (RadselEventSourceType)Enum.Parse(typeof(RadselEventSourceType), str);
        return result;
    }
    /// <summary>
    ///     Read <see cref="RadselEventSourceTouchMemory"/>
    /// </summary>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventSourceTouchMemory"/></returns>
    public static RadselEventSourceTouchMemory ReadEventSourceTouchMemory(JToken json) {
        var key = json.SelectString("$.Key");
        var keyName = json.SelectString("$.KeyName");
        return new(key, keyName);
    }
    /// <summary>
    ///     Read <see cref="RadselEventSourceDTMF"/>
    /// </summary>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventSourceDTMF"/></returns>
    public static RadselEventSourceDTMF ReadEventSourceDTMF(JToken json) {
        var phone = json.SelectString("$.Phone");
        return new(phone);
    }
    /// <summary>
    ///     Read <see cref="RadselEventSourceSMS"/>
    /// </summary>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventSourceSMS"/></returns>
    public static RadselEventSourceSMS ReadEventSourceSMS(JToken json) {
        var phone = json.SelectString("$.Phone");
        return new(phone);
    }
    /// <summary>
    ///     Read <see cref="RadselEventSourceCSD"/>
    /// </summary>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventSourceCSD"/></returns>
    public static RadselEventSourceCSD ReadEventSourceCSD(JToken json) {
        var phone = json.SelectString("$.Phone");
        return new(phone);
    }
    /// <summary>
    ///     Read <see cref="RadselEventSourceCall"/>
    /// </summary>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventSourceCall"/></returns>
    public static RadselEventSourceCall ReadEventSourceCall(JToken json) {
        var phone = json.SelectString("$.Phone");
        return new(phone);
    }
    /// <summary>
    ///     Read <see cref="RadselEventSourceUGuardNet"/>
    /// </summary>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventSourceUGuardNet"/></returns>
    public static RadselEventSourceUGuardNet ReadEventSourceUGuardNet(JToken json) {
        var username = json.SelectStringOrThrow("$.UserName");
        return new(username);
    }
    /// <summary>
    ///     Read <see cref="RadselEventSourceShell"/>
    /// </summary>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventSourceShell"/></returns>
    public static RadselEventSourceShell ReadEventSourceShell(JToken json) {
        var username = json.SelectStringOrThrow("$.UserName");
        return new(username);
    }
    /// <summary>
    ///     Read <see cref="RadselEventProfileApplied"/>
    /// </summary>
    /// <param name="eventId">Event identifier</param>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventProfileApplied"/></returns>
    public static RadselEventProfileApplied ReadProfileApplied(RadselEventId eventId, JToken json) {
        var number = json.SelectIntOrThrow("$.Number");
        return new RadselEventProfileApplied(eventId, number);
    }
    /// <summary>
    ///     Read <see cref="RadselEventExtRuntimeError"/>
    /// </summary>
    /// <param name="eventId">Event identifier</param>
    /// <param name="json">Json of event</param>
    /// <returns><see cref="RadselEventExtRuntimeError"/></returns>
    public static RadselEventExtRuntimeError ReadExtRuntimeError(RadselEventId eventId, JToken json) {
        var errorCode = json.SelectIntOrThrow("$.ErrorCode");
        return new RadselEventExtRuntimeError(eventId, errorCode);
    }
    /// <summary>
    ///     Serialize <see cref="RadselPartitionState"/> to string
    /// </summary>
    /// <param name="state"><see cref="RadselPartitionState"/></param>
    /// <returns>String</returns>
    /// <exception cref="RadselException">Unexpected partition state</exception>
    public static string Serialize(RadselPartitionState state) {
        if (state == RadselPartitionState.Arm) {
            return "Arm";
        }
        if (state == RadselPartitionState.Disarm) {
            return "Disarm";
        }
        if (state == RadselPartitionState.Protect) {
            return "Protect";
        }
        throw new RadselException($"Unexpected partition state: {state}");
    }
}
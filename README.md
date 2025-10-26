# Radsel CCU API .NET Client
Simple client implementation of [CCU API interface](https://radsel.ru/files/docs/ccu-api/ccu-api.html).

```csharp
var credentials = new RadselCredentials("username", "password", "IMEI device");
var client = new RadselClient(credentials);

var state = await client.GetStateAndEventsAsync(CancellationToken.None);

await foreach (var message in client.ListenAsync(CancellationToken.None)) {
    if (message == RadselClient.RadselSSEventType.NewEvents) {
        var state = await client.GetStateAndEventsAsync(CancellationToken.None);
        foreach (var radselEvent in state.Events) {
            await Console.Out.WriteLineAsync(radselEvent.ToString());
        }
        await client.AckEventsAsync([.. state.Events.Select(e => e.Id)], CancellationToken.None);
    }
}
```

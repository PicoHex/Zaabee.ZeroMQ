namespace Aoxe.ZeroMQ.Test;

public partial class AoxeZeroMessageBusTest
{
    [Fact]
    public async Task ReqRepTestAsync()
    {
        using var server = new AoxeZeroMessageBus(
            new SystemTextJson.Serializer(),
            serverBindAddress: "inproc://test-client-server-async"
        );
        using var clientA = new AoxeZeroMessageBus(
            new SystemTextJson.Serializer(),
            clientConnectAddress: "inproc://test-client-server-async"
        );
        using var clientB = new AoxeZeroMessageBus(
            new SystemTextJson.Serializer(),
            clientConnectAddress: "inproc://test-client-server-async"
        );

        var modelA = TestModelFactory.Create();
        var modelB = TestModelFactory.Create();

        await clientA.ClientSendAsync(modelA);
        await clientB.ClientSendAsync(modelB);

        var (routingIdA, clientMsgA) = await server.ServerReceiveAsync<TestModel>();
        var (routingIdB, clientMsgB) = await server.ServerReceiveAsync<TestModel>();

        Assert.True(EqualModel(modelA, clientMsgA));
        Assert.True(EqualModel(modelB, clientMsgB));

        await server.ServerSendAsync(routingIdA, clientMsgA);
        await server.ServerSendAsync(routingIdB, clientMsgB);

        var serverMsgA = await clientA.ClientReceiveAsync<TestModel>();
        var serverMsgB = await clientB.ClientReceiveAsync<TestModel>();

        Assert.True(EqualModel(modelA, serverMsgA));
        Assert.True(EqualModel(modelB, serverMsgB));
    }
}

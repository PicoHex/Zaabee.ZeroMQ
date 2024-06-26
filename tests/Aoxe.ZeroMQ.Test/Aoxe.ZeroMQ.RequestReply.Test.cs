namespace Aoxe.ZeroMQ.Test;

public partial class AoxeZeroMessageBusTest
{
    [Fact]
    public void ReqRepTest()
    {
        using var server = new AoxeZeroMessageBus(
            new SystemTextJson.Serializer(),
            serverBindAddress: "inproc://test-client-server"
        );
        using var clientA = new AoxeZeroMessageBus(
            new SystemTextJson.Serializer(),
            clientConnectAddress: "inproc://test-client-server"
        );
        using var clientB = new AoxeZeroMessageBus(
            new SystemTextJson.Serializer(),
            clientConnectAddress: "inproc://test-client-server"
        );

        var modelA = TestModelFactory.Create();
        var modelB = TestModelFactory.Create();

        clientA.ClientSend(modelA);
        clientB.ClientSend(modelB);

        var (routingIdA, clientMsgA) = server.ServerReceive<TestModel>();
        var (routingIdB, clientMsgB) = server.ServerReceive<TestModel>();

        Assert.True(EqualModel(modelA, clientMsgA));
        Assert.True(EqualModel(modelB, clientMsgB));

        server.ServerSend(routingIdA, clientMsgA);
        server.ServerSend(routingIdB, clientMsgB);

        var serverMsgA = clientA.ClientReceive<TestModel>();
        var serverMsgB = clientB.ClientReceive<TestModel>();

        Assert.True(EqualModel(modelA, serverMsgA));
        Assert.True(EqualModel(modelB, serverMsgB));
    }
}

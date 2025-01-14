namespace CubeTrainer.API.Common.Endpoints;

internal interface IEndpoint
{
    public void Map(IEndpointRouteBuilder builder);
}
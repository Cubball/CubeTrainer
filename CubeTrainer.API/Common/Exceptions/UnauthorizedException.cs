namespace CubeTrainer.API.Common.Exceptions;

internal sealed class UnauthorizedException(string message) : Exception(message)
{
}
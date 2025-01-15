namespace CubeTrainer.API.Common.Exceptions;

internal class UnauthorizedException(string message) : Exception(message)
{
}
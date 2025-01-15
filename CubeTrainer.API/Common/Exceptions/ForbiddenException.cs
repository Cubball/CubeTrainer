namespace CubeTrainer.API.Common.Exceptions;

internal sealed class ForbiddenException(string message) : Exception(message)
{
}
namespace CubeTrainer.API.Common.Exceptions;

internal sealed class NotFoundException(string message) : Exception(message)
{
}
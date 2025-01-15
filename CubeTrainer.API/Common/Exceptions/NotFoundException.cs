namespace CubeTrainer.API.Common.Exceptions;

internal class NotFoundException(string message) : Exception(message)
{
}
namespace Domain.Exceptions;

public class EmptyEventStreamException(string message) : Exception(message);
namespace Domain.Utility;

/// <summary>
/// Abstraction for DateTime creation that allows simple time warping in (unit) testing.
/// </summary>
public interface IClock
{
    public DateTime Now();
}
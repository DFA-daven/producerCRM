namespace CallForm.Core.Services
{
    public interface ILocationService
    {
        bool TryGetLatestLocation(out double lat, out double lng);
    }
}
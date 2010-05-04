namespace FSNEP.Core.Domain
{
    public interface ITrackable
    {
        bool isTracked();

        bool arePropertiesTracked();
    }
}
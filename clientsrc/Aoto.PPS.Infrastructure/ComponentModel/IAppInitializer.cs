namespace Aoto.PPS.Infrastructure.ComponentModel
{
    public interface IAppInitializer
    {
        void Initializing();

        void Closing();
    }
}

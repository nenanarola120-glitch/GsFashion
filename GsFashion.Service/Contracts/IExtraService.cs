using GsFashion.Repository.Models.Extra;

namespace GsFashion.Service.Contracts
{
    public interface IExtraService
    {
        Task<IEnumerable<Response>> GetAppointment();
    }
}

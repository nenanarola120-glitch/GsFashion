using GsFashion.Repository.Models.Extra;

namespace GsFashion.Repository.Contracts
{
    public interface IExtraRepo
    {
        Task<IEnumerable<Response>> GetAppointment();
    }
}

using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models.Extra;
using GsFashion.Service.Contracts;

namespace GsFashion.Service.Services
{
    public class ExtraService(IExtraRepo _extraRepo) : IExtraService
    {
        #region Get All Appointment
        public async Task<IEnumerable<Response>> GetAppointment()
        {
            return await _extraRepo.GetAppointment();
        }
        #endregion
    }
}

using Dapper;
using GsFashion.Repository.Contracts;
using GsFashion.Repository.Dapper;
using GsFashion.Repository.Enums;
using GsFashion.Repository.Models.Extra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsFashion.Repository.Repository
{
    public class ExtraRepo : IExtraRepo
    {
        private const string _extraSp = "extra";
        public static IDbConnection _context;
        public ExtraRepo(DapperContext context)
        {
            _context = context.CreateConnection();
        }
        #region Get All Appointment
        public async Task<IEnumerable<Response>> GetAppointment()
        {
            var result = await _context.QueryAsync<Response>(
               _extraSp,
               new
               {
                   Type = SPEnum.GetAll.ToString()
               },
               commandType: CommandType.StoredProcedure
           );

            return result;
        }
        #endregion
    }
}

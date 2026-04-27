using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormCreation.Models;

namespace FormCreation.IRepository
{
    public interface IUserRepository
    {
        List<DropdownItem> GetCountries();
        List<DropdownItem> GetStatesByCountry(int countryId);
        List<DropdownItem> GetDistrictsByState(int stateId);
        void SaveUser(UserDetails u);
        List<UserDetails> GetUsers();
        UserDetails GetUserById(int id);
        void UpdateUser(UserDetails u);
        void DeleteUser(int id);
        List<AuditLogModel> GetAuditLogs();
    }
}

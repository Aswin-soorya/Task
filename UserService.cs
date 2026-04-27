using System.Collections.Generic;
using FormCreation.IRepository;
using FormCreation.IService;
using FormCreation.Models;

namespace FormCreation.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public List<DropdownItem> GetCountries()
            => _repo.GetCountries();

        public List<DropdownItem> GetStatesByCountry(int countryId)
            => _repo.GetStatesByCountry(countryId);

        public List<DropdownItem> GetDistrictsByState(int stateId)
            => _repo.GetDistrictsByState(stateId);
        public void SaveUser(UserDetails u)
    => _repo.SaveUser(u);

        public List<UserDetails> GetUsers()
            => _repo.GetUsers();

        public UserDetails GetUserById(int id)
            => _repo.GetUserById(id);

        public void UpdateUser(UserDetails u)
            => _repo.UpdateUser(u);

        public void DeleteUser(int id)
            => _repo.DeleteUser(id);
        public List<AuditLogModel> GetAuditLogs()
    => _repo.GetAuditLogs();
    }
}
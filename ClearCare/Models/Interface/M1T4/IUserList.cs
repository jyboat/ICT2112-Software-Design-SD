using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IUserList
    {
        Task<List<User>> retrieveAllUsers();

        Task<List<User>> retrieveAllDoctors();

        Task<List<User>> retrieveAllPatients();

        Task<List<User>> retrieveAllNurses();
    }
}
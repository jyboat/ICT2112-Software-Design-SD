using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IUserList
    {
        Task<List<User>> retrieveAllUsers();
    }
}
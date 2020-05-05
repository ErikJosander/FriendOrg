using FriendOrganizer.Models;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IMeetingRepository
    {
        Task<Meeting> GetByIdAsync(int id);
    }
}
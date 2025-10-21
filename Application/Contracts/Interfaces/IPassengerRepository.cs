using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Contracts.Interfaces
{
    public interface IPassengerRepository
    {
        Task AddPassengerAsync(Passenger passenger);
        Task<Passenger> GetPassengerByIdAsync(Guid passengerId);
    }
}

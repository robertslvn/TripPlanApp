using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetTripsByUsername(string name);
        Trip GetTripByName(string tripName);
        Trip GetUserTripByName(string tripname, string username);
        void RemoveTrip(Trip trip);
        
        void AddTrip(Trip trip);
        void AddStop(string tripname, Stop newStop, string username);

        Task<bool> SaveChangesAsync();
        
    }
}
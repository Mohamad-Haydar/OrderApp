using OrderApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApp.Services.Interfaces
{
    public interface IEventRepository : IRepository<EventModel>
    {
        Task<List<EventModel>> GetEvents(DateOnly date);

        Task<List<EventModel>> GetAllEvents();


        Task<int> AddEvent(string eventName, string description, string eventType, string startDateTime, string endDateTime);

        Task<int> UpdateEvent(int id, string eventName, string description, string eventType, string startDateTime, string endDateTime);

        Task<bool> DeleteEvent(int id);
    }
}

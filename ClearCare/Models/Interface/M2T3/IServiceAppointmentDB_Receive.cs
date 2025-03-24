// defines the methods for sending (or initiating) database operations such as fetching, creating, updating, and deleting nurse availabilities.
// implemented by NurseAvailabilityGateway; used by NurseAvailabilityManagement 

using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IServiceAppointmentDB_Receive
   {
    Task receiveServiceAppointmentList(List<ServiceAppointment> allServiceAppointments);
    Task receiveServiceAppointmentById(Dictionary<string, object> serviceAppointment) ;
    Task receiveCreatedServiceAppointmentId(string serviceAppointmentId); 
    Task receiveUpdatedServiceAppointmentStatus(bool updateStatus);
    Task receiveDeletedServiceAppointmentStatus(bool deleteStatus);
    Task receiveServiceAppointmentTimeById(DateTime? dateTime);
   }
}

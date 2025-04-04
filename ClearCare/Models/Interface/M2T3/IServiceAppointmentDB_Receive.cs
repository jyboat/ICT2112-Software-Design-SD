using ClearCare.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Interfaces
{
    public interface IServiceAppointmentDB_Receive
    {
        Task receiveServiceAppointmentList(List<ServiceAppointment> allServiceAppointments);
        Task receiveServiceAppointmentById(ServiceAppointment serviceAppointment);
        Task receiveCreatedServiceAppointmentId(string serviceAppointmentId);
        Task receiveUpdatedServiceAppointmentStatus(bool updateStatus);
        Task receiveDeletedServiceAppointmentStatus(bool deleteStatus);
        Task receiveServiceAppointmentTimeById(DateTime? dateTime);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;

    namespace ClearCare.Interfaces
    {
        public interface IServiceCompletion
        {
            Task UpdateAppointmentStatus(string appointmentId, string patientId, string nurseId);
        }
    }

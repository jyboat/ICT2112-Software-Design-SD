using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Google.Protobuf.WellKnownTypes;
using ClearCare.Interfaces;

namespace ClearCare.Models.Control
{
    public class ManualAppointmentScheduler
    {
        private readonly ICreateAppointment _iCreateAppointment;
        private readonly INurseAvailability _iNurseAvailability;
        public ManualAppointmentScheduler (ICreateAppointment ICreateAppointment, INurseAvailability INurseAvailability)
        {
            _iCreateAppointment = ICreateAppointment;
            _iNurseAvailability = INurseAvailability;
        }

        public async Task TestInterface()
        {
           //  await _iCreateAppointment.CreateAppointment();
            var staffAvailability = await _iNurseAvailability.getAllStaffAvailability();
        }   


    }

}
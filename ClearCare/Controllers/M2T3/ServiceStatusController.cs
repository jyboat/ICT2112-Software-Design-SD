using ClearCare.DataSource;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearCare.Interfaces;
using System.Text.Json;
using ClearCare.Interfaces;
using ClearCare.Models.Interface;

namespace ClearCare.Controllers
{
    [Route("ServiceStatus")]
    [ApiController]
    public class ServiceStatusController : Controller
    {
        private readonly ServiceAppointmentStatusManagement _manager;
        private readonly IUserList _userList;

        public ServiceStatusController()
        {
            _manager = new ServiceAppointmentStatusManagement();
            _userList = new AdminManagement(new UserGateway());
        }

        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> display(String? patientId)
        {
            if (patientId != null) {
                ViewBag.PatientIdFilter = patientId;
            }
            //  await to wait for task complete or data to retrieve before executing
            var appointment =  await _manager.getAppointmentDetails();

            var users = await _userList.retrieveAllUsers();
            var usersFiltered = users
            .Select(p => new
            {
                UserID = p.getProfileData()["UserID"].ToString(),
                Name = p.getProfileData()["Name"].ToString(),
                Role = p.getProfileData()["Role"].ToString()
            })
            .ToList();

            // Filter for Doctors
            ViewBag.Doctors = usersFiltered
                .Where(u => u.Role == "Doctor")
                .ToList();

            // Filter for Patients
            ViewBag.Patients = usersFiltered
                .Where(u => u.Role == "Patient")
                .ToList();

            // Filter for Nurses
            ViewBag.Nurses = usersFiltered
                .Where(u => u.Role == "Nurse")
                .ToList();


            var services = await _manager.getServices();
            ViewBag.Services = services;
            
            
            // No record exists
            if (appointment != null && appointment.Any())
            {
                return View("~/Views/M2T3/ServiceStatus/ServiceStatus.cshtml", appointment);
            }
            else
            {
                return View("~/Views/M2T3/ServiceStatus/ServiceStatus.cshtml", null);
            }

               
            }



    }
}
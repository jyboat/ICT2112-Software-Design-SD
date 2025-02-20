using Google.Cloud.Firestore;
using ClearCare.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClearCare.DataSource
{
    public class ServiceAppointmentGateway
    {
        private readonly FirestoreDb _db;

        public ServiceAppointmentGateway()
        {
            // Initialize Firebase
            _db = FirebaseService.Initialize();
        }

        // Create a new appointment
        public async Task<string> CreateAppointmentAsync(ServiceAppointment appointment)
        {
            // Get Collection in Firebase
            DocumentReference docRef = _db.Collection("ServiceAppointments").Document();

            appointment.SetAppointmentId(docRef.Id);

            // Convert input data to firestore data format for insert
            Dictionary<string, object> appointmentData = appointment.ToFirestoreDictionary();

            // Overwrite field if exist, create new if doesn't
            await docRef.SetAsync(appointmentData);

            return docRef.Id;
        }

        // Retrieve all
        public async Task<List<ServiceAppointment>> GetAllAppointmentsAsync()
        {
            CollectionReference appointmentsRef = _db.Collection("ServiceAppointments");

            QuerySnapshot snapshot = await appointmentsRef.GetSnapshotAsync();
            List<ServiceAppointment> appointmentList = new List<ServiceAppointment>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    var data = document.ToDictionary();
                    ServiceAppointment appointment = ServiceAppointment.FromFirestoreData(document.Id, data);
                    appointmentList.Add(appointment);
                }
            }

            return appointmentList;
        }

        // Retrieve an appointment by ID
        public async Task<ServiceAppointment?> GetAppointmentByIdAsync(string appointmentId)
        {
            DocumentReference docRef = _db.Collection("ServiceAppointments").Document(appointmentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"Firestore Document Not Found: {appointmentId}");
                return null;
            }

            // Convert to Dictionary from firebase key-value format
            var data = snapshot.ToDictionary();
            return ServiceAppointment.FromFirestoreData(appointmentId, data);
        }
    }
}

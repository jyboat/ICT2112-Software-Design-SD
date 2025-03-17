using Google.Cloud.Firestore;
using ClearCare.Models.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ClearCare.Interfaces;

namespace ClearCare.DataSource
{
    public class ServiceAppointmentGateway_2: IServiceAppointmentDB_Send {
        private readonly FirestoreDb _db;
        private IServiceAppointmentDB_Receive _receiver;

        public ServiceAppointmentGateway_2()
        {
            _db = FirebaseService.Initialize();
        }

        // Property for setting the receiver after instantiation (Since gateway handle receiver callback - creates circular dependency. SO need break cycle by property injection)
        public IServiceAppointmentDB_Receive Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }

         public async Task<List<ServiceAppointment>> fetchAllServiceAppointments() {
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
            await _receiver.receiveServiceAppointmentList(appointmentList);
            return appointmentList;
         }

         public async Task<Dictionary<string, object>> fetchServiceAppointmentByID(string appointmentId) {
            DocumentReference docRef = _db.Collection("ServiceAppointments").Document(appointmentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"Firestore Document Not Found: {appointmentId}");
                return null;
            }

            // Convert to Dictionary from firebase key-value format
            var data = snapshot.ToDictionary();
            
            var appt = ServiceAppointment.FromFirestoreData(appointmentId, data).ToFirestoreDictionary();
            await _receiver.receiveServiceAppointmentById(appt);
            return appt;
         }

         public async Task<string> CreateAppointment(ServiceAppointment appointment)
        {
            // Get Collection in Firebase
            DocumentReference docRef = _db.Collection("ServiceAppointments").Document();

            appointment.SetAppointmentId(docRef.Id);

            // Convert input data to firestore data format for insert
            Dictionary<string, object> appointmentData = appointment.ToFirestoreDictionary();

            // Overwrite field if exist, create new if doesn't
            await docRef.SetAsync(appointmentData);

            await _receiver.receiveCreatedServiceAppointmentId(docRef.Id);

            return docRef.Id;
        }

        public async Task<bool> UpdateAppointment(ServiceAppointment appointment)
        {
            try
            {
                DocumentReference docRef = _db.Collection("ServiceAppointments").Document(appointment.GetAttribute("AppointmentId"));
        
                // check if document exists
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (!snapshot.Exists)
                {
                    return false;
                }
        
                // update with the new data
                Dictionary<string, object> appointmentData = appointment.ToFirestoreDictionary();
                await docRef.SetAsync(appointmentData);
                _receiver.receiveUpdatedServiceAppointmentStatus(true);
        
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error updating appointment in firestore: {ex.Message}");
                _receiver.receiveUpdatedServiceAppointmentStatus(false);
                return false;
            }
        }

        public async Task<bool> DeleteAppointment (string appointmentId)
        {
             try
            {
                DocumentReference docRef = _db.Collection("ServiceAppointments").Document(appointmentId);
                
                // check if document exists
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (!snapshot.Exists)
                {
                    await _receiver.receiveDeletedServiceAppointmentStatus(false);
                    return false;
                }
                
                // delete doc
                await docRef.DeleteAsync();
               
                await _receiver.receiveDeletedServiceAppointmentStatus(true);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"error deleting appointment in firestore: {e.Message}");
                await _receiver.receiveDeletedServiceAppointmentStatus(false);
                throw;
            }
        }
        

        
    }
}
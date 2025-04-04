using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;

namespace ClearCare.DataSource
{
    public class ServiceTypeRepository : AbstractDatabaseSubject
    {
        private FirestoreDb _firestoreDb;

        public ServiceTypeRepository()
        {
            _firestoreDb = FirebaseService.Initialize();
        }

        public async Task getServiceTypesAsync()
        {
            List<ServiceType> serviceTypes = new List<ServiceType>();
            Query query = _firestoreDb.Collection("service_type");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    Dictionary<string, object> data = doc.ToDictionary();
                    ServiceType serviceType = new ServiceType
                    {
                        ServiceTypeId = Convert.ToInt32(data["serviceTypeId"]),
                        Name = data["name"].ToString(),
                        Duration = Convert.ToInt32(data["duration"]),
                        Requirements = data["requirements"].ToString(),
                        Modality = data["modality"]?.ToString(), // if you added modality
                        Status = data.ContainsKey("status") ? data["status"].ToString() : "active"
                    };


                    serviceTypes.Add(serviceType);
                }
            }
            notifyObservers(serviceTypes); //  Notify observers instead of returning
        }

        public async Task addServiceType(ServiceType serviceType)
        {
            CollectionReference colRef = _firestoreDb.Collection("service_type");
            Dictionary<string, object> serviceData = new Dictionary<string, object>
            {
                { "serviceTypeId", serviceType.ServiceTypeId },
                { "name", serviceType.Name },
                { "duration", serviceType.Duration },
                { "modality", serviceType.Modality },
                { "requirements", serviceType.Requirements }
            };
            await colRef.AddAsync(serviceData);
        }

        public async Task updateServiceType(int id, ServiceType serviceType)
        {
            Query query = _firestoreDb.Collection("service_type").WhereEqualTo("serviceTypeId", id);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                DocumentReference docRef = doc.Reference;
                Dictionary<string, object> updatedData = new Dictionary<string, object>
                {
                    { "name", serviceType.Name },
                    { "duration", serviceType.Duration },
                    { "modality", serviceType.Modality },
                    { "requirements", serviceType.Requirements }
                };
                await docRef.UpdateAsync(updatedData);
            }
        }

        public async Task discontinueServiceType(int id)
        {
            Query query = _firestoreDb.Collection("service_type").WhereEqualTo("serviceTypeId", id);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                DocumentReference docRef = doc.Reference;

                Dictionary<string, object> updatedData = new Dictionary<string, object>
        {
            { "status", "discontinued" }
        };

                await docRef.UpdateAsync(updatedData);
            }
        }


    }
}

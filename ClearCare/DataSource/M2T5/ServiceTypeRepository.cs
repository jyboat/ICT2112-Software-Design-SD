using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;

namespace ClearCare.DataSource
{
    public class ServiceTypeRepository
    {
        private FirestoreDb _firestoreDb;

        public ServiceTypeRepository()
        {
            _firestoreDb = FirebaseService.Initialize();
        }

        public async Task<List<ServiceType_SDM>> GetServiceTypes()
        {
            List<ServiceType_SDM> serviceTypes = new List<ServiceType_SDM>();
            Query query = _firestoreDb.Collection("service_type");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    Dictionary<string, object> data = doc.ToDictionary();
                    ServiceType_SDM serviceType = new ServiceType_SDM
                    {
                        ServiceTypeId = Convert.ToInt32(data["serviceTypeId"]),
                        Name = data["name"].ToString(),
                        Duration = Convert.ToInt32(data["duration"]),
                        Requirements = data["requirements"].ToString(),
                        Modality = data.ContainsKey("modality") ? data["modality"].ToString() : "Virtual" // fallback
                    };

                    serviceTypes.Add(serviceType);
                }
            }
            return serviceTypes;
        }

        public async Task AddServiceType(ServiceType_SDM serviceType)
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

        public async Task UpdateServiceType(int id, ServiceType_SDM serviceType)
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

        public async Task DiscontinueServiceType(int id)
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

using Google.Cloud.Firestore;

namespace ClearCare.Models.DTO.M3T2
{
    [FirestoreData]
    public class PatientDrugLogDTO
    {
        [FirestoreProperty]
        public string PatientId { get; set; }

        [FirestoreProperty]
        public string DrugName { get; set; }

        [FirestoreProperty]
        public string DrugInformation { get; set; }

        [FirestoreProperty]
        public string SideEffects { get; set; }

        [FirestoreProperty]
        public string Status { get; set; }
    }
}

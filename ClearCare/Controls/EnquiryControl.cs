using ClearCare.Models;
using ClearCare.Gateways;
using Microsoft.Extensions.Logging;
using ClearCare.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearCare.Controls
{
    public class EnquiryControl : ISubject<Enquiry>
    {
        private readonly ILogger<EnquiryControl> _logger;
        private readonly EnquiryGateway _enquiryGateway;

        public static List<Enquiry> Enquiries { get; set; } = new List<Enquiry>();
        private readonly List<Observer.IObserver<Enquiry>> _observers = new();

        public EnquiryControl(ILogger<EnquiryControl> logger, EnquiryGateway enquiryGateway)
        {
            _logger = logger;
            _enquiryGateway = enquiryGateway;
        }

        //=============================================================
        // ISubject<Enquiry> Implementation
        //=============================================================
        public void Attach(Observer.IObserver<Enquiry> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(Observer.IObserver<Enquiry> observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
            }
        }

        //=============================================================
        // Private Notification Methods
        //=============================================================
        private void notifyCreated(Enquiry enquiry)
        {
            foreach (var obs in _observers)
            {
                obs.OnCreated(enquiry);
            }
        }

        private void notifyUpdated(Enquiry enquiry)
        {
            foreach (var obs in _observers)
            {
                obs.OnUpdated(enquiry);
            }
        }

        private void notifyDeleted(string enquiryId)
        {
            foreach (var obs in _observers)
            {
                obs.OnDeleted(enquiryId);
            }
        }

        //=============================================================
        // Business Logic Methods
        //=============================================================
        public async Task createEnquiryAsync(
            Enquiry enquiry,
            string userUUID,
            string doctorUUID,
            string topic // <-- Accept Topic as a parameter
        )
        {
            _logger.LogInformation($"Creating enquiry from {enquiry.Name} for user {userUUID}, doctor {doctorUUID}, topic: {topic}.");

            // Assign the UUIDs and Topic on the Enquiry object.
            enquiry.UserUUID = userUUID;
            enquiry.DoctorUUID = doctorUUID;
            enquiry.Topic = topic;

            // Optionally store in your in-memory list (if you're using it).
            Enquiries.Add(enquiry);

            // Persist to Firestore via the gateway.
            await _enquiryGateway.saveEnquiryAsync(enquiry);

            // Notify observers of the new Enquiry.
            notifyCreated(enquiry);
        }

        public async Task saveReplyAsync(string enquiryId, Reply reply)
        {
            await _enquiryGateway.saveReplyAsync(enquiryId, reply);
        }

        public async Task<List<Reply>> fetchRepliesForEnquiryAsync(string enquiryId)
        {
            return await _enquiryGateway.getRepliesForEnquiryAsync(enquiryId);
        }

        public List<Enquiry> fetchAllEnquiries()
        {
            return Enquiries;
        }

        public async Task<List<Enquiry>> fetchEnquiriesByUserAsync(string userUUID)
        {
            return await _enquiryGateway.getEnquiriesForUserAsync(userUUID);
        }

        public async Task<List<Enquiry>> fetchEnquiriesByDoctorAsync(string userUUID)
        {
            return await _enquiryGateway.getEnquiriesForDoctorAsync(userUUID);
        }

        public async Task<Enquiry> fetchEnquiryByFirestoreIdAsync(string firestoreId)
        {
            var result = await _enquiryGateway.getEnquiryByIdAsync(firestoreId);

            if (result is null)
            {
                // For example, throw if not found:
                throw new KeyNotFoundException($"No enquiry found for Firestore ID {firestoreId}.");
            }

            return result;
        }


        // Future: Add update/delete methods using notifyUpdated/notifyDeleted
    }
}

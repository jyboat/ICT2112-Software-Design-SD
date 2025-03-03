using ClearCare.Models;
using ClearCare.Gateways;
using Microsoft.Extensions.Logging;
using ClearCare.Observer;  // For ISubject<T>, IObserver<T>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearCare.Controls
{
    // Make sure you have ISubject<Enquiry> and IObserver<Enquiry> defined in your ClearCare.Observer namespace
    public class EnquiryControl : ISubject<Enquiry>
    {
        private readonly ILogger<EnquiryControl> _logger;
        private readonly EnquiryGateway _enquiryGateway;

        // Optional in-memory list of enquiries
        public static List<Enquiry> Enquiries { get; set; } = new List<Enquiry>();

        // List of observers interested in Enquiry events
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

        // Notify all observers that an Enquiry was created
        private void NotifyCreated(Enquiry enquiry)
        {
            foreach (var obs in _observers)
            {
                obs.OnCreated(enquiry);
            }
        }

        // (Optional) If you add Update or Delete methods later, call these:
        private void NotifyUpdated(Enquiry enquiry)
        {
            foreach (var obs in _observers)
            {
                obs.OnUpdated(enquiry);
            }
        }

        private void NotifyDeleted(string enquiryId)
        {
            foreach (var obs in _observers)
            {
                obs.OnDeleted(enquiryId);
            }
        }

        //=============================================================
        // Existing Methods
        //=============================================================

        /// <summary>
        /// Create a new Enquiry (store in memory or Firestore).
        /// </summary>
        public async Task CreateEnquiryAsync(Enquiry enquiry)
        {
            _logger.LogInformation($"Creating enquiry from {enquiry.Name} with email {enquiry.Email}.");

            // Example: In-memory ID assignment
            enquiry.Id = Guid.NewGuid().ToString();

            // Add to local list
            Enquiries.Add(enquiry);

            // Also persist to Firestore (if using Firestore)
            await _enquiryGateway.SaveEnquiryAsync(enquiry);

            // Notify all observers that a new Enquiry was created
            NotifyCreated(enquiry);
        }

        /// <summary>
        /// Save a Reply to the given Enquiry in Firestore.
        /// </summary>
        public async Task SaveReplyAsync(string enquiryId, Reply reply)
        {
            await _enquiryGateway.SaveReplyAsync(enquiryId, reply);
        }

        /// <summary>
        /// Fetch Replies for a specific Enquiry from Firestore.
        /// </summary>
        public async Task<List<Reply>> FetchRepliesForEnquiryAsync(string enquiryId)
        {
            return await _enquiryGateway.GetRepliesForEnquiryAsync(enquiryId);
        }

        /// <summary>
        /// Return all enquiries (in-memory list).
        /// </summary>
        public List<Enquiry> FetchAllEnquiries()
        {
            return Enquiries;
        }

        /// <summary>
        /// Fetch all Enquiries for a specific user from Firestore.
        /// </summary>
        public async Task<List<Enquiry>> FetchEnquiriesByUserAsync(string userUUID)
        {
            var userEnquiries = await _enquiryGateway.GetEnquiriesForUserAsync(userUUID);
            return userEnquiries;
        }

        /// <summary>
        /// Fetch a single Enquiry by its Firestore ID.
        /// </summary>
        public async Task<Enquiry> FetchEnquiryByFirestoreIdAsync(string firestoreId)
        {
            return await _enquiryGateway.GetEnquiryByIdAsync(firestoreId);
        }

        // Future: Add Update or Delete methods, then call NotifyUpdated/NotifyDeleted.
    }
}

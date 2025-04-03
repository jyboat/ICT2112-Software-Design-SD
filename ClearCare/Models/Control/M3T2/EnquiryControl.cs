using Microsoft.Extensions.Logging;
using ClearCare.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearCare.DataSource.M3T2;
using ClearCare.Models.Entities.M3T2;

namespace ClearCare.Models.Control.M3T2
{
    public class EnquiryControl : ISubject<Enquiry>
    {
        private readonly ILogger<EnquiryControl> _logger;
        private readonly EnquiryGateway _enquiryGateway;

        /// <summary>
        ///   A list of enquiries stored in memory.
        /// </summary>
        public static List<Enquiry> Enquiries { get; set; } = new List<Enquiry>();
        private readonly List<Observer.IObserver<Enquiry>> _observers = new();

        /// <summary>
        ///   Initializes a new instance of the <see cref="EnquiryControl"/>
        ///   class.
        /// </summary>
        /// <param name="logger">
        ///   The logger for this control to record information and errors.
        /// </param>
        /// <param name="enquiryGateway">
        ///   The gateway for interacting with the enquiry data store.
        /// </param>
        public EnquiryControl(
            ILogger<EnquiryControl> logger,
            EnquiryGateway enquiryGateway
        )
        {
            _logger = logger;
            _enquiryGateway = enquiryGateway;
        }

        //=============================================================
        // ISubject<Enquiry> Implementation
        //=============================================================
        /// <summary>
        ///   Attaches an observer to the subject for receiving enquiry
        ///   notifications.
        /// </summary>
        /// <param name="observer">The observer to attach.</param>
        public void Attach(Observer.IObserver<Enquiry> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        /// <summary>
        ///   Detaches an observer from the subject, removing it from the list
        ///   of notification receivers.
        /// </summary>
        /// <param name="observer">The observer to detach.</param>
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
        /// <summary>
        ///   Notifies all attached observers that a new enquiry has been
        ///   created.
        /// </summary>
        /// <param name="enquiry">The newly created enquiry.</param>
        private void notifyCreated(Enquiry enquiry)
        {
            foreach (var obs in _observers)
            {
                obs.OnCreated(enquiry);
            }
        }

        /// <summary>
        ///   Notifies all attached observers that an enquiry has been updated.
        /// </summary>
        /// <param name="enquiry">The updated enquiry.</param>
        private void notifyUpdated(Enquiry enquiry)
        {
            foreach (var obs in _observers)
            {
                obs.OnUpdated(enquiry);
            }
        }

        /// <summary>
        ///   Notifies all attached observers that an enquiry has been deleted.
        /// </summary>
        /// <param name="enquiryId">The ID of the deleted enquiry.</param>
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
        /// <summary>
        ///   Creates a new enquiry.
        /// </summary>
        /// <param name="enquiry">The enquiry details.</param>
        /// <param name="userUUID">The UUID of the user creating the
        ///   enquiry.</param>
        /// <param name="doctorUUID">The UUID of the doctor to assign to the
        ///   enquiry.</param>
        /// <param name="topic">The topic of the enquiry.</param>
        public async Task createEnquiryAsync(
            Enquiry enquiry,
            string userUUID,
            string doctorUUID,
            string topic // <-- Accept Topic as a parameter
        )
        {
            _logger.LogInformation(
                $"Creating enquiry from {enquiry.Name} for user {userUUID}, doctor {doctorUUID}, topic: {topic}."
            );

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

        /// <summary>
        ///   Saves a reply to an existing enquiry.
        /// </summary>
        /// <param name="enquiryId">The ID of the enquiry to reply to.</param>
        /// <param name="reply">The reply details.</param>
        public async Task saveReplyAsync(string enquiryId, Reply reply)
        {
            await _enquiryGateway.saveReplyAsync(enquiryId, reply);
        }

        /// <summary>
        ///   Fetches all replies for a specific enquiry.
        /// </summary>
        /// <param name="enquiryId">The ID of the enquiry.</param>
        /// <returns>A list of replies for the specified enquiry.</returns>
        public async Task<List<Reply>> fetchRepliesForEnquiryAsync(string enquiryId)
        {
            return await _enquiryGateway.getRepliesForEnquiryAsync(enquiryId);
        }

        /// <summary>
        ///   Fetches all enquiries from memory.
        /// </summary>
        /// <returns>A list of all enquiries.</returns>
        public List<Enquiry> fetchAllEnquiries()
        {
            return Enquiries;
        }

        /// <summary>
        ///   Fetches all enquiries for a specific user.
        /// </summary>
        /// <param name="userUUID">The UUID of the user.</param>
        /// <returns>A list of enquiries for the specified user.</returns>
        public async Task<List<Enquiry>> fetchEnquiriesByUserAsync(string userUUID)
        {
            return await _enquiryGateway.getEnquiriesForUserAsync(userUUID);
        }

        /// <summary>
        ///   Fetches all enquiries for a specific doctor.
        /// </summary>
        /// <param name="userUUID">The UUID of the doctor.</param>
        /// <returns>A list of enquiries for the specified doctor.</returns>
        public async Task<List<Enquiry>> fetchEnquiriesByDoctorAsync(string userUUID)
        {
            return await _enquiryGateway.getEnquiriesForDoctorAsync(userUUID);
        }

        /// <summary>
        ///   Fetches a specific enquiry by its Firestore ID.
        /// </summary>
        /// <param name="firestoreId">The Firestore ID of the enquiry.</param>
        /// <returns>The enquiry with the specified Firestore ID.</returns>
        /// <exception cref="KeyNotFoundException">
        ///   Thrown if no enquiry is found with the specified Firestore ID.
        /// </exception>
        public async Task<Enquiry> fetchEnquiryByFirestoreIdAsync(string firestoreId)
        {
            var result = await _enquiryGateway.getEnquiryByIdAsync(firestoreId);

            if (result is null)
            {
                // For example, throw if not found:
                throw new KeyNotFoundException(
                    $"No enquiry found for Firestore ID {firestoreId}."
                );
            }

            return result;
        }

    }
}

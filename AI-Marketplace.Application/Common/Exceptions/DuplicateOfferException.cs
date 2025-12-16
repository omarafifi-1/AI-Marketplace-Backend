using System;

namespace AI_Marketplace.Application.Common.Exceptions
{
    public class DuplicateOfferException : Exception
    {
        public int? StoreId { get; }

        public int? CustomRequestId { get; }
        public DuplicateOfferException() : base("Store has already submitted an offer for this custom request.")
        {}

        public DuplicateOfferException(string message) : base(message)
        {}

        public DuplicateOfferException(string message, Exception innerException) : base(message, innerException)
        {}

        public DuplicateOfferException(int storeId, int customRequestId) : base($"Store {storeId} has already submitted an offer for custom request {customRequestId}.")
        {
            StoreId = storeId;
            CustomRequestId = customRequestId;
        }

    }
}
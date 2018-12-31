using System;
using RestSharp.Deserializers;
namespace Safaricom.Mpesa.Responses
{
    /// <summary>
    /// Generic response.
    /// Base class for common responses from Daraja 
    /// </summary>
    public abstract class GenericResponse
    {
        /// <summary>
        /// Gets or sets the conversation identifier.
        /// </summary>
        /// <value>The conversation identifier.</value>
        [DeserializeAs(Name = "ConversationID")]
        public string ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the originator coversation identifier.
        /// </summary>
        /// <value>The originator coversation identifier.</value>
        [DeserializeAs(Name = "OriginatorCoversationID")]
        public string OriginatorCoversationId { get; set; }

        /// <summary>
        /// Gets or sets the response description.
        /// </summary>
        /// <value>The response description.</value>
        [DeserializeAs(Name = "ResponseDescription")]
        public string ResponseDescription { get; set; }
    }
}

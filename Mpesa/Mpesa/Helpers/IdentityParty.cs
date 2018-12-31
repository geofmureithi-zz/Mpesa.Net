using System;
namespace Safaricom.Mpesa.Helpers
{
    public class IdentityParty
    {
        /// <summary>
        /// The actual party msisdn, till or shortcode.
        /// </summary>
        public int Party { get; }

        /// <summary>
        /// Identifier types as provided by Daraja
        /// </summary>
        /// <see cref="https://developer.safaricom.co.ke/docs#identifier-types"/>
        /// 
        public enum IdentifierType
        {
            MSISDN = 1,
            TILL = 2,
            SHORTCODE = 4
        }

        /// <summary>
        /// The type.
        /// </summary>
        public IdentifierType Type { get;}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Safaricom.Mpesa.Helpers.IdentityParty"/> class.
        /// </summary>
        /// <param name="party">Party.</param>
        /// <param name="type">Type.</param>
        public IdentityParty(int party, IdentifierType type)
        {
            Party = party;
            Type = type;
        }
    }
}

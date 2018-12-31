using System;
namespace Safaricom.Mpesa.Helpers
{
    public class CommandID
    {
        /// <summary>
        /// The command.
        /// </summary>
        private string Command;
        /// <summary>
        /// The description.
        /// </summary>
        private string Description;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Safaricom.Mpesa.Helpers.CommandID"/> class.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <param name="description">Description.</param>
        public CommandID(string command, string description)
        {
            Command = command;
            Description = description;
        }

        
        public override string ToString()
        {
            return Command;
        }
        public static CommandID TransactionReversal { get { return new CommandID("TransactionReversal", "Reversal for an erroneous C2B transaction.");  } }
        public static CommandID SalaryPayment { get { return new CommandID("SalaryPayment", "Used to send money from an employer to employees e.g. salaries"); } }
        public static CommandID BusinessPayment { get { return new CommandID("BusinessPayment", "Used to send money from business to customer e.g. refunds."); } }
        public static CommandID PromotionPayment { get { return new CommandID("PromotionPayment", "Used to send money when promotions take place e.g. raffle winners"); } }
        public static CommandID AccountBalance { get { return new CommandID("AccountBalance", "Used to check the balance in a paybill/buy goods account (includes utility, MMF, Merchant, Charges paid account)"); } }
        public static CommandID CustomerPayBillOnline { get { return new CommandID("CustomerPayBillOnline", "Used to simulate a transaction taking place in the case of C2B Simulate Transaction or to initiate a transaction on behalf of the customer (STK Push)."); } }
        public static CommandID TransactionStatusQuery { get { return new CommandID("TransactionStatusQuery", "Used to query the details of a transaction."); } }
        public static CommandID CheckIdentity { get { return new CommandID("CheckIdentity", "Similar to STK push, uses M-Pesa PIN as a service."); } }
        public static CommandID BusinessPayBill { get { return new CommandID("BusinessPayBill", "Sending funds from one paybill to another paybill."); } }
        public static CommandID BusinessBuyGoods { get { return new CommandID("BusinessBuyGoods", "Sending funds from buy goods to another buy goods."); } }
        public static CommandID DisburseFundsToBusiness { get { return new CommandID("DisburseFundsToBusiness", "Transfer of funds from utility to MMF account."); } }
        public static CommandID BusinessToBusinessTransfer { get { return new CommandID("BusinessToBusinessTransfer", "Transferring funds from one paybills MMF to another paybills MMF account"); } }
        public static CommandID BusinessTransferFromMMFToUtility { get { return new CommandID("BusinessTransferFromMMFToUtility", "Transferring funds from paybills MMF to another paybills utility account."); } }


    }
   }

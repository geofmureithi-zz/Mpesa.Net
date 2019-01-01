using System;
namespace Safaricom.Mpesa.Helpers
{
    public class CommandID
    {
        /// <summary>
        /// The command to be sent to Daraja in String format
        /// </summary>
        private string Command;
        /// <summary>
        /// The description of the command. Not used anywhere currently.
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

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Safaricom.Mpesa.Helpers.CommandID"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Safaricom.Mpesa.Helpers.CommandID"/>.</returns>
        public override string ToString()
        {
            return Command;
        }
        /// <summary>
        /// Reversal for an erroneous C2B transaction.
        /// </summary>
        /// <value>Transaction Reversal Command</value>
        public static CommandID TransactionReversal { get { return new CommandID("TransactionReversal", "Reversal for an erroneous C2B transaction.");  } }

        /// <summary>
        /// Used to send money from an employer to employees e.g. salaries
        /// </summary>
        /// <value>Salary Payment Command</value>
        public static CommandID SalaryPayment { get { return new CommandID("SalaryPayment", "Used to send money from an employer to employees e.g. salaries"); } }
        /// <summary>
        /// Used to send money from business to customer e.g. refunds
        /// </summary>
        /// <value>Business Payment Command</value>
        public static CommandID BusinessPayment { get { return new CommandID("BusinessPayment", "Used to send money from business to customer e.g. refunds."); } }
        /// <summary>
        /// Used to send money when promotions take place e.g. raffle winners
        /// </summary>
        /// <value>Promotion Payment Command</value>
        public static CommandID PromotionPayment { get { return new CommandID("PromotionPayment", "Used to send money when promotions take place e.g. raffle winners"); } }
        /// <summary>
        /// Used to check the balance in a paybill/buy goods account (includes utility, MMF, Merchant, Charges paid account)
        /// </summary>
        /// <value>Account Balance Command</value>
        public static CommandID AccountBalance { get { return new CommandID("AccountBalance", "Used to check the balance in a paybill/buy goods account (includes utility, MMF, Merchant, Charges paid account)"); } }
        /// <summary>
        /// Used to simulate a transaction taking place in the case of C2B Simulate Transaction or to initiate a transaction on behalf of the customer (STK Push).
        /// </summary>
        /// <value>Customer PayBill Online Command.</value>
        public static CommandID CustomerPayBillOnline { get { return new CommandID("CustomerPayBillOnline", "Used to simulate a transaction taking place in the case of C2B Simulate Transaction or to initiate a transaction on behalf of the customer (STK Push)."); } }
        /// <summary>
        /// Used to query the details of a transaction
        /// </summary>
        /// <value>Transaction Status Query Command </value>
        public static CommandID TransactionStatusQuery { get { return new CommandID("TransactionStatusQuery", "Used to query the details of a transaction."); } }
        /// <summary>
        /// Similar to STK push, uses M-Pesa PIN as a service
        /// </summary>
        /// <value>Check Identity Command</value>
        public static CommandID CheckIdentity { get { return new CommandID("CheckIdentity", "Similar to STK push, uses M-Pesa PIN as a service."); } }
        /// <summary>
        /// Gets the business pay bill.
        /// </summary>
        /// <value>Sending funds from one paybill to another paybill.</value>
        public static CommandID BusinessPayBill { get { return new CommandID("BusinessPayBill", "Sending funds from one paybill to another paybill."); } }
        /// <summary>
        /// Sending funds from buy goods to another buy goods
        /// </summary>
        /// <value> Business BuyGoods Command</value>
        public static CommandID BusinessBuyGoods { get { return new CommandID("BusinessBuyGoods", "Sending funds from buy goods to another buy goods."); } }
        /// <summary>
        /// Transfer of funds from utility to MMF account
        /// </summary>
        /// <value>Disburse Funds To Business Command</value>
        public static CommandID DisburseFundsToBusiness { get { return new CommandID("DisburseFundsToBusiness", "Transfer of funds from utility to MMF account."); } }
        /// <summary>
        /// Transferring funds from one paybills MMF to another paybills MMF account
        /// </summary>
        /// <value>Business To Business Transfer Command</value>
        public static CommandID BusinessToBusinessTransfer { get { return new CommandID("BusinessToBusinessTransfer", "Transferring funds from one paybills MMF to another paybills MMF account"); } }
        /// <summary>
        /// Transferring funds from paybills MMF to another paybills utility account
        /// </summary>
        /// <value>Business Transfer From MMF To Utility</value>
        public static CommandID BusinessTransferFromMMFToUtility { get { return new CommandID("BusinessTransferFromMMFToUtility", "Transferring funds from paybills MMF to another paybills utility account."); } }


    }
   }

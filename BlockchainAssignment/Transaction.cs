using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    class Transaction
    {
        public String senderAddress;  // Public key of sender
        public String recipientAddress;  // Public key of recipient

        public double amount;  // Amount of currency being sent over
        public double fee;  // Fee added to the transaction

        DateTime timestamp;  // Time when transaction as made

        public String hash;  // Hash of the transaction and its contents
        String signature;  // The hash of the transaction signed with the private key of the sender

        public Transaction(String from, String to, double amount, double fee, String privateKey)  // Constructor
        {
            senderAddress = from;  // Setup the sender address
            recipientAddress = to;  // Setup the recipient address
            this.amount = amount;  // Setup the amount
            this.fee = fee;  // Setup the fee
            timestamp = DateTime.Now;  // Creates a time stamp

            hash = CreateHash();  // Generates a hash
            signature = Wallet.Wallet.CreateSignature(from, privateKey, hash);  // Checks the signature
        }

        public String CreateHash()  // Generates a hash
        {
            String hash = String.Empty;  // Empties the string
            SHA256 hasher = SHA256Managed.Create();  // Creates a hasher

            String input = senderAddress + recipientAddress + timestamp.ToString() + amount.ToString() + fee.ToString();  // Input for the hasher
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));  // Sets the input to bytes
            foreach (byte x in hashByte)
            {
                hash += String.Format("{0:x2}", x);  // Loops over bytes and formats them into a string
            }

            return hash;  // Returns the hash
        }

        public override string ToString()  // Generates a string for the transaction
        {
            return "Transaction Hash: " + hash + "\n"
                + "Digital Signature: " + signature + "\n"
                + "Timestamp: " + timestamp.ToString() + "\n" 
                + "Transferred: " + amount.ToString() + " AssignmentCoin\n"
                + "Fees: " + fee.ToString() + "\n"
                + "Sender Address: " + senderAddress + "\n"
                + "Receiver Address: " + recipientAddress + "\n";  // Returns a string of all the information
        }
    }
}

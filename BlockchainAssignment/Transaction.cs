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
        /* Transaction Variables */
        DateTime timestamp; // the time it was generated
        public String senderAddress, recipientAddress; // sender and reciever public key addresses
        public double amount, fee; // the amount of crypto involved 
        public String hash, signature; // the values needed to authorise the transaction

        /* Transaction Constructor */
        public Transaction(String from, String to, double amount, double fee, String privateKey)
        {
            timestamp = DateTime.Now;
            senderAddress = from;
            recipientAddress = to;
            this.amount = amount;
            this.fee = fee;
            hash = CreateHash(); // hash the transaction attributes
            signature = Wallet.Wallet.CreateSignature(from, privateKey, hash); // authorises the transaction as the hash has a signature unique to the sender
        }

        /* Hash the transaction attributes using SHA256 */
        public String CreateHash()
        {
            String hash = String.Empty;
            SHA256 hasher = SHA256Managed.Create();
            /* combine the transaction properties */
            String input = timestamp + senderAddress + recipientAddress + amount + fee;
            /* the hash function is applied to the "input" string */
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            /* changed to a string */
            foreach (byte x in hashByte)
                hash += String.Format("{0:x2}", x);
            return hash;
        }

        // Represent a transaction as a string for output to UI
        public override string ToString()
        {
            
            return "  [TRANSACTION START]" 
                + "\n  Timestamp: " + timestamp
                + "\n  -- Verification --"
                + "\n  Hash: " + hash
                + "\n  Signature: " + signature
                + "\n  -- Quantities --"
                + "\n  Transferred: " + amount + " Assignment Coin"
                + "\t  Fee: " + fee
                + "\n  -- Participants --"
                + "\n  Sender: " + senderAddress
                + "\n  Reciever: " + recipientAddress 
                + "\n  [TRANSACTION END]";
        }
    }
}

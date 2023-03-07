using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    class Block
    {
        /* Block Variables */
        private DateTime timestamp; // when the block is made

        private int index, // this is where the block is situated in the blockchain
            difficulty = 4; // the random number of 0's that proceed a hash value

        public String prevHash, // this is the pointer that calls to the previous block
            hash, // this is the unique number associated with that specific block
            merkleRoot,  // this is the merkle root of all transactions in the block
            minerAddress; // this is the wallet address or Public Key of the Miner
        public List<Transaction> transactionList; // keeps a record of all transactions within this block
        
        // Proof-of-work
        public long nonce; // number used once for Proof-of-Work and mining

        // Rewards
        public double reward; // Simple fixed reward established by "Coinbase"

        /* Genesis block constructor */
        public Block()
        {
            timestamp = DateTime.Now;
            index = 0;
            transactionList = new List<Transaction>();
            hash = Mine();
        }

        /* New Block constructor */
        public Block(Block lastBlock, List<Transaction> transactions, String minerAddress)
        {
            timestamp = DateTime.Now;

            index = lastBlock.index + 1;
            prevHash = lastBlock.hash;

            this.minerAddress = minerAddress; // wallet  to be credited the reward for the mining effort
            reward = 1.0; // a simple fixed value reward is created
            transactions.Add(createRewardTransaction(transactions)); // the reward transaction is produced
            transactionList = new List<Transaction>(transactions); // assign provided transactions to the block

            merkleRoot = MerkleRoot(transactionList); // the merkle root of the blocks transactions is calculated
            hash = Mine(); // PoW is conducted to create a hash which meets the difficulty requirement
        }

        /* creates a hash out of the entire block object */
        public String CreateHash()
        {
            String hash = String.Empty;
            SHA256 hasher = SHA256Managed.Create();

            /* combines the block properties to create an overall hash for each call */
            String input = timestamp.ToString() + index + prevHash + nonce + merkleRoot;

            /* apply the hash function to the block as represented by the string "input" */
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            /* turns the value into a string */
            foreach (byte x in hashByte)
                hash += String.Format("{0:x2}", x);
            
            return hash;
        }

        // Create a Hash which satisfies the difficulty level required for PoW
        public String Mine()
        {
            nonce = 0; // Initalise the nonce
            String hash = CreateHash(); // Hash the block

            String re = new string('0', difficulty); // A string for analysing the PoW requirement

            while(!hash.StartsWith(re)) // Check the resultant hash against the "re" string
            {
                nonce++; // Increment the nonce should the difficulty level not be satisfied
                hash = CreateHash(); // Rehash with the new nonce as to generate a different hash
            }

            return hash; // Return the hash meeting the difficulty requirement
        }

        // Merkle Root Algorithm - Encodes transactions within a block into a single hash
        public static String MerkleRoot(List<Transaction> transactionList)
        {
            List<String> hashes = transactionList.Select(t => t.hash).ToList(); // get a list of transaction hashes for "combining"
            // Handle Blocks with...
            if (hashes.Count == 0) // No transactions
            {
                return String.Empty;
            }
            if (hashes.Count == 1) // One transaction - hash with "self"
            {
                return HashCode.HashTools.combineHash(hashes[0], hashes[0]); 
            }
            while (hashes.Count != 1) // Multiple transactions - Repeat until tree has been traversed
            {
                List<String> merkleLeaves = new List<String>(); // Keep track of current "level" of the tree
                for (int i = 0; i < hashes.Count; i += 2) // Step over neighbouring pair combining each
                {
                    if (i == hashes.Count - 1)
                    {
                        merkleLeaves.Add(HashCode.HashTools.combineHash(hashes[i], hashes[i])); // Handle an odd number of leaves
                    }

                    else
                    {
                        merkleLeaves.Add(HashCode.HashTools.combineHash(hashes[i], hashes[i + 1]));
                        // Hash neighbours leaves
                    }
                }
                hashes = merkleLeaves; // Update the working "layer"                               
            }
            return hashes[0]; // return the root node
             }

        // create a reward to incentivise the mining of block
        public Transaction createRewardTransaction(List<Transaction> transactions)
        {
            double fees = transactions.Aggregate(0.0, (acc, t) => acc + t.fee); // Sum all transaction fees
            return new Transaction("Mine Rewards", minerAddress, (reward + fees), 0, ""); // Issue reward as a transaction in the new block
        }

        /* combine all properties to output to the UI */
        public override string ToString()
        {
            return "[BLOCK START]"
                + "\nIndex: " + index
                + "\tTimestamp: " + timestamp
                + "\nPrevious Hash: " + prevHash
                + "\n-- PoW --"
                + "\nDifficulty Level: " + difficulty
                + "\nNonce: " + nonce
                + "\nHash: " + hash
                + "\nBalance: " + balance
                + "\n-- Rewards --"
                + "\nReward: " + reward
                + "\nMiners Address: " + minerAddress
                + "\n-- " + transactionList.Count + " Transactions --"
                + "\nMerkle Root: " + merkleRoot
                + "\n" + String.Join("\n", transactionList)
                + "\n[BLOCK END]";
        }
    }
}

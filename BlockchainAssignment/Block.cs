using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace BlockchainAssignment
{
    class Block
    {
        DateTime timestamp;  // For when the block is made
        public int index;  // Position of the block
        public String hash;  // The hash
        public String prevHash;  // The hash of the previous block

        public List<Transaction> transactionList = new List<Transaction>();  // List of the transactions
        public String merkleRoot;  // For the merkle root

        // Proof-of-work
        public long nonce = 0;  // Number only used once
        public int difficulty = 5;  // Difficulty setting
        public double benchmark = 20.0; // Benchmark timestamp

        //Rewards and fees
        public double reward = 20.0; // The reward 
        public double fees = 0.2;  // The fee

        public String minerAddress = String.Empty;  // The miner address

        public Block()  // Used to create the genesis block
        {
            timestamp = DateTime.Now;  // Sets the timestamp
            index = 0;  // Starts the index at 0
            prevHash = String.Empty;  // No previous hash
            reward = 0;  // Reward for genesis block is 0
            hash = Mine();  // Mines for a new hash
        }

        public Block(String prevHash, int index)  // Constructor with previous hash and current index
        {
            timestamp = DateTime.Now;  // Sets the timestamp to the current time
            this.prevHash = prevHash;  // Sets the previous hash
            this.index = index + 1;  // Creates a new index
            hash = CreateHash();  // Creates a new hash
        }

        public Block(Block lastBlock, List<Transaction> transactions, String address = "")  // Constructor using transactions
        {
            timestamp = DateTime.Now;  // Takes a time stamp
            prevHash = lastBlock.hash;  // Gets the previous hash
            index = lastBlock.index + 1;  // Creates a new index

            minerAddress = address;  // Address for the miner address

            transactions.Add(CreateRewardTransaction(transactions));  // Creates a new transaction
            transactionList = transactions;  // Sets the variable transactionList to the transactions

            merkleRoot = MerkleRoot(transactionList);  // Uses the merkle root

            hash = Mine();  // Mines
        }

        public Transaction CreateRewardTransaction(List<Transaction> transactions)
        {
            // Sum the fees in the list of transactions in the mined block
            fees = transactions.Aggregate(0.0, (acc, t) => acc + t.fee);

            // Create the "Reward Transaction" being the sum of fees and reward being transferred from "Mine Rewards" (Coin Base) to miner
            return new Transaction("Mine Rewards", minerAddress, (reward + fees), 0, "");
        }

        public String CreateHash()  // Used for creating the hash
        {
            String hash = String.Empty;  // Empties the string
            SHA256 hasher = SHA256Managed.Create();  // Creates the hasher

            String input = index.ToString() + timestamp.ToString() + prevHash + nonce.ToString() + reward.ToString() + merkleRoot;  //Concatenate all Block properties for hashing
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));  // Ceates the hashbyte
            foreach(byte x in hashByte)  // Loops over all of the bytes
            {
                hash += String.Format("{0:x2}", x);  // Formats the string
            }

            return hash;  // Returns the hash
        }

        public String Mine()
        {
            Stopwatch stWatch  = new Stopwatch();  // Starts a new stopwatch
            stWatch.Start();  // Starts the stopwatch
            Thread th = new Thread(threadedMine);  // Threads it temporarily
            th.Start();  // Starts the thread  
            threadedMine();  // Calls the method to race
            th.Join();  // Stops the code runnin before it is finished
            stWatch.Stop();  // Stops the stopwatch
            TimeSpan ts = stWatch.Elapsed;  // Gets the elapsed time
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("TotalTime " + elapsedTime);  // Prints to console the time
            if(ts.Seconds > benchmark)
            {
                difficulty--; // Difficulty goes down if the benchmark is smaller than the amount of time it took to get the hash
            }
            else if(ts.Seconds < benchmark / 2)
            {
                difficulty++; // Difficulty goes up
            }
            return CreateHash();  // Returns the hash
        }

        public void threadedMine()
        {
            //Difficulty critera definition
            String re = new string('0', difficulty); //Create a "regex" string of N (difficulty i.e. 4) 0's
            String hash = CreateHash();  // Creates a hash
            // Re-Hash until difficulty critera is met
            while (!hash.StartsWith(re))
            {
                nonce++; // Increment nonce
                hash = CreateHash();  // Creates a hash until difficulty satisfied
            }
        }

        public static String MerkleRoot(List<Transaction> transactionList)
        {
            List<String> hashes = transactionList.Select(t => t.hash).ToList();  // Sets up the hashes
            if(hashes.Count == 0)
            {
                return String.Empty;  // If there are no hashes, return empty
            }
            if(hashes.Count == 1)
            {
                return HashCode.HashTools.combineHash(hashes[0], hashes[0]);  // If there is only one hash, return that hash
            }
            while(hashes.Count != 1)  // Loops over all hashes
            {
                List<String> merkleLeaves = new List<String>();  // Creates a list
                for (int i=0; i<hashes.Count; i += 2)  // loops over all hashes
                {
                    if(i == hashes.Count - 1)
                    {
                        merkleLeaves.Add(HashCode.HashTools.combineHash(hashes[i], hashes[i]));  // Adds itself
                    }
                    else
                    {
                        merkleLeaves.Add(HashCode.HashTools.combineHash(hashes[i], hashes[i + 1]));  // Adds a combined hash
                    }
                }
                hashes = merkleLeaves;  // Sets the hashes
            }
            return hashes[0];  // Return the first hashes
        }


        public override string ToString()  // method for creating a string
        {
            String output = String.Empty;  // Empties the string
            transactionList.ForEach(t => output += (t.ToString() + "\n"));

            return "Index: " + index.ToString() +  //For the index
                "\nTimestamp: " + timestamp.ToString() +  //For the timestamp
                "\nPrevious Hash: " + prevHash +  //For the previous hash
                "\nHash: " + hash +   // For the current hash
                "\nNonce: " + nonce.ToString() +  // For the nonce
                "\nDifficulty: " + difficulty.ToString() +  // For the difficulty
                "\nReward: " + reward.ToString() +  // For the rewrds
                "\nFees: " + fees.ToString() +  // For the fees
                "\nMiner's Address: " + minerAddress +  // For the miners address
                "\nTransactions: " + transactionList.Count + "\n" + output;  // For the transactions
        }
    }
}

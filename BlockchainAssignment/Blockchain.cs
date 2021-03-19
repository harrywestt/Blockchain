using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    class Blockchain
    {
        public List<Block> Blocks = new List<Block>();  // List of blocks
        public List<Transaction> transactionPool = new List<Transaction>();  // List of transactions
        int transactionsPerBlock = 5;  // How many transactions can be in each block

        public Blockchain()
        {
            Blocks.Add(new Block());  // Will generate the genesis block
        }

        public String getBlockAsString(int index)  // Method for getting blocks as a string
        {
            return Blocks[index].ToString();  // Returns the string from the ToString method
        }

        public Block GetLastBlock()
        {
            return Blocks[Blocks.Count - 1];  // Gets the latest block in the chain
        }

        public List<Transaction> getPendingTransactions()  // Gets any pending transactions
        {
            int n = Math.Min(transactionsPerBlock, transactionPool.Count);  // Gets the amount of transactions 
            List<Transaction> transactions = transactionPool.GetRange(0, n);  // Sets up the transactions list
            transactionPool.RemoveRange(0, n);  // removes the set amount of transactions

            return transactions;  // returns the transactions
        }

        public override string ToString()  // ToString method
        {
            String output = String.Empty;  // Empties the current string
            Blocks.ForEach(b => output += (b.ToString() + "\n\n"));  // Runs through each block and adds them to the string
            return output;  // Returns the output
        }

        public double GetBalance(String address)
        {
            double balance = 0.0;  // Sets the balance to 0
            foreach(Block b in Blocks)  // For each block
            {
                foreach(Transaction t in b.transactionList)  // For each transaction in the block
                {
                    if (t.recipientAddress.Equals(address))
                    {
                        balance += t.amount;  // If they were the recipient, add to balance
                    }
                    if (t.senderAddress.Equals(address))
                    {
                        balance -= (t.amount + t.fee);  // If they were the sender, remove from balance
                    }
                }
            }

            return balance;  // Returns the balance
        }

        public bool validateMerkleRoot(Block b)  // For validating the merkle root
        {
            String reMerkle = Block.MerkleRoot(b.transactionList);  // merkle roots the transaction list
            return reMerkle.Equals(b.merkleRoot);  // returns the result
        }
    }
}

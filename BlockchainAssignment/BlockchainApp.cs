using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BlockchainAssignment
{
    public partial class BlockchainApp : Form
    {
        Blockchain blockchain;

        public BlockchainApp()
        {
            // Initialises the blockchain
            InitializeComponent();
            blockchain = new Blockchain();
            richTextBox1.Text = "New Blockchain Initialised!";  // Prints a message when the blockchain is initialised
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)  // For printing blocks
        {
            int index = 0;  // Sets the index to 0
            if (Int32.TryParse(textBox1.Text, out index))
            {
                richTextBox1.Text = blockchain.getBlockAsString(index);  // Sets the rich textbox to the emthod for creating a string out of the blocks
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String privKey;  // Sets up the private key
            Wallet.Wallet myNewWallet = new Wallet.Wallet(out privKey);  // Creates a wallet
            publicKey.Text = myNewWallet.publicID;  // Outputs the public key
            privateKey.Text = privKey;  // Outputs the private key
        }

        /* Validate Keys */
        private void button3_Click(object sender, EventArgs e)
        {
            if (Wallet.Wallet.ValidatePrivateKey(privateKey.Text, publicKey.Text))
            {
                richTextBox1.Text = "Keys are valid!";  // The keys are valid if the validate method comes back as true
            }
            else
            {
                richTextBox1.Text = "Keys are not valid";  // Else the keys are not valid
            }
        }

        private void createTransaction_Click(object sender, EventArgs e)
        {
            Transaction transaction = new Transaction(publicKey.Text, receiverKey.Text, Double.Parse(amount.Text), Double.Parse(fee.Text), privateKey.Text);  // Creates a transaction
            blockchain.transactionPool.Add(transaction);  // adds the transaction to the transaction pool
            richTextBox1.Text = transaction.ToString();  // Updates the rich text box
        }

        private void newBlock_Click(object sender, EventArgs e)
        {
            List<Transaction> transactions = blockchain.getPendingTransactions();  // Gets the pending transactions
            Block newBlock = new Block(blockchain.GetLastBlock(), transactions, publicKey.Text);  // Generates a new block
            blockchain.Blocks.Add(newBlock);  // Adds a new block to the blockchain

            richTextBox1.Text = "New block generated!";  // Outputs to the rich text box that a new block was generated
        }

        private void validateChain_Click(object sender, EventArgs e)
        {
            if (blockchain.Blocks.Count == 1) // If there is only one block
            {
                if (!blockchain.validateMerkleRoot(blockchain.Blocks[0]))
                {
                    richTextBox1.Text = "Blockchain is invalid";  // Blockchain is invalid if returns false
                }
                else
                {
                    richTextBox1.Text = "Blockchain is valid";  // Else is valid
                }
                return;
            }
            bool valid = true;  // Sets up valid
            for (int i = 1; i < blockchain.Blocks.Count - 1; i++)  // Loops over all blocks
            {
                if (blockchain.Blocks[i].prevHash != blockchain.Blocks[i - 1].hash || !blockchain.validateMerkleRoot(blockchain.Blocks[i]))
                {
                    richTextBox1.Text = "Blockchain is invalid";  // If the hashes dont match they are invalid
                    return;
                }
            }
            if (valid)
            {
                richTextBox1.Text = "Blockchain is valid";  //If it is valid then it is valid
            }
            else
            {
                richTextBox1.Text = "Blockchain is invalid";  // Else is invalid
            }
        }

        private void checkBalancce_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = blockchain.GetBalance(publicKey.Text).ToString() + " Assignment Coin";  // Checks the balance of the wallet
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = blockchain.ToString();  // Returns all of the blocks in a long string
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cabinet
{
    class User
    {
        private string privateHash;
        private string publicHash;
        private string keyHash;

        public static User user;
        public static Hashtable ipHash = new Hashtable();
        private List<char> alphabet = new List<char>();
        private List<char[]> table = new List<char[]>();

        public Node userTree = new Node();
        
        //string[] table
        public User(string publicHash, String privateHash)
        {
            this.privateHash = privateHash;
            this.publicHash = publicHash;
            userTree = new Node(publicHash, null);
            this.encrytionInit();
        }
        public void encrytionInit()
        {
            for (char c = '0'; c <= '9'; c++)
            {
                alphabet.Add(c);
            }
            for (char c = 'A'; c <= 'Z'; c++)
            {
                alphabet.Add(c);

            }
            for (char c = 'a'; c <= 'z'; c++)
            {
                alphabet.Add(c);

            }
            for (int i = 0; i < alphabet.ToArray().Length; i++)
            {
                char[] temp = new char[alphabet.ToArray().Length];
                int count = 0;
                for (int j = i; j < alphabet.ToArray().Length; j++)
                {
                    temp[count] = alphabet[j];
                    count++;
                }
                for (int j = 0; j < i; j++)
                {
                    temp[count] = alphabet[j];
                    count++;
                }
                table.Add(temp);
            }
        }
        public string encryptHash(string hash)
        {

            string result = "";
            string word = hash;
            string key = "";
            if (privateHash.Length > word.Length)
            {
                key = (this.privateHash).Substring(0, word.Length);
            }
            else if (privateHash.Length < word.Length)
            {
                int divisor = word.Length / privateHash.Length;
                for (int i = 0; i < divisor; i++)
                {
                    key += privateHash;
                }
            }
            else
            {
                key = privateHash;
            }
            for (int j = 0; j < word.Length; j++)
            {
                int num = 0;
                for (int i = 0; i < alphabet.ToArray().Length; i++)
                {
                    if (alphabet[i] == word.ToCharArray()[j])
                    {
                        num = i;
                    }
                }
                int num1 = 0;
                for (int i = 0; i < alphabet.ToArray().Length; i++)
                {
                    if (alphabet[i] == key.ToCharArray()[j])
                    {
                        num1 = i;
                    }
                }
                result += table[num][num1].ToString();
            }
            return result;


        }
        public string decryptHash(string hash)
        {
            string result = hash;
            string key = "";
            if (privateHash.Length > result.Length)
            {
                key = (this.privateHash).Substring(0, result.Length);
            }
            else if (privateHash.Length < result.Length)
            {
                int divisor = result.Length / privateHash.Length;
                for (int i = 0; i < divisor; i++)
                {
                    key += privateHash;
                }
            }
            else
            {
                key = privateHash;
            }
            string returnS = "";
            for (int i = 0; i < result.Length; i++)
            {
                int num = 0;
                for (int j = 0; j < alphabet.ToArray().Length; j++)
                {
                    if (alphabet[j] == key.ToCharArray()[i])
                    {
                        num = j;
                    }
                }
                int num1 = 0;
                for (int j = 0; j < table[num].Length; j++)
                {
                    if (table[num][j] == result.ToCharArray()[i])
                    {
                        num1 = j;
                    }
                }
                returnS += alphabet[num1];
            }
            return returnS;
        }
             
        public string getPrivateHash()
        {
            return privateHash;
        }
        public string getPublicHash()
        {
            return publicHash;
        }

    }
}

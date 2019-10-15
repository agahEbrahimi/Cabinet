using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cabinet
{
    class Node
    {
        private Hashtable connected = new Hashtable();
        private string refrenceHash;
        private string valueHash;

        public Node(string refrenceHash, string valueHash)
        {
            this.refrenceHash = refrenceHash;
            this.valueHash = valueHash;
        }
        public Node()
        {
            this.refrenceHash = null;
            this.valueHash = null;
        }

        public string getRefrenceHash()
        {
            return refrenceHash;
        }
        public string getValueHash()
        {
            return valueHash;
        }
        public void changeVal(string s)
        {
            valueHash = s;
        }
        public void changeRef(string s)
        {
            refrenceHash = s;
        }
        public void addNode(string refrenceHash, string valueHash)
        {
            if (valueHash ==("null"))
            {
                valueHash = null;
            }
            connected.Add(refrenceHash, new Node(refrenceHash, valueHash));
        }
        public Node getNode(string refrenceHash)
        {
            return (Node)connected[refrenceHash];
        }
        public Hashtable getConnected()
        {
            return connected;
        }
        public string toString()
        {
            if (valueHash == null || valueHash=="")
            {
                return "(" + refrenceHash + ", null)";

            }
            else {
                return "(" + refrenceHash + ", " + valueHash + ")";
            }
        }
    }
}

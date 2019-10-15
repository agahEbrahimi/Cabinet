using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cabinet
{
    class Tree : Hashtable
    {
        public static Tree globalTree = new Tree();
        public static Tree globalTreeCheck = new Tree();
        public static Boolean validateTree(Tree toCheck, string publicHash)
        {
            Boolean b = true;
            foreach (DictionaryEntry pair in toCheck)
            {
                Node hashDir = (Node)(pair.Value);
                foreach (DictionaryEntry pairs in hashDir.getConnected())
                {
                    Node hashS = (Node)((((Node)(pairs.Value))));
                    Node hashSub =(Node)((((Node)(pairs.Value)).getConnected())["Directory"]);
                    Node hashSubCheck = (Node)(((Node)(((Node)(Tree.globalTree[hashDir.getRefrenceHash()])).getConnected()[((Node)(pairs.Value)).getRefrenceHash()])).getConnected()["Directory"]);
                    if (hashSub.getValueHash() != hashSubCheck.getValueHash() && (hashDir.getRefrenceHash()!=publicHash)&&(hashS.getRefrenceHash()!=publicHash))
                    {
                        b = false;
                    }
                }
            }
            return b;
        }
        public static Tree readTree(string path, string txt, User use)
        {
            string[] content;
            Tree temp = new Tree();
            if (path != null)
            {
                content = System.IO.File.ReadAllText(path).Split('-');
            }
            else
            {
                content = txt.Split('-');
            }
            for (int i = 0; i < content.Length; i++)
            {
                string user = content[i].Substring(1, content[i].IndexOf(",") - 1);
                temp.Add(user, new Node(user, null));
                content[i] = content[i].Substring(content[i].IndexOf('[') + 1, content[i].Length - content[i].IndexOf('[') - 1);
                string[] subContent = content[i].Split('/');
                for (int j = 0; j < subContent.Length; j++)
                {
                    string subNode = subContent[j].Substring(1, subContent[j].IndexOf(",") - 1);
                    ((Node)temp[user]).addNode(subNode, null);
                    subContent[j] = subContent[j].Substring(subContent[j].IndexOf('[') + 1, subContent[j].Length - subContent[j].IndexOf('[') - 2);

                        if (subContent[j].Contains("."))
                        { 
                            string[] subSubContent = subContent[j].Split('.');
                            ((Node)temp[user]).getNode(subNode).addNode("Directory", subSubContent[0].Substring(subSubContent[0].IndexOf(','),subSubContent[0].LastIndexOf(')')- subSubContent[0].IndexOf(',')));
                            ((Node)temp[user]).getNode(subNode).addNode("Key", subSubContent[1].Substring(subSubContent[1].IndexOf(','), subSubContent[1].LastIndexOf(')') - subSubContent[1].IndexOf(',')));

                        }
                        else
                        {
                            ((Node)temp[user]).getNode(subNode).addNode("Directory", subContent[j].Substring(subContent[j].IndexOf(','), subContent[j].LastIndexOf(')') - subContent[j].IndexOf(',')));
                        }
                }
            }
            if (use != null)
            {
                use.userTree = (Node)temp[use.getPublicHash()];
            }
            return temp;
        }
        public static string exportTree()
        {
            string toAppend = "";
            foreach (DictionaryEntry pair in Tree.globalTree)
            {

                toAppend += ((Node)(pair.Value)).toString() + "[";
                int count1 = 0;
                foreach (DictionaryEntry nodePairs in ((Node)(pair.Value)).getConnected())
                {
                    toAppend += ((Node)nodePairs.Value).toString()+"[";
                    int count = 0;
                    foreach (DictionaryEntry node in ((Node)(nodePairs.Value)).getConnected())
                    {
                        if (count > 0)
                        {
                            toAppend += ".";
                        }
                        toAppend += ((Node)(node.Value)).toString();
                        count++;
                    }
                    toAppend += "]/";
                }
                toAppend = toAppend.Substring(0, toAppend.Length - 1);
                toAppend += "]-"; 
            }
            toAppend = toAppend.Substring(0, toAppend.Length - 1);
            return toAppend;
        }
    }
}

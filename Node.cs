using System;
using System.Collections.Generic;

namespace  DCasm {
    public abstract class Node {
        private List<Node> childrens;
        public List<Node> Childrens { get => childrens; set => childrens = value; }

        public Node() {
            Childrens = new List<Node>();
        }

        public Node(Node child1) {
            Childrens = new List<Node>();
            Childrens.Add(child1);
        }

        public Node(Node child1, Node child2) {
            Childrens = new List<Node>();
            Childrens.Add(child1);
            Childrens.Add(child2);
        }


        public abstract void Accept(Visitor v);
    }
}
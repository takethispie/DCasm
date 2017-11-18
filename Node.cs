using System;
using System.Collections.Generic;

namespace  DCasm {
    public class Node {
        List<Node> childrens;

        public Node() {
            childrens = new List<Node>();
        }

        public Node(Node child1) {
            childrens = new List<Node>();
            childrens.Add(child1);
        }

        public Node(Node child1, Node child2) {
            childrens = new List<Node>();
            childrens.Add(child1);
            childrens.Add(child2);
        }

        public virtual void accept(Visitor v) {}
    }
}
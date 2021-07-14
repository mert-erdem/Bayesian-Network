using System;
using System.Collections.Generic;
using System.Text;

namespace Bayesian_Network
{
    //bayes ağındaki node ve gerekli özellikleri;
    class Attribute
    {
        public string name;
        public int index;
        public List<string> values;
        public Attribute[] parents;
        public Dictionary<string, Dictionary<string, double>> cpt = new Dictionary<string, Dictionary<string, double>>();

        public Attribute(string name, int index, List<string> values, Attribute[] parents)
        {
            this.name = name;
            this.index = index;
            this.values = values;
            this.parents = parents;
        }
    }
}

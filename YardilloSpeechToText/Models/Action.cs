using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBADCases.Models
{
    public class Action
    {

        public Action()
        {
            Adapterresponses = new List<Adapterresponse>();
            Fields = new List<Casetypefield>();
        }
        public int Actionseq { get; set; }
        public bool Actioncomplete { get; set; }
        public string Actionname { get; set; }
        public string Actionstatus { get; set; }
        //public string Actionparentid { get; set; }
        //public string Actionparentresponse { get; set; }
        public bool Isdisabled { get; set; }
        public string Actionid { get; set; }
        public string Activityid { get; set; }
        public string Caseid { get; set; }
        public string Actiontype { get; set; }
        public Actionauth Actionauth { get; set; }
        public List<Adapterresponsemap> Adapterresponsemaps { get; set; }
        public List<Adapterresponse> Adapterresponses { get; set; }
        public List<Casetypefield> Fields { get; set; }
        public int Activityseq { get; set; }

        // Default comparer for Part type.
        public int CompareTo(Action compareSeq)
        {
            // A null value means that this object is greater.
            if (compareSeq == null)
                return 1;
            else
                return this.Actionseq.CompareTo(compareSeq.Actionseq);
        }
    }
    public class Actionauth
    {
        public string Fieldid { get; set; }
        public string ValueX { get; set; }
        public string Type { get; set; }
        public string Operator { get; set; }
        public string ValueY { get; set; }
        public bool Defaultreturn { get; set; }
        public bool Returniftrue { get; set; }
        public bool Returniffalse { get; set; }
    }
}

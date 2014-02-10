using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CallForm.Core.Models
{
    public class ReasonCode
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

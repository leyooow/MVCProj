using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MVCProj.Models
{
    public class Pdf
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}
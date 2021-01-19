using liftmarket.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace liftmarket.Models
{
    public class Rct : BaseModel
    {
        public Hammadde Hammadde { get; set; }
        public int HammaddeId { get; set; }
        public Urun Urun { get; set; }
        public int UrunId { get; set; }
        public int NeKadar { get; set; }

    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Dto
{
    // for HOTEOAS
    public class Links
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class DefectCodeDto
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Group1 { get; set; }

        public string Group2 { get; set; }

        public string Group3 { get; set; }

        public int Grade { get; set; } = 5!;

        public string Note { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Options
{
    public class EmailOptions
    {
        public string EmailHost { get; set; }
        public int EmailPort { get; set; }
        public string EmailUsername { get; set; }
        public string EmailPassword { get; set; }
    }

}

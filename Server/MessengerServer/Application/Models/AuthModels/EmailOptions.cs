using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.AuthModels
{
    public class EmailOptions
    {
        public string email { get; set; }

        public string password { get; set; }

        public string from { get; set; }

        public string confirmlink { get; set; }

        public string subject { get; set; }

        public string message { get; set; }
    }
}

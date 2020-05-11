using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DanielBogdan.Passwordless.Identity.Email
{
    public static class TemplateConstants
    {
        //Templates path
        public const string TemplatesPath = @"Email\Templates";

        //Hardcode templates
        public const string Passwordless = "Passwordless.html";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication11
{
    public class PluggableComponent
    {
        private int _count = 0;

        public string Echo()
        {
            _count++;
            return $"Echo echo echo echo...({_count})";
        }
    }
}
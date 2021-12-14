using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZeeUtils
{
   
    public class ComponentModelValidationError : IComponentModelValidationError
    {
        public string Name { get; set; }
        public string ErrorMessage { get; set; }

        public int Priority { get; set; }

        public ComponentModelValidationError(string name, string message,int priority)
        {
            Name = name;
            ErrorMessage = message;
            Priority = priority;
        }

    }

}

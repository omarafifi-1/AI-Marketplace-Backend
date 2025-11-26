using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AI_Marketplace.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public NotFoundException() : base("The requested resource was not found.")
        {
            Errors = new Dictionary<string, string[]>();
        }
        public NotFoundException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>
            {
                { "Content", new[] { message } }
            };
        }
        
        public NotFoundException(IDictionary<string, string[]> errors) : base("The requested resource was not found.")
        {
            Errors = errors;
        }
    }
}

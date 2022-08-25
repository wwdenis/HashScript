using System;
using System.Collections.Generic;
using System.Linq;
using HashScript.Providers;

namespace HashScript.Samples
{
    class ObjectSamples
    {
        static readonly Dictionary<string, object> Samples = new Dictionary<string, object>
        {
            {
                @"#+Items# #.# #!.Last# >#!# #+#",
                new
                {
                    Items = new[] { 1, 2, 3 },
                }
            },
            {
                @"class #Name# 
{
#+Columns#
    public #Type# #Name#
    {
        get;
        #!ReadOnly#
        set;
        #!#
    }

#+#
}",
                new
                {
                    Name = "Customer",
                    Columns = new[]
                    {
                        new { Name = "Id", Type = "int", ReadOnly = true, },
                        new { Name = "Name", Type = "Text", ReadOnly = false, },
                        new { Name = "BirthDate", Type = "DateTime", ReadOnly = false },
                    }
                }
            }
        };

        public static IDictionary<string, IValueProvider> GetAll()
        {
            return Samples.ToDictionary(k => k.Key, v => (IValueProvider)new ObjectValueProvider(v.Value));
        }
    }
}

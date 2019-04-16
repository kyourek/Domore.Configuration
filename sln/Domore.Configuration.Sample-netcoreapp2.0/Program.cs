﻿using System;

namespace Domore.Configuration.Sample {
    internal class Program {
        private static void Main(string[] args) {
            var conf = (IConfigurationContainer)new ConfigurationContainer();
            var resp = conf.Configure(new Response());
            Console.WriteLine(resp.WhatToSay);
        }

        private class Response {
            public string WhatToSay { get; set; }
        }
    }
}

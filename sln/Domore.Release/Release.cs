﻿using System;
using System.Linq;

namespace Domore {
    using Configuration;
    using ReleaseActions;

    internal class Release {
        public Release(string[] args) {
            string arg(string name) => args?
                .FirstOrDefault(a => a.StartsWith("-" + name + "="))?
                .Split('=')[1];

            try {
                var stage = arg("Stage");
                var release = arg("Release") == "yes";
                if (release == false && string.IsNullOrWhiteSpace(stage)) {
                    throw new InvalidOperationException("Must either stage or release");
                }

                var solutionDirectory = arg("SolutionDirectory");
                var configuration = (IConfigurationContainer)new AppConfigContainer();
                var context = new ReleaseContext();
                var actions = new ReleaseAction[] {
                    new Bump(),
                    new Build(),
                    new Pack(),
                    new Tag(),
                    new Push()
                };

                foreach (var action in actions) {
                    configuration.Configure(action, "ReleaseAction");
                    configuration.Configure(action);

                    if (arg(action.GetType().Name) != "no") {
                        action.Context = context;
                        action.Log = s => Console.WriteLine(s);
                        action.Stage = stage;
                        action.SolutionDirectory = solutionDirectory;
                        action.Work();
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }

            if (arg("Debug") == "yes") {
                Console.ReadLine();
            }
        }
    }
}
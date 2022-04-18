using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Helpers
{
    public static class TaskRunnerHelper
    {
        public static async Task RunDelegates(Dictionary<string, List<Func<Task>>> dict, bool runParallel = true)
        {
            if (runParallel)
                await RunDelegatesParallel(dict);
            else
                await RunDelegatesSerial(dict);
        }

        public static async Task RunDelegatesParallel(Dictionary<string, List<Func<Task>>> dict)
        {
            foreach (string key in dict.Keys)
            {
                List<Task> results = new List<Task>();
                foreach (Func<Task> funcs in dict[key])
                {
                    results.Add(funcs.Invoke());
                }
                await Task.WhenAll(results.ToArray());
            }
        }

        public static async Task RunDelegatesSerial(Dictionary<string, List<Func<Task>>> dict)
        {
            foreach (string key in dict.Keys)
                foreach (Func<Task> funcs in dict[key])
                    await funcs.Invoke();
        }
    }
}

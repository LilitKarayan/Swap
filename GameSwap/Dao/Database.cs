using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Models
{
    public class Database
    {
        protected Database() { }

        public static void Init()
        {
            DotNetEnv.Env.Load(Path.Combine(Environment.CurrentDirectory, ".env"));
        }

        public static string ConnectionStr { get; private set; } = Environment.GetEnvironmentVariable("CONN_STR");
    }
}

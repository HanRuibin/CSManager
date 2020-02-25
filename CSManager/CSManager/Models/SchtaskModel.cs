using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSManager.Models
{
    public class SchtaskModel
    {
        /// <summary>
        /// arg /tn
        /// </summary>
        public string TaskName { get; set; }
        public string TaskCommand { get; set; }
        /// <summary>
        /// create/query/run
        /// </summary>
        public string TaskMode { get; set; }
        /// <summary>
        /// arg /tr
        /// </summary>
        public string TaskPath { get; set; }
        public SchtaskStatus TaskStatus { get; set; }
    }
    public enum SchtaskStatus
    {
        Normal=0,
        NotExist=1,
        Created=2,
        Exist=3,
        Failed=4,
        Executed=5
    }
}

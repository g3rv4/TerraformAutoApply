using System.Runtime.Serialization;

namespace AutoApply.Models
{
    public class TerraformNotification
    {
        [DataMember(Name = "run_id")]
        public string RunId { get; set; }

        public Notification[] Notifications { get; set; }

        public class Notification
        {
            public string Trigger { get; set; }
        }
    }
}
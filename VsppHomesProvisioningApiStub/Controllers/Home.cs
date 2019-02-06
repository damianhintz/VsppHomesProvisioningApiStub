using System.ComponentModel.DataAnnotations;

namespace VsppHomesProvisioningApiStub.Controllers
{
    public class Home
    {
        [Required]
        public string HomeId { get; set; }

        [Required]
        public string GeoId { get; set; }

        [Required]
        public long Quota { get; set; }
        
        public short? QuotaType { get; set; }
        
        public short? RecordingType { get; set; }
    
    }
}
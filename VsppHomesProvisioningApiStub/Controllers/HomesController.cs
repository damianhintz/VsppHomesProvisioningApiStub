using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;

namespace VsppHomesProvisioningApiStub.Controllers
{
    public class HomesController : ApiController
    {
        public class ProvisionHome
        {
            [Required]
            public string HomeId { get; set; }

            [Required]
            public string GeoId { get; set; }

            [Required]
            public long Quota { get; set; }
        
            public short QuotaType { get; set; }
        
            public short Type { get; set; }
    
        }

        private static readonly List<ProvisionHome> m_Homes = new List<ProvisionHome>();

        [HttpPost]
        public async Task<HttpResponseMessage> PostProvisionHome(HttpRequestMessage request)
        {
            if (true)
            {
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "");
                throw new HttpResponseException(errorResponse);
            }

            var data = await request.Content.ReadAsStringAsync();
            var document = XDocument.Parse(data);
            var provisionHomeElement = document.Root;
            var homeId = provisionHomeElement.Attribute("HomeID").Value;
            var geoId = provisionHomeElement.Attribute("GeoID").Value;
            var quota = provisionHomeElement.Attribute("Quota").Value;
            var quotaType = provisionHomeElement.Attribute("QuotaType")?.Value;
            var type = provisionHomeElement.Attribute("Type")?.Value;
            var provisionHome = new ProvisionHome
            {
                HomeId = homeId,
                GeoId = geoId,
                Quota = long.Parse(quota),
                QuotaType = short.Parse(quotaType),
                Type = short.Parse(type)
            };
            m_Homes.Add(provisionHome);

            return new HttpResponseMessage
            {
                Content = new StringContent(
                    @"<ProvisionHomeReply ManagerName=""Man1"" ManagerVIP=""192.168.5.6:5673"" />",
                    Encoding.UTF8,
                    "text/xml"
                )
            };
        }

        // GET api/homes
        public IEnumerable<string> Get()
        {
            return m_Homes.Select(home => home.HomeId);
        }

        // GET api/homes/5
        public ProvisionHome Get(string id)
        {
            return m_Homes.FirstOrDefault(h => h.HomeId == id);
        }
        
        // PUT api/homes/5
        public void Put(string id, [FromBody]string value)
        {
        }

        // DELETE api/homes/5
        [HttpDelete]
        public IHttpActionResult DeleteDeprovisionHome(string id)
        {
            var home = m_Homes.FirstOrDefault(h => h.HomeId == id);
            if (home == null) return StatusCode((HttpStatusCode)471);
            
            m_Homes.Remove(home);
            return Ok(); //HttpResponseMessage
        }
    }
}

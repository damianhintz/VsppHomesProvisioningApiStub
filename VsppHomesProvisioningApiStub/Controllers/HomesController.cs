using System;
using System.Collections.Generic;
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
        private static readonly List<Home> Homes = new List<Home>
        {
            new Home
            {
                HomeId = "HomeIdAlreadyProvisioned",
                GeoId = "ProvisionedGeoId"
            }
        };

        [HttpPost]
        public async Task<HttpResponseMessage> ProvisionHome(HttpRequestMessage request)
        {
            var data = await request.Content.ReadAsStringAsync();
            var document = XDocument.Parse(data);
            var provisionHomeElement = document.Root;
            
            if (provisionHomeElement == null)
            {
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No root element");
                throw new HttpResponseException(errorResponse);
            }

            var homeIdAttribute = provisionHomeElement.Attribute("HomeID");
            if (homeIdAttribute == null)
            {
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "HomeID is required");
                throw new HttpResponseException(errorResponse);
            }
            
            if (homeIdAttribute.Value.Equals("BadHomeId"))
            {
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "HomeID is invalid");
                throw new HttpResponseException(errorResponse);
            }

            if (Homes.Any(h => h.HomeId.Equals(homeIdAttribute.Value)))
            {
                var errorResponse = Request.CreateErrorResponse((HttpStatusCode)478, "HomeID already provisioned");
                throw new HttpResponseException(errorResponse);
            }
            
            var geoIdAttribute = provisionHomeElement.Attribute("GeoID");
            if (geoIdAttribute == null)
            {
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "GeoID is required");
                throw new HttpResponseException(errorResponse);
            }
            
            if (geoIdAttribute.Value.Equals("BadGeoId"))
            {
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "GeoID is invalid");
                throw new HttpResponseException(errorResponse);
            }

            if (geoIdAttribute.Value.Equals("UnknownGeoId"))
            {
                var errorResponse = Request.CreateErrorResponse((HttpStatusCode)476, "Unknown GeoID");
                throw new HttpResponseException(errorResponse);
            }

            var quotaAttribute = provisionHomeElement.Attribute("Quota");
            if (quotaAttribute == null)
            {
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Quota is required");
                throw new HttpResponseException(errorResponse);
            }

            if (!long.TryParse(quotaAttribute.Value, out var quota))
            {
                var errorResponse = Request.CreateErrorResponse((HttpStatusCode)470, "Argument Validation Error");
                throw new HttpResponseException(errorResponse);
            }

            if (quota < 0)
            {
                var errorResponse = Request.CreateErrorResponse((HttpStatusCode)470, "Argument Validation Error");
                throw new HttpResponseException(errorResponse);
            }

            var quotaType = provisionHomeElement.Attribute("QuotaType")?.Value ?? "1";

            var recordingType = provisionHomeElement.Attribute("Type")?.Value ?? "3";

            var provisionHome = new Home
            {
                HomeId = homeIdAttribute.Value,
                GeoId = geoIdAttribute.Value,
                Quota = quota,
                QuotaType = short.Parse(quotaType),
                RecordingType = short.Parse(recordingType)
            };

            Homes.Add(provisionHome);

            return new HttpResponseMessage
            {
                Content = new StringContent(
                    @"<ProvisionHomeReply ManagerName=""StubManager"" ManagerVIP=""localhost:1234"" />",
                    Encoding.UTF8,
                    "text/xml"
                )
            };
        }

        // GET api/homes
        public IEnumerable<Home> Get()
        {
            return Homes;
        }

        // GET api/homes/5
        public Home Get(string id)
        {
            return Homes.FirstOrDefault(h => h.HomeId == id);
        }
        
        // PUT api/homes/5
        public void Put(string id, [FromBody]string value)
        {
        }
        
        [HttpDelete]
        public IHttpActionResult DeprovisionHome(string id)
        {
            var home = Homes.FirstOrDefault(h => h.HomeId == id);
            if (home == null) return StatusCode((HttpStatusCode)471);
            
            Homes.Remove(home);

            return Ok();
        }
    }
}

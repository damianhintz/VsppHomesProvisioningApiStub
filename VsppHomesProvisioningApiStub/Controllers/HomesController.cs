using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, Home> Homes = new ConcurrentDictionary<string, Home>();

        public HomesController()
        {
            var value = new Home
            {
                HomeId = "HomeIdAlreadyProvisioned",
                GeoId = "ProvisionedGeoId"
            };
            Homes.TryAdd("HomeIdAlreadyProvisioned", value);
        }

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

            if (Homes.ContainsKey(homeIdAttribute.Value))
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

            Homes.TryAdd(provisionHome.HomeId, provisionHome);

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
            return Homes.Values;
        }

        // GET api/homes/5
        public Home Get(string id)
        {
            Homes.TryGetValue(id, out var home);
            return home;
        }

        [HttpDelete]
        public IHttpActionResult DeprovisionHome(string id)
        {
            if (Homes.TryGetValue(id, out var home))
            {
                return Ok();
            }

            return StatusCode((HttpStatusCode)471);
        }
    }
}

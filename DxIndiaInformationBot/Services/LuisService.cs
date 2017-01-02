using DxIndiaInformationBot.Models;
using Microsoft.ApplicationInsights;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DxIndiaInformationBot.Services
{
    public class LuisService
    {
        TelemetryClient telemetry = new TelemetryClient();

        private string errorCode = string.Empty;
        private string errorDescription = string.Empty;
        

        public string ErrorDescription
        {
            get
            {
                return errorDescription;
            }

            set
            {
                errorDescription = value;
            }
        }

        public string ErrorCode
        {
            get
            {
                return errorCode;
            }

            set
            {
                errorCode = value;
            }
        }


        public async Task<LuisResponse> CaptureIntent(string luisUrl, string command)
        {
            LuisResponse resp = null;
            try
            {
                using (var client = new HttpClient())
                {
                    // HttpResponseMessage response = await client.GetAsync("&q=get blueprints authored by srikantan");
                    telemetry.TrackTrace("Executing Luis request at> " + luisUrl + command);
                    HttpResponseMessage response = await client.GetAsync(luisUrl + command);
                    if (response.IsSuccessStatusCode)
                    {
                        resp = await response.Content.ReadAsAsync<LuisResponse>();
                        telemetry.TrackTrace("Luis response obtained .... ");

                    }
                    else
                    {
                        errorCode = response.StatusCode.ToString();
                        errorDescription = response.ReasonPhrase;
                        telemetry.TrackTrace("Luis response error code "+ errorCode +" ,\n reason phrase :"+ errorDescription);
                    }

                    if (resp.intents.Count > 0)
                    {
                        if ("None".Equals(resp.intents[0].intent))
                        {
                            errorCode = "Sorry, I could not interpret your question." + 
                                ServiceConstants.ERROR_INTERPRETING_INPUT;
                            errorDescription = "Sorry, I could not interpret your question. Please review";
                            telemetry.TrackTrace("No intent returned from Luis" + errorCode +
                                " , reason phrase :\n" + errorDescription);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ex.Message;
                errorDescription = ex.StackTrace;
                telemetry.TrackTrace("Exception executing Luis" + errorCode + " , Stack Trace :\n" + errorDescription);
            }
            return resp;
        }
    }
}

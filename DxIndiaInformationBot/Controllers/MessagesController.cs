using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using DxIndiaInformationBot.Services;
using DxIndiaInformationBot.Models;
using Microsoft.ApplicationInsights;

namespace DxIndiaInformationBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private TelemetryClient telemetry = new TelemetryClient();
        private ServiceParameters parameters = new ServiceParameters();
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            initializeServiceParameters();
            string result = string.Empty;

            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                string command = "&q=" + activity.Text;

                //telemetry.TrackTrace("Luis request received (" + command + ")");

                LuisService obj = new LuisService();
                LuisResponse l_response = await obj.CaptureIntent(parameters.Luiserviceurl, command);
                telemetry.TrackTrace("The intent captured for query (" + command + ") is " + l_response.intents[0].intent);


                if (!(string.IsNullOrEmpty(obj.ErrorCode)))
                {
                    //return the error Message back to the caller
                    result = obj.ErrorCode;
                    telemetry.TrackTrace("Luis returned an error" + obj.ErrorCode + ", executing this command "+command);

                }
                else
                {
                    telemetry.TrackTrace("Calling Azure Search ...");

                    // Now call the Azure Search Service
                    ServiceIntegrationClient client = new ServiceIntegrationClient(parameters.Searchservicenamespace,
                        parameters.Searchservicekey, parameters.Azureaccountsindexname,parameters.Searchservicenamespacedocs,
                        parameters.Searchservicedocskey,parameters.Blueprintindexname);
                    result = client.ExecuteSearch(l_response);
                }

                // return our reply to the user
                //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                Activity reply = activity.CreateReply(result);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private void initializeServiceParameters()
        {
            foreach (string key in System.Configuration.ConfigurationManager.AppSettings.Keys)
            {
                switch (key)
                {
                    case "luisserviceurl":
                        {
                            parameters.Luiserviceurl = System.Configuration.ConfigurationManager.AppSettings[key];
                            if(string.IsNullOrEmpty(parameters.Luiserviceurl))
                            {
                                parameters.Luiserviceurl  = "https://api.projectoxford.ai/luis/v1/application?id=b519ad78-a9b9-4708-b6bf-aa6f8d332259&subscription-key=84f8545836a449d0bb09b2aaf68ab417";
                            }
                            break;
                        }
                    case "indexnameblueprints":
                        {
                            parameters.Blueprintindexname = System.Configuration.ConfigurationManager.AppSettings[key];
                            break;
                        }
                    case "indexnameazureaccounts":
                        {
                            parameters.Azureaccountsindexname = System.Configuration.ConfigurationManager.AppSettings[key];
                            break;
                        }
                    case "searchservicens":
                        {
                            parameters.Searchservicenamespace = System.Configuration.ConfigurationManager.AppSettings[key];
                            break;
                        }
                    case "searchapikey":
                        {
                            parameters.Searchservicekey = System.Configuration.ConfigurationManager.AppSettings[key];
                            break;
                        }
                    case "searchservicedocsns":
                        {
                            parameters.Searchservicenamespacedocs = System.Configuration.ConfigurationManager.AppSettings[key];
                            break;
                        }
                    case "searchservicedocsapikey":
                        {
                            parameters.Searchservicedocskey = System.Configuration.ConfigurationManager.AppSettings[key];
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

            }
            //telemetry.TrackTrace("** Application Parameters - The Lui Service url: " + parameters.Luiserviceurl+ " , Index Name for Blueprints "+
            //     parameters.Blueprintindexname+", Index Name for Azure Accounts "+ parameters.Azureaccountsindexname+
            //     " , Service Name space for Blueprints "+parameters.Searchservicenamespacedocs+" , Service namespace for Azure Accounts "+parameters.Searchservicenamespace+
            //     " , API Key for Search Service Blueprints "+parameters.Searchservicedocskey+" , API Key for Search Azure Accounts "+parameters.Searchservicekey);

        }

    }
}
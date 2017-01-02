using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using DxIndiaInformationBot.Models;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;

namespace DxIndiaInformationBot.Services
{
    public class SearchService
    {
        TelemetryClient telemetry = new TelemetryClient();

        private string errorCode;
        private string errorDescription;

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

        public string ExecuteSearch(string command, string filter, string indexName,
            string searchServiceName, string apiKey, bool countonly, bool searchModeAll)
        {
            telemetry.TrackTrace("Azure Search request received >> " + command);
            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            SearchIndexClient indexClient = serviceClient.Indexes.GetClient(indexName);
            return SearchDocuments(indexClient, countonly, searchModeAll, command, filter);
        }

        private string SearchDocuments(SearchIndexClient indexClient, bool countonly, bool searchModeAll,string searchText, 
            string filter = null)
        {
            string searchResponse = string.Empty;
            int counter = 1;
            try
            {
                // Execute search based on search text and optional filter
                var sp = new SearchParameters();
                if (!String.IsNullOrEmpty(filter))
                {
                    sp.Filter = filter;
                }
                if (searchModeAll)
                {
                    sp.QueryType = QueryType.Full;
                    sp.SearchMode = SearchMode.All;
                }
                else
                {
                    sp.QueryType = QueryType.Full;
                    sp.SearchMode = SearchMode.Any;
                }
                sp.Top = 500;
                telemetry.TrackTrace("Executing Azure Search request ......");

                DocumentSearchResult<BlueprintsSearchResponse> response = indexClient.Documents.Search<BlueprintsSearchResponse>(searchText, sp);

                if(countonly)
                {
                    searchResponse = response.Results.Count +" documents found..";
                    return searchResponse;
                }
                if (response.Results.Count == 0)
                {
                    errorCode = "Sorry, I did not find anything matching this criteria";
                    errorDescription = "No documents found matching the criteria. Please review the criteria and resubmit";
                    telemetry.TrackTrace("No document found in Azure Search matching the critieria ......");
                    return searchResponse;
                }
                searchResponse += response.Results.Count + " records found \n\n";
                foreach (SearchResult<BlueprintsSearchResponse> result in response.Results)
                {
                   searchResponse += counter+ ". authored by **"+ result.Document.author+"**"+
                   " , Link: [" + result.Document.title + "]("+ result.Document.fileurl + ") \n";
                    counter++;
                }
                telemetry.TrackTrace("Azure Search response :"+searchResponse);
            }
            catch (Exception ex)
            {
                errorCode = ex.Message;
                errorDescription = ex.StackTrace;
                telemetry.TrackTrace("Exception executing Azure Search" + errorCode + " , Stack Trace :\n" + errorDescription);
            }
            return searchResponse;
        }
    }
}

using DxIndiaInformationBot.Models;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DxIndiaInformationBot.Services
{
    public class ISVAccountsService
    {
        private TelemetryClient telemetry = new TelemetryClient();
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


        public string GetAzureAccountMetadata(string indexName, string searchServiceName, string apiKey,
            bool anyOrAllFlag, string command, string filter = null)
        {
            string searchResponse = string.Empty;
            int counter = 1;
            IList<SearchResult<ISVAccountsAzureResponse>> response = null;
            try
            {

                response = SearchDocuments(indexName, searchServiceName, apiKey, anyOrAllFlag, command, filter);

            }
            catch (Exception ex)
            {
                errorCode = ex.Message;
                errorDescription = ex.StackTrace;
                searchResponse = "Error retrieving Azure Accounts information :" + ex.Message;
                return searchResponse;
                //ToDo: Log the detailed error description and return only the Message code to the caller
            }
            if (response.Count == 0)
            {
                errorCode = "Sorry, I could not find any Accounts matching the criteria";
                errorDescription = "No Accounts found matching the criteria. Please review the criteria and resubmit";
                return searchResponse;
            }
            searchResponse += "Found **" + response.Count + "** Azure Account(s) in all ..\n\n";

            IEnumerable<IGrouping<string, SearchResult<ISVAccountsAzureResponse>>> groupedAccounts = response.GroupBy(x => x.Document.EngagementStatus);
            foreach (IGrouping<string, SearchResult<ISVAccountsAzureResponse>> eachGroup in groupedAccounts)
            {
                counter = 1;
                searchResponse += "These are the "+ eachGroup.Count<SearchResult<ISVAccountsAzureResponse>>()+ " "+eachGroup.Key+ " Accounts \n\n";
                foreach(SearchResult<ISVAccountsAzureResponse> result in eachGroup)
                {
                   //int ttt= eachGroup.Count<SearchResult<ISVAccountsAzureResponse>>();
                    searchResponse += (counter) + ". **" + result.Document.ISV + "**" +
                  ", Account Type: **" + result.Document.AccountType + "**, PBE :**" + result.Document.PBE +
                  "**, TE :**" + result.Document.TE + "**, # wins: **" + result.Document.WinStatus + "**\n\n";
                    counter++;
                }
            }
            //foreach (SearchResult<ISVAccountsAzureResponse> result in response)
            //{
            //    searchResponse += (counter ) + ". **" + result.Document.ISV + "**" +
            //        ", Account Type: **" + result.Document.AccountType + "**, PBE :**" + result.Document.PBE +
            //        "**, TE :**" + result.Document.TE + "**, # wins: **" + result.Document.WinStatus + "** , Status :**"+ result.Document.EngagementStatus+"**\n\n";
            //    counter++;
            //}
            return searchResponse;

        }

        public string GetAssignedPBE(string indexName, string searchServiceName, string apiKey,
           string command, string filter = null)
        {
            string searchResponse = string.Empty;
            IList<SearchResult<ISVAccountsAzureResponse>> response = null;
            try
            {
                response = SearchDocuments(indexName, searchServiceName, apiKey, true,command, filter);
            }
            catch (Exception ex)
            {
                errorCode = ex.Message;
                errorDescription = ex.StackTrace;
                searchResponse = "Error retrieving PBE assignment to the Account:" + ex.Message;
                return searchResponse;
                //ToDo: Log the detailed error description and return only the Message code to the caller
            }
            if (response.Count == 0)
            {
                errorCode = "No PBE found matching the criteria";
                errorDescription = "No PBE found matching the criteria. Please review the criteria and resubmit";
                return searchResponse;
            }
            searchResponse += "PBE(s) Assigned to the Account(s) is/are \n\n";
            foreach (SearchResult<ISVAccountsAzureResponse> result in response)
            {
                searchResponse += "**"+result.Document.PBE + "** is the PBE for **"+ result.Document.ISV+"** \n\n";
            }
            return searchResponse;

        }

        public string GetAssignedTE(string indexName, string searchServiceName, string apiKey,
         string command, string filter = null)
        {
            string searchResponse = string.Empty;
            IList<SearchResult<ISVAccountsAzureResponse>> response = null;
            try
            {
                response = SearchDocuments(indexName, searchServiceName, apiKey, true,command, filter);
            }
            catch (Exception ex)
            {
                errorCode = ex.Message;
                errorDescription = ex.StackTrace;
                searchResponse = "Error retrieving TE assignment to Account :" + ex.Message;
                return searchResponse;
                //ToDo: Log the detailed error description and return only the Message code to the caller
            }
            if (response.Count == 0)
            {
                errorCode = "No TE found matching the criteria";
                errorDescription = "No TE found matching the criteria. Please review the criteria and resubmit";
                return searchResponse;
            }
            searchResponse += "TE(s) Assigned to the Account(s) is/are \n\n";
            foreach (SearchResult<ISVAccountsAzureResponse> result in response)
            {
                searchResponse += "**"+result.Document.TE + "** is the TE for **" + result.Document.ISV + "** \n\n";
            }
            return searchResponse;

        }

        public string GetAccountOwners(string indexName, string searchServiceName, string apiKey,
        string command, string filter = null)
        {
            string searchResponse = string.Empty;
            IList<SearchResult<ISVAccountsAzureResponse>> response = null;
            try
            {
                response = SearchDocuments(indexName, searchServiceName, apiKey, true,command, filter);
            }
            catch (Exception ex)
            {
                errorCode = ex.Message;
                errorDescription = ex.StackTrace;
                searchResponse = "Error retrieving TE assignment to Account :" + ex.Message;
                return searchResponse;
                //ToDo: Log the detailed error description and return only the Message code to the caller
            }
            if (response.Count == 0)
            {
                errorCode = "No Account found matching the criteria";
                errorDescription = "No Account found matching the criteria. Please review the criteria and resubmit";
                return searchResponse;
            }
            searchResponse += "The PBE and TE Assigned to the Account(s) is/are ..\n\n";
            foreach (SearchResult<ISVAccountsAzureResponse> result in response)
            {
                searchResponse += "PBE : **" + result.Document.PBE + "**, and TE :**" + result.Document.TE + "**\n\n";
            }
            return searchResponse;

        }

        public string GetAccountsByWorkload(string indexName, string searchServiceName, string apiKey,
      string command, string filter = null)
        {
            string searchResponse = string.Empty;
            IList<SearchResult<ISVAccountsAzureResponse>> response = null;
            try
            {
                response = SearchDocuments(indexName, searchServiceName, apiKey, true,command, filter);
            }
            catch (Exception ex)
            {
                errorCode = ex.Message;
                errorDescription = ex.StackTrace;
                searchResponse = "Error retrieving Accounts by Azure Workload :" + ex.Message;
                return searchResponse;
                //ToDo: Log the detailed error description and return only the Message code to the caller
            }
            if (response.Count == 0)
            {
                errorCode = "No Account found matching the Azure workload criteria";
                errorDescription = "No Account found matching the criteria. Please review the criteria and resubmit";
                return searchResponse;
            }
            int counter = 1;
            searchResponse += "Found **"+ response.Count +"** Account(s) with Azure workloads deployed/planned ..\n\n";
            string val1 = string.Empty;
            string val2 = string.Empty;
            foreach (SearchResult<ISVAccountsAzureResponse> result in response)
            {
                val1 = (string.IsNullOrEmpty(result.Document.PaasWorkloadsActual) ?"none":result.Document.PaasWorkloadsActual);
                val2 = (string.IsNullOrEmpty(result.Document.PaasWorkloadsPlanned) ? "none" : result.Document.PaasWorkloadsPlanned);

                searchResponse += counter+". **" + result.Document.ISV + "**, Workloads deployed **" +
                  val1 + "**, Workloads planned **" + val2 + "**\n\n";
                counter++;
            }
            return searchResponse;
        }

      
        public string GetAccountWithWins(string indexName, string searchServiceName, string apiKey, bool anyOrAllFlag,
     string command, string filter = null)
        {
            string searchResponse = string.Empty;
            IList<SearchResult<ISVAccountsAzureResponse>> response = null;
            try
            {
                telemetry.TrackTrace("executing azure search for winstatus query ..");
                response = SearchDocuments(indexName, searchServiceName, apiKey, anyOrAllFlag,command, filter);
               // telemetry.TrackTrace("response obtained from Azure Search "+response.);
            }
            catch (Exception ex)
            {
                telemetry.TrackTrace("Error executing search :" + ex.StackTrace + "\n  Error Message" + ex.Message);
                errorCode = ex.Message;
                errorDescription = ex.StackTrace;
                searchResponse = "Error retrieving Account with wins :" + ex.Message;
                return searchResponse;
                //ToDo: Log the detailed error description and return only the Message code to the caller
            }
            if (response.Count == 0)
            {
                errorCode = "No Account found having wins";
                errorDescription = "Please make sure the win is recorded in the SharePoint Account List ..";
                return searchResponse;
            }
            telemetry.TrackTrace("Found " + response.Count + " Account(s) matching the criteria ");
            searchResponse += "Found **"+ response.Count+ "** Account(s) with registered wins ..\n\n";
            foreach (SearchResult<ISVAccountsAzureResponse> result in response)
            {
                searchResponse += "ISV :**" + result.Document.ISV + "**, Win Count :**" +
                    result.Document.WinStatus + "** \n\n";
            }
            telemetry.TrackTrace(searchResponse);
            return searchResponse;

        }
        private IList<SearchResult<ISVAccountsAzureResponse>> SearchDocuments(string indexName, string searchServiceName, 
            string apiKey,bool anyOrAllFlag,   string searchText, string filter = null)
        {
            string searchResponse = string.Empty;
            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            SearchIndexClient indexClient = serviceClient.Indexes.GetClient(indexName);

            // Execute search based on search text and optional filter
            var sp = new SearchParameters();
            if (!String.IsNullOrEmpty(filter))
                sp.Filter = filter;
            if (anyOrAllFlag)
            {
                sp.QueryType = QueryType.Full;
                sp.SearchMode = SearchMode.All;
            }
            sp.Top = 500;
            telemetry.TrackTrace("Executing Azure Search Request: "+searchText);
            DocumentSearchResult<ISVAccountsAzureResponse> response = indexClient.Documents.Search<ISVAccountsAzureResponse>(searchText, sp);
            IList<SearchResult<ISVAccountsAzureResponse>> results = response.Results;
            return results;
        }

    }
}

using DxIndiaInformationBot.Models;
using DxIndiaInformationBot.Services;
using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DxIndiaInformationBot
{
    public class ServiceIntegrationClient
    {
        private TelemetryClient telemetry = new TelemetryClient();
        private string result = string.Empty;

        public ServiceIntegrationClient(string searchServiceAccountsNamespace,  string searchServiceAccountsApiKey, 
            string searchServiceIndexNameForAzureAccounts,
            string searchServiceDocsNamespace, string searchServiceDocsApiKey, string searchServiceDocsIndexName)
        {
            indexnameBlueprints = searchServiceDocsIndexName;
            indexnameAzureAccounts = searchServiceIndexNameForAzureAccounts;
            searchserviceAccountsns = searchServiceAccountsNamespace;
            Accountsapikey = searchServiceAccountsApiKey;
            searchservicedocsns = searchServiceDocsNamespace;
            docsapikey = searchServiceDocsApiKey;
        }

        private string indexnameBlueprints ;
        private string indexnameAzureAccounts;
        private string searchserviceAccountsns;
        private string Accountsapikey;
        private string searchservicedocsns;
        private string docsapikey;

        public string ExecuteSearch(LuisResponse request)
        {
            string intentname = request.intents[0].intent;
            switch (intentname)
            {
                case "GetArchitectureBlueprints":
                    {
                        result = FetchArchitectureBlueprintsByCriteria(request,false);
                        break;
                    }
                case "CountBlueprints":
                    {
                        result = FetchArchitectureBlueprintsByCriteria(request, true);
                        break;
                    }
                case "GetAzureAccounts":
                    {
                        result = GetAzureAccounts(request);
                        break;
                    }
                case "GetAssignedPBE":
                    {
                        result = GetAssignedPBE(request);
                        break;
                    }
                case "GetAssignedTE":
                    {
                        result = GetAssignedTE(request);
                        break;
                    }
                case "GetAccountOwners":
                    {
                        result = GetAccountOwners(request);
                        break;
                    }
                case "GetAccountsByAzureWorkloads":
                    {
                        result = GetAccountsByAzureWorkloads(request);
                        break;
                    }
                case "GetAccountsWithWins":
                    {
                        result = GetAzureWins(request);
                        break;
                    }
                case "GetAllAzureAccounts":
                    {
                        result = GetAllAzureAccounts(request);
                        break;
                    }
                default:
                    {
                        result = string.Empty;
                        break;
                    }
            }
            return result;
        }
        private string GetAllAzureAccounts(LuisResponse request)
        {
            string searchResult = string.Empty;
            string command = string.Empty;
            ISVAccountsService searchservice = new ISVAccountsService();
            searchResult = searchservice.GetAzureAccountMetadata(indexnameAzureAccounts,
                    searchserviceAccountsns, Accountsapikey, false, command, string.Empty);
         
            if (!string.IsNullOrEmpty(searchservice.ErrorCode))
            {
                searchResult = searchservice.ErrorCode;
            }
            return searchResult;
        }
        private string GetAzureWins(LuisResponse request)
        {
            string searchResult = string.Empty;
            ISVAccountsService searchservice = new ISVAccountsService();

            // When there are no Entities to be added as parameters to the query, frame a condition
            // in the query so that it returns all Wins
            if (request.entities == null || request.entities.Count == 0)
            {
                Entity l_entity1 = new Entity();
                l_entity1.type = "WinStatus";
                l_entity1.entity = "-0";
                List<Entity> allentities = new List<Entity>();
                allentities.Add(l_entity1);
                request.entities = allentities;
                string command = BuildIsvAccountSearchClause(request);
                searchResult = searchservice.GetAccountWithWins(indexnameAzureAccounts, searchserviceAccountsns,
                    Accountsapikey, false,command, string.Empty);
            }
            else
            {
                string command = BuildIsvAccountSearchClause(request);
                searchResult = searchservice.GetAccountWithWins(indexnameAzureAccounts, searchserviceAccountsns,
                    Accountsapikey, true, command, string.Empty);
            }
            if (!string.IsNullOrEmpty(searchservice.ErrorCode))
            {
                searchResult = searchservice.ErrorCode;
            }
            return searchResult;
        }
        private string GetAccountsByAzureWorkloads(LuisResponse request)
        {
            string searchResult = string.Empty;

            if (request.entities == null || request.entities.Count == 0)
            {
                // No data returned for some reason
                searchResult = "Sorry, I could not interpret your question." + 
                    ServiceConstants.ERROR_INTERPRETING_INPUT;
                telemetry.TrackTrace("No entity returned from Luis");
                return searchResult;
            }
            string command = BuildIsvAccountSearchClause(request);
            ISVAccountsService searchservice = new ISVAccountsService();
            searchResult = searchservice.GetAccountsByWorkload(indexnameAzureAccounts, searchserviceAccountsns, Accountsapikey, 
                command, string.Empty);
            if (!string.IsNullOrEmpty(searchservice.ErrorCode))
            {
                searchResult = searchservice.ErrorCode;
            }
            return searchResult;
        }
        private string GetAssignedTE(LuisResponse request)
        {
            string searchResult = string.Empty;
            if (request.entities == null || request.entities.Count == 0)
            {
                // No data returned for some reason
                searchResult = "Sorry, I could not interpret your question." + 
                    ServiceConstants.ERROR_INTERPRETING_INPUT;
                telemetry.TrackTrace("No entity returned from Luis");
                return searchResult;
            }
            string command = BuildIsvAccountSearchClause(request);
            ISVAccountsService searchservice = new ISVAccountsService();
            searchResult = searchservice.GetAssignedTE(indexnameAzureAccounts, searchserviceAccountsns, Accountsapikey,
                command, string.Empty);
            if (!string.IsNullOrEmpty(searchservice.ErrorCode))
            {
                searchResult = searchservice.ErrorCode;
            }
            return searchResult;
        }
        private string GetAccountOwners(LuisResponse request)
        {
            string searchResult = string.Empty;
            if (request.entities == null || request.entities.Count == 0)
            {
                // No data returned for some reason
                searchResult = "Sorry, I could not interpret your question." + 
                    ServiceConstants.ERROR_INTERPRETING_INPUT;
                telemetry.TrackTrace("No entity returned from Luis");
                return searchResult;
            }
            string command = BuildIsvAccountSearchClause(request);
            ISVAccountsService searchservice = new ISVAccountsService();
            searchResult = searchservice.GetAccountOwners(indexnameAzureAccounts, searchserviceAccountsns, 
                Accountsapikey, command, string.Empty);
            if (!string.IsNullOrEmpty(searchservice.ErrorCode))
            {
                searchResult = searchservice.ErrorCode;
            }
            return searchResult;
        }
        private string GetAzureAccounts(LuisResponse request)
        {
            string searchResult = string.Empty;
            string command = string.Empty;
            ISVAccountsService searchservice = new ISVAccountsService();
            if (request.entities == null || request.entities.Count == 0)
            {
                // No data returned for some reason
                searchResult = "Sorry, I could not interpret your question." +
                    ServiceConstants.ERROR_INTERPRETING_INPUT;
                telemetry.TrackTrace("No entity returned from Luis");
                return searchResult;
            }
            else
            {
                command = BuildIsvAccountSearchClause(request);
                searchResult = searchservice.GetAzureAccountMetadata(indexnameAzureAccounts,
                    searchserviceAccountsns, Accountsapikey, true, command, string.Empty);
            }
            if (!string.IsNullOrEmpty(searchservice.ErrorCode))
            {
                searchResult = searchservice.ErrorCode;
            }
            return searchResult;
        }
        private string GetAssignedPBE(LuisResponse request)
        {
            string searchResult = string.Empty;
            if (request.entities == null || request.entities.Count == 0)
            {
                // No data returned for some reason
                searchResult = "Sorry, I could not interpret your question." + 
                    ServiceConstants.ERROR_INTERPRETING_INPUT;
                telemetry.TrackTrace("No entity returned from Luis");
                return searchResult;
            }
            string command = BuildIsvAccountSearchClause(request);
            ISVAccountsService searchservice = new ISVAccountsService();
            searchResult = searchservice.GetAssignedPBE(indexnameAzureAccounts, searchserviceAccountsns, Accountsapikey, 
                command, string.Empty);
            if (!string.IsNullOrEmpty(searchservice.ErrorCode))
            {
                searchResult = searchservice.ErrorCode;
            }
            return searchResult;
        }
        private string BuildIsvAccountSearchClause(LuisResponse request)
        {
            string command = string.Empty;
            int counter = 0;
            foreach (Entity entityParam in request.entities)
            {
                if ("TE".Equals(entityParam.type))
                {
                    if (counter == 0)
                    {
                        command += "TE:(" + entityParam.entity + ")";
                    }
                    else
                    {
                        command += " AND TE:(" + entityParam.entity + ")";
                    }
                }
                if ("PBE".Equals(entityParam.type))
                {
                    if (counter == 0)
                    {
                        command += "PBE:(" + entityParam.entity + ")";
                    }
                    else
                    {
                        command += " AND PBE:(" + entityParam.entity + ")";
                    }
                }
                if ("ISV".Equals(entityParam.type))
                {
                    if (counter == 0)
                    {
                        command += "ISV:(" + entityParam.entity + ")";
                    }
                    else
                    {
                        command += " AND ISV:(" + entityParam.entity + ")";
                    }
                }
                if ("AccountType".Equals(entityParam.type))
                {
                    if (counter == 0)
                    {
                        command += "AccountType:(" + entityParam.entity + ")";
                    }
                    else
                    {
                        command += " AND AccountType:(" + entityParam.entity + ")";
                    }
                }
                if ("PaasWorkloadsPlanned".Equals(entityParam.type))
                {
                    if (string.IsNullOrEmpty(entityParam.entity))
                        continue;
                    char[] delimiterChars = { '|', ',', '.', ':', '\t' };
                    string[] vals = entityParam.entity.Split(delimiterChars);
                    string tokenisedparams = string.Empty;
                    foreach(string curval in vals)
                    {
                        tokenisedparams += "'"+curval.Trim() + "'";
                    }
                    if (counter == 0)
                    {
                        command += "PaasWorkloadsPlanned:(" + tokenisedparams + ")";
                    }
                    else
                    {
                        command += " AND PaasWorkloadsPlanned:(" + tokenisedparams + ")";
                    }
                }
                if ("PaasWorkloadsActual".Equals(entityParam.type))
                {
                    if (string.IsNullOrEmpty(entityParam.entity))
                        continue;
                    char[] delimiterChars = { '|', ',', '.', ':', '\t' };
                    string[] vals = entityParam.entity.Split(delimiterChars);
                    string tokenisedparams = string.Empty;
                    foreach (string curval in vals)
                    {
                        tokenisedparams += "'" + curval.Trim() + "'";
                    }
                    if (counter == 0)
                    {
                        command += "PaasWorkloadsActual:(" + tokenisedparams + ")";
                    }
                    else
                    {
                        command += " AND PaasWorkloadsActual:(" + tokenisedparams + ")";
                    }
                }
                if ("EngagementStatus".Equals(entityParam.type))
                {
                    if (counter == 0)
                    {
                        command += "EngagementStatus:(" + entityParam.entity + ")";
                    }
                    else
                    {
                        command += " AND EngagementStatus:(" + entityParam.entity + ")";
                    }
                }
                if ("ExpertAdviceStatus".Equals(entityParam.type))
                {
                    if (counter == 0)
                    {
                        command += "ExpertAdviceStatus:(" + entityParam.entity + ")";
                    }
                    else
                    {
                        command += " AND ExpertAdviceStatus:(" + entityParam.entity + ")";
                    }
                }
                if ("CmatWorkloadsProposed".Equals(entityParam.type))
                {
                    if (counter == 0)
                    {
                        command += "CmatWorkloadsProposed:(" + entityParam.entity + ")";
                    }
                    else
                    {
                        command += " AND CmatWorkloadsProposed:(" + entityParam.entity + ")";
                    }
                }
                if ("CmatWorkloadsActual".Equals(entityParam.type))
                {
                    if (counter == 0)
                    {
                        command += "CmatWorkloadsActual:(" + entityParam.entity + ")";
                    }
                    else
                    {
                        command += " AND CmatWorkloadsActual:(" + entityParam.entity + ")";
                    }
                }
                if ("LastConnectDate".Equals(entityParam.type))   // not implemented correctly yet
                {
                    if (counter == 0)
                    {
                        command += "LastConnectDate:(" + entityParam.entity + ")";
                    }
                    else
                    {
                        command += " AND LastConnectDate:(" + entityParam.entity + ")";
                    }
                }
                if ("WinStatus".Equals(entityParam.type))
                {
                    
                    if (counter == 0)
                    {
                        command += "WinStatus:(" + entityParam.entity + ")";
                    }
                    else
                    {
                        command += " AND WinStatus:(>" + entityParam.entity + ")";
                    }
                }
                counter++;
            }
            return command;
        }

        private string FetchArchitectureBlueprintsByCriteria(LuisResponse request, bool countonly)
        {
            string command = "";
            bool searchModeAll = true;
            bool noInputParametersFound = false;
            string filter = string.Empty;
            string searchResult = string.Empty;
            // string inputparam = request.entities[0].type;
            if (request.entities.Count == 0) // No input parameters
            {
               // filter += "slidecount ge '1'";
                searchModeAll = false;
                noInputParametersFound = true;
            }
            else if (request.entities.Count == 1) // wildcard for single parameter possible, hence suffixing with *
            {
                switch (request.entities[0].type)
                {
                    case "TE":
                        {
                            //command += "author:(" + request.entities[0].entity + "*|"+request.entities[0].entity+")";
                            command += "author:(" + request.entities[0].entity +")";
                            break;
                        }
                    case "Title":
                        {
                            command += "title:(" + request.entities[0].entity +")";
                            break;
                        }
                    case "keyword":
                        {
                            command += request.entities[0].entity;
                            break;
                        }
                    case "builtin.datetime.date":
                        {
                            if ("last month".Equals(request.entities[0].entity.ToLower()) || "this month".Equals(request.entities[0].entity.ToLower())
                                || "previous month".Equals(request.entities[0].entity.ToLower()) || "current month".Equals(request.entities[0].entity.ToLower())
                                || "present month".Equals(request.entities[0].entity.ToLower()))
                            {
                                filter += "modifieddate ge '" + request.entities[0].resolution.date + "-01T00:00:00Z' and modifieddate le '" +
                                    request.entities[0].resolution.date + "-31T00:00:00Z'";
                            }
                            else if ("today".Equals(request.entities[0].entity.ToLower()) || "yesterday".Equals(request.entities[0].entity.ToLower()))
                            {
                                filter += "modifieddate ge '" + request.entities[0].resolution.date + "T00:00:00Z' and modifieddate le '" +
                                    request.entities[0].resolution.date + "T24:00:00Z'";
                            }
                            else if ("last year".Equals(request.entities[0].entity.ToLower()) || "previous year".Equals(request.entities[0].entity.ToLower())
                           || "current year".Equals(request.entities[0].entity.ToLower()) || "this year".Equals(request.entities[0].entity.ToLower())
                           || "present year".Equals(request.entities[0].entity.ToLower()))

                            {
                                filter += "modifieddate ge '" + request.entities[0].resolution.date + "-01-01T00:00:00Z' and modifieddate le '" +
                                    request.entities[0].resolution.date + "-12-31T24:00:00Z'";
                            }
                            else if ("last week".Equals(request.entities[0].entity.ToLower()) || "previous week".Equals(request.entities[0].entity.ToLower())
                       || "current week".Equals(request.entities[0].entity.ToLower()) || "this week".Equals(request.entities[0].entity.ToLower())
                       || "present week".Equals(request.entities[0].entity.ToLower()))

                            {
                                string firstParameter = string.Empty;
                                string secondParameter = string.Empty;
                                string refdate = request.entities[0].resolution.date;

                                try
                                {
                                    Console.WriteLine("Parsing the week information" + refdate);
                                    int weeknumber = int.Parse(refdate.Substring(6, 2));
                                    DateTime stDate = FirstDateOfWeekISO8601(DateTime.UtcNow.Year, weeknumber);
                                    Console.WriteLine("The date is :" + stDate + " week number was :" + weeknumber);
                                    DateTime secDateTime = stDate.AddDays(6);
                                    firstParameter = stDate.Year.ToString() + "-0" + stDate.Month.ToString() + "-0" + stDate.Day.ToString() + "T00:00:00Z";
                                    secondParameter = secDateTime.Year.ToString() + "-0" + secDateTime.Month.ToString() + "-0" + secDateTime.Day.ToString() + "T24:00:00Z";

                                }
                                catch (Exception)
                                {
                                    //Console.WriteLine("Error .. silently ignore");
                                }
                                filter += "modifieddate ge '" + firstParameter + "' and modifieddate le '" +
                                    secondParameter + "'";
                            }
                            else
                            {
                                Console.WriteLine("special case .. dates entered directly ....");
                                string refdate = request.entities[0].resolution.date;
                                Console.WriteLine("Input date is " + refdate);
                                string firstParameter = string.Empty;
                                string secondParameter = string.Empty;
                                try
                                {
                                    refdate = refdate.Replace("XXXX", DateTime.UtcNow.Year.ToString());
                                    if (refdate.Length == 7) // only month and year.. then day has to be added
                                    {
                                        string givenYear = refdate.Substring(0, 4);
                                        string givenMonth = refdate.Substring(5, 2);

                                        int igivenmonth = int.Parse(givenMonth);
                                        int igivenyear = int.Parse(givenYear);
                                        DateTime givenDateTime = new DateTime(igivenyear, igivenmonth, 1);
                                        DateTime newDateTime = givenDateTime.AddMonths(1);
                                        newDateTime = newDateTime.AddDays(-1);
                                        int newmonth = newDateTime.Month;
                                        string newwmonthstr = string.Empty;

                                        if (newmonth < 10)
                                        {
                                            newwmonthstr = "0" + newmonth;
                                        }
                                        else
                                        {
                                            newwmonthstr = "" + newmonth;
                                        }
                                        refdate += "-01";
                                        firstParameter = refdate + "T00:00:00Z";
                                        secondParameter = newDateTime.Year.ToString() + "-" + newwmonthstr + "-" + newDateTime.Day.ToString() + "T24:00:00Z";
                                    }
                                    else if(refdate.Length == 10)  // full year, month, date available
                                    {
                                        firstParameter = refdate + "T00:00:00Z";
                                        secondParameter = refdate + "T24:00:00Z";
                                    }
                                    else // only year specified
                                    {
                                        string givenYear = refdate.Substring(0, 4);
                                        firstParameter = givenYear+"-01-01T00:00:00Z";
                                        secondParameter = givenYear + "-12-31T24:00:00Z";
                                    }
                                }
                                catch (Exception)
                                { }
                                filter += "modifieddate ge '" + firstParameter + "' and modifieddate le '" +
                                    secondParameter + "'";
                            }
                            break;
                        }
                    case "builtin.datetime.duration":
                        {
                            Console.WriteLine("duration based parameter");
                            string refduration = request.entities[0].resolution.date;
                            string parameter = request.entities[0].resolution.duration;
                            DateTime curDate = DateTime.Now;
                            int curMonth = curDate.Month;
                            int curYear = curDate.Year;
                            int curDay = curDate.Day;

                            string firstParameter = string.Empty;
                            string secondParameter = string.Empty;

                            if ("M".Equals(parameter.Substring(2, 1)))
                            {
                                DateTime paramDate = curDate.AddMonths(-int.Parse(parameter.Substring(1, 1)));
                                firstParameter = paramDate.Year.ToString() + "-0" + paramDate.Month.ToString() + "-0" + paramDate.Day.ToString() + "T00:00:00Z";
                                secondParameter = curDate.Year.ToString() + "-0" + curDate.Month.ToString() + "-0" + curDate.Day.ToString() + "T24:00:00Z";
                            }
                            else if ("Y".Equals(parameter.Substring(2, 1)))
                            {
                                DateTime paramDate = curDate.AddYears(-int.Parse(parameter.Substring(1, 1)));
                                firstParameter = paramDate.Year.ToString() + "-0" + paramDate.Month.ToString() + "-0" + paramDate.Day.ToString() + "T00:00:00Z";
                                secondParameter = curDate.Year.ToString() + "-0" + curDate.Month.ToString() + "-0" + curDate.Day.ToString() + "T24:00:00Z";

                            }
                            else if ("D".Equals(parameter.Substring(2, 1)))
                            {
                                DateTime paramDate = curDate.AddDays(-int.Parse(parameter.Substring(1, 1)));
                                firstParameter = paramDate.Year.ToString() + "-0" + paramDate.Month.ToString() + "-0" + paramDate.Day.ToString() + "T00:00:00Z";
                                secondParameter = curDate.Year.ToString() + "-0" + curDate.Month.ToString() + "-0" + curDate.Day.ToString() + "T24:00:00Z";

                            }
                            else if ("W".Equals(parameter.Substring(2, 1)))
                            {
                                DateTime paramDate = curDate.AddDays(-((int.Parse(parameter.Substring(1, 1)) * 7) + 1));
                                firstParameter = paramDate.Year.ToString() + "-0" + paramDate.Month.ToString() + "-0" + paramDate.Day.ToString() + "T00:00:00Z";
                                secondParameter = curDate.Year.ToString() + "-0" + curDate.Month.ToString() + "-0" + curDate.Day.ToString() + "T24:00:00Z";

                            }
                            else
                            {
                                // No other condition ..
                            }
                            filter += "modifieddate ge '" + firstParameter + "' and modifieddate le '" +
                                    secondParameter + "'";
                            break;
                        }
                    default:
                        {
                            command += "*";  // returns everything
                            break;
                        }
                }
            }
            else if (request.entities.Count > 1) // No wildcard search when there are multiple input parameters
            {
                int counter = 0;
                command = "search=*";
                foreach (Entity entityParam in request.entities)
                {
                    if ("TE".Equals(entityParam.type))
                    {
                        if (counter == 0)
                        {
                            command += "&$filter=(author eq '" + entityParam.entity + "'";
                        }
                        else
                        {
                            command += " and author eq '" + entityParam.entity + "'";
                        }
                    }
                  
                    if ("Title".Equals(entityParam.type))
                    {
                        if (counter == 0)
                        {
                            command += "&$filter=(title eq '" + entityParam.entity + "'";
                        }
                        else
                        {
                            command += " and title eq '" + entityParam.entity + "'";
                        }
                    }
                    if ("keyword".Equals(entityParam.type))
                    {
                        if (counter == 0)
                        {
                            command += "&$filter=(title eq '" + entityParam.entity + "'";
                        }
                        else
                        {
                            command += " and title eq '" + entityParam.entity + "'";
                        }
                    }
                    counter++;
                }
                command += ")";
           }

            // Now call the Azure Search Service
            SearchService searchservice = new SearchService();
            telemetry.TrackEvent("The command executed is "+ command +", and the filter is "+filter);
            searchResult += searchservice.ExecuteSearch(command, filter, indexnameBlueprints, searchservicedocsns, docsapikey,countonly, searchModeAll);
            if (!(string.IsNullOrEmpty(searchservice.ErrorCode)))
            {
               telemetry.TrackEvent("Error executing Blueprints search : " + searchservice.ErrorCode + ", and the command passed is " + command + " and filter is :" + filter);
               searchResult = searchservice.ErrorCode ;
            }
            if (noInputParametersFound)
            {
                searchResult = "You either did not enter a valid filter criteria or I did not find any matching it. Returning unfiltered set of results ... \n\n" + 
                    searchResult;
            }
            return searchResult;
        }

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

    }
}
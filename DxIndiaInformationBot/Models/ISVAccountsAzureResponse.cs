using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DxIndiaInformationBot.Models
{
    public class ISVAccountsAzureResponse
    {
        public int ID;
        public string ISV;
        public string Title;
        public string AccountType;
        public string PBE;
        public string TE;
        public string PaasWorkloadsPlanned;
        public string PaasWorkloadsActual;
        public string EngagementStatus;
        public int WinStatus;
        public string ExpertAdviceStatus;
        public string ArchitectureBlueprint;
        public string CmatWorkloadsProposed;
        public string CmatWorkloadsActual;
        public DateTime LastConnectDate;
        public string ModifiedBy;
    }
}
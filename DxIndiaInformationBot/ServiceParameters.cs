using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DxIndiaInformationBot
{
    public class ServiceParameters
    {
        private string luiserviceurl;
        private string blueprintindexname;
        private string azureaccountsindexname;
        private string searchservicenamespace;
        private string searchservicenamespacedocs;

        private string searchservicekey;
        private string searchservicedocskey;

        public string Luiserviceurl
        {
            get
            {
                return luiserviceurl;
            }

            set
            {
                luiserviceurl = value;
            }
        }

        public string Blueprintindexname
        {
            get
            {
                return blueprintindexname;
            }

            set
            {
                blueprintindexname = value;
            }
        }

        public string Searchservicenamespace
        {
            get
            {
                return searchservicenamespace;
            }

            set
            {
                searchservicenamespace = value;
            }
        }

        public string Searchservicekey
        {
            get
            {
                return searchservicekey;
            }

            set
            {
                searchservicekey = value;
            }
        }

        public string Azureaccountsindexname
        {
            get
            {
                return azureaccountsindexname;
            }

            set
            {
                azureaccountsindexname = value;
            }
        }

        public string Searchservicenamespacedocs
        {
            get
            {
                return searchservicenamespacedocs;
            }

            set
            {
                searchservicenamespacedocs = value;
            }
        }

        public string Searchservicedocskey
        {
            get
            {
                return searchservicedocskey;
            }

            set
            {
                searchservicedocskey = value;
            }
        }
    }
}
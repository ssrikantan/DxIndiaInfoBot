namespace DxIndiaInformationBot.Models
{

    //  [SerializePropertyNamesAsCamelCase]
    public class BlueprintsSearchResponse
    {
        public string id { get; set; }
        public string content { get; set; }
        public string filesize { get; set; }
        public string author { get; set; }
        public string slidecount { get; set; }
        public string title { get; set; }

        public string fileurl { get; set; }

        public string contenttype { get; set; }
        public System.DateTime modifieddate { get; set; }
    }
  }

namespace SwaggerDocsRazorViews.Models
{
    public class ApiError
    {
        /// <summary>
        /// Eror Code: 200, 422, 500 etc.
        /// <a href='errors'>Error Codes</a>
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Erfolgs- oder Misserfolgsmeldung
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Die Nummer des Fehlers oder der Ausnahme
        /// <a href='apicodes'>Api Codes</a>
        /// </summary>      
        public string ApiCode { get; set; }        
    }
}
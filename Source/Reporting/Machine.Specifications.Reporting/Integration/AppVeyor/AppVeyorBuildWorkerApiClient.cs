namespace Machine.Specifications.Reporting.Integration.AppVeyor
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Web.Script.Serialization;
    
    public class AppVeyorBuildWorkerApiClient : IAppVeyorBuildWorkerApiClient
    {
        const string ApiResource = "/api/tests";
        
        readonly string _apiUrl;

        public AppVeyorBuildWorkerApiClient(string apiUrl)
        {
            if (apiUrl == null)
            {
                throw new ArgumentNullException("apiUrl");
            }

            _apiUrl = apiUrl;
        }

        public void AddTest(string testName,
                            string testFramework,
                            string fileName,
                            string outcome,
                            long? durationMilliseconds,
                            string errorMessage,
                            string errorStackTrace,
                            string stdOut,
                            string stdErr)
        {
            var body = new
                       {
                           TestName = testName,
                           TestFramework = testFramework,
                           FileName = fileName,
                           Outcome = outcome,
                           DurationMilliseconds = durationMilliseconds,
                           ErrorMessage = errorMessage,
                           ErrorStackTrace = errorStackTrace,
                           StdOut = TrimStdOut(stdOut),
                           StdErr = TrimStdOut(stdErr)
                       };

            try
            {
                using (WebClient wc = GetClient())
                {
                    wc.UploadData(ApiResource, "POST", Json(body));
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error communicating AppVeyor Build Worker API: " + ex.Message);
            }
        }

        public void UpdateTest(string testName,
                               string testFramework,
                               string fileName,
                               string outcome,
                               long? durationMilliseconds,
                               string errorMessage,
                               string errorStackTrace,
                               string stdOut,
                               string stdErr)
        {
            var body = new
                       {
                           TestName = testName,
                           TestFramework = testFramework,
                           FileName = fileName,
                           Outcome = outcome,
                           DurationMilliseconds = durationMilliseconds,
                           ErrorMessage = errorMessage,
                           ErrorStackTrace = errorStackTrace,
                           StdOut = TrimStdOut(stdOut),
                           StdErr = TrimStdOut(stdErr)
                       };

            try
            {
                using (WebClient wc = GetClient())
                {
                    wc.UploadData(ApiResource, "PUT", Json(body));
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error communicating AppVeyor Build Worker API: " + ex.Message);
            }
        }

        static string TrimStdOut(string str)
        {
            const int MaxLength = 4096;

            if (str == null)
            {
                return null;
            }

            return (str.Length > MaxLength) ? str.Substring(0, MaxLength) : str;
        }

        static byte[] Json(object data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);
        }

        WebClient GetClient()
        {
            var wc = new WebClient {BaseAddress = _apiUrl};
            wc.Headers["Accept"] = "application/json";
            wc.Headers["Content-type"] = "application/json";
            return wc;
        }
    }
}
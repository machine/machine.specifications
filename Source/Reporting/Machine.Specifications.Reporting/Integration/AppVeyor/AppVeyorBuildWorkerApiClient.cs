namespace Machine.Specifications.Reporting.Integration.AppVeyor
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    
    public class AppVeyorBuildWorkerApiClient : IAppVeyorBuildWorkerApiClient
    {
        readonly string _apiUrl;

        public AppVeyorBuildWorkerApiClient(string apiUrl)
        {
            if (apiUrl == null)
            {
                throw new ArgumentNullException("apiUrl");
            }

            _apiUrl = apiUrl.TrimEnd('/') + "/";
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
                    wc.UploadData("api/tests", "POST", Json(body));
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
                    wc.UploadData("api/tests", "PUT", Json(body));
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
            var jsonProperties = new List<string>();
            foreach (var property in data.GetType().GetProperties())
            {
                var value = property.GetValue(data, null);
                var name = property.Name;
                if (value != null)
                    jsonProperties.Add(string.Format("{0}:{1}", name, value));
            }
            return Encoding.UTF8.GetBytes("{" + string.Join(",", jsonProperties.ToArray()) + "}");
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
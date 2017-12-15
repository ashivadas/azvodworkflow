using Microsoft.WindowsAzure.MediaServices.Client;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace vodworkflow
{
    class Program
    {
        private static readonly string MediaFiles = @"Media\BigBuckBunny.mp4";

        // Read values from the App.config file.
        static string _AADTenantDomain = ConfigurationManager.AppSettings["AMSAADTenantDomain"];
        static string _RESTAPIEndpoint = ConfigurationManager.AppSettings["AMSRESTAPIEndpoint"];
        static string _ClientID = ConfigurationManager.AppSettings["clientid"];
        static string _ClientSecret = ConfigurationManager.AppSettings["clientsecret"];

        // Field for service context.
        private static CloudMediaContext _context = null;

        static void Main(string[] args)
        {
            try
            {
                AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(
                    _AADTenantDomain,
                    new AzureAdClientSymmetricKey(_ClientID, _ClientSecret),
                    AzureEnvironments.AzureCloudEnvironment);

                AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);

                _context = new CloudMediaContext(new Uri(_RESTAPIEndpoint), tokenProvider);

                // If you want to secure your high quality input media files with strong encryption 
                // at rest on disk, use AssetCreationOptions.StorageEncrypted instead of 
                // AssetCreationOptions.None.

                Console.WriteLine($"Uploading a file: {MediaFiles} \n");
                IAsset inputAsset = UploadFile(MediaFiles, AssetCreationOptions.None);

                // Create adaptive bitrate set
                EncodeJobResponse encJob = EncodeToAdaptiveBitrateMP4Set(inputAsset.Id);

                // Create associated .vtt file

                // Upload text file to asset name

                // Check status of encode
                CheckJobStatusResponse chkJob;

                do
                {
                    chkJob = CheckJobStatus(encJob.JobId);
                } while (chkJob.IsRunning.Equals("true"));

            }
            catch (Exception exception)
            {
                // Parse the XML error message in the Media Services response and create a new
                // exception with its content.
                exception = MediaServicesExceptionParser.Parse(exception);

                Console.Error.WriteLine(exception.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static public IAsset UploadFile(string fileName, AssetCreationOptions options)
        {
            Console.WriteLine("Upload File: filename, options", fileName, OperationState.Succeeded);

            IAsset inputAsset = _context.Assets.CreateFromFile(fileName, options, (af, p) =>
            {
                Console.WriteLine("Uploading '{0}' - Progress: {1:0.##}%", af.Name, p.Progress);
            });

            Console.WriteLine("Asset {0} created.", inputAsset.Id);

            return inputAsset;
        }

        // Send a message to encode to MP4
        static public EncodeJobResponse EncodeToAdaptiveBitrateMP4Set(string assetId)
        {
            // send a message to submit job 
            // string url = "https://azmediafunctionsforlogicappwdahb73ofbb5k.azurewebsites.net/api/submit-job?code=J3mX1K4aWOMC6PXiTDkHY/BL1sxxgQy2IBJs0L9Vhs6Z158ucNjNpA==&clientId=default";
            string url = "http://192.168.1.16:3000/submitjob/1";

            EncodeJobRequest request = new EncodeJobRequest()
            {
                AssetId = assetId,
                MesPreset = "Content Adaptive Multiple Bitrate MP4"
            };

            HttpWebResponse httpResponse = MakeHttpRequest(url, Utils.SerializeObject(request), "POST");

            EncodeJobResponse response = ReadEncodeJobResponse(httpResponse);

            return response;
        }

        static public void AddSubtitletoAsset(string srtFile, IAsset asset)
        {

        }

        private static IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
        {
            var processor = _context.MediaProcessors.Where(p => p.Name == mediaProcessorName).
            ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();

            if (processor == null)
                throw new ArgumentException(string.Format("Unknown media processor", mediaProcessorName));

            return processor;
        }

        public static CheckJobStatusResponse CheckJobStatus(string jobid)
        {
            string url = "http://192.168.1.16:3000/submitjob/1";

            CheckJobStatusRequest request = new CheckJobStatusRequest
            {
                JobId = jobid,
                ExtendedInfo = true
            };

            HttpWebResponse httpResponse = MakeHttpRequest(url, Utils.SerializeObject(request), "POST");
            CheckJobStatusResponse response = ReadCheckJobResponse(httpResponse);

            return response;
        }

        public static HttpWebResponse MakeHttpRequest(string uri, string json, string verb)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(json);
            HttpWebRequest objHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);

            if (verb.Equals("POST"))
            {
                objHttpWebRequest.Method = "POST";
                objHttpWebRequest.ContentType = "application/json";
                objHttpWebRequest.KeepAlive = true;
                objHttpWebRequest.ContentLength = bytes.Length;
                Stream objStream = objHttpWebRequest.GetRequestStream();
                objStream.Write(bytes, 0, bytes.Length);
                objStream.Close();
            }
            else
            {
                objHttpWebRequest.Method = "GET";

            }


            try
            {
                HttpWebResponse objHttpWebResponse = (HttpWebResponse)objHttpWebRequest.GetResponse();
                return objHttpWebResponse;
            }
            catch (WebException exception)
            {
                Console.Error.WriteLine(exception.Message);
            }

            return null;
        }

        public static EncodeJobResponse ReadEncodeJobResponse(HttpWebResponse httpResponse)
        {
            EncodeJobResponse encJob = new EncodeJobResponse();
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    using (Stream stream = httpResponse.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string result = reader.ReadToEnd();
                            encJob = JsonConvert.DeserializeObject<EncodeJobResponse>(result);
                        }
                    }
                }
                catch (WebException exception)
                {
                    Console.Error.WriteLine(exception.Message);
                }
            }

            return encJob;
        }

        public static CheckJobStatusResponse ReadCheckJobResponse(HttpWebResponse httpResponse)
        {
            Stream stream = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            var result = reader.ReadToEnd();
            CheckJobStatusResponse chkJob = JsonConvert.DeserializeObject<CheckJobStatusResponse>(result);
            return chkJob;
        }
    }
}

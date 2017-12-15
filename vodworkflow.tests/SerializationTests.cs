using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using vodworkflow;

namespace vodworkflow.tests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void ItShouldProperlyDeserializeEncodeJobResponse()
        {
            var raw = File.ReadAllText(@"Mocks\encode-job-response.json");

            var response = JsonConvert.DeserializeObject<EncodeJobResponse>(raw);

            // Root Object
            Assert.AreEqual("nb:jid:UUID:19cab2ff-0300-80c0-da01-f1e7d467f6f9", response.JobId);
            Assert.AreEqual(1, response.OtherJobsQueue);

            // Mes Object 
            Assert.AreEqual("nb:cid:UUID:7346ded3-fdb5-4e97-b508-4e20b054d0b4", response.Mes.AssetId);
            Assert.AreEqual("nb:tid:UUID:19cab2ff-0300-80c0-da02-f1e7d467f6f9", response.Mes.TaskId);

            // Mepw Object
            Assert.IsNull(response.Mepw.AssetId);
            Assert.IsNull(response.Mepw.TaskId);

            // Index V1
            Assert.IsNull(response.IndexV1.AssetId);
            Assert.IsNull(response.IndexV1.Language);
            Assert.IsNull(response.IndexV1.TaskId);

            // Index V2
            Assert.AreEqual("nb:cid:UUID:7205d19f-21da-4cb1-9dbb-47d251103d75", response.IndexV2.AssetId);
            Assert.AreEqual("nb:tid:UUID:19cab2ff-0300-80c0-da03-f1e7d467f6f9", response.IndexV2.TaskId);
            Assert.AreEqual("EnUs", response.IndexV2.Language);

            // Ocr
            Assert.IsNull(response.Ocr.AssetId);
            Assert.IsNull(response.Ocr.TaskId);

            // Face Detection
            Assert.IsNull(response.FaceDetection.AssetId);
            Assert.IsNull(response.FaceDetection.TaskId);

            // Face Redaction
            Assert.IsNull(response.FaceRedaction.AssetId);
            Assert.IsNull(response.FaceRedaction.TaskId);

            // Motion Detection
            Assert.IsNull(response.MotionDetection.AssetId);
            Assert.IsNull(response.MotionDetection.TaskId);

            // Summarization
            Assert.IsNull(response.Summarization.AssetId);
            Assert.IsNull(response.Hyperlapse.TaskId);
        }

        [TestMethod]
        public void ItShouldDeserializeCheckJobStatusResponseProperly()
        {
            var raw = File.ReadAllText(@"Mocks\check-jobs-status-response.json");

            var response = JsonConvert.DeserializeObject<CheckJobStatusResponse>(raw);

            Assert.AreEqual(2, response.JobState);
            Assert.AreEqual(false, response.IsRunning);
            Assert.AreEqual(true, response.IsSuccessful);
            Assert.IsTrue(string.IsNullOrEmpty(response.ErrorText));
            Assert.IsNull(response.StartTime);
            Assert.IsNull(response.EndTime);
            Assert.IsTrue(string.IsNullOrEmpty(response.RunningDuration));

            // Extended Information
            Assert.AreEqual(2, response.ExtendedInfo.MediaUnitNumber);
            Assert.AreEqual("S2", response.ExtendedInfo.MediaUnitSize);
            Assert.AreEqual(2, response.ExtendedInfo.OtherJobsProcessing);
            Assert.AreEqual(1, response.ExtendedInfo.OtherJobsScheduled);
            Assert.AreEqual(1, response.ExtendedInfo.OtherJobsQueue);
            Assert.AreEqual("http://path/to/ams-REST-API-Endpoint", response.ExtendedInfo.AmsRESTAPIEndpoint);
        }

        [TestMethod]
        public void ItShouldSerializeCheckJobStatusRequestProperly()
        {
            var request = new CheckJobStatusRequest()
            {
                JobId = "nb:jid:UUID:1ceaa82f-2607-4df9-b034-cd730dad7097",
                ExtendedInfo = true
            };
            
            var expected = "{\"jobId\":\"nb:jid:UUID:1ceaa82f-2607-4df9-b034-cd730dad7097\",\"extendedInfo\":true}";
            Assert.AreEqual(expected, Utils.SerializeObject(request));
        }

        [TestMethod]
        public void ItShouldSerializeEncodeJobRequestProperly()
        {
            var request = new EncodeJobRequest()
            {
                AssetId = "nb:jid:UUID:1ceaa82f-2607-4df9-b034-cd730dad7097",
                MesPreset = "Content Adaptive Multiple Bitrate MP4"
            };

            var expected = "{\"assetId\":\"nb:jid:UUID:1ceaa82f-2607-4df9-b034-cd730dad7097\",\"mesPreset\":\"Content Adaptive Multiple Bitrate MP4\"}";
            Assert.AreEqual(expected, Utils.SerializeObject(request));
        }
    }
}

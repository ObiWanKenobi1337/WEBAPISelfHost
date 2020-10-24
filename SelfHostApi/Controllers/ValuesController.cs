using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SelfHostApi.Models;

namespace SelfHostApi.Controllers
{
    [Route("api/GetItems")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly List<ETypes> listOfSupportedTypes;
        private readonly object locker = new object();
        private readonly string path = @"C:\Users\Администратор\Desktop\configs";
        private ReaderWriterLock rwl = new ReaderWriterLock();
        private readonly Semaphore sem = new Semaphore(1, 1);

        public ValuesController()
        {
            listOfSupportedTypes = new List<ETypes> {ETypes.ConnectionSettings, ETypes.DeviceSettings};
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var info = new DirectoryInfo(path);
            var dictCanWork = new Dictionary<string, bool>();
            foreach (var item in info.GetFiles())
            {
                sem.WaitOne();
                using (var stream = item.OpenText())
                {
                    try
                    {
                        var fileText = await stream.ReadToEndAsync().ConfigureAwait(true);
                        if (string.IsNullOrEmpty(fileText))
                        {
                            stream.Close();
                            continue;
                        }

                        var deserializedType = JsonConvert.DeserializeObject<BaseAbstractModel>(fileText,
                            new JsonSerializerSettings {Converters = {new CustomJsonConverter()}});
                        dictCanWork.Add(item.Name,
                            deserializedType != null && listOfSupportedTypes.Contains(deserializedType.Type));
                    }
                    finally
                    {
                        sem.Release();
                    }
                }
            }

            return dictCanWork.Where(item => item.Value).Select(item => item.Key);
        }

        // GET api/<ValuesController>/5
        [HttpGet("{fileName}")]
        public async Task<ActionResult<BaseAbstractModel>> Get(string fileName)
        {
            var pathToFile = Path.Combine(path, $"{fileName}.json");
            try
            {
                sem.WaitOne();
                if (System.IO.File.Exists(pathToFile))
                    using (var sr = new StreamReader(pathToFile))
                    {
                        var str = string.Empty;
                        while (!sr.EndOfStream)
                            str += await sr.ReadLineAsync();
                        sr.Close();
                        return Ok(JsonConvert.DeserializeObject<BaseAbstractModel>(str,
                            new JsonSerializerSettings {Converters = {new CustomJsonConverter()}}));
                    }
            }
            finally
            {
                sem.Release();
            }

            return BadRequest(new FileNotFoundException());
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{fileName}")]
        public async Task<ActionResult> Put(string fileName, [FromBody] dynamic obj)
        {
            var pathToFile = Path.Combine(path, $"{fileName}.json");
            var body = obj.ToString();
            try
            {
                sem.WaitOne();
                if (!System.IO.File.Exists(pathToFile))
                    return BadRequest(new FileNotFoundException());
                await Task.Run(()=>System.IO.File.WriteAllText(pathToFile, ""));
                System.IO.File.WriteAllText(pathToFile,
                    JsonConvert.SerializeObject(JsonConvert.DeserializeObject<BaseAbstractModel>(body,
                        new JsonSerializerSettings {Converters = {new CustomJsonConverter()}})));
            }
            finally
            {
                sem.Release();
            }

            //System.IO.File.WriteAllText(pathToFile, "");
            //System.IO.File.WriteAllText(pathToFile,
            //    JsonConvert.SerializeObject(JsonConvert.DeserializeObject<BaseAbstractModel>(body,
            //        new JsonSerializerSettings { Converters = { new CustomJsonConverter() } })));
            return Ok();
        }
    }
}
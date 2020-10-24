using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly string path = @"C:\Users\Администратор\Desktop\configs";

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
                var fileText = await item.OpenText().ReadToEndAsync();
                var deserializedType = JsonConvert.DeserializeObject<BaseAbstractModel>(fileText,
                    new JsonSerializerSettings {Converters = {new CustomJsonConverter()}});
                dictCanWork.Add(item.Name,
                    deserializedType != null && listOfSupportedTypes.Contains(deserializedType.Type));
            }

            return dictCanWork.Where(item => item.Value).Select(item => item.Key);
        }

        // GET api/<ValuesController>/5
        [HttpGet("{fileName}")]
        public BaseAbstractModel Get(string fileName)
        {
            using (var sr = new StreamReader(Path.Combine(path, $"{fileName}.json")))
            {
                var str = string.Empty;
                while (!sr.EndOfStream)
                    str += sr.ReadLine();
                return JsonConvert.DeserializeObject<BaseAbstractModel>(str,
                    new JsonSerializerSettings {Converters = {new CustomJsonConverter()}});
            }
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{fileName}")]
        public async Task<IActionResult> Put(string fileName)
        {
            var bodyStream = await new StreamReader(Request.Body).ReadToEndAsync();
            System.IO.File.WriteAllText(Path.Combine(path, $"{fileName}.json"), "");
            System.IO.File.WriteAllText(Path.Combine(path, $"{fileName}.json"),
                JsonConvert.SerializeObject(JsonConvert.DeserializeObject<BaseAbstractModel>(bodyStream,
                    new JsonSerializerSettings {Converters = {new CustomJsonConverter()}})));
            return Ok();
        }
    }
}
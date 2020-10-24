using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SelfHostApi.Controllers;
using SelfHostApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTestProject
{
    public class UnitTest1
    {
        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _controller = new ValuesController();
        }

        private readonly ITestOutputHelper _testOutputHelper;
        public ValuesController _controller;

        private Task<ActionResult<BaseAbstractModel>> CheckReturnCorrectTypeOfConfig(string fileName)
        {
            return _controller.Get(fileName);
        }

        private Task<ActionResult> CheckReturnRightResult(string fileName, dynamic obj)
        {
            return _controller.Put(fileName, obj);
        }

        [Fact]
        public void GetConfigInfoAfterUpdate()
        {
            _controller.Put("TestConfig",
                JsonConvert.SerializeObject(new DeviceSettings
                    {IP = "192.168.1.201", Port = 1111, CanUseEthernet = false, Name = "Device1"}));
            var expected = new DeviceSettings
                {IP = "192.168.1.201", Port = 1111, CanUseEthernet = false, Name = "Device1"};
            try
            {
                Assert.Equal(expected,
                    (_controller.Get("TestConfig").Result.Result as OkObjectResult)?.Value as DeviceSettings);
            }
            finally
            {
                _controller.Put("TestConfig",
                    JsonConvert.SerializeObject(new DeviceSettings()));
            }
        }

        [Fact]
        public void GetRightCountOfSupportedTypes()
        {
            var result = _controller.Get();
            Assert.Equal(5, result.Result.Count());
        }

        [Fact]
        public void GetRightItemFromConfigByFileName()
        {
            Assert.IsType<ConnectionSettings>(
                (CheckReturnCorrectTypeOfConfig("conf1").Result.Result as OkObjectResult)?.Value as ConnectionSettings);
            Assert.IsType<DeviceSettings>(
                (CheckReturnCorrectTypeOfConfig("conf2").Result.Result as OkObjectResult)?.Value as DeviceSettings);
            Assert.Null(
                (CheckReturnCorrectTypeOfConfig("confWhichDoesNotExist").Result.Result as OkObjectResult)?.Value);
        }

        [Fact]
        public void PutNewInfoToConfig()
        {
            Assert.IsType<OkResult>(CheckReturnRightResult("conf1",
                JsonConvert.SerializeObject(new ConnectionSettings
                {
                    EndPoint = "http://test123.com",
                    Timeout = 1000, IsRetry = false, Attempts = 0
                })).Result as OkResult);
            Assert.IsType<BadRequestObjectResult>(CheckReturnRightResult("configWhichDoesNotExist",
                    JsonConvert.SerializeObject(new DeviceSettings()))
                .Result as BadRequestObjectResult);
        }
    }
}